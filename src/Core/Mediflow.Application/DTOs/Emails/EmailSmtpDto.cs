namespace Mediflow.Application.DTOs.Emails;

public class EmailSmtpDto
{
    public required string Host { get; set; } = string.Empty;

    public required int Port { get; set; }

    public required string Password { get; set; } = string.Empty;

    public required string Username { get; set; } = string.Empty;
}