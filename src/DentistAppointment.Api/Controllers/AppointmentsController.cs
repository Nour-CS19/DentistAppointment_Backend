using DentistAppointment.Application.Features.Appointments.Commands;
using DentistAppointment.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentistAppointment.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly ISender _mediator;

    public AppointmentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    public record UpdateAppointmentRequest(
        string Service, int Price, DateOnly AppointmentDate, string AppointmentTime, string? Notes);

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyAppointmentsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAppointmentRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateAppointmentCommand(
            id, request.Service, request.Price, request.AppointmentDate, request.AppointmentTime, request.Notes),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAppointmentCommand(id), cancellationToken);
        return NoContent();
    }
}
