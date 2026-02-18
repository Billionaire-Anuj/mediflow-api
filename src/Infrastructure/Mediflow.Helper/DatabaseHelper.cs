using Mediflow.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Helper;

public static class DatabaseHelper
{
    public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
    {
        return dbProvider.ToLowerInvariant() switch
        {
            Constants.DbProviderKeys.Npgsql => builder.UseNpgsql(connectionString, e =>
                e.MigrationsAssembly("Mediflow.Migrators.PostgreSQL")),
            Constants.DbProviderKeys.SqlServer => builder.UseSqlServer(connectionString, e =>
                e.MigrationsAssembly("Mediflow.Migrators.SQLServer")),
            _ => throw new InvalidOperationException($"DB Provider {dbProvider} is not supported."),
        };
    }
}