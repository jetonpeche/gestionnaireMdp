using gestionnaireMdp.BddModels;
using gestionnaireMdp.Dialogs;
using gestionnaireMdp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace gestionnaireMdp.Pages;

public partial class Index: IAsyncDisposable
{
    [Inject] IdentifiantService IdentifiantService { get; set; }
    [Inject] CategorieService CategorieService { get; set; }
    [Inject] OutilService ToastrService { get; set; }
    [Inject] IJSRuntime JSRuntime { get; set; }
    [Inject] IDialogService DialogService { get; set; }

    IJSObjectReference jSObjectReference;
    List<Categorie> listeCategorie = new();
    List<Identifiant> listeIdentifiant = new();

    int idCategorieActuelle = default;

    protected override async Task OnInitializedAsync()
    {
        listeCategorie = await CategorieService.ListerAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            jSObjectReference = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/script.js");
        }
    }

    async Task FilterIdentifiantParCategorieAsync(int _id)
    {
        idCategorieActuelle = _id;
        listeIdentifiant = await IdentifiantService.ListerAsync(_id);
    }

    async Task SupprimerAsync(Identifiant _identifiant)
    {
        bool etat = await ToastrService.MessageConfirmationAsync("Suppression Identifiant", 
                                                                 $"Supprimer l'indentifiant {_identifiant.Titre} ?");

        if(etat)
        {
           bool retour = await IdentifiantService.SupprimerAsync(_identifiant);

            if(retour)
            {
                listeIdentifiant.Remove(_identifiant);
                ToastrService.AfficherToastr($"{_identifiant.Titre} est supprimé", Severity.Success);
            }
            else
            {
                ToastrService.AfficherToastr("Supression impossible", Severity.Error);
            }
        }

        StateHasChanged();
    }

    async Task OuvrirDialogAjouterModifierIdentifiantAsync(Identifiant _identifiantActuelle = null)
    {
        DialogParameters param = new()
        {
            ["ListeCategorie"] = listeCategorie
        };

        if (_identifiantActuelle is not null)
        {
            Identifiant identifiantTempo = new()
            {
                Id = _identifiantActuelle.Id,
                IdCategorie = _identifiantActuelle.IdCategorie,
                Titre = _identifiantActuelle.Titre,
                Login = _identifiantActuelle.Login,
                Mdp = _identifiantActuelle.Mdp,
                UrlSiteWeb = _identifiantActuelle.UrlSiteWeb
            };

            param.Add("Identifiant", identifiantTempo);
        }

        var dialogRef = await DialogService.ShowAsync<DialogAjouterModifierIdentifiant>("Ajouter identifiant", param);

        // retour du dialog
        var resultatDialog = await dialogRef.Result;

        if(resultatDialog.Data is not null)
        {
            Identifiant nouveauIdentifiant = resultatDialog.Data as Identifiant;

            // pour ajouter
            if (_identifiantActuelle is null)
            {
                if (nouveauIdentifiant.IdCategorie == idCategorieActuelle)
                    listeIdentifiant.Add(nouveauIdentifiant);
            }
            else
            {
                _identifiantActuelle.UrlSiteWeb = nouveauIdentifiant.UrlSiteWeb;
                _identifiantActuelle.Login = nouveauIdentifiant.Login;
                _identifiantActuelle.IdCategorie = nouveauIdentifiant.IdCategorie;
                _identifiantActuelle.Mdp = nouveauIdentifiant.Mdp;
                _identifiantActuelle.Titre = nouveauIdentifiant.Titre;
            }
        }
    }

    async Task OuvrirModalInfosAsync(Identifiant _identifiant)
    {
        string html = "<ul>" +
                    $"<li>Login: {_identifiant.Login}</li>" +
                    $"<li>Mot de passe: {_identifiant.Mdp}</li>" +
                    $"{(string.IsNullOrEmpty(_identifiant.UrlSiteWeb) ? "<li>Url: <a href='{_identifiant.UrlSiteWeb}'>Liens web</a></li>" : "")}" +
                    "</ul>";

        var param = new DialogParameters()
        {
            ["ContenueHtmlString"] = html,
            ["Titre"] = $"Identifiant: {_identifiant.Titre}"
        };

        await DialogService.ShowAsync<DialogInfos>("Infos identifiant", param);
    }

    async Task ExportAsync(int _idIndentifiant)
    {
        var bddExport = await IdentifiantService.ExporterAsync(new int[] { _idIndentifiant });
        await jSObjectReference.InvokeVoidAsync("FaireTelecharger", bddExport.NomFichier, bddExport.FichierBase64);
    }

    private async Task ExporterBdd()
    {
        try
        {
            //await IdentifiantService.AjouterAsync<Identifiant>("INSERT INTO identifiant (login, mdp, urlSiteWeb, idCategorie, titre) VALUES ('titi', 'mdp', '', 3, 'titre 1'), ('tata', 'mdp2', 'https', 3, 'titre 2');");
            //BddExport bddExport = await IdentifiantService.ExporterAsync(new int[] { 1, 2 });
            //BddExport bddExport = await BddService.ExporterToutAsync();
            //await jSObjectReference.InvokeVoidAsync("FaireTelecharger", bddExport.NomFichier, bddExport.FichierBase64);
        }
        catch
        {
            await jSObjectReference.InvokeVoidAsync("MsgErreurDl");
        }  
    }

    public async ValueTask DisposeAsync()
    {
        if(jSObjectReference != null)
            await jSObjectReference.DisposeAsync();
    }
}