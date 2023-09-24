using gestionnaireMdp.BddModels;
using gestionnaireMdp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Outil.Services;

namespace gestionnaireMdp.Dialogs;

public partial class DialogAjouterModifierIdentifiant
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public Identifiant Identifiant { get; set; }
    [Parameter, EditorRequired] public List<Categorie> ListeCategorie { get; set; }

    [Inject] OutilService OutilService { get; set; }
    [Inject] IdentifiantService IdentifiantService { get; set; }
    [Inject] ProtectionService ProtectionService { get; set; }
    [Inject] MotDePasseService MotDePasseService { get; set; }

    EditContext EditContext { get; set; }

    InputType inputMdp = InputType.Password;
    bool estPourAjouter = false;
    bool contientCaractereSpeciaux = false;

    int nbCaractereSpeciaux;
    int longeurMdp = 8;

    private void Fermer() => MudDialog.Close();

    void GenererMdp()
    {
        Identifiant.Mdp = MotDePasseService.Generer((ushort)longeurMdp, contientCaractereSpeciaux, nbCaractereSpeciaux);
        inputMdp = InputType.Text;
    }

    void ChangerTypeInputMdp()
    {
        if(inputMdp == InputType.Text)
        {
            inputMdp = InputType.Password;
        }
        else
        {
            inputMdp = InputType.Text;
        }
    }

    protected override void OnInitialized()
    {
        if (Identifiant is null)
        {
            estPourAjouter = true;

            Identifiant = new();
            Identifiant.UrlSiteWeb = "";
        }
        else
        {
            Identifiant.Login = ProtectionService.Dechiffrer(Identifiant.Login);
            Identifiant.Mdp = ProtectionService.Dechiffrer(Identifiant.Mdp);
        }

        EditContext = new(Identifiant);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        //GenererMdp();
    }

    async Task FormEnvoyerAsync()
    {
        if(Identifiant.IdCategorie == 0)
        {
            OutilService.AfficherToastr("La catégorie est vide", Severity.Warning);
            return;
        }

        Identifiant.Login = ProtectionService.Chiffrer<string>(Identifiant.Login);
        Identifiant.Mdp = ProtectionService.Chiffrer<string>(Identifiant.Mdp);

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
            {
                OutilService.AfficherToastr("L'identifiant a été modifié", Severity.Success);
                MudDialog.Close(Identifiant);
            }     
            else
                OutilService.AfficherToastr("L'identifiant n'a pas été modifié", Severity.Error);
        }
    }
}