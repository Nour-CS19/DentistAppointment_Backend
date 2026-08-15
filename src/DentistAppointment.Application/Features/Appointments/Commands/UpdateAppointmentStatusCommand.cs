using DentistAppointment.Application.Common.Exceptions;
using DentistAppointment.Application.Common.Interfaces;
using DentistAppointment.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DentistAppointment.Application.Features.Appointments.Commands;

// Admin-only status change — replaces the "Admins can update all appointments" RLS policy.
public record UpdateAppointmentStatusCommand(
    Guid Id,
    AppointmentStatus Status,
    PaymentStatus PaymentStatus) : IRequest<Unit>;

public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAppointmentStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Admins only");

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Appointment not found");

        appointment.Status = request.Status;
        appointment.PaymentStatus = request.PaymentStatus;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
