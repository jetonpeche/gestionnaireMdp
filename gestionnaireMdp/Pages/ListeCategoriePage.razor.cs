using gestionnaireMdp.BddModels;
using gestionnaireMdp.Dialogs;
using gestionnaireMdp.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace gestionnaireMdp.Pages;

public partial class ListeCategoriePage
{
    [Inject] CategorieService CategorieService { get; set; }
    [Inject] OutilService OutilService { get; set; }
    [Inject] IDialogService DialogService { get; set; }

    List<Categorie> listeCategorie = new();

    protected override async Task OnInitializedAsync()
    {
        listeCategorie = await CategorieService.ListerAsync();
    }

    async Task OuvrirDialogAjouterModifierCategorieAsync(Categorie _categorie = null)
    {
        DialogParameters param = null;

        if(_categorie is not null)
        {
            Categorie categorieTempo = new()
            {
                Id = _categorie.Id,
                Nom = _categorie.Nom
            };

            param = new()
            {
                ["Categorie"] = categorieTempo
            };
        }

        var dialogRef = await DialogService.ShowAsync<DialogAjouterModifierCategorie>("", param);

        var resultatDialog = await dialogRef.Result;

        if(resultatDialog.Data is not null)
        {
            Categorie nouvelleCategorie = resultatDialog.Data as Categorie;

            if (_categorie is not null)
                listeCategorie.Add(nouvelleCategorie);
            else
            {
                _categorie.Nom = nouvelleCategorie.Nom;
            }
        }
    }

    async Task SupprimerAsync(Categorie _categorie)
    {
        bool retour = await OutilService.MessageConfirmationAsync($"Supprimer categorie", $"Supprimer la categorie {_categorie.Nom} et tous les identifiants ?");

        if(retour) 
        {
           bool estSupprimer = await CategorieService.SupprimerAsync(_categorie);

            if (estSupprimer)
            {
                listeCategorie.Remove(_categorie);
                OutilService.AfficherToastr("La catégorie a été supprimée", Severity.Success);
            }
            else
                OutilService.AfficherToastr("Impossible de supprimer", Severity.Error);
        }
    }
}