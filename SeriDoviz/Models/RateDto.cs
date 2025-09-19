namespace SeriDoviz.Models;

// change bazen null
public sealed record RateDto(
    string Symbol,
    decimal? Bid,
    decimal? Ask,
    decimal? Change,
    string Dir,
    string? Title = null
);
