using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.UpdateQrCodeTarget.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeTargetPut;

internal static class Mapper
{
    internal static ApplicationCommand ToCore(QrCodeTargetPutRequest request, string id, string organizationId, string customerId)
        => request is null ? throw new ArgumentNullException(nameof(request)) : new ApplicationCommand
        {
            Id = id,
            OrganizationId = organizationId,
            CustomerId = customerId,
            Value = request.Value
        };

    public static QrCodeTargetPutResponse ToContract(ApplicationResponse response)
        => response is null ? throw new ArgumentNullException(nameof(response)) : new QrCodeTargetPutResponse
        {
            Id = response.Id,
        };
}
