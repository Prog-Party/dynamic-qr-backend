namespace DynamicQR.Api.Endpoints.QrCodeTargets.QrCodeTargetPut;

public sealed record QrCodeTargetPutRequest
{
    public string Value { get; init; } = string.Empty;
}