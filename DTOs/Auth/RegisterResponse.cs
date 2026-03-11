namespace CoachManagement_Api.DTOs.Auth;

public class RegisterResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Message { get; set; } = "Inscription réussie.";
}
