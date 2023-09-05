using MudBlazor;
using System.Security.Cryptography;
using System.Text;

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

    public string GenererMdp(int _longueurMdp, bool _contientCaractereSpeciaux, int _nbCaractereSpeciaux = 0)
    {
        const string CARACTERE_SPECIAUX = "!@#$%^&*()_-+=[{]};:>|./?";
        const string CARACTERE_ALPHANUMERIQUE = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] motDePasse = new char[_longueurMdp];

        if (_longueurMdp < 1 || _longueurMdp > 100)
            return "La longueur du mot de passe doit être comprise entre 1 et 128 caractères.";

        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] octetsAleatoires = new byte[_longueurMdp];
            rng.GetBytes(octetsAleatoires);

            Random rand = new();
            int nbCaracteresSpeciaux = 0;

            if (_contientCaractereSpeciaux)
                nbCaracteresSpeciaux = _nbCaractereSpeciaux > 0 ? _nbCaractereSpeciaux : rand.Next(1, _longueurMdp + 1);

            for (int i = 0; i < _longueurMdp; i++)
            {
                int indexAleatoire;

                if (i < nbCaracteresSpeciaux)
                {
                    indexAleatoire = octetsAleatoires[i] % CARACTERE_SPECIAUX.Length;
                    motDePasse[i] = CARACTERE_SPECIAUX[indexAleatoire];
                }
                else
                {
                    indexAleatoire = octetsAleatoires[i] % CARACTERE_ALPHANUMERIQUE.Length;
                    motDePasse[i] = CARACTERE_ALPHANUMERIQUE[indexAleatoire];
                }
            }

            // Mélangez le mot de passe pour plus de sécurité
            for (int i = motDePasse.Length - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                char temp = motDePasse[i];
                motDePasse[i] = motDePasse[j];
                motDePasse[j] = temp;
            }

            return new string(motDePasse);
        }
    }
}
