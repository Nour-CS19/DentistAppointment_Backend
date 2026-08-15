using DentistAppointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentistAppointment.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Appointment> Appointments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
