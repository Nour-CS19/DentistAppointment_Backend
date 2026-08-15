using DentistAppointment.Application.Features.Appointments.Commands;
using DentistAppointment.Application.Features.Appointments.Queries;
using DentistAppointment.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentistAppointment.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminController(ISender mediator)
    {
        _mediator = mediator;
    }

    public record UpdateStatusRequest(AppointmentStatus Status, PaymentStatus PaymentStatus);

    public record CreateAppointmentRequest(
        string PatientEmail, string Service, int Price, DateOnly AppointmentDate,
        string AppointmentTime, AppointmentStatus Status, PaymentStatus PaymentStatus, string? Notes);

    [HttpPost("appointments")]
    public async Task<IActionResult> Create(CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new AdminCreateAppointmentCommand(
            request.PatientEmail, request.Service, request.Price, request.AppointmentDate,
            request.AppointmentTime, request.Status, request.PaymentStatus, request.Notes),
            cancellationToken);
        return Ok(new { id });
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllAppointmentsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("appointments/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateStatusRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateAppointmentStatusCommand(id, request.Status, request.PaymentStatus), cancellationToken);
        return NoContent();
    }
}
