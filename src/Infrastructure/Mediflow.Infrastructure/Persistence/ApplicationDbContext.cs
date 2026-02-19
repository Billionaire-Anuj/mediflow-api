using System.Data;
using Mediflow.Helper;
using System.Reflection;
using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Settings;
using Microsoft.EntityFrameworkCore;
using Mediflow.Domain.Entities.Audits;
using Mediflow.Application.Common.User;
using Microsoft.Extensions.Configuration;
using Mediflow.Application.Interfaces.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mediflow.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IApplicationUserService? applicationUserService = null) : DbContext(options), IApplicationDbContext
{
    #region User & Role Management with Permission Module
    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Resource> Resources { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<UserLoginLog> UserLoginLogs { get; set; }

    public DbSet<UserProperty> UserProperties { get; set; }
    #endregion

    #region Appointments
    public DbSet<Appointment> Appointments { get; set; }

    public DbSet<AppointmentDiagnostics> AppointmentDiagnostics { get; set; }

    public DbSet<AppointmentDiagnosticTestResult> AppointmentDiagnosticTestResults { get; set; }

    public DbSet<AppointmentDiagnosticTests> AppointmentDiagnosticTests { get; set; }

    public DbSet<AppointmentMedicationDrugs> AppointmentMedicationDrugs { get; set; }

    public DbSet<AppointmentMedications> AppointmentMedications { get; set; }

    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    #endregion

    #region Doctor Information
    public DbSet<DoctorProfile> DoctorProfiles { get; set; }

    public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }

    public DbSet<Schedule> Schedules { get; set; }

    public DbSet<Timeslot> Timeslots { get; set; }
    #endregion

    #region Patient Information
    public DbSet<PatientCredit> PatientCredits { get; set; }
    #endregion

    #region Core Data
    public DbSet<DiagnosticType> DiagnosticTypes { get; set; }

    public DbSet<DiagnosticTest> DiagnosticTests { get; set; }

    public DbSet<MedicationType> MedicationTypes { get; set; }

    public DbSet<Medicine> Medicines { get; set; }

    public DbSet<Specialization> Specializations { get; set; }
    #endregion

    #region Gloal Settings
    public DbSet<Property> Properties { get; set; }

    public DbSet<EmailOutbox> EmailOutboxes { get; set; }
    #endregion

    #region Audit Logs
    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<AuditLogHistory> AuditLogHistories { get; set; }
    #endregion

    #region Functions
    public override int SaveChanges()
    {
        UpdateLogs();
    
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // UpdateLogs();
    
        return await base.SaveChangesAsync(cancellationToken);
    }
    #endregion
    
    #region Properties
    public IDbConnection Connection => Database.GetDbConnection();
    #endregion

    #region Configurations
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var basePath = AppContext.BaseDirectory;

        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"The directory '{basePath}' does not exist.");
        }

        var configurationDirectory = Path.Combine(AppContext.BaseDirectory, "Configurations");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(configurationDirectory, "database.json"), true, true)
            .AddJsonFile(Path.Combine(configurationDirectory, $"database.{environmentName}.json"), optional: true, reloadOnChange: true)
            .Build();

        var databaseSettings = new DatabaseSettings();

        configuration.GetSection(nameof(DatabaseSettings)).Bind(databaseSettings);

        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;

        optionsBuilder = optionsBuilder.UseDatabase(databaseSettings.DbProvider, connectionString!);

        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        #region Date Time Conversions
        var dateTimePeriod = new ValueConverter<DateTime, DateTime>(
            toProvider   => toProvider.ToUniversalTime(),
            fromProvider => fromProvider.ToLocalTime()
        );

        var dateTimeNullablePeriod = new ValueConverter<DateTime?, DateTime?>(
            toProvider   => toProvider.HasValue ? toProvider.Value.ToUniversalTime() : toProvider,
            fromProvider => fromProvider.HasValue ? fromProvider.Value.ToLocalTime() : fromProvider
        );

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimePeriod);

                if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(dateTimeNullablePeriod);
            }
        }

        if (!Database.IsNpgsql()) return;
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        property.SetColumnType("timestamp with time zone");
                }
            }
        }
        #endregion
    }
    #endregion
    
    #region Logs and Data
    private void UpdateLogs()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (entries.Count == 0) return;
    
        bool IsUserType(Type type) => type == typeof(User);
        bool IsRoleType(Type type) => type == typeof(Role);
        bool IsPropertyType(Type type) => type == typeof(Property);
        bool IsResourceType(Type type) => type == typeof(Resource);
        bool IsLoginType(Type type) => type == typeof(UserLoginLog);
        bool IsPermissionType(Type type) => type == typeof(Permission);
        bool IsEmailOutboxType(Type type) => type == typeof(EmailOutbox);
        bool IsUserLoginLogType(Type type) => type == typeof(UserLoginLog);
        bool IsUserConfigurationType(Type type) => type == typeof(UserProperty);
        bool IsAuditType(Type type) => type == typeof(AuditLog) || type == typeof(AuditLogHistory);

        // Used During the First Migration of Location Data Seed. 
        // bool IsLocationType(Type type) => type == typeof(Province) || type == typeof(District) || type == typeof(Locality) || type == typeof(Ward);

        var userId = applicationUserService?.GetUserId;
    
        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType();

            StampEntity(entry.Entity, entry.State, userId ?? Guid.Empty, DateTime.Now);

            if (entry.State == EntityState.Added) continue;

            if (IsUserType(entityType)) continue;
            if (IsRoleType(entityType)) continue;
            if (IsAuditType(entityType)) continue;
            if (IsLoginType(entityType)) continue;
            if (IsPropertyType(entityType)) continue;
            if (IsResourceType(entityType)) continue;
            if (IsPermissionType(entityType)) continue;
            if (IsEmailOutboxType(entityType)) continue;
            if (IsUserLoginLogType(entityType)) continue;
            if (IsUserConfigurationType(entityType)) continue;

            var (entityName, entityId) = GetEntityNameAndId(entry);

            var changeType = entry.State switch
            {
                EntityState.Added => ChangeType.Created,
                EntityState.Modified => ChangeType.Updated,
                EntityState.Deleted => ChangeType.Deleted,
                _ => ChangeType.Updated
            };

            var auditLog = new AuditLog(entityName, entityId, changeType, null, false, userId)
            {
                AuditLogHistories = new List<AuditLogHistory>()
            };

            foreach (var propertyEntry in entry.Properties)
            {
                if (propertyEntry.Metadata.IsShadowProperty()) continue;
                if (propertyEntry.Metadata.IsConcurrencyToken) continue;
                if (IgnoredProps.Contains(propertyEntry.Metadata.Name)) continue;

                // Skipping of Status History Updates for Candidate Entity.
                if (entityName == "Candidate" && propertyEntry.Metadata.Name is "Status" or "CompanyId") continue;

                var shouldAddDetail = entry.State switch
                {
                    // TODO: Verification if Added History is Required or Not.
                    EntityState.Added   => false,
                    EntityState.Deleted => true,
                    EntityState.Modified => propertyEntry.IsModified && !Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue),
                    _ => false
                };

                if (!shouldAddDetail) continue;

                var (oldValue, newValue) = entry.State switch
                {
                    EntityState.Added   => (null, propertyEntry.CurrentValue),
                    EntityState.Deleted => (propertyEntry.OriginalValue, null),
                    EntityState.Modified => (propertyEntry.OriginalValue, propertyEntry.CurrentValue),
                    _ => (null, null)
                };

                var auditLogHistory = new AuditLogHistory(
                    auditLog.Id,
                    propertyEntry.Metadata.Name.ToDisplayName(propertyEntry.Metadata.ClrType),
                    MapFieldType(propertyEntry.Metadata.ClrType),
                    Serialize(oldValue),
                    Serialize(newValue));

                auditLog.AuditLogHistories.Add(auditLogHistory);
            }

            if (auditLog is { ChangeType: ChangeType.Updated, AuditLogHistories.Count: > 0 })
            {
                AuditLogs.Add(auditLog);
            }
        }
    }
    #endregion

    #region Change Log Interceptor
    private static readonly HashSet<string> IgnoredProps = new(StringComparer.OrdinalIgnoreCase)
    {
        "CreatedBy", "CreatedAt", "LastModifiedBy", "LastModifiedAt", "DeletedBy", "DeletedAt"
    };

    private static void StampEntity(object entity, EntityState state, Guid userId, DateTime dateTime)
    {
        switch (entity)
        {
            case AuditableEntity<Guid> baseEntity when state == EntityState.Added:
                baseEntity.CreatedAt = dateTime;
                if (baseEntity.CreatedBy == Guid.Empty) baseEntity.CreatedBy = userId; break;
    
            case AuditableEntity<Guid> baseEntity when state == EntityState.Modified:
                baseEntity.LastModifiedAt = dateTime;
                baseEntity.LastModifiedBy ??= userId;
                break;
    
            case AuditableEntity<Guid> baseEntity when state == EntityState.Deleted:
                baseEntity.DeletedAt = dateTime; 
                baseEntity.DeletedBy ??= userId; break;
    
            case AuditableEntity<string> baseEntity when state == EntityState.Added:
                baseEntity.CreatedAt = dateTime; 
                if (baseEntity.CreatedBy == Guid.Empty) baseEntity.CreatedBy = userId; break;
    
            case AuditableEntity<string> baseEntity when state == EntityState.Modified:
                baseEntity.LastModifiedAt = dateTime; 
                baseEntity.LastModifiedBy ??= userId; break;
    
            case AuditableEntity<string> baseEntity when state == EntityState.Deleted:
                baseEntity.DeletedAt = dateTime; 
                baseEntity.DeletedBy ??= userId; break;
        }
    }

    // TODO: Handle Internal and FK Navigation References.
    private static (string entityName, string entityId) GetEntityNameAndId(EntityEntry entry)
    {
        var name = entry.Entity.GetType().Name;
        var key = entry.Metadata.FindPrimaryKey()!;
        var keyProp = key.Properties.Single();
        var value = entry.Property(keyProp.Name).CurrentValue ?? entry.Property(keyProp.Name).OriginalValue;
        return (name, value?.ToString() ?? string.Empty);
    }

    private static FieldDataType MapFieldType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type == typeof(string)) return FieldDataType.STRING;
        if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return FieldDataType.INTEGER;
        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) return FieldDataType.DECIMAL;
        if (type == typeof(bool)) return FieldDataType.BOOLEAN;
        if (type == typeof(DateOnly)) return FieldDataType.DATE_ONLY;
        if (type == typeof(DateTime)) return FieldDataType.DATE_TIME;
        if (type == typeof(Guid)) return FieldDataType.GUID;

        return FieldDataType.OTHER;
    }

    private static string? Serialize(object? value)
    {
        if (value is null) return null;

        return value switch
        {
            DateOnly date => date.ToString("yyyy-MM-dd"),
            DateTime dateTime => dateTime.ToUniversalTime().ToString("O"),
            decimal @decimal => @decimal.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
            bool boolean => boolean ? "TRUE" : "FALSE",
            _ => value.ToString()?.ToDisplayName()
        };
    }
    #endregion
}