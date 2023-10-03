namespace BinanceTask.Core.Helpers
{
    /// <summary>
    /// Represents a class for calculating a Simple Moving Average (SMA) for a series of data points.
    /// </summary>
    public class SimpleMovingAverageHelper
    {
        private readonly int period;
        private int numberOfDataPoints;
        private readonly Queue<Tuple<DateTime, decimal>> dataQueue;
        private decimal sum;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMovingAverageHelper"/> class with the specified period and number of data points.
        /// </summary>
        /// <param name="period">The time period (in minutes) used for calculating the SMA.</param>
        /// <param name="numberOfDataPoints">The number of data points to consider for the SMA calculation.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="period"/> or <paramref name="numberOfDataPoints"/> is less than or equal to zero.</exception>
        public SimpleMovingAverageHelper(int period, int numberOfDataPoints)
        {
            if (period <= 0 || numberOfDataPoints <= 0)
            {
                throw new ArgumentException("Period and number of data points must be greater than zero.");
            }

            this.period = period;
            this.numberOfDataPoints = numberOfDataPoints;
            this.dataQueue = new Queue<Tuple<DateTime, decimal>>(numberOfDataPoints);
            this.sum = 0;
        }

        /// <summary>
        /// Adds a data point with a timestamp to the SMA calculation.
        /// </summary>
        /// <param name="timestamp">The timestamp of the data point.</param>
        /// <param name="dataPoint">The value of the data point.</param>
        /// <returns>
        /// The current SMA if enough data points are available; otherwise, returns <c>null</c>.
        /// </returns>
        public decimal? AddDataPoint(DateTime timestamp, decimal dataPoint)
        {
            dataQueue.Enqueue(new Tuple<DateTime, decimal>(timestamp, dataPoint));
            sum += dataPoint;

            // Remove data points that are outside of the current period
            while (dataQueue.Count > 0 && (timestamp - dataQueue.Peek().Item1) > TimeSpan.FromMinutes(period))
            {
                var removedData = dataQueue.Dequeue();
                sum -= removedData.Item2;
            }

            // Check if we have enough data points to calculate the SMA
            if (dataQueue.Count < numberOfDataPoints)
            {
                return null;
            }

            return sum / numberOfDataPoints;
        }
    }
}
