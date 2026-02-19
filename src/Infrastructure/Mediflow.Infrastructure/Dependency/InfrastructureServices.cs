using System.Text;
using Mediflow.Helper;
using Mediflow.Domain.Common;
using Microsoft.Extensions.Logging;
using Mediflow.Application.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mediflow.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Mediflow.Infrastructure.Persistence;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Common.Permission;
using Mediflow.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Mediflow.Infrastructure.Implementation.Services;

namespace Mediflow.Infrastructure.Dependency;

public static class InfrastructureServices
{
    public static void AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        #region Settings Initialization and Binding
        var databaseSettings = new DatabaseSettings();

        configuration.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);

        var jwtSettings = new JwtSettings();
        
        configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
        #endregion

        #region Setup of Database Context
        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;
        
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetService<ApplicationDbContext>() ?? throw new NotFoundException("Invalid Database Context."));
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseDatabase(databaseSettings.DbProvider, connectionString!);
        });
        #endregion

        #region Registration of Services
        services.AddHttpClient();

        services.AddHttpContextAccessor();
        
        services.AddDistributedMemoryCache();
        #endregion

        #region Handler of Database Migration 
        EnsureDatabaseMigrated(services);
        #endregion

        #region Authentication and Authorization
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.Audience = jwtSettings.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        return !context.Response.HasStarted 
                            ? throw new UnauthorizedException("Authentication failed, please try again.")
                            : Task.CompletedTask;
                    },
                    OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource.")
                };
            });
    
        services.AddAuthorization();
        #endregion

        #region Registration of Permission Services
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        #endregion

        #region Configuration of Settings
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        services.Configure<SeedSettings>(configuration.GetSection(nameof(SeedSettings)));

        services.Configure<SmtpSettings>(configuration.GetSection(nameof(SmtpSettings)));

        services.Configure<ClientSettings>(configuration.GetSection(nameof(ClientSettings)));

        services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
        #endregion

        #region CORS Configuration
        services.EnableCors(configuration);
        #endregion"

        #region Registration of Background Hosted Services
        services.AddHostedService<EmailOutboxService>();
        #endregion

        #region Registration of Email Outbox Service
        // TODO: The following service is only manually registered as it supports both background hosted services and REST API endpoints.
        // TODO: Should we define a new IHostedService interface for background hosted services and register it automatically?.
        // TODO: We might as well separate the hosted background services from the REST API exposed services.
        services.AddTransient<IEmailOutboxService, EmailOutboxService>();
        #endregion
    }
    
    private static void EnsureDatabaseMigrated(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Pending migrations found: {string.Join(", ", pendingMigrations)}.");
                
                logger.LogInformation("Applying pending migrations...");

                dbContext.Database.Migrate();

                logger.LogInformation("Database migration completed successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found. Database is up to date.");
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred while migrating the database.");
            throw;
        }
    }

    private static void EnableCors(this IServiceCollection services, IConfiguration configuration)
    {
        var logger = LoggerFactory.Create(x => x.AddConsole()).CreateLogger(nameof(InfrastructureServices));

        var clientSettings = new ClientSettings();

        configuration.GetSection(nameof(ClientSettings)).Bind(clientSettings);

        var baseUrls = clientSettings.BaseUrl.Split(";");

        foreach (var baseUrl in baseUrls)
        {
            logger.LogInformation("CORS Allowed Environment: {BaseUrl}.", baseUrl);
        }

        services.AddCors(options =>
        {
            options.AddPolicy(Constants.Cors.MyAllowSpecificOrigins,
                builder =>
                {
                    builder
                        .WithOrigins(baseUrls)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
    }
}