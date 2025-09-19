using Refit;
using SeriDoviz.Models;
using System.Text.RegularExpressions;

namespace SeriDoviz.Services
{
    public sealed class RatesService : IRatesService
    {
        private readonly ISeriDovizApi _api;

        public RatesService(HttpClient http)
        {
            _api = RestService.For<ISeriDovizApi>(http);
        }

        public async Task<IReadOnlyList<RateDto>> GetCurrenciesAsync()
        {
            var all = await SafeLatest();
            return all.Where(IsCurrency).ToList();
        }

        public async Task<IReadOnlyList<RateDto>> GetGoldAsync()
        {
            var all = await SafeLatest();
            return all.Where(IsGold).ToList();
        }

        // === 1 saniyelik akış: Currencies ===
        public async IAsyncEnumerable<IReadOnlyList<RateDto>> StreamCurrenciesAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (!ct.IsCancellationRequested)
            {
                IReadOnlyList<RateDto> batch;
                try { batch = await SafeLatest(); }
                catch { batch = Array.Empty<RateDto>(); }

                yield return batch.Where(IsCurrency).ToList();

                try { await timer.WaitForNextTickAsync(ct); }
                catch (OperationCanceledException) { yield break; }
            }
        }

        // === 1 saniyelik akış: Gold ===
        public async IAsyncEnumerable<IReadOnlyList<RateDto>> StreamGoldAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (!ct.IsCancellationRequested)
            {
                IReadOnlyList<RateDto> batch;
                try { batch = await SafeLatest(); }
                catch { batch = Array.Empty<RateDto>(); }

                yield return batch.Where(IsGold).ToList();

                try { await timer.WaitForNextTickAsync(ct); }
                catch (OperationCanceledException) { yield break; }
            }
        }

        private async Task<IReadOnlyList<RateDto>> SafeLatest()
            => await _api.GetLatestAsync() ?? Array.Empty<RateDto>();

        private static readonly Regex IsoPair = new(@"^[A-Z]{3}/[A-Z]{3}$", RegexOptions.Compiled);
        private static readonly string[] GoldKeys =
        [
            "ALTIN","XAU","AYAR","GRAM","ATA","ÇEYREK","CEYREK","YARIM","TAM","GREMSE","REŞAT","RESAT","ONS"
        ];

        private static bool IsGold(RateDto x)
        {
            var s = (x.Symbol ?? string.Empty).ToUpperInvariant();
            if (GoldKeys.Any(k => s.Contains(k))) return true;
            return !IsoPair.IsMatch(s);
        }

        private static bool IsCurrency(RateDto x) => !IsGold(x);
    }
}
