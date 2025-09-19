using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SeriDoviz.ViewModels;

namespace SeriDoviz.Views;

public partial class AltinPage : ContentPage
{
    bool _run;

    public AltinPage() : this(App.Services.GetRequiredService<AltinViewModel>()) { }
    public AltinPage(AltinViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
#if IOS
        this.On<iOS>().SetUseSafeArea(false);
#endif
    }

    AltinViewModel Vm => (AltinViewModel)BindingContext;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.LoadAsync();

        _run = true;
        int idx = 0;
        Device.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (!_run) return false;
            if (Vm.HeaderItems.Count == 0) return true;
            idx = (idx + 1) % Vm.HeaderItems.Count;
            var item = Vm.HeaderItems[idx];
            HeaderTicker.ScrollTo(item, position: ScrollToPosition.Center, animate: true);
            return true;
        });
    }

    protected override void OnDisappearing()
    {
        _run = false;
        Vm.Stop();
        base.OnDisappearing();
    }
}
