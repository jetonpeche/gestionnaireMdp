using gestionnaireMdp.BddModels;
using gestionnaireMdp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace gestionnaireMdp.Dialogs;

public partial class DialogAjouterModifierIdentifiant
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public Identifiant Identifiant { get; set; }
    [Parameter, EditorRequired] public List<Categorie> ListeCategorie { get; set; }

    [Inject] OutilService OutilService { get; set; }
    [Inject] IdentifiantService IdentifiantService { get; set; }

    EditContext EditContext { get; set; }

    InputType inputMdp = InputType.Password;
    string iconMdpOeil = Icons.Material.Filled.Visibility;
    bool estPourAjouter = false;

    private void Fermer() => MudDialog.Close();

    protected override void OnInitialized()
    {
        if (Identifiant is null)
        {
            estPourAjouter = true;

            Identifiant = new();
            Identifiant.UrlSiteWeb = "";
        }

        EditContext = new(Identifiant);
    }

    void VoirCacherMdp()
    {
        if(iconMdpOeil == Icons.Material.Filled.VisibilityOff)
        {
            iconMdpOeil = Icons.Material.Filled.Visibility;
            inputMdp = InputType.Password;
        }
        else
        {
            iconMdpOeil = Icons.Material.Filled.VisibilityOff;
            inputMdp = InputType.Text;
        }
    }

    async Task FormEnvoyerAsync()
    {
        if(Identifiant.IdCategorie == 0)
        {
            OutilService.AfficherToastr("La catégorie est vide", Severity.Warning);
            return;
        }

        if (estPourAjouter)
        {
            int id = await IdentifiantService.AjouterAsync(Identifiant);

            if (id == default)
                OutilService.AfficherToastr("L'identifiant n'a pas été ajouté", Severity.Error);
            else
            {
                Identifiant.Id = id;
                OutilService.AfficherToastr("L'identifiant a été ajouté", Severity.Success);

                MudDialog.Close(Identifiant);
            }
        }
        else
        {
            bool estModifier = await IdentifiantService.ModifierAsync(Identifiant);

            if (estModifier)
                MudDialog.Close(Identifiant);
            else
                OutilService.AfficherToastr("L'identifiant a été modifié", Severity.Success);
        }
    }
}