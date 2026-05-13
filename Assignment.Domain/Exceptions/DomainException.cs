namespace Assignment.Domain.Exceptions;

public class DomainException(string? message) : Exception(message ?? "A domain error occurred.")
{

}