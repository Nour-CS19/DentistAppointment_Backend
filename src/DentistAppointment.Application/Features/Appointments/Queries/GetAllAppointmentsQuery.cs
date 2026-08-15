using DentistAppointment.Application.Common.Exceptions;
using DentistAppointment.Application.Common.Interfaces;
using DentistAppointment.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DentistAppointment.Application.Features.Appointments.Queries;

// Replaces the "Admins can view all appointments" RLS policy — enforced in code instead of SQL.
public record GetAllAppointmentsQuery : IRequest<List<AppointmentDto>>;

public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllAppointmentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<AppointmentDto>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Admins only");

        return await _context.Appointments
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                UserId = a.UserId,
                Service = a.Service,
                Price = a.Price,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Status = a.Status.ToString().ToLower(),
                PaymentStatus = a.PaymentStatus.ToString().ToLower(),
                StripeSessionId = a.StripeSessionId,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                PatientFirstName = a.User != null ? a.User.FirstName : null,
                PatientLastName = a.User != null ? a.User.LastName : null,
                PatientEmail = a.User != null ? a.User.Email : null,
                PatientPhone = a.User != null ? a.User.PhoneNumber : null,
            })
            .ToListAsync(cancellationToken);
    }
}
