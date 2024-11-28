namespace Dental_Clinic.Exceptions;
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
