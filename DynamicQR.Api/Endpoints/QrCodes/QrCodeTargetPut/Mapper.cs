using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeTargetPut;

internal static class Mapper
{
    internal static ApplicationCommand? ToCore(QrCodeTargetPutRequest request, string id, string organizationId, string customerId)
        => request is null ? null : new ApplicationCommand
        {
            Id = id,
            OrganizationId = organizationId,
            CustomerId = customerId,
            Value = request.Value
        };

    internal static QrCodeTargetPutResponse? ToContract(ApplicationResponse response)
        => response is null ? null : new QrCodeTargetPutResponse
        {
            Id = response.Id,
        };
}
