namespace DentistAppointment.Application.Common.Interfaces;

// Reads the authenticated user's id/role from the JWT — the replacement for
// Supabase's `auth.uid()` and RLS role checks, enforced here instead of in SQL policies.
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }
}
