using SeriDoviz.Services;
using SeriDoviz.ViewModels;
using SeriDoviz.Views;

namespace SeriDoviz;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        const string ProdUrl = "https://seridoviz.com";

#if DEBUG
        // Emülatörden yerel API'ye vuracaksan:
        // const string BaseUrl = "http://10.0.2.2:5000";
        const string BaseUrl = ProdUrl;
#else
        const string BaseUrl = ProdUrl;
#endif

#if DEBUG
        // Dev sertifikaları için gevşek doğrulama (PROD’DA ASLA AÇMA)
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
        };
        builder.Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) });
#else
        builder.Services.AddSingleton(new HttpClient() { BaseAddress = new Uri(BaseUrl) });
#endif

        builder.Services.AddSingleton<IRatesService>(sp =>
            new RatesService(sp.GetRequiredService<HttpClient>())
        );

        builder.Services.AddTransient<DovizViewModel>();
        builder.Services.AddTransient<AltinViewModel>();
        builder.Services.AddTransient<CeviriciViewModel>();

        builder.Services.AddTransient<DovizPage>();
        builder.Services.AddTransient<AltinPage>();
        builder.Services.AddTransient<CeviriciPage>();

        return builder.Build();
    }
}
