using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Club;

public class ClubUpdateRequest
{
    [Required]
    [MaxLength(50)]
    public string name { get; set; } = string.Empty;
}
