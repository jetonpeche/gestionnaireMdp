using SQLite;

namespace gestionnaireMdp.BddModels;

public class Categorie: IBddModels
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public int Id { get; set; }

    [MaxLength(100), Column("nom"), NotNull]
    public string Nom { get; set; }
}
