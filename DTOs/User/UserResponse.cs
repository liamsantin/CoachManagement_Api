namespace CoachManagement_Api.DTOs.User;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
