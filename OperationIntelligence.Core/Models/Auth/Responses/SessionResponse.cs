namespace OperationIntelligence.Core
{
    public class SessionResponse
    {
        public Guid Id { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceName { get; set; }
        public string? Browser { get; set; }
        public string? OperatingSystem { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime LastSeenAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }

        public bool IsActive => RevokedAtUtc == null;
    }
}
