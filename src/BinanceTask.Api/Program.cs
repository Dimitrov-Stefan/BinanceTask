using BinanceTask.Core.DomainServices;
using BinanceTask.Core.Entities;
using BinanceTask.Core.Interfaces.DataAccess;
using BinanceTask.Core.Interfaces.DomainServices;
using BinanceTask.Infrastructure.Data;
using BinanceTask.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRepository<PriceData>, Repository<PriceData>>();
builder.Services.AddScoped<IPriceDataService, PriceDataService>();

builder.Services.AddControllers(options => 
    options.RespectBrowserAcceptHeader = true).AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// TODO: Use caching where needed.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
});

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{

    var services = serviceScope.ServiceProvider;

    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();


    applicationDbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
