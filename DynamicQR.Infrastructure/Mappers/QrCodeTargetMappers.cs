using DynamicQR.Infrastructure.Entities;

namespace DynamicQR.Infrastructure.Mappers;

public static class QrCodeTargetMappers
{
    public static QrCodeTarget ToInfrastructure(this Domain.Models.QrCodeTarget qrCodeTarget)
    {
        return new QrCodeTarget
        {
            Value = qrCodeTarget.Value,
            PartitionKey = qrCodeTarget.QrCodeId,
            RowKey = qrCodeTarget.QrCodeId
        };
    }

    public static Domain.Models.QrCodeTarget ToCore(this QrCodeTarget qrCodeTarget)
    {
        return new Domain.Models.QrCodeTarget
        {
            Value = qrCodeTarget.Value,
            QrCodeId = qrCodeTarget.PartitionKey
        };
    }
}