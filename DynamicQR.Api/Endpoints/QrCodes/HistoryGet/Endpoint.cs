using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

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
    [OpenApiPathIdentifier]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiJsonResponse(typeof(List<Response>), Description = "The retrieved history items for the QR code")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "No history found for the given QR code identifier. Or Missing organization identifier header")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "qr-codes/{id}/history")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();

        Application.QrCodes.Queries.GetQrCodeHistory.Request coreRequest = new() { QrCodeId = id, OrganizationId = organizationId };

        List<Application.QrCodes.Queries.GetQrCodeHistory.Response> coreResponse = await _mediator.Send(coreRequest, cancellationToken);

        List<Response> historyResponses = coreResponse?.Where(x => x != null).Select(x => x.ToContract()!)?.ToList() ?? new();

        return await CreateJsonResponse(req, historyResponses);
    }
}
