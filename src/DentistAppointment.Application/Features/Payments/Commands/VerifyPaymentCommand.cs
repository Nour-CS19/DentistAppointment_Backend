using DentistAppointment.Application.Common.Exceptions;
using DentistAppointment.Application.Common.Interfaces;
using DentistAppointment.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DentistAppointment.Application.Features.Payments.Commands;

// Replaces the `verify-payment` Supabase Edge Function.
public record VerifyPaymentCommand(string SessionId) : IRequest<VerifyPaymentResult>;

public record VerifyPaymentResult(bool Success, string PaymentStatus);

public class VerifyPaymentCommandHandler : IRequestHandler<VerifyPaymentCommand, VerifyPaymentResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;

    public VerifyPaymentCommandHandler(IApplicationDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<VerifyPaymentResult> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
            throw new ValidationAppException("Session ID is required");

        var session = await _paymentService.RetrieveSessionAsync(request.SessionId, cancellationToken);

        if (session.IsPaid)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.StripeSessionId == request.SessionId, cancellationToken);

            if (appointment is not null)
            {
                appointment.PaymentStatus = PaymentStatus.Paid;
                appointment.Status = AppointmentStatus.Confirmed;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new VerifyPaymentResult(true, "paid");
        }

        return new VerifyPaymentResult(false, session.PaymentStatus);
    }
}
