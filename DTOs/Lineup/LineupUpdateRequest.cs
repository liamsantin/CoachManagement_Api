using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Lineup;

public class LineupUpdateRequest
{
    [Required] public int fk_formations_id { get; set; }
    [Required, MaxLength(50)] public string name { get; set; } = string.Empty;
    public string? notes { get; set; }
}
