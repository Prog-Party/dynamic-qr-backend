namespace DynamicQR.Domain.Exceptions;

public class QrCodeTargetNotFoundException : ItemNotFoundException
{
    public QrCodeTargetNotFoundException(string id) : this(id, null)
    {
    }

    public QrCodeTargetNotFoundException(string id, Exception? innerException) : base($"No QR code target was found with QR code id {id}.", innerException)
    {
    }
}