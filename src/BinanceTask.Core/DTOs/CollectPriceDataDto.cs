namespace BinanceTask.Core.DTOs;

/// <summary>
/// Represents a dto class for collecting price data.
/// </summary>
public class CollectPriceDataDto
{
    public List<string> Symbols { get; set; } = new();
}
