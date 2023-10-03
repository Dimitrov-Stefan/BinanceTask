namespace BinanceTask.DataAccess
{
    /// <summary>
    /// Price data of transactions.
    /// </summary>
    public class PriceData
    {
        /// <summary>
        /// The price data identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The symbol of the currencies being traded.
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// The symbol of the currency being traded.
        /// </summary>
        public decimal Price { get; set; }
    }
}