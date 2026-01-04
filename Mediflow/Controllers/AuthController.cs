using System.Security.Claims;
using mediflow.Data;
using mediflow.Entities;
using mediflow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mediflow.Controllers;

[ApiController]
[Route("api/authentication")]
public sealed class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _tokens;

    public AuthController(AppDbContext db, IJwtTokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        req = req.Trimmed();

        if (string.IsNullOrWhiteSpace(req.FullName) ||
            string.IsNullOrWhiteSpace(req.Email) ||
            string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("FullName, Email, and Password are required.");

        if (req.Password.Length < 8)
            return BadRequest("Password must be at least 8 characters.");

        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (exists)
            return Conflict("Email already exists.");

        var user = new User
        {
            FullName = req.FullName,
            Email = req.Email,
            Phone = req.Phone,
            IsActive = true
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, req.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Default role: Patient
        var patientRoleId = await _db.Roles
            .Where(r => r.Name == "Patient")
            .Select(r => r.Id)
            .SingleAsync();

        _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = patientRoleId });
        await _db.SaveChangesAsync();

        var roles = new[] { "Patient" };
        var token = _tokens.CreateToken(user, roles);

        return Ok(new AuthResponse(token, user.Id, user.FullName, user.Email, roles));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        req = req.Trimmed();

        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Email and Password are required.");

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == req.Email);
        if (user == null || !user.IsActive)
            return Unauthorized();

        var hasher = new PasswordHasher<User>();
        var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);

        if (verify == PasswordVerificationResult.Failed)
            return Unauthorized();

        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        var token = _tokens.CreateToken(user, roles);
        return Ok(new AuthResponse(token, user.Id, user.FullName, user.Email, roles.ToArray()));
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        var name = User.FindFirstValue("name") ?? User.Identity?.Name;
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

        return Ok(new { userId, email, name, roles });
    }
}

public sealed record RegisterRequest(string FullName, string Email, string Password, string? Phone)
{
    public RegisterRequest Trimmed() =>
        this with
        {
            FullName = (FullName ?? "").Trim(),
            Email = (Email ?? "").Trim().ToLowerInvariant(),
            Password = Password ?? "",
            Phone = Phone?.Trim()
        };
}

public sealed record LoginRequest(string Email, string Password)
{
    public LoginRequest Trimmed() =>
        this with
        {
            Email = (Email ?? "").Trim().ToLowerInvariant(),
            Password = Password ?? ""
        };
}

public sealed record AuthResponse(string Token, Guid UserId, string FullName, string Email, string[] Roles);
