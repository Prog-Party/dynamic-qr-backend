using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using ApplicationRequest = DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory.Request;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.HistoryGet;

public sealed class HistoryGet : EndpointsBase
{
    public HistoryGet(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<HistoryGet>())
    { }

    [Function(nameof(HistoryGet))]
    [OpenApiOperation(nameof(HistoryGet), Tags.QrCode,
       Summary = "Retrieve the history of a specific QR code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiPathIdentifier]
    [OpenApiJsonResponse(typeof(List<HistoryGetResponse>), Description = "The retrieved history items for the QR code")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "No history found for the given QR code identifier. Or Missing organization identifier header.")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "qr-codes/{id}/history")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();

        ApplicationRequest coreRequest = new() { QrCodeId = id, OrganizationId = organizationId };

        List<ApplicationResponse> coreResponse = await _mediator.Send(coreRequest, cancellationToken);

        List<HistoryGetResponse> historyResponses = coreResponse.Select(Mapper.ToContract).ToList();

        return await CreateJsonResponse(req, historyResponses);
    }
}
