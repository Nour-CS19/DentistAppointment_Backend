namespace DentistAppointment.Application.Common.Interfaces;

public record CheckoutSessionRequest(
    string CustomerEmail,
    string Service,
    int Price,
    string SuccessUrl,
    string CancelUrl,
    Dictionary<string, string> Metadata);

public record CheckoutSessionResult(string SessionId, string Url);

public record VerifiedSessionResult(bool IsPaid, string PaymentStatus, string? StripeSessionId);

public interface IPaymentService
{
    Task<CheckoutSessionResult> CreateCheckoutSessionAsync(CheckoutSessionRequest request, CancellationToken cancellationToken = default);
    Task<VerifiedSessionResult> RetrieveSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}
