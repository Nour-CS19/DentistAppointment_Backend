namespace DentistAppointment.Application.DTOs;

public class ProfileDto
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Role { get; set; } = "client";
}

public class AuthResultDto
{
    public string Token { get; set; } = string.Empty;
    public ProfileDto Profile { get; set; } = new();
}
