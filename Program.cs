using Hazelcast;
using Hazelcast.Configuration;
using Hazelcast.Core;
using Hazelcast.DependencyInjection;
using Hazelcast.NearCaching;
using HazelcastDemo;
using HazelcastDemo.Common;
using HazelcastDemo.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
//configuration.AddHazelcast(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

services.Configure<RedisSettings>(configuration.GetSection("Redis"));

services.AddSingleton<IConnectionMultiplexer>(x =>
{
    var settings = x.GetRequiredService<IOptions<RedisSettings>>().Value;
    // ConfigurationOptions configurationOptions = settings.MapToConfigurationOptions();

    return ConnectionMultiplexer.Connect("10.26.7.84:6379,allowAdmin=true");
});

services.AddServices(configuration);


//services.AddDbContext<HazelCastContext>(p => p.UseOracle(configuration.GetConnectionString("OracleDB")));
services.AddHazelcast(builder.Configuration);
services.AddSingleton<OrderingHazelcastContext>();
services.AddSingleton<HazelcastFactory>();

services.AddSingleton<IHazelcastClient>(sp =>
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    var options = sp.GetService<IOptions<HazelcastOptions>>().Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    options.NearCaches.Add("longpv-test-nearcache", new NearCacheOptions()
    {
        MaxSize = 5999999,
        InvalidateOnChange = true,
        EvictionPolicy = EvictionPolicy.Lru,
        InMemoryFormat = InMemoryFormat.Object,
        TimeToLiveSeconds = 60,
        MaxIdleSeconds = 3600,
    });
    return HazelcastClientFactory.StartNewClientAsync(options).Result;
});

var app = builder.Build();

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
