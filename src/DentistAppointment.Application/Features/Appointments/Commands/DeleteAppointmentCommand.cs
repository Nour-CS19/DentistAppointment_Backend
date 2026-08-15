using DentistAppointment.Application.Common.Exceptions;
using DentistAppointment.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DentistAppointment.Application.Features.Appointments.Commands;

public record DeleteAppointmentCommand(Guid Id) : IRequest<Unit>;

public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteAppointmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Appointment not found");

        if (appointment.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenException();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
