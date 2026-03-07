using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace OperationIntelligence.DB;

public class ApplicationUser : IdentityUser
{

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
}
