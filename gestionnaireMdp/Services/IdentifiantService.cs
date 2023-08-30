using gestionnaireMdp.BddModels;
using gestionnaireMdp.Models;

namespace gestionnaireMdp.Services;

public class IdentifiantService
{
    private readonly BddService bddService;

    public IdentifiantService(BddService _bddService)
    {
        bddService = _bddService;
    }

    public async Task<List<Identifiant>> ListerAsync()
    {
        return await bddService.ListerAsync<Identifiant>();
    }

    public async Task<List<Identifiant>> ListerAsync(int _idCategorie)
    {
        string sql = $"SELECT * FROM identifiant WHERE idCategorie = {_idCategorie}";
        return await bddService.ListerAsync<Identifiant>(sql);
    }

    public async Task<int> AjouterAsync(Identifiant _identifiant)
    {
        return await bddService.AjouterAsync(_identifiant);
    }

    public async Task<List<Identifiant>> AjouterAsync(string _insertInto)
    {
        return await bddService.AjouterAsync<Identifiant>(_insertInto);
    }

    public async Task<bool> ModifierAsync(Identifiant _identifiant)
    {
       return await bddService.ModifierAsync(_identifiant);
    }

    public async Task<bool> SupprimerAsync(Identifiant _identifiant)
    {
        return await bddService.SupprimerAsync(_identifiant);
    }

    public async Task<BddExport> ExporterAsync(int[] _listIdIdentifiant)
    {
        if (_listIdIdentifiant.Length is 0)
            return null;

        return await bddService.ExporterAsync<Identifiant>(_listIdIdentifiant);
    }
}
