using gestionnaireMdp.Services;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using Outil.SDK.DependencyInjection;

namespace gestionnaireMdp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                config.SnackbarConfiguration.VisibleStateDuration = 4000;
                config.SnackbarConfiguration.HideTransitionDuration = 200;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
            });

            builder.Services.AjouterBoiteOutil(option =>
            {
                option.ActiverDonneeService = true;
                option.ActiverProtectionService = true;
                option.ActiverMdpService = true;

                option.ProtectionOptions.IVsecret = "2GaF]%[_hB7W4ux8";
                option.ProtectionOptions.CleSecrete = "qi9-m_6DY.*7853#CTq!6TNcf^fC)C4c";
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<BddService>()
                .AddSingleton<IdentifiantService>()
                .AddSingleton<CategorieService>()
                .AddTransient<OutilService>();

            return builder.Build();
        }
    }
}