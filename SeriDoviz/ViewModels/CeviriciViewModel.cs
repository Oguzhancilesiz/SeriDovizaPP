using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SeriDoviz.Models;
using SeriDoviz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriDoviz.ViewModels
{
    public partial class CeviriciViewModel : BaseViewModel
    {
        private readonly IRatesService _service;

        public ObservableCollection<string> Symbols { get; } = new();

        [ObservableProperty] private string from = "USD";
        [ObservableProperty] private string to = "TRY";
        [ObservableProperty] private decimal amount = 1m;
        [ObservableProperty] private decimal result;

        public CeviriciViewModel(IRatesService service)
        {
            Title = "Çevirici";
            _service = service;
        }

        [RelayCommand]
        public async Task InitAsync()
        {
            var list = await _service.GetCurrenciesAsync() ?? Array.Empty<RateDto>();
            Symbols.Clear();
            foreach (var s in list.Select(x => x.Symbol.Split('/')[0]).Distinct().OrderBy(x => x))
                Symbols.Add(s);
            if (!Symbols.Contains("TRY")) Symbols.Add("TRY");
        }

        [RelayCommand]
        public void Swap()
        {
            (From, To) = (To, From);
            _ = CalculateAsync();
        }

        [RelayCommand]
        public async Task CalculateAsync()
        {
            var all = await _service.GetCurrenciesAsync() ?? Array.Empty<RateDto>();

            decimal? ToTry(string sym)
            {
                var hit = all.FirstOrDefault(x =>
                    x.Symbol?.StartsWith(sym, StringComparison.OrdinalIgnoreCase) == true &&
                    x.Symbol.EndsWith("TRY", StringComparison.OrdinalIgnoreCase));
                return hit?.Ask ?? hit?.Bid;
            }

            if (From == To) { Result = Amount; return; }

            var f = From == "TRY" ? 1m : ToTry(From) ?? 0m;
            var t = To == "TRY" ? 1m : ToTry(To) ?? 0m;

            if (f <= 0 || t <= 0) { Result = 0; return; }

            var tryAmount = From == "TRY" ? Amount : Amount * f;
            Result = To == "TRY" ? tryAmount : tryAmount / t;
        }
    }
}
