using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Application.DTOs;
using WebAPI.Application.Services;
using WebAPI.Domain.Entities;
using WebAPI.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Application.Interfaces;
using WebAPI.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebAPI.Domain.Constants;
using WebAPI.Infrastructure;
using WebAPI.Presentation;
using System.Text.Json;
using WebAPI.Infrastructure.Repository;
using Microsoft.AspNetCore.DataProtection;
using Confluent.Kafka;
using MongoDB.Driver;
using WebAPI.Application.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddOpenTelemetry().WithMetrics(opts => opts
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("dotnet_project2"))
    .AddAspNetCoreInstrumentation()
    .AddRuntimeInstrumentation()
    .AddHttpClientInstrumentation()
    .AddPrometheusExporter());
builder.Services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    return ConnectionMultiplexer.Connect(settings.ConnectionString);
});

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IRepository<Account>, Repository<Account>>();
builder.Services.AddScoped<IRepository<Transaction>, Repository<Transaction>>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<ICurrencyApiClient, CurrencyApiClient>();
builder.Services.AddHttpClient<CurrencyApiClient>();
builder.Services.Configure<CurrencyServiceOptions>(builder.Configuration.GetSection("CurrencyService"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITransactionStatisticsService, TransactionStatisticsService>();
builder.Services.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));
builder.Services.AddScoped(typeof(IKafkaService<>), typeof(KafkaService<>));
builder.Services.AddScoped<IKafkaEventService, KafkaEventService>();
builder.Services.AddScoped<IUserInfoService, UserInfoService>();
builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICachingService, MemoryCacheService>();

builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
    var config = new ProducerConfig { BootstrapServers = kafkaSettings.BootstrapServers };
    return new ProducerBuilder<string, string>(config).Build();
});

builder.Services.AddScoped(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddHealthChecks();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not defined in the configuration.");
}
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserManagerDecorator<ApplicationUser>, UserManagerDecorator<ApplicationUser>>();
builder.Services.AddScoped<ISignInManagerDecorator<ApplicationUser>, SignInManagerDecorator<ApplicationUser>>();
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateAccountRequestValidator>());

builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenSettings.Issuer,
        ValidAudience = tokenSettings.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}",

        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

builder.Services.AddHealthChecks()
   .AddNpgSql(connectionString, name: "PostgreSQL");

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.OAuthClientId("swagger");
        c.OAuthAppName("Swagger UI");
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();