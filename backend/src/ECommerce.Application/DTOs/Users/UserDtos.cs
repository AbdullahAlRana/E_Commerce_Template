namespace ECommerce.Application.DTOs.Users;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}