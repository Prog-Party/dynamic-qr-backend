namespace DynamicQR.Api.Mappers;

public static class QrCodeTargetsMappers
{
    public static Application.QrCodes.Commands.UpdateQrCodeTarget.Command? ToCore(this Endpoints.QrCodeTargets.QrCodeTargetPut.Request request, string id)
    {
        return request is null ? null : new Application.QrCodes.Commands.UpdateQrCodeTarget.Command
        {
            Id = id,
            Value = request.Value
        };
    }

    public static Endpoints.QrCodeTargets.QrCodeTargetPut.Response? ToContract(this Application.QrCodes.Commands.UpdateQrCodeTarget.Response response)
    {
        return response is null ? null : new Endpoints.QrCodeTargets.QrCodeTargetPut.Response
        {
            Id = response.Id,
        };
    }
}