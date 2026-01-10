using mediflow.Contracts;
using mediflow.Data;
using mediflow.Entities;
using mediflow.Infrastructure;
using mediflow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mediflow.Controllers;

[ApiController]
public sealed class SchedulesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITimeSlotGenerator _slotGen;

    public SchedulesController(AppDbContext db, ITimeSlotGenerator slotGen)
    {
        _db = db;
        _slotGen = slotGen;
    }

    // POST /api/doctors/{doctorId}/schedules
    [Authorize]
    [HttpPost("api/doctors/{doctorId:guid}/schedules")]
    public async Task<IActionResult> CreateForDoctor(Guid doctorId, [FromBody] CreateScheduleRequest req)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor == null) return NotFound("Doctor not found.");

        // Authorization: Admin OR doctor self
        var callerUserId = UserContext.GetUserId(User);
        var isAdmin = UserContext.IsAdmin(User);
        var isSelf = callerUserId.HasValue && doctor.UserId == callerUserId.Value;

        if (!isAdmin && !isSelf)
            return Forbid();

        var slotMinutes = req.SlotMinutes ?? doctor.DefaultSlotMinutes;
        if (slotMinutes <= 0) return BadRequest("SlotMinutes must be > 0.");
        if (req.EndTime <= req.StartTime) return BadRequest("EndTime must be after StartTime.");

        // Ensure no duplicate schedule per doctor per day (you already have a unique index)
        var exists = await _db.Schedules.AnyAsync(s => s.DoctorId == doctorId && s.WorkDate == req.WorkDate);
        if (exists) return Conflict("Schedule already exists for this doctor on this date.");

        var schedule = new Schedule
        {
            DoctorId = doctorId,
            WorkDate = req.WorkDate,
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            SlotMinutes = slotMinutes,
            Location = req.Location?.Trim(),
            IsPublished = req.Publish ?? true
        };

        _db.Schedules.Add(schedule);
        await _db.SaveChangesAsync();

        // Generate slots
        var slots = _slotGen.Generate(schedule.Id, schedule.StartTime, schedule.EndTime, schedule.SlotMinutes);
        _db.TimeSlots.AddRange(slots);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetScheduleById), new { scheduleId = schedule.Id }, new { schedule.Id });
    }

    // GET /api/doctors/{doctorId}/schedules?from=&to=
    [HttpGet("api/doctors/{doctorId:guid}/schedules")]
    public async Task<ActionResult<List<ScheduleDto>>> GetDoctorSchedules(
        Guid doctorId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        var q = _db.Schedules
            .AsNoTracking()
            .Where(s => s.DoctorId == doctorId);

        if (from.HasValue) q = q.Where(s => s.WorkDate >= from.Value);
        if (to.HasValue) q = q.Where(s => s.WorkDate <= to.Value);

        var list = await q
            .OrderBy(s => s.WorkDate)
            .Select(s => new ScheduleDto(
                s.Id,
                s.DoctorId,
                s.WorkDate,
                s.StartTime,
                s.EndTime,
                s.SlotMinutes,
                s.Location,
                s.IsPublished
            ))
            .ToListAsync();

        return Ok(list);
    }

    // GET /api/schedules/{scheduleId}
    [HttpGet("api/schedules/{scheduleId:guid}")]
    public async Task<ActionResult<object>> GetScheduleById(Guid scheduleId)
    {
        var schedule = await _db.Schedules
            .AsNoTracking()
            .Include(s => s.TimeSlots)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule == null) return NotFound();

        return Ok(new
        {
            schedule.Id,
            schedule.DoctorId,
            schedule.WorkDate,
            schedule.StartTime,
            schedule.EndTime,
            schedule.SlotMinutes,
            schedule.Location,
            schedule.IsPublished,
            TimeSlots = schedule.TimeSlots
                .OrderBy(t => t.StartTime)
                .Select(t => new TimeSlotDto(
                    t.Id,
                    t.ScheduleId,
                    t.StartTime,
                    t.EndTime,
                    t.Capacity,
                    t.BookedCount,
                    t.Status.ToString()
                ))
        });
    }

    // DELETE /api/schedules/{scheduleId} (Doctor/Admin)
    [Authorize]
    [HttpDelete("api/schedules/{scheduleId:guid}")]
    public async Task<IActionResult> DeleteSchedule(Guid scheduleId)
    {
        var schedule = await _db.Schedules
            .Include(s => s.Doctor)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule == null) return NotFound();

        var callerUserId = UserContext.GetUserId(User);
        var isAdmin = UserContext.IsAdmin(User);
        var isSelf = callerUserId.HasValue && schedule.Doctor.UserId == callerUserId.Value;

        if (!isAdmin && !isSelf)
            return Forbid();

        _db.Schedules.Remove(schedule);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // POST /api/schedules/{scheduleId}/publish or /unpublish (optional)
    [Authorize]
    [HttpPost("api/schedules/{scheduleId:guid}/publish")]
    public async Task<IActionResult> Publish(Guid scheduleId)
    {
        var schedule = await _db.Schedules.Include(s => s.Doctor).FirstOrDefaultAsync(s => s.Id == scheduleId);
        if (schedule == null) return NotFound();

        var callerUserId = UserContext.GetUserId(User);
        var isAdmin = UserContext.IsAdmin(User);
        var isSelf = callerUserId.HasValue && schedule.Doctor.UserId == callerUserId.Value;

        if (!isAdmin && !isSelf) return Forbid();

        schedule.IsPublished = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpPost("api/schedules/{scheduleId:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid scheduleId)
    {
        var schedule = await _db.Schedules.Include(s => s.Doctor).FirstOrDefaultAsync(s => s.Id == scheduleId);
        if (schedule == null) return NotFound();

        var callerUserId = UserContext.GetUserId(User);
        var isAdmin = UserContext.IsAdmin(User);
        var isSelf = callerUserId.HasValue && schedule.Doctor.UserId == callerUserId.Value;

        if (!isAdmin && !isSelf) return Forbid();

        schedule.IsPublished = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
