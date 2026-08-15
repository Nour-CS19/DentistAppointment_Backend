using DentistAppointment.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace DentistAppointment.Infrastructure.Payments;

public class StripePaymentService : IPaymentService
{
    public StripePaymentService(IConfiguration configuration)
    {
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
    }

    public async Task<CheckoutSessionResult> CreateCheckoutSessionAsync(
        CheckoutSessionRequest request, CancellationToken cancellationToken = default)
    {
        var customerService = new CustomerService();
        var existingCustomers = await customerService.ListAsync(new CustomerListOptions
        {
            Email = request.CustomerEmail,
            Limit = 1
        }, cancellationToken: cancellationToken);

        var customerId = existingCustomers.Data.FirstOrDefault()?.Id;

        var options = new SessionCreateOptions
        {
            Customer = customerId,
            CustomerEmail = customerId is null ? request.CustomerEmail : null,
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.Service,
                        },
                        UnitAmount = request.Price * 100,
                    },
                    Quantity = 1,
                }
            },
            Mode = "payment",
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            Metadata = request.Metadata,
        };

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(options, cancellationToken: cancellationToken);

        return new CheckoutSessionResult(session.Id, session.Url);
    }

    public async Task<VerifiedSessionResult> RetrieveSessionAsync(
        string sessionId, CancellationToken cancellationToken = default)
    {
        var sessionService = new SessionService();
        var session = await sessionService.GetAsync(sessionId, cancellationToken: cancellationToken);

        return new VerifiedSessionResult(
            IsPaid: session.PaymentStatus == "paid",
            PaymentStatus: session.PaymentStatus,
            StripeSessionId: session.Id);
    }
}
