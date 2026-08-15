using DentistAppointment.Application.Common.Interfaces;
using DentistAppointment.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DentistAppointment.Application.Features.Appointments.Queries;

public record GetMyAppointmentsQuery : IRequest<List<AppointmentDto>>;

public class GetMyAppointmentsQueryHandler : IRequestHandler<GetMyAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyAppointmentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<AppointmentDto>> Handle(GetMyAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        return await _context.Appointments
            .Where(a => a.UserId == userId)
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
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
