namespace gestionnaireMdp.Models;

public record BddExport
{
    public required string NomFichier { get; init; }
    public required string FichierBase64 { get; init; }
}
