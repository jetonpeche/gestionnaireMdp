using gestionnaireMdp.BddModels;
using gestionnaireMdp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace gestionnaireMdp.Dialogs;

public partial class DialogAjouterModifierCategorie
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public Categorie Categorie { get; set; }

    [Inject] OutilService OutilService { get; set; }
    [Inject] CategorieService CategorieService { get; set; }

    EditContext EditContext { get; set; }
    bool estPourAjouter = false;

    protected override void OnInitialized()
    {
        if(Categorie is null)
        {
            estPourAjouter = true;
            Categorie = new();
        }

        EditContext = new(Categorie);
    }

    void Fermer() => MudDialog.Close();

    async Task FormEnvoyerAsync()
    {
        if(string.IsNullOrEmpty((EditContext.Model as Categorie).Nom))
        {
            OutilService.AfficherToastr("Le nom est obligatoire", Severity.Warning);
            return;
        }

        if(estPourAjouter)
        {
            int id = await CategorieService.AjouterAsync(Categorie);

            if (id == default)
                OutilService.AfficherToastr("La cat�gorie n'a pas pu �tre ajouter", Severity.Error);
            else
            {
                Categorie.Id = id;
                OutilService.AfficherToastr("La cat�gorie a �t� ajout�", Severity.Success);

                MudDialog.Close(Categorie);
            }
        }
        else
        {
            bool estModifier = await CategorieService.ModifierAsync(Categorie);

            if (estModifier)
            {
                OutilService.AfficherToastr("La cat�gorie a �t� modifi�", Severity.Success);
                MudDialog.Close(Categorie);
            }    
            else
                OutilService.AfficherToastr("La cat�gorie n'a pas �t� modifi�", Severity.Error);
        }
    }
}