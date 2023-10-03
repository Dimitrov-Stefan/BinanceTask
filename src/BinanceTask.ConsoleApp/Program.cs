using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BinanceTask.Infrastructure.Data;
using BinanceTask.Core.Interfaces.DomainServices;
using BinanceTask.Core.DomainServices;
using BinanceTask.Core.Interfaces.DataAccess;
using BinanceTask.Core.Entities;
using BinanceTask.Infrastructure.Repositories;
using BinanceTask.DataCollector;
using System.Globalization;

HostApplicationBuilder builder = new HostApplicationBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
builder.Services.AddScoped<IRepository<PriceData>, Repository<PriceData>>();
builder.Services.AddScoped<IPriceDataService, PriceDataService>();

IHost host = builder.Build();

await RunCode();

host.Run();

async Task RunCode()
{
    Console.WriteLine("Availavle commands:");
    Console.WriteLine();
    Console.WriteLine("24h {symbol} - Returns the average price for the last 24h of data in the database ( or the oldest available, if 24h of data is not available ).");
    Console.WriteLine("\targuments:");
    Console.WriteLine("\t\t{symbol} - The symbol the average price is being calculated for.");
    Console.WriteLine();
    Console.WriteLine("sma {symbol} {n} {p} {s} - Returns the current Simple Moving average of the symbol's price.");
    Console.WriteLine("\targuments:");
    Console.WriteLine("\t\t{symbol} - The symbol the average price is being calculated for.\r\n\t\t{n} - The amount of data points\r\n\t\t{p} - The time period represented by each data point. Acceptable values: 1w, 1d, 30m, 5m, 1m.\r\n\t\t{s} - The datetime from which to start the SMA calculation ( a date ).");
    Console.WriteLine();


    while (true)
    {
        Console.Write("Enter a command (e.g., '24h BTC' or 'sma BTC 10 7 2023-01-01'): ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input. Please enter a valid command.");
            continue;
        }

        string[] commandParts = input.Split(' ');

        if (commandParts.Length < 2)
        {
            Console.WriteLine("Invalid input. Please enter a valid command.");
            continue;
        }

        string command = commandParts[0].Trim().ToLower();
        string symbol = commandParts[1].Trim();

        switch (command)
        {
            case "24h" when commandParts.Length == 2:
                Console.WriteLine($"The average price for 24h is: {await GetAveragePriceFor24h(symbol)}");
                break;

            case "sma" when commandParts.Length == 6
            && int.TryParse(commandParts[2], out int n)
            && commandParts[3] is "1w" or "1d" or "30m" or "5m" or "1m"

            && DateTime.TryParseExact(commandParts[4] + " " + commandParts[5], "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime s):
                Console.WriteLine($"The simple moving average is: {await GetSimpleMovingAverageAsync(symbol, n, commandParts[3], s)}");
                
                break;

            default:
                Console.WriteLine("Invalid command. Please enter a valid command.");
                break;
        }
    }

    async Task<string> GetAveragePriceFor24h(string symbol)
    {
        using var serviceScope = host.Services.CreateScope();
        var services = serviceScope.ServiceProvider;

        var priceDataService = services.GetRequiredService<IPriceDataService>();

        DateTime now = DateTime.UtcNow;
        DateTime past24Hours = now.AddHours(-24);
        var avereagePrice24h = await priceDataService.GetAveragePriceForPeriodAsync(symbol, past24Hours, now);

        return avereagePrice24h != null ? avereagePrice24h.Value.ToString() : "null";
    }

    async Task<string> GetSimpleMovingAverageAsync(string symbol, int numberOfDataPoints, string timePeriod, DateTime? startDateTime)
    {
        using var serviceScope = host.Services.CreateScope();
        var services = serviceScope.ServiceProvider;

        var priceDataService = services.GetRequiredService<IPriceDataService>();

        var sma = await priceDataService.GetSimpleMovingAverageAsync(symbol, numberOfDataPoints, timePeriod, startDateTime);

        return sma != null ? sma.Value.ToString() : "null";
    }
}