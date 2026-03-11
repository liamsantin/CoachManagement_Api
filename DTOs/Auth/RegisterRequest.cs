using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }
}
