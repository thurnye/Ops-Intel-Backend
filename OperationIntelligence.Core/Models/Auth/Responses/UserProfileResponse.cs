namespace OperationIntelligence.Core
{
    public class UserProfileResponse
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }

        public DateTime? Birthdate { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }

        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateOrProvince { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        public Guid? AvatarFileId { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
