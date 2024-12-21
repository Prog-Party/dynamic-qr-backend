namespace DynamicQR.Api.Endpoints.QrCodes.HistoryGet;

internal static class Mapper
{
    public static Response? ToContract(this Application.QrCodes.Queries.GetQrCodeHistory.Response response)
    {
        return response is null ? null : new Response
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
}
