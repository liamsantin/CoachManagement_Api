using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Club;

public class ClubCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string name { get; set; } = string.Empty;
}
