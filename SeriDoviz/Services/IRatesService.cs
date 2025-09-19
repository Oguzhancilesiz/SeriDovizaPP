using SeriDoviz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriDoviz.Services
{
    public interface IRatesService
    {
        Task<IReadOnlyList<RateDto>> GetCurrenciesAsync();
        Task<IReadOnlyList<RateDto>> GetGoldAsync();
        IAsyncEnumerable<IReadOnlyList<RateDto>> StreamCurrenciesAsync(CancellationToken ct);
        IAsyncEnumerable<IReadOnlyList<RateDto>> StreamGoldAsync(CancellationToken ct);
    }
}
