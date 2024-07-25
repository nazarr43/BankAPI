using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReverseProxy;
using System;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var myRateLimitOptions = builder.Configuration.GetSection("MyRateLimitOptions").Get<MyRateLimitOptions>();

var slidingPolicy = "sliding";
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter(policyName: slidingPolicy, options =>
    {
        options.PermitLimit = myRateLimitOptions.PermitLimit;
        options.Window = TimeSpan.FromSeconds(myRateLimitOptions.WindowSeconds);
        options.SegmentsPerWindow = myRateLimitOptions.SegmentsPerWindow;
        options.QueueLimit = myRateLimitOptions.QueueLimit;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapReverseProxy();

static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

app.MapGet("/", () => Results.Ok($"Sliding Window Limiter {GetTicks()}"))
    .RequireRateLimiting(slidingPolicy);

app.Run();
