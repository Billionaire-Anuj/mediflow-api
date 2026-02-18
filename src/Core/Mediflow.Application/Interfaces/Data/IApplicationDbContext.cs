using System.Data;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Domain.Entities.Audits;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Data;

public interface IApplicationDbContext : IScopedService
{
    #region User & Role Management with Permission Module
    DbSet<User> Users { get; set; }

    DbSet<Role> Roles { get; set; }

    DbSet<Resource> Resources { get; set; }

    DbSet<Permission> Permissions { get; set; }

    DbSet<UserLoginLog> UserLoginLogs { get; set; }

    DbSet<UserConfiguration> UserConfigurations { get; set; }
    #endregion

    #region Gloal Settings
    DbSet<Property> Properties { get; set; }

    DbSet<EmailOutbox> EmailOutboxes { get; set; }
    #endregion

    #region Audit Logs
    DbSet<AuditLog> AuditLogs { get; set; }

    DbSet<AuditLogHistory> AuditLogHistories { get; set; }
    #endregion

    #region Functions
    int SaveChanges();
    #endregion

    #region Properties
    IDbConnection Connection { get; }
    #endregion
}