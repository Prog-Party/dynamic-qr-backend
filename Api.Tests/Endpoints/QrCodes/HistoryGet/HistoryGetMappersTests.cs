using DynamicQR.Api.Endpoints.QrCodes.HistoryGet;
using DynamicQR.Api.Mappers;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using ApplicationResponse = DynamicQR.Application.QrCodes.Queries.GetQrCodeHistory.Response;

namespace Api.Tests.Endpoints.QrCodes.HistoryGet;

[ExcludeFromCodeCoverage]
public sealed class HistoryGetMappersTests
{
    [Fact]
    public void ToContract_HistoryGet_NullResponse_ReturnsNull()
    {
        // Arrange
        ApplicationResponse? response = null;

        // Act
        var result = Mapper.ToContract(response!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToContract_HistoryGet_ValidResponse_MapsToResponse()
    {
        // Arrange
        var response = new ApplicationResponse
        {
            QrCodeId = "qr123",
            Order = "abcd",
            Timestamp = DateTime.UtcNow,
            CustomerId = "cust123",
            OrganizationId = "org123",
            Details = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" }
            },
            EventType = "EventType"
        };

        // Act
        var result = response.ToContract();

        // Assert
        result.Should().NotBeNull();
        result!.QrCodeId.Should().Be(response.QrCodeId);
        result.Order.Should().Be(response.Order);
        result.Timestamp.Should().Be(response.Timestamp);
        result.CustomerId.Should().Be(response.CustomerId);
        result.OrganizationId.Should().Be(response.OrganizationId);
        result.Details.Should().BeEquivalentTo(response.Details);
        result.EventType.Should().Be(response.EventType);
    }
}