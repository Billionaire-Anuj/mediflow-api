using Mediflow.Application.Settings;
using Mediflow.Domain.Common;
using Mediflow.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mediflow.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configurationDirectory = ResolveConfigurationDirectory();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(configurationDirectory, "database.json"), optional: false, reloadOnChange: false)
            .AddJsonFile(Path.Combine(configurationDirectory, $"database.{environmentName}.json"), optional: true, reloadOnChange: false)
            .Build();

        var databaseSettings = new DatabaseSettings();
        configuration.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);

        if (string.IsNullOrWhiteSpace(databaseSettings.DbProvider))
        {
            throw new InvalidOperationException("DatabaseSettings.DbProvider is not configured.");
        }

        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"DatabaseSettings does not contain a connection string for provider '{databaseSettings.DbProvider}'.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseDatabase(databaseSettings.DbProvider, connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string ResolveConfigurationDirectory()
    {
        var currentDir = Directory.GetCurrentDirectory();

        var candidate = Path.Combine(currentDir, "Configurations");
        if (File.Exists(Path.Combine(candidate, "database.json")))
        {
            return candidate;
        }

        var fromRepoRoot = FindUpwards(currentDir, Path.Combine("src", "API", "Mediflow.API", "Configurations"));
        if (fromRepoRoot is not null)
        {
            return fromRepoRoot;
        }

        var fromSrcRoot = FindUpwards(currentDir, Path.Combine("API", "Mediflow.API", "Configurations"));
        if (fromSrcRoot is not null)
        {
            return fromSrcRoot;
        }

        candidate = Path.Combine(AppContext.BaseDirectory, "Configurations");
        if (File.Exists(Path.Combine(candidate, "database.json")))
        {
            return candidate;
        }

        throw new DirectoryNotFoundException(
            "Unable to locate database.json. Run the EF command from the repository root or the API project directory.");
    }

    private static string? FindUpwards(string startDirectory, string relativePath)
    {
        var current = new DirectoryInfo(startDirectory);

        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, relativePath);
            if (File.Exists(Path.Combine(candidate, "database.json")))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return null;
    }
}
