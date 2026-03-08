using System.ComponentModel.DataAnnotations;

namespace OperationIntelligence.DB.Entities
{
    public class PlatformUser
    {
        [Key]
        [MaxLength(64)]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(512)]
        public string Avatar { get; set; } = string.Empty;

        public DateTime? Birthdate { get; set; }

        [MaxLength(50)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;
    }
}
