using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BinanceTask.Infrastructure.Data;
using BinanceTask.Core.Entities;
using Binance.Net.Clients;

HostApplicationBuilder builder = new HostApplicationBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

IHost host = builder.Build();

await RunCode();

host.Run();


Console.ReadLine();


async Task RunCode()
{
    var socketClient = new BinanceSocketClient();

    await CollectPriceDataAsync(socketClient);

    Console.WriteLine("Enter x to cancel subscriptions.");

    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.X)
    {
        await socketClient.UnsubscribeAllAsync();
    }
}

async Task CollectPriceDataAsync(BinanceSocketClient socketClient)
{
    using var serviceScope = host.Services.CreateScope();
    var services = serviceScope.ServiceProvider;

    using var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    List<PriceData> priceDataBatch = new();

    foreach (var symbol in new List<string> { "BTCUSDT", "ADAUSDT", "ETHUSDT" })
    {
        var subscription = await socketClient.SpotApi.ExchangeData.SubscribeToTradeUpdatesAsync(symbol, data =>
        {
            using var serviceScope = host.Services.CreateScope();
            var services = serviceScope.ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            dbContext.AddAsync(new PriceData { Symbol = symbol, Price = data.Data.Price, EventTime = data.Data.EventTime });
            dbContext.SaveChanges();
        });

        if (!subscription.Success)
        {
            // TODO: Error log here

            throw new Exception($"Subscription failed. Reason: {subscription.Error}");
        }
    }
}