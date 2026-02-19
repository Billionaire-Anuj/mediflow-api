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

    DbSet<UserProperty> UserProperties { get; set; }
    #endregion

    #region Appointments
    DbSet<Appointment> Appointments { get; set; }

    DbSet<AppointmentDiagnostics> AppointmentDiagnostics { get; set; }

    DbSet<AppointmentDiagnosticTestResult> AppointmentDiagnosticTestResults { get; set; }

    DbSet<AppointmentDiagnosticTests> AppointmentDiagnosticTests { get; set; }

    DbSet<AppointmentMedicationDrugs> AppointmentMedicationDrugs { get; set; }

    DbSet<AppointmentMedications> AppointmentMedications { get; set; }

    DbSet<MedicalRecord> MedicalRecords { get; set; }
    #endregion

    #region Doctor Information
    DbSet<DoctorInformation> DoctorInformation { get; set; }

    DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }

    DbSet<Schedule> Schedules { get; set; }

    DbSet<Timeslot> Timeslot { get; set; }
    #endregion

    #region Patient Information
    DbSet<PatientCredit> PatientCredits { get; set; }
    #endregion

    #region Core Data
    DbSet<DiagnosticType> DiagnosticTypes { get; set; }

    DbSet<DiagnosticTest> DiagnosticTests { get; set; }

    DbSet<MedicationType> MedicationTypes { get; set; }

    DbSet<Medicine> Medicines { get; set; }

    DbSet<Specialization> Specializations { get; set; }
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