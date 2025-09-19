using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SeriDoviz.ViewModels;

namespace SeriDoviz.Views;

public partial class CeviriciPage : ContentPage
{
    public CeviriciPage() : this(App.Services.GetRequiredService<CeviriciViewModel>()) { }
    public CeviriciPage(CeviriciViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm ?? throw new ArgumentNullException(nameof(vm));
#if IOS
        this.On<iOS>().SetUseSafeArea(false);
#endif
    }

    CeviriciViewModel Vm => (CeviriciViewModel)BindingContext;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.InitAsync();
        await Vm.CalculateAsync();
    }
}
