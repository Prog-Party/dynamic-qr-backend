namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePut;

public sealed record QrCodePutResponse
{
    public string Id { get; init; } = string.Empty;
}