using MudBlazor;

namespace gestionnaireMdp.Services;

public class OutilService
{
    ISnackbar snackbarService;
    IDialogService dialogService;

    public OutilService(ISnackbar _snackbarService, IDialogService _dialogService)
    {
        snackbarService = _snackbarService;
        dialogService = _dialogService;
    }

    public async Task<bool> MessageConfirmationAsync(string _titre, string _message)
    {
        bool? reponse = await dialogService.ShowMessageBox(
            _titre,
            _message,
            yesText: "Oui", cancelText: "Non");

        return reponse != null ? reponse.Value : false;
    }

    public void AfficherToastr(string _message, Severity _couleur) => snackbarService.Add(_message, _couleur);
}
