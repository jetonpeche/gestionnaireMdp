using gestionnaireMdp.BddModels;
using gestionnaireMdp.Models;
using SQLite;
using System.Reflection;
using System.Text;

namespace gestionnaireMdp.Services;

public sealed class BddService
{
    private SQLiteAsyncConnection connexion;
    private readonly string connexionString = Path.Combine(FileSystem.AppDataDirectory, "bdd.db3");
    private readonly string nomFichier = "export.sql";

    public BddService()
    {
        Task _ = InitAsync();
    }

    public async Task<List<T>> ListerAsync<T>() where T : IBddModels, new()
    {
        try
        {
            await InitAsync();

            return await connexion.Table<T>().ToListAsync();
        }
        catch
        {
            return Array.Empty<T>().ToList();
        }
    }

    /// <summary>
    /// Pour des requetes particulières
    /// </summary>
    /// <typeparam name="T">Classe de la table de la Bdd</typeparam>
    /// <param name="_requeteSQL">requete sql</param>
    /// <returns></returns>
    public async Task<List<T>> ListerAsync<T>(string _requeteSQL) where T : IBddModels, new()
    {
        try
        {
            await InitAsync();

            return await connexion.QueryAsync<T>(_requeteSQL);
        }
        catch
        {
            return Array.Empty<T>().ToList();
        }
    }

