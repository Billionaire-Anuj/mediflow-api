using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class DoctorSpecialization(Guid doctorId, Guid specializationId) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Doctor))]
    public Guid DoctorId { get; private set; } = doctorId;

    [ForeignKey(nameof(Specialization))]
    public Guid SpecializationId { get; private set; } = specializationId;

    public virtual User? Doctor { get; set; }

    public virtual Specialization? Specialization { get; set; }
}