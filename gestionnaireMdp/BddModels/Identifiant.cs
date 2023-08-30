using SQLite;
namespace gestionnaireMdp.BddModels;

public sealed class Identifiant: IBddModels
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public int Id { get; set; }

    [MaxLength(100), Column("titre"), NotNull]
    public string Titre { get; set; }

    [MaxLength(100), Column("login"), NotNull]
    public string Login { get; set; }

    [MaxLength(300), Column("mdp"), NotNull]
    public string Mdp { get; set; }

    [MaxLength(500), Column("urlSiteWeb"), NotNull]
    public string UrlSiteWeb { get; set; }

    [Indexed, Column("idCategorie"), NotNull]
    public int IdCategorie { get; set; }
}
