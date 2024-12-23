using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using ApplicationRequest = DynamicQR.Application.QrCodes.Queries.GetQrCode.Request;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetQrCode.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeGet;

public sealed class QrCodeGet : EndpointsBase
{
    private const string BadRequestNoQrCodeFoundMessage = "No qr code found with the given identifier.";

    public QrCodeGet(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodeGet>())
    { }

    [Function(nameof(QrCodeGet))]
    [OpenApiOperation(nameof(QrCodeGet), Tags.QrCode,
       Summary = "Retrieve a certain qr code.")
    ]
    [OpenApiPathIdentifier]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiJsonResponse(typeof(Response), Description = "The retrieved qr code by its identifier")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "No qr code found with the given identifier. Or Missing organization identifier header")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "qr-codes/{id}")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();

        ApplicationRequest coreRequest = new() { Id = id, OrganizationId = organizationId };

        ApplicationResponse coreResponse = await _mediator.Send(coreRequest, cancellationToken);

        Response? qrCodeResponse = Mapper.ToContract(coreResponse);

        if (qrCodeResponse == null)
            return await CreateResponse(req, HttpStatusCode.BadRequest, BadRequestNoQrCodeFoundMessage);

        return await CreateJsonResponse(req, qrCodeResponse);
    }
}