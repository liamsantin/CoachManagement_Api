using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Team;

public class TeamUpdateRequest
{
    public int? fk_clubs_id { get; set; }

    [Required]
    public int fk_leagues_id { get; set; }

    [Required]
    [MaxLength(50)]
    public string name { get; set; } = string.Empty;
}
