using mediflow.Contracts;
using mediflow.Data;
using mediflow.Entities;
using Mediflow.Entities.Enums;
using mediflow.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mediflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DoctorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DoctorsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/doctors?specialty=&name=&availableOn=YYYY-MM-DD
    [HttpGet]
    public async Task<ActionResult<List<DoctorListItemDto>>> GetAll(
        [FromQuery] string? specialty,
        [FromQuery] string? name,
        [FromQuery] DateOnly? availableOn)
    {
        var q = _db.Doctors
            .AsNoTracking()
            .Include(d => d.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(specialty))
            q = q.Where(d => d.Specialty.Contains(specialty));

        if (!string.IsNullOrWhiteSpace(name))
            q = q.Where(d => d.User.FullName.Contains(name));

        // availableOn means: has at least 1 open slot that day (published schedules only)
        if (availableOn.HasValue)
        {
            var day = availableOn.Value;
            q = q.Where(d =>
                d.Schedules.Any(s =>
                    s.WorkDate == day &&
                    s.IsPublished &&
                    s.TimeSlots.Any(ts => ts.Status == TimeSlotStatus.Open && ts.BookedCount < ts.Capacity)
                ));
        }

        var list = await q
            .OrderBy(d => d.User.FullName)
            .Select(d => new DoctorListItemDto(
                d.Id,
                d.UserId,
                d.User.FullName,
                d.User.Email,
                d.User.Phone,
                d.Specialty,
                d.LicenseNumber,
                d.ConsultationFee,
                d.User.ProfilePictureUrl
            ))
            .ToListAsync();

        return Ok(list);
    }

    // GET /api/doctors/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorDetailDto>> GetById(Guid id)
    {
        var doctor = await _db.Doctors
            .AsNoTracking()
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null) return NotFound();

        return Ok(new DoctorDetailDto(
            doctor.Id,
            doctor.UserId,
            doctor.User.FullName,
            doctor.User.Email,
            doctor.User.Phone,
            doctor.Specialty,
            doctor.LicenseNumber,
            doctor.ConsultationFee,
            doctor.Bio,
            doctor.DefaultSlotMinutes,
            doctor.User.ProfilePictureUrl
        ));
    }

    // POST /api/doctors (Admin only) -> creates User + Doctor profile
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDoctorRequest req)
    {
        req = req with
        {
            FullName = (req.FullName ?? "").Trim(),
            Email = (req.Email ?? "").Trim().ToLowerInvariant(),
            Specialty = (req.Specialty ?? "").Trim(),
            LicenseNumber = req.LicenseNumber?.Trim(),
            Bio = req.Bio?.Trim(),
            Phone = req.Phone?.Trim()
        };

        if (string.IsNullOrWhiteSpace(req.FullName) ||
            string.IsNullOrWhiteSpace(req.Email) ||
            string.IsNullOrWhiteSpace(req.Password) ||
            string.IsNullOrWhiteSpace(req.Specialty))
            return BadRequest("FullName, Email, Password, Specialty are required.");

        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (exists) return Conflict("Email already exists.");

        var user = new User
        {
            FullName = req.FullName,
            Email = req.Email,
            Phone = req.Phone,
            IsActive = true,
            ProfilePictureUrl = null
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, req.Password);

        var doctor = new Doctor
        {
            User = user,
            Specialty = req.Specialty,
            LicenseNumber = req.LicenseNumber,
            ConsultationFee = req.ConsultationFee,
            DefaultSlotMinutes = req.DefaultSlotMinutes ?? 15,
            Bio = req.Bio
        };

        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();

        // Assign Doctor role
        var doctorRoleId = await _db.Roles.Where(r => r.Name == "Doctor").Select(r => r.Id).SingleAsync();
        _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = doctorRoleId });
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, new { doctor.Id });
    }

    // PUT /api/doctors/{id} (Admin OR the doctor themselves)
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorRequest req)
    {
        var doctor = await _db.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null) return NotFound();

        var callerUserId = UserContext.GetUserId(User);
        var isAdmin = UserContext.IsAdmin(User);
        var isSelf = callerUserId.HasValue && doctor.UserId == callerUserId.Value;

        if (!isAdmin && !isSelf)
            return Forbid();

        // Update allowed fields
        if (!string.IsNullOrWhiteSpace(req.Specialty)) doctor.Specialty = req.Specialty.Trim();
        if (req.LicenseNumber is not null) doctor.LicenseNumber = req.LicenseNumber.Trim();
        if (req.ConsultationFee.HasValue) doctor.ConsultationFee = req.ConsultationFee;
        if (req.DefaultSlotMinutes.HasValue) doctor.DefaultSlotMinutes = req.DefaultSlotMinutes.Value;
        if (req.Bio is not null) doctor.Bio = req.Bio.Trim();

        if (req.Phone is not null) doctor.User.Phone = req.Phone.Trim();
        if (!string.IsNullOrWhiteSpace(req.FullName)) doctor.User.FullName = req.FullName.Trim();
        if (req.ProfilePictureUrl is not null) doctor.User.ProfilePictureUrl = req.ProfilePictureUrl.Trim();

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/doctors/{id} (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        if (doctor == null) return NotFound();

        _db.Doctors.Remove(doctor);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/doctors/{doctorId}/timeslots?date=YYYY-MM-DD
    [HttpGet("{doctorId:guid}/timeslots")]
    public async Task<ActionResult<List<TimeSlotDto>>> GetDoctorTimeSlots(Guid doctorId, [FromQuery] DateOnly date)
    {
        // Only published schedules
        var slots = await _db.TimeSlots
            .AsNoTracking()
            .Where(ts => ts.Schedule.DoctorId == doctorId
                         && ts.Schedule.WorkDate == date
                         && ts.Schedule.IsPublished)
            .OrderBy(ts => ts.StartTime)
            .Select(ts => new TimeSlotDto(
                ts.Id,
                ts.ScheduleId,
                ts.StartTime,
                ts.EndTime,
                ts.Capacity,
                ts.BookedCount,
                ts.Status.ToString()
            ))
            .ToListAsync();

        return Ok(slots);
    }
}
