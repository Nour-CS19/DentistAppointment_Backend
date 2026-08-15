namespace DentistAppointment.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Forbidden") : base(message) { }
}

public class ValidationAppException : Exception
{
    public ValidationAppException(string message) : base(message) { }
}
