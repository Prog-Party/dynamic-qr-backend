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
using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.DeleteQrCode.Command;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeDelete;

public sealed class QrCodeDelete : EndpointsBase
{
    public QrCodeDelete(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodeDelete>())
    {
    }

    [Function(nameof(QrCodeDelete))]
    [OpenApiOperation(nameof(QrCodeDelete), Tags.QrCode,
       Summary = "Delete a specific new qr code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiHeaderCustomerIdentifier]
    [OpenApiPathIdentifier]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "Missing organization identifier header. Or missing customer identifier header.")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "qr-codes/{id}")] HttpRequestData req,
        string id,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();
        string customerId = req.GetHeaderAttribute<OpenApiHeaderCustomerIdentifierAttribute>();

        _logger.LogInformation($"{typeof(QrCodeDelete).FullName}.triggered");

        ApplicationCommand coreCommand = new()
        {
            Id = id,
            OrganizationId = organizationId,
            CustomerId = customerId
        };

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
