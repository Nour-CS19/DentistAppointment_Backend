using DentistAppointment.Application.Common.Exceptions;
using DentistAppointment.Application.Common.Interfaces;
using DentistAppointment.Domain.Entities;
using DentistAppointment.Domain.Enums;
using MediatR;

namespace DentistAppointment.Application.Features.Payments.Commands;

// Replaces the `create-payment` Supabase Edge Function.
public record CreateCheckoutSessionCommand(
    string Service,
    int Price,
    DateOnly AppointmentDate,
    string AppointmentTime,
    string? Notes,
    string OriginUrl) : IRequest<string>;

public class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _paymentService;

    public CreateCheckoutSessionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentService paymentService)
    {
        _context = context;
        _currentUser = currentUser;
        _paymentService = paymentService;
    }

    public async Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null || _currentUser.Email is null)
            throw new ForbiddenException("User not authenticated or email not available");

        var checkoutResult = await _paymentService.CreateCheckoutSessionAsync(new CheckoutSessionRequest(
            CustomerEmail: _currentUser.Email,
            Service: request.Service,
            Price: request.Price,
            SuccessUrl: $"{request.OriginUrl}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl: $"{request.OriginUrl}/book",
            Metadata: new Dictionary<string, string>
            {
                ["user_id"] = _currentUser.UserId.Value.ToString(),
                ["service"] = request.Service,
                ["price"] = request.Price.ToString(),
                ["appointment_date"] = request.AppointmentDate.ToString("yyyy-MM-dd"),
                ["appointment_time"] = request.AppointmentTime,
                ["notes"] = request.Notes ?? string.Empty,
            }), cancellationToken);

        var appointment = new Appointment
        {
            UserId = _currentUser.UserId.Value,
            Service = request.Service,
            Price = request.Price,
            AppointmentDate = request.AppointmentDate,
            AppointmentTime = request.AppointmentTime,
            Status = AppointmentStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            StripeSessionId = checkoutResult.SessionId,
            Notes = request.Notes,
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        return checkoutResult.Url;
    }
}
