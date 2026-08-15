using DentistAppointment.Application.Features.Payments.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentistAppointment.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _mediator;

    public PaymentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    public record CreateCheckoutRequest(
        string Service, int Price, DateOnly AppointmentDate, string AppointmentTime, string? Notes);

    public record VerifyRequest(string SessionId);

    [HttpPost("create-checkout-session")]
    [Authorize]
    public async Task<IActionResult> CreateCheckoutSession(CreateCheckoutRequest request, CancellationToken cancellationToken)
    {
        var origin = Request.Headers.Origin.ToString();
        var url = await _mediator.Send(new CreateCheckoutSessionCommand(
            request.Service, request.Price, request.AppointmentDate, request.AppointmentTime, request.Notes, origin),
            cancellationToken);
        return Ok(new { url });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(VerifyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new VerifyPaymentCommand(request.SessionId), cancellationToken);
        return Ok(new { success = result.Success, paymentStatus = result.PaymentStatus });
    }
}
