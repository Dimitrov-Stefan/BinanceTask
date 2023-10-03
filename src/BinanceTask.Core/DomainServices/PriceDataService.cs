using Binance.Net.Clients;
using BinanceTask.Core.Entities;
using BinanceTask.Core.Helpers;
using BinanceTask.Core.Interfaces.DataAccess;
using BinanceTask.Core.Interfaces.DomainServices;
using System.Diagnostics;

namespace BinanceTask.Core.DomainServices;

/// <summary>
/// Represents a service class for manupuating price data.
/// </summary>
public class PriceDataService : IPriceDataService
{
    private readonly IRepository<PriceData> _priceDataRepository;

    public PriceDataService(IRepository<PriceData> priceDataRepository)
    {
        _priceDataRepository = priceDataRepository;
    }

    /// <inheritdoc />
    public async Task<decimal?> GetAveragePriceForPeriodAsync(string symbol, DateTime fromDate, DateTime toDate)
    {
        // TODO: This can be in a separate repository with the ListAsync method overriden.
        var priceData = await _priceDataRepository.ListAsync(pd => pd.Symbol == symbol && pd.EventTime >= fromDate && pd.EventTime <= toDate);

        if (priceData == null || !priceData.Any())
        {
            return null;
        }

        return priceData.Average(pd => pd.Price);
    }

    /// <inheritdoc />
    public async Task<decimal?> GetSimpleMovingAverageAsync(string symbol, int numberOfDataPoints, string timePeriod, DateTime? startDateTime)
    {
        var priceData = await _priceDataRepository.ListAsync(pd => pd.Symbol == symbol && (!startDateTime.HasValue || startDateTime >= pd.EventTime), numberOfDataPoints);

        if (!priceData.Any())
        {
            return null;
        }

        int GetTimePeriod(string timePeriod) => timePeriod switch
        {
            "1w" => 10_080,
            "1d" => 1440,
            "30m" => 30,
            "5m" => 5,
            "1m" => 1,
            _ => throw new ArgumentException("Invalid period.")
            
            // TODO: The service should return some kind of DTO instead of throwing exceptions. Then the controllers should return the appropriate status code.
        };

        int period = GetTimePeriod(timePeriod); // Time period in minutes for SMA calculation

        decimal? currentSMA = 0m;
        var sma = new SimpleMovingAverageHelper(period, numberOfDataPoints);

        foreach (var record in priceData)
        {
            var priceDataPrice = sma.AddDataPoint(record.EventTime, record.Price);

            if (priceData == null)
            {
                return null;
            }

            currentSMA = sma.AddDataPoint(record.EventTime, record.Price);
        }

        return currentSMA;
    }

    /// <inheritdoc />
    public async Task AddPriceDataAsync(IEnumerable<PriceData> data)
    {
        await _priceDataRepository.AddRangeAsync(data);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PriceData>> CollectData(List<string> symbols, int batchSize = 10)
    {
        List<PriceData> priceDataBatch = new();
        int counter = 0;

        var socketClient = new BinanceSocketClient();

        foreach (var symbol in symbols)
        {
            var subscription = await socketClient.SpotApi.ExchangeData.SubscribeToTradeUpdatesAsync(symbol, data =>
            {
                priceDataBatch.Add(new PriceData { Symbol = symbol, Price = data.Data.Price, EventTime = data.Data.EventTime });
                counter++;
            });

            if (!subscription.Success)
            {
                // TODO: Error log here

                throw new Exception($"Subscription failed. Reason: {subscription.Error}");
            }

            subscription.Data.ConnectionLost += () => Debug.WriteLine("Connection lost, trying to reconnect..");
            subscription.Data.ConnectionRestored += (t) => Debug.WriteLine("Connection restored");
        }


        if (counter >= batchSize)
        {
            counter = 0;

            //await socketClient.UnsubscribeAllAsync();
        }

        return priceDataBatch;
    }
}