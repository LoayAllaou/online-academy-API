using Microsoft.AspNetCore.Identity;

namespace ScaleUp.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Country { get; set; }
    public string? ProfilePictureUrl { get; set; } // store image URL or path
}
