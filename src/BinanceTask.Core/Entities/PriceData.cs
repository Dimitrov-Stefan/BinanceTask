using BinanceTask.Core.Entities.Abstract;

namespace BinanceTask.Core.Entities;

/// <summary>
/// Represents the price data entity of events (transactions).
/// </summary>
public class PriceData : EntityBase
{
    /// <summary>
    /// Gets or sets the symbol of the currencies being traded.
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets the price of the currency being traded.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The time of when the event happened.
    /// </summary>
    public DateTime EventTime { get; set; }
}