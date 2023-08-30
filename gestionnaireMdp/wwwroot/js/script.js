export function FaireTelecharger(_nomFichier, _base64)
{
    try
    {
        let divA = document.createElement("a");
        divA.href = "data:plain/text;base64," + _base64;
        divA.download = _nomFichier;

        divA.click();

        setTimeout(() => {
            document.getElementById("msg").innerHTML = "<div id='msgOkDl' class='alert alert-success' role='alert'>A simple danger alert—check it out!</div>";
        }, 1000);

        setTimeout(() => {
            document.getElementById("msgOkDl").remove();
        }, 4000);
    }
    catch (e)
    {
        console.log(e)
        MsgErreurDl();
    }
}

export function MsgErreurDl()
{
    document.getElementById("msg").innerHTML += "<div id='msgErreurDl' class='alert alert-danger' role='alert'>A simple danger alert—check it out!</div>";

    setTimeout(() => {
        document.getElementById("msgErreurDl").remove();
    }, 3000)
}