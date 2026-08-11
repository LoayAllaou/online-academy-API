namespace ScaleUp.Core.DTOs;

public class UpdateUserInfoRequest
{
    public string FullName { get; set; }
    public DateTime? Birthday { get; set; }
    public string Country { get; set; }
    //public string ProfilePictureUrl { get; set; }
    public string Phone { get; set; }
}
