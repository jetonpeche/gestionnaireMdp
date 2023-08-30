using gestionnaireMdp.BddModels;

namespace gestionnaireMdp.Services;

public class CategorieService
{
    private readonly BddService bddService;

    public CategorieService(BddService _bddService)
    {
        bddService = _bddService;
    }

    public async Task<List<Categorie>> ListerAsync()
    {
        return await bddService.ListerAsync<Categorie>();
    }

    public async Task<int> AjouterAsync(Categorie _categorie)
    {
        return await bddService.AjouterAsync(_categorie);
    }

    public async Task<List<Categorie>> AjouterAsync(string _insertInto)
    {
        return await bddService.AjouterAsync<Categorie>(_insertInto);
    }

    public async Task<bool> ModifierAsync(Categorie _categorie)
    {
        return await bddService.ModifierAsync(_categorie);
    }

    public async Task<bool> SupprimerAsync(Categorie _categorie)
    {
        await bddService.SupprimerAsync($"DELETE FROM {nameof(Identifiant)} WHERE idCategorie = {_categorie.Id}");
        return await bddService.SupprimerAsync(_categorie);
    }
}
