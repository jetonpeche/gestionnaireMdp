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
                OutilService.AfficherToastr("La catégorie n'a pas pu être ajouter", Severity.Error);
            else
            {
                Categorie.Id = id;
                OutilService.AfficherToastr("La catégorie a été ajouté", Severity.Success);

                MudDialog.Close(Categorie);
            }
        }
        else
        {
            bool estModifier = await CategorieService.ModifierAsync(Categorie);

            if (estModifier)
            {
                OutilService.AfficherToastr("La catégorie a été modifié", Severity.Success);
                MudDialog.Close(Categorie);
            }    
            else
                OutilService.AfficherToastr("La catégorie n'a pas été modifié", Severity.Error);
        }
    }
}