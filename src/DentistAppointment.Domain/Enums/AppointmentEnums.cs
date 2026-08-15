namespace DentistAppointment.Domain.Enums;

public static class AppRoles
{
    public const string Admin = "admin";
    public const string Client = "client";
}

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

public enum PaymentStatus
{
    Unpaid,
    Pending,
    Paid,
    Failed,
    Refunded
}
