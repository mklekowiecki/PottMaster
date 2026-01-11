namespace PottMasterLib.Models;

public interface IUserProfile
{
    string Id { get; set; }
    string Email { get; set; }
    string Initials { get; set; }
    DateTime CreatedAt { get; set; }
}
