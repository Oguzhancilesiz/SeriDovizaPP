using CommunityToolkit.Mvvm.Input;
using SeriDoviz.Models;
using SeriDoviz.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel; // MainThread

namespace SeriDoviz.ViewModels;

public partial class DovizViewModel : BaseViewModel
{
    private readonly IRatesService _service;
    private CancellationTokenSource? _cts;

    public ObservableCollection<RateRow> Items { get; } = new();
    public ObservableCollection<RateRow> TickerItems { get; } = new();

    private readonly Dictionary<string, RateRow> _index = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string[] TickerSymbols = { "USD/TRY", "EUR/TRY", "GBP/TRY", "XAU/USD" };

    public DovizViewModel(IRatesService service) { Title = "Döviz"; _service = service; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        OverlayVisible = true;

        try
        {
            _cts?.Cancel();
            _cts = new();

            _index.Clear(); Items.Clear(); TickerItems.Clear();

            var first = await _service.GetCurrenciesAsync() ?? Array.Empty<RateDto>();
            ApplyBatchOnUi(first);

            _ = StreamLoopAsync(_cts.Token);

            _ = ShowOverlayForAsync(TimeSpan.FromSeconds(2));
        }
        finally { IsBusy = false; }
    }

    private async Task StreamLoopAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var batch in _service.StreamCurrenciesAsync(ct))
            {
                var safe = batch ?? Array.Empty<RateDto>();
                ApplyBatchOnUi(safe); // <-- her zaman UI thread
            }
        }
        catch (OperationCanceledException) { }
    }

    [RelayCommand]
    public Task RefreshAsync() => LoadAsync();

    private void ApplyBatchOnUi(IEnumerable<RateDto> batch)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var dto in batch)
            {
                if (!_index.TryGetValue(dto.Symbol, out var row))
                {
                    row = RateRow.From(dto);
                    _index[dto.Symbol] = row;
                    Items.Add(row);
                }
                else
                {
                    _ = row.UpdateFromAsync(dto);
                }
            }

            TickerItems.Clear();
            foreach (var key in TickerSymbols)
                if (_index.TryGetValue(key, out var rr)) TickerItems.Add(rr);
        });
    }

    public void Stop() => _cts?.Cancel();
}
