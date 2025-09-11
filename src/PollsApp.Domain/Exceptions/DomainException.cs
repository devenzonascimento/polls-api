using PollsApp.Domain.Primitives;

namespace PollsApp.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message)
        : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with ID '{key}' not found.") { }
}

public class InvalidStateException : DomainException
{
    public InvalidStateException(string message)
        : base(message) { }
}
