using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using DynamicQR.Domain.Exceptions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeDelete;

public sealed class QrCodeDelete : EndpointsBase
{
    public QrCodeDelete(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodeDelete>())
    { }

    [Function(nameof(QrCodeDelete))]
    [OpenApiOperation(nameof(QrCodeDelete), Tags.QrCode,
       Summary = "Delete a specific new qr code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiPathIdentifier]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "Missing organization identifier header")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "qr-codes/{id}")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();

        _logger.LogInformation($"{typeof(QrCodeDelete).FullName}.triggered");

        Application.QrCodes.Commands.DeleteQrCode.Command coreCommand = new() { Id = id, OrganisationId = organizationId };

        try
        {
            await _mediator.Send(coreCommand, cancellationToken);
        }
        catch (StorageException)
        {
            return req.CreateResponse(HttpStatusCode.BadGateway);
        }
        catch (QrCodeNotFoundException)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}