using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response;

namespace DynamicQR.Api.Endpoints.QrCodeTargets.QrCodeTargetPut;

internal static class Mapper
{
    internal static ApplicationCommand? ToCore(Request request, string id)
        => request is null ? null : new ApplicationCommand
        {
            Id = id,
            Value = request.Value
        };

    public static Response? ToContract(ApplicationResponse response)
        => response is null ? null : new Response
        {
            Id = response.Id,
        };
}
