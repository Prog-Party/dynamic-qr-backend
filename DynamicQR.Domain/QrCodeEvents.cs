public class QrCodeEvents
{
    public class Lifecycle
    {
        public const string Created = "Lifecycle_Created";
        public const string Deleted = "Lifecycle_Deleted";
        public const string Updated = "Lifecycle_Updated";
        public const string UpdatedTarget = "Lifecycle_UpdatedTarget";
    }

    public class ScanEvents
    {
        public const string Scanned = "ScanEvents_Scanned";
    }
}
