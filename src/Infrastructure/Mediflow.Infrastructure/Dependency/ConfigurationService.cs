using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Mediflow.Infrastructure.Dependency;

public static class ConfigurationService
{
    public static void AddConfigurations(this WebApplicationBuilder builder)
    {
        var logger = LoggerFactory.Create(x => x.AddConsole()).CreateLogger(nameof(ConfigurationService));

        var environment = builder.Environment;
        var root = environment.ContentRootPath;
        var configurationDirectory = Path.Combine(root, "Configurations");

        logger.LogInformation("Environment: {EnvironmentName}", environment.EnvironmentName);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, "database.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"database.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, "seed.json"),optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"seed.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, "jwt.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"jwt.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, "client.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"client.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, "smtp.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"smtp.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, "khalti.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"khalti.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, "esewa.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"esewa.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
    }
}
