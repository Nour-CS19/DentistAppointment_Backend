namespace DentistAppointment.Application.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Service { get; set; } = string.Empty;
    public int Price { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public string AppointmentTime { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? StripeSessionId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Only populated by GetAllAppointmentsQuery (admin view) — null for GetMyAppointmentsQuery.
    public string? PatientFirstName { get; set; }
    public string? PatientLastName { get; set; }
    public string? PatientEmail { get; set; }
    public string? PatientPhone { get; set; }
}
