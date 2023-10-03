using BinanceTask.Core.Entities;

namespace BinanceTask.Core.Interfaces.DomainServices;

/// <summary>
/// Represents a service interface for manupuating price data.
/// </summary>
public interface IPriceDataService
{
    /// <summary>
    /// Gets the average price asynchronously.
    /// </summary>
    /// <param name="symbol">The event symbol.</param>
    /// <param name="fromDate">The from date.</param>
    /// <param name="toDate">The to date.</param>
    /// <returns>The average of the all symbol prices in the selected period.</returns>
    Task<decimal?> GetAveragePriceForPeriodAsync(string symbol, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Adds price data entries asynchronously.
    /// </summary>
    /// <param name="data">The price data entries.</param>
    /// <returns>A task.</returns>
    Task AddPriceDataAsync(IEnumerable<PriceData> data);

    /// <summary>
    /// Collects price data for a list of symbols asynchronously.
    /// </summary>
    /// <param name="symbols">The symbols list to collect data for.</param>
    /// <param name="batchSize">The batch size of the inserted price data entries.</param>
    /// <returns>The collected price data as a list.</returns>
    Task<IEnumerable<PriceData>> CollectData(List<string> symbols, int batchSize = 10);

    /// <summary>
    /// Gets the simple moving average of prices based on parameters asynchronously.
    /// </summary>
    /// <param name="symbol">The event symbol.</param>
    /// <param name="numberOfDataPoints">The number of data points.</param>
    /// <param name="timePeriod">The time period as a string. Possible values 1w, 1d, 30m, 5m, 1m.</param>
    /// <param name="startDateTime">The date from which we want to do the calculations.</param>
    /// <returns>The simple moving average of prices.</returns>
    Task<decimal?> GetSimpleMovingAverageAsync(string symbol, int numberOfDataPoints, string timePeriod, DateTime? startDateTime);
}
