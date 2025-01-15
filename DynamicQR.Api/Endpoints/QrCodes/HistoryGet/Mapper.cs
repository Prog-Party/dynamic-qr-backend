namespace DynamicQR.Api.Endpoints.QrCodes.HistoryGet;
using ApplicationResponse = Application.QrCodes.Queries.GetQrCodeHistory.Response;

internal static class Mapper
{
    internal static HistoryGetResponse ToContract(ApplicationResponse response)
        => response is null ? throw new ArgumentNullException(nameof(response)) : new HistoryGetResponse
        {
            QrCodeId = response.QrCodeId,
            Order = response.Order,
            Timestamp = response.Timestamp,
            CustomerId = response.CustomerId,
            OrganizationId = response.OrganizationId,
            Details = response.Details,
            EventType = response.EventType
        };
}
