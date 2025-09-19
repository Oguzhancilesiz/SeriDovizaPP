using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SeriDoviz.ViewModels;

namespace SeriDoviz.Views;

public partial class DovizPage : ContentPage
{
    bool _tickerRun;

    public DovizPage() : this(App.Services.GetRequiredService<DovizViewModel>()) { }
    public DovizPage(DovizViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
#if IOS
        this.On<iOS>().SetUseSafeArea(false);
#endif
    }

    DovizViewModel Vm => (DovizViewModel)BindingContext;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.LoadAsync();

        _tickerRun = true;
        int idx = 0;
        Device.StartTimer(TimeSpan.FromSeconds(2.8), () =>
        {
            if (!_tickerRun) return false;
            if (Vm.TickerItems.Count == 0) return true;
            idx = (idx + 1) % Vm.TickerItems.Count;
            var item = Vm.TickerItems[idx];
            Ticker.ScrollTo(item, position: ScrollToPosition.Center, animate: true);
            return true;
        });
    }

    protected override void OnDisappearing()
    {
        _tickerRun = false;
        Vm.Stop();
        base.OnDisappearing();
    }
}
