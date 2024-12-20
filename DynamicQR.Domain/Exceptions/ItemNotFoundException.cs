namespace DynamicQR.Domain.Exceptions;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}