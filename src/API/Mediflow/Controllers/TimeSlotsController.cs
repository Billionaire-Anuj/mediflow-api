using mediflow.Contracts;
using mediflow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mediflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TimeSlotsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TimeSlotsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/timeslots/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TimeSlotDto>> GetById(Guid id)
    {
        var ts = await _db.TimeSlots
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ts == null) return NotFound();

        return Ok(new TimeSlotDto(
            ts.Id,
            ts.ScheduleId,
            ts.StartTime,
            ts.EndTime,
            ts.Capacity,
            ts.BookedCount,
            ts.Status.ToString()
        ));
    }
}