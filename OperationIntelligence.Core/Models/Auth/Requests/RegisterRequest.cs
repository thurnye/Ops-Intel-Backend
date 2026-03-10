namespace OperationIntelligence.Core
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }

        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

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
    }
}
