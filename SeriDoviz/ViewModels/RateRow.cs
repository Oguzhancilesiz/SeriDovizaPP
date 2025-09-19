using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Animations;
using Microsoft.Maui.ApplicationModel; // MainThread
using Microsoft.Maui.Controls;
using SeriDoviz.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SeriDoviz.ViewModels
{
    public partial class RateRow : ObservableObject
    {
        [ObservableProperty] string symbol = "";
        [ObservableProperty] decimal? bid;
        [ObservableProperty] decimal? ask;
        [ObservableProperty] decimal? change;
        [ObservableProperty] string dir = "up";

        // Liste satırı arka planına bağlı geçici fırça (blink)
        [ObservableProperty] Brush? flash;

        // Son tik yönü (-1: down, 0: flat, +1: up)
        [ObservableProperty] int tickDir;

        public string TickArrow => TickDir > 0 ? "▲" : TickDir < 0 ? "▼" : "•";
        partial void OnTickDirChanged(int value) => OnPropertyChanged(nameof(TickArrow));

        // Günlük ok (varsa kullanmaya devam edebilirsin)
        public string Arrow => (Change ?? 0m) >= 0 ? "▲" : "▼";
        partial void OnChangeChanged(decimal? value) => OnPropertyChanged(nameof(Arrow));

        // Karşılaştırma için önbellek
        private decimal? _lastAsk;
        private decimal? _lastChange;

        // Çakışan animasyonları iptal için
        private CancellationTokenSource? _flashCts;

        private const int BlinkMs = 280;  // kısa, net bir yanıp sönme
        private const decimal Eps = 0.00001m; // hassasiyet

        public static RateRow From(RateDto d) => new()
        {
            Symbol = d.Symbol,
            Bid = d.Bid,
            Ask = d.Ask,
            Change = d.Change,
            Dir = d.Dir,
            TickDir = 0,
            _lastAsk = d.Ask,
            _lastChange = d.Change
        };

        public async Task UpdateFromAsync(RateDto d)
        {
            var prevAsk = _lastAsk ?? Ask ?? 0m;
            var prevChange = _lastChange ?? Change ?? 0m;

            var nextAsk = d.Ask ?? 0m;
            var nextChange = d.Change ?? 0m;

            // Önce değerleri UI'da güncelle
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Symbol = d.Symbol;
                Bid = d.Bid;
                Ask = d.Ask;
                Change = d.Change;
                Dir = d.Dir;
            });

            // Yön (Ask öncelikli, eşitse Change)
            int dirSign = 0;
            var dAsk = nextAsk - prevAsk;
            if (Math.Abs(dAsk) >= Eps)
                dirSign = dAsk > 0 ? +1 : -1;
            else
            {
                var dCh = nextChange - prevChange;
                if (Math.Abs(dCh) >= Eps)
                    dirSign = dCh > 0 ? +1 : -1;
            }

            // Son tik yönünü property olarak yayımla (rozet/ok için)
            if (dirSign != 0)
                await MainThread.InvokeOnMainThreadAsync(() => TickDir = dirSign);

            // Blink sadece hareket eden satıra
            if (dirSign != 0)
                _ = BlinkAsync(dirSign > 0); // fire-and-forget

            // Son değerleri güncelle
            _lastAsk = d.Ask;
            _lastChange = d.Change;
        }

        private async Task BlinkAsync(bool up)
        {
            var newCts = new CancellationTokenSource();
            var old = Interlocked.Exchange(ref _flashCts, newCts);
            try { old?.Cancel(); } catch { }
            old?.Dispose();
            var token = newCts.Token;

            var brush = CreateFreshFlashBrush(up);

            // küçük frame flush → gerçek "yanıp sönme" etkisi
            await MainThread.InvokeOnMainThreadAsync(() => Flash = null);
            await Task.Delay(1);
            await MainThread.InvokeOnMainThreadAsync(() => Flash = brush);

            try { await Task.Delay(BlinkMs, token); }
            catch (TaskCanceledException) { /* yeni tick geldi */ }
            finally
            {
                if (ReferenceEquals(_flashCts, newCts))
                    await MainThread.InvokeOnMainThreadAsync(() => Flash = null);
            }
        }

        private static Brush CreateFreshFlashBrush(bool up)
        {
            var key = up ? "FlashUpBrush" : "FlashDownBrush";
            var res = Application.Current?.Resources[key];

            if (res is LinearGradientBrush lg)
            {
                var copy = new LinearGradientBrush { StartPoint = lg.StartPoint, EndPoint = lg.EndPoint };
                foreach (var gs in lg.GradientStops)
                    copy.GradientStops.Add(new GradientStop { Color = gs.Color, Offset = gs.Offset });
                return copy;
            }
            if (res is SolidColorBrush sb)
                return new SolidColorBrush(sb.Color);

            return new SolidColorBrush(up ? Colors.DarkGreen : Colors.DarkRed);
        }
    }
}