    public async Task<int> AjouterAsync<T>(T _item) where T : IBddModels, new()
    {
        try
        {
            await InitAsync();

            await connexion.InsertAsync(_item);

            return _item.Id;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Ajouter dans la BDD via des "insert into"
    /// </summary>
    /// <typeparam name="T">Nom de la classe des infos retour et de la table utilisée</typeparam>
    /// <param name="_insertInto">insert into SQL (exemple format obligatoire: INSERT INTO Table (.., ..) VALUES (), ();)</param>
    /// <returns></returns>
    public async Task<List<T>> AjouterAsync<T>(string _insertInto) where T : IBddModels, new()
    {
        try
        {
            int nbInsertInto = _insertInto.Split(')').Count() - 2;
            List<T> listeRetour = new();

            await connexion.QueryAsync<int>(_insertInto);

            listeRetour = await connexion
                .Table<T>()
                .OrderByDescending(x => x.Id)
                .Take(nbInsertInto)
                .ToListAsync();

            return listeRetour;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ModifierAsync<T>(T _item) where T : IBddModels, new()
    {
        try
        {
            await InitAsync();

            int retour = await connexion.UpdateAsync(_item);

            return retour > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SupprimerAsync<T>(T _item) where T : IBddModels, new()
    {
        try
        {
            await InitAsync();

            int retour = await connexion.DeleteAsync(_item);

            return retour > 0;
        }
        catch
        {
            return false;
        }      
    }

    /// <summary>
    /// Pour des requetes particulieres
    /// </summary>
    /// <typeparam name="T">Classe de la table de la bdd</typeparam>
    /// <param name="_requeteSQL">requete sql</param>
    /// <returns></returns>
    public async Task<bool> SupprimerAsync(string _requeteSQL)
    {
        try
        {
            int retour = await connexion.ExecuteScalarAsync<int>(_requeteSQL);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Exporter la bdd (tables, données)
    /// </summary>
    /// <returns>chemin d'acces au fichier</returns>
    public async Task<BddExport> ExporterToutAsync()
    {
        await InitAsync();      

        StringBuilder stringBuilderInsertInto = new();
        StringBuilder stringBuilderCreateTable = new();

        List<Categorie> listeCategorie = await connexion.Table<Categorie>().ToListAsync();
        List<Identifiant> listeIdentifiant = await connexion.Table<Identifiant>().ToListAsync();

        stringBuilderInsertInto.AppendLine(InitInsertIntoSQL(listeCategorie));
        stringBuilderInsertInto.AppendLine(InitInsertIntoSQL(listeIdentifiant));

        stringBuilderCreateTable.AppendLine(InitCreateTableSQL(new Categorie()));
        stringBuilderCreateTable.AppendLine(InitCreateTableSQL(new Identifiant()));

        string fichierBase64 = TransformerDonneeEnBase64(stringBuilderCreateTable.ToString() + stringBuilderInsertInto.ToString());

        return new BddExport()
        {
            NomFichier = nomFichier,
            FichierBase64 = fichierBase64
        };

        string InitCreateTableSQL<T>(T _modelBdd) where T : IBddModels, new()
        {
            StringBuilder stringBuilderCreateTable = new();

            PropertyInfo[] listePropriete = typeof(T).GetProperties();

            stringBuilderCreateTable.Append($"CREATE TABLE {typeof(T).Name} (");

            for (int i = 0; i < listePropriete.Length; i++)
            {
                // propriete ignorer => passer a la suivante
                if (listePropriete[i].CustomAttributes.Where(x => x.AttributeType.Name == nameof(IgnoreAttribute)).Count() == 1)
                    continue;

                StringBuilder stringBuilderChampSQL = new();

                int? longeurMaxString = (int?)listePropriete[i].CustomAttributes.Where(x => x.AttributeType.Name == nameof(MaxLengthAttribute)).Select(x => x.ConstructorArguments[0].Value).FirstOrDefault();
                string nomColonne = (string)listePropriete[i].CustomAttributes.Where(x => x.AttributeType.Name == nameof(ColumnAttribute)).Select(x => x.ConstructorArguments[0].Value).FirstOrDefault();

                bool isNotNull = listePropriete[i].CustomAttributes.Where(x => x.AttributeType.Name == nameof(NotNullAttribute)).Count() == 1;
                bool estClePrimaire = listePropriete[i].CustomAttributes.Where(x => x.AttributeType.Name == nameof(PrimaryKeyAttribute)).Count() == 1;
                bool estAutoIncremente = listePropriete[i].CustomAttributes.Where(x => x.AttributeType.Name == nameof(AutoIncrementAttribute)).Count() == 1;
                
                // ********* construction du champ SQL ************

                stringBuilderChampSQL.Append($"{(nomColonne != default ? nomColonne : listePropriete[i].Name)} ");

                if(estClePrimaire)
                    stringBuilderChampSQL.Append($"PRIMARY KEY {(estAutoIncremente ? "AUTOINCREMENT" : "")} ");

                // definition du type de la donnée
                if (longeurMaxString != default)
                {
                    stringBuilderChampSQL.Append($"VARCHAR({longeurMaxString})");
                }
                else
                {
                    switch (listePropriete[i].PropertyType.Name)
                    {
                        case nameof(String):
                            stringBuilderChampSQL.Append("VARCHAR(1000)");
                            break;

                        case nameof(Int32):
                            stringBuilderChampSQL.Append("INT");
                            break;

                        case nameof(Boolean):
                            stringBuilderChampSQL.Append("BIT");
                            break;
                    }
                }

                if (isNotNull)
                    stringBuilderChampSQL.Append(" NOT NULL");

                // dernier element => pas de ','
                if(i != listePropriete.Length - 1)
                    stringBuilderChampSQL.Append(",");

                stringBuilderCreateTable.Append(stringBuilderChampSQL.ToString());    
            }

            stringBuilderCreateTable.Append(");");
            stringBuilderCreateTable.AppendLine();

            return stringBuilderCreateTable.ToString();
        }
    }

    /// <summary>
    /// Exporter les infos de la table choisi
    /// </summary>
    /// <typeparam name="T">La classe de la table qui est interogoré</typeparam>
    /// <param name="_listeId">Liste des id pour recuperer les infos</param>
    /// <returns></returns>
    public async Task<BddExport> ExporterAsync<T>(int[] _listeId) where T : IBddModels, new()
    {
        await InitAsync();

        List<T> listeDonnee = await connexion.QueryAsync<T>($"SELECT {string.Join(',', typeof(T).GetProperties().Select(x => x.Name))} FROM {typeof(T).Name} WHERE id IN ({string.Join(',', _listeId)})");

        string insertIntoString = InitInsertIntoSQL(listeDonnee, true);
        string fichierBase64 = TransformerDonneeEnBase64(insertIntoString);

        return new BddExport()
        {
            FichierBase64 = fichierBase64,
            NomFichier = nomFichier
        };
    }

    private string InitInsertIntoSQL<T>(List<T> _listeDonnee, bool _ignorerClePrimaire = false) where T : new()
    {
        if (_listeDonnee.Count == 0)
            return "";

        StringBuilder stringBuilderNomProprieter = new();
        StringBuilder stringBuilderInsertInto = new();

        PropertyInfo[] listePropriete = typeof(T).GetProperties();

        // liste des noms des proprietés de la classe
        for (int i = 0; i < listePropriete.Length; i++)
        {
            if(_ignorerClePrimaire)
            {
                if (listePropriete[i].GetCustomAttribute(typeof(PrimaryKeyAttribute)) == default)
                    continue;
            }

            stringBuilderNomProprieter.Append(listePropriete[i].Name + (i == listePropriete.Length - 1 ? "" : ","));
        }

        stringBuilderInsertInto.Append($"INSERT INTO {typeof(T).Name} ({stringBuilderNomProprieter}) VALUES ");

        // liste des insert into
        for (int i = 0; i < _listeDonnee.Count; i++)
        {
            stringBuilderInsertInto.Append("(");

            // ajout des valeurs des proprietés
            for (int j = 0; j < listePropriete.Length; j++)
            {
                stringBuilderInsertInto.Append($"'{listePropriete[j].GetValue(_listeDonnee[i])}'");

                if (j == listePropriete.Length - 1)
                    stringBuilderInsertInto.Append(i == _listeDonnee.Count - 1 ? ")" : "),");
                else
                    stringBuilderInsertInto.Append(",");
            }
        }

        stringBuilderInsertInto.Append(";");

        return stringBuilderInsertInto.ToString();
    }

    private string TransformerDonneeEnBase64(string _donnee)
    {
        string chemin = Path.Combine(FileSystem.CacheDirectory, nomFichier);

        using (StreamWriter reader = File.CreateText(chemin))
        {
            reader.Write(_donnee);
        }

        byte[] FichierBytes = File.ReadAllBytes(chemin);
        string fichierBase64 = Convert.ToBase64String(FichierBytes);

        File.Delete(chemin);

        return fichierBase64;
    }

    private async Task InitAsync()
    {
        if (connexion is not null)
            return;

        connexion = new(connexionString);

        await connexion.CreateTablesAsync<Categorie, Identifiant>();
    }
}
