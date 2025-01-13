using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using ApplicationCommand = DynamicQR.Application.QrCodes.Commands.CreateQrCode.Command;
using ApplicationResponse = DynamicQR.Application.QrCodes.Commands.CreateQrCode.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodePost;

public sealed class QrCodePost : EndpointsBase
{
    public QrCodePost(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodePost>())
    {
    }

    [Function(nameof(QrCodePost))]
    [OpenApiOperation(nameof(QrCodePost), Tags.QrCode,
       Summary = "Create a new qr code.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiHeaderCustomerIdentifier]
    [OpenApiJsonPayload(typeof(QrCodePostRequest))]
    [OpenApiJsonResponse(typeof(QrCodePostResponse), HttpStatusCode.Created, Description = "Get a certain qr code")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway)]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "Missing organization identifier header. Or missing customer identifier header.")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "qr-codes")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();
        string customerId = req.GetHeaderAttribute<OpenApiHeaderCustomerIdentifierAttribute>();

        var request = await ParseBody<QrCodePostRequest>(req);
        if (request.Error != null) return request.Error;

        ApplicationCommand? coreCommand = Mapper.ToCore(request.Result, organizationId, customerId);

        ApplicationResponse coreResponse;

        try
        {
            coreResponse = await _mediator.Send(coreCommand!, cancellationToken);
        }
        catch (StorageException)
        {
            return req.CreateResponse(HttpStatusCode.BadGateway);
        }

        QrCodePostResponse? responseContent = Mapper.ToContract(coreResponse);

        return await CreateJsonResponse(req, responseContent, HttpStatusCode.Created);
    }
}
