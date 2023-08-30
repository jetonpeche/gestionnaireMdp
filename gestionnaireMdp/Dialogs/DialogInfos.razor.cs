using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace gestionnaireMdp.Dialogs;

public partial class DialogInfos
{
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }

    [Parameter, EditorRequired]
    public string Titre { get; set; }

    [Parameter, EditorRequired]
    public string ContenueHtmlString { get; set; }

    void Fermer() => MudDialog.Close();
}