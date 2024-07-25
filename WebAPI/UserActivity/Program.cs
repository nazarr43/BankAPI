using Confluent.Kafka;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.IO;
using UserActivity.Application.Configuration;
using UserActivity.Application.Interfaces;
using UserActivity.Application.Services;
using UserActivity.Infrastructure;
using UserActivity.Infrastructure.Repository;
using UserActivity.Infrastructure.Extensions;
using StackExchange.Redis;
using FluentAssertions.Common;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;
using DeviceDetectorNET.Parser.Device;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Extensions.Hosting;
using Prometheus;
using static IdentityModel.ClaimComparer;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddOpenTelemetry().WithMetrics(opts => opts
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("dotnet_project"))
    .AddAspNetCoreInstrumentation()
    .AddRuntimeInstrumentation()
    .AddHttpClientInstrumentation()
    .AddPrometheusExporter());

var metricServer = new MetricServer(port: 7278); 
metricServer.Start();
var customMetric = Metrics.CreateCounter("custom_metric_total", "Total number of custom metric events.");


builder.Services.AddScoped<IAuthCountService, AuthCountService>();
builder.Services.Configure<MongoDBSettings>(configuration.GetSection("MongoDBSettings"));
builder.Services.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));
builder.Services.Configure<UserInfoServiceOptions>(configuration.GetSection("UserInfoService"));
builder.Services.AddHttpClient<IUserInfoService, UserInfoService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<UserInfoServiceOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseAddress);
});
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(mongoDbSettings.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbSettings.DatabaseName);
});

builder.Services.AddScoped<ILoginEventRepository>(sp =>
{
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(mongoDbSettings.DatabaseName);
    return new LoginEventRepository(database, sp.GetRequiredService<IOptions<MongoDBSettings>>());
});

builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
    var config = new ProducerConfig { BootstrapServers = kafkaSettings.BootstrapServers };
    return new ProducerBuilder<string, string>(config).Build();
});

builder.Services.AddKafkaConsumer<string, string>(config =>
{
    config.AutoOffsetReset = AutoOffsetReset.Latest;
});

builder.Services.AddHostedService<LoginEventConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/api/useractivity/AuthCount/logins/count")
    {
        customMetric.Inc();
    }
    await next.Invoke();
});


app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
