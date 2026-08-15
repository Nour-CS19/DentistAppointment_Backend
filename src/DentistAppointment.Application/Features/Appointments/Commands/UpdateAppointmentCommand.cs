using DentistAppointment.Application.Common.Exceptions;
using DentistAppointment.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DentistAppointment.Application.Features.Appointments.Commands;

public record UpdateAppointmentCommand(
    Guid Id,
    string Service,
    int Price,
    DateOnly AppointmentDate,
    string AppointmentTime,
    string? Notes) : IRequest<Unit>;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAppointmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Appointment not found");

        // Owner-only, same as the original "Users can update their own appointments" RLS policy.
        if (appointment.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenException();

        appointment.Service = request.Service;
        appointment.Price = request.Price;
        appointment.AppointmentDate = request.AppointmentDate;
        appointment.AppointmentTime = request.AppointmentTime;
        appointment.Notes = request.Notes;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
