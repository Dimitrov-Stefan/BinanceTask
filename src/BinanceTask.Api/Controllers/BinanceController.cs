using BinanceTask.Core.DTOs;
using BinanceTask.Core.Interfaces.DomainServices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BinanceTask.Api.Controllers
{
    /// <summary>
    /// The Binance controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BinanceController : ControllerBase
    {
        private readonly IPriceDataService _priceDataService;

        public BinanceController(IPriceDataService priceDataService)
        {
            _priceDataService = priceDataService;
        }

        /// <summary>
        /// Returns the average price for the last 24h of data in the database ( or the oldest available, if 24h of data is not available ).
        /// </summary>
        /// <param name="symbol">The symbol to get the average price for.</param>
        /// <returns>The average price by symbol for a period of 24 hours.</returns>
        [HttpGet]
        [Route("/api/{symbol}/24hAvgPrice")]
        public async Task<decimal?> Get(string symbol)
        {
            DateTime now = DateTime.UtcNow;
            DateTime past24Hours = now.AddHours(-24);

            return await _priceDataService.GetAveragePriceForPeriodAsync(symbol, past24Hours, now);
        }

        /// <summary>
        /// Returns the current Simple Moving average of the symbol's price.
        /// </summary>
        /// <param name="symbol">The symbol to get the average price for.</param>
        /// <param name="n">The amount of data points.</param>
        /// <param name="p">The time period represented by each data point. Acceptable values: 1w, 1d, 30m, 5m, 1m.</param>
        /// <param name="s">The datetime from which to start the SMA calculation ( a date ).</param>
        /// <returns>
        /// The Simple Moving Average of symbols' prices in the database calculated for the given parameters.
        /// </returns>
        [HttpGet]
        [Route("/api/{symbol}/SimpleMovingAverage/")]
        public async Task<decimal?> GetSimpleMovingAverage([FromRoute]string symbol, [FromQuery] int n, [FromQuery] string p, [FromQuery] DateTime s)
        {
            var sma = await _priceDataService.GetSimpleMovingAverageAsync(symbol, n, p, s);
            return sma;
        }

        /// <summary>
        /// The collect price data endpoint is used to collect price data for symbols.
        /// </summary>
        /// <param name="priceDataDto"></param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <remarks>
        /// Sample request:
        /// 
        /// {
        ///     "symbols": ["BTCUSDT", "ADAUSDT", "ETHUSDT"]
        /// }
        /// </remarks>
        [HttpPost]
        [Route("/api/collectPriceData")]
        public async Task CollectPriceData([FromBody] CollectPriceDataDto priceDataDto)
        {
            var data = await _priceDataService.CollectData(priceDataDto.Symbols).ContinueWith(async data => _priceDataService.AddPriceDataAsync(await data));
        }
    }
}
