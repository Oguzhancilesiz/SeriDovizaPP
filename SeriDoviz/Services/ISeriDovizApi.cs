using Refit;
using SeriDoviz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriDoviz.Services
{
    public interface ISeriDovizApi
    {
        [Get("/api/rates/latest")]
        Task<IReadOnlyList<RateDto>> GetLatestAsync();
    }
}
