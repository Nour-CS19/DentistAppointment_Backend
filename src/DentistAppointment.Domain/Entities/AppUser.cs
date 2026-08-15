using Microsoft.AspNetCore.Identity;

namespace DentistAppointment.Domain.Entities;

// Extends IdentityUser<Guid> — replaces Supabase's auth.users + profiles table combined.
public class AppUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
