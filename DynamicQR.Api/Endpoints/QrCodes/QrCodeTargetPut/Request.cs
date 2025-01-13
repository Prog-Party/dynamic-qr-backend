namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeTargetPut;

public sealed record QrCodeTargetPutRequest
{
    public string Value { get; init; } = string.Empty;
}