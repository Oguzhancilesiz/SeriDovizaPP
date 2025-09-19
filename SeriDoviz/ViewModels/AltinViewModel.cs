using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel; // MainThread
using SeriDoviz.Models;
using SeriDoviz.Services;
using System.Collections.ObjectModel;

namespace SeriDoviz.ViewModels;

public partial class AltinViewModel : BaseViewModel
{
    private readonly IRatesService _service;
    private CancellationTokenSource? _cts;

    public ObservableCollection<RateRow> Items { get; } = new();
    public ObservableCollection<RateRow> HeaderItems { get; } = new();

    private readonly Dictionary<string, RateRow> _index = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string[] HeaderSymbols = { "ALTIN/GRAM", "AYAR22/GRAM", "AYAR14/GRAM", "XAU/USD" };

    public AltinViewModel(IRatesService service)
    {
        Title = "Altın";
        _service = service;
    }

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

            _index.Clear(); Items.Clear(); HeaderItems.Clear();

            var first = await _service.GetGoldAsync() ?? Array.Empty<RateDto>();
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
            await foreach (var batch in _service.StreamGoldAsync(ct))
                ApplyBatchOnUi(batch ?? Array.Empty<RateDto>());
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

            HeaderItems.Clear();
            foreach (var s in HeaderSymbols)
                if (_index.TryGetValue(s, out var rr)) HeaderItems.Add(rr);
        });
    }

    public void Stop() { try { _cts?.Cancel(); } catch { } }
}
