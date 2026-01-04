using mediflow.Entities;
using Microsoft.EntityFrameworkCore;

namespace mediflow.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<LabResult> LabResults => Set<LabResult>();
    public DbSet<PendingPointPurchases> PendingPointPurchases => Set<PendingPointPurchases>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.User)
            .WithOne(u => u.Doctor)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.UserId)
            .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.User)
            .WithOne(u => u.Patient)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        modelBuilder.Entity<Schedule>()
            .HasIndex(s => new { s.DoctorId, s.WorkDate })
            .IsUnique();

        modelBuilder.Entity<TimeSlot>()
            .HasIndex(t => new { t.ScheduleId, t.StartTime, t.EndTime })
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.PatientId, a.TimeSlotId })
            .IsUnique();

        modelBuilder.Entity<LabResult>(entity =>
        {
            // Patient (required)
            entity.HasOne(x => x.Patient)
                .WithMany(p => p.LabResults)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // MedicalRecord (optional)
            entity.HasOne(x => x.MedicalRecord)
                .WithMany(m => m.LabResults)
                .HasForeignKey(x => x.MedicalRecordId)
                .OnDelete(DeleteBehavior.SetNull);

            // OrderedByDoctor (optional)
            entity.HasOne(x => x.OrderedByDoctor)
                .WithMany(d => d.LabResultsOrdered)
                .HasForeignKey(x => x.OrderedByDoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // ReviewedByDoctor (optional)
            entity.HasOne(x => x.ReviewedByDoctor)
                .WithMany(d => d.LabResultsReviewed)
                .HasForeignKey(x => x.ReviewedByDoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        base.OnModelCreating(modelBuilder);
    }
}