using DynamicQR.Api.Attributes;
using DynamicQR.Api.Extensions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using ApplicationRequest = DynamicQR.Application.QrCodes.Queries.GetAllQrCodes.Request;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetAllQrCodes.Response;

namespace DynamicQR.Api.Endpoints.QrCodes.QrCodeGetAll;

public sealed class QrCodeGetAll : EndpointsBase
{
    public QrCodeGetAll(IMediator mediator, ILoggerFactory loggerFactory) :
        base(mediator, loggerFactory.CreateLogger<QrCodeGetAll>())
    { }

    [Function(nameof(QrCodeGetAll))]
    [OpenApiOperation(nameof(QrCodeGetAll), Tags.QrCode,
       Summary = "Retrieve all QR codes for a specific organization.")
    ]
    [OpenApiHeaderOrganizationIdentifier]
    [OpenApiJsonResponse(typeof(List<QrCodeGetAllResponse>), Description = "The retrieved QR codes for the organization")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "Missing organization identifier header")]
    public async Task<HttpResponseData> RunAsync(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "qr-codes")]
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        string organizationId = req.GetHeaderAttribute<OpenApiHeaderOrganizationIdentifierAttribute>();

        ApplicationRequest coreRequest = new() { OrganizationId = organizationId };

        List<ApplicationResponse> coreResponse = await _mediator.Send(coreRequest, cancellationToken);

        List<QrCodeGetAllResponse> qrCodeResponses = coreResponse.Select(Mapper.ToContract).ToList();

        return await CreateJsonResponse(req, qrCodeResponses);
    }
}
