﻿namespace DynamicQR.Api.Endpoints.QrCodes.HistoryGet;
using ApplicationResponse = Application.QrCodes.Queries.GetQrCodeHistory.Response;

public static class Mapper
{
    public static HistoryGetResponse? ToContract(ApplicationResponse response)
        => response is null ? null : new HistoryGetResponse
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
