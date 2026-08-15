using DentistAppointment.Domain.Entities;

namespace DentistAppointment.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(AppUser user, IList<string> roles);
}
