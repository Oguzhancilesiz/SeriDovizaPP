using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriDoviz.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string? title;

        // Overlay'i kontrol etmek için
        [ObservableProperty] private bool overlayVisible;

        // En fazla verilen süre kadar overlay'i gösterip kapatır
        protected async Task ShowOverlayForAsync(TimeSpan max)
        {
            OverlayVisible = true;
            try { await Task.Delay(max); } catch { }
            OverlayVisible = false;
        }
    }

}
