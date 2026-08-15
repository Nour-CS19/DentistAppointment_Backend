using DentistAppointment.Application.Common.Exceptions;
using DentistAppointment.Application.Common.Interfaces;
using DentistAppointment.Domain.Entities;
using DentistAppointment.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DentistAppointment.Application.Features.Appointments.Commands;

// Admin-only — replaces the "look up profile by email, then insert appointment" flow in Admin.tsx.
public record AdminCreateAppointmentCommand(
    string PatientEmail,
    string Service,
    int Price,
    DateOnly AppointmentDate,
    string AppointmentTime,
    AppointmentStatus Status,
    PaymentStatus PaymentStatus,
    string? Notes) : IRequest<Guid>;

public class AdminCreateAppointmentCommandHandler : IRequestHandler<AdminCreateAppointmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<AppUser> _userManager;

    public AdminCreateAppointmentCommandHandler(
        IApplicationDbContext context, ICurrentUserService currentUser, UserManager<AppUser> userManager)
    {
        _context = context;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(AdminCreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Admins only");

        var patient = await _userManager.FindByEmailAsync(request.PatientEmail)
            ?? throw new NotFoundException("Patient with this email not found");

        var appointment = new Appointment
        {
            UserId = patient.Id,
            Service = request.Service,
            Price = request.Price,
            AppointmentDate = request.AppointmentDate,
            AppointmentTime = request.AppointmentTime,
            Status = request.Status,
            PaymentStatus = request.PaymentStatus,
            Notes = request.Notes,
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}
