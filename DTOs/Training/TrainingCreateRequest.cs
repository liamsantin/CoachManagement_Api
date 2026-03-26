using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Training;

public class TrainingCreateRequest
{
    [Required]
    public int fk_teams_id { get; set; }

    [Required]
    public int fk_localites_id { get; set; }

    public int? fk_types_id { get; set; }

    public string? description { get; set; }

    public int nbrPlayer { get; set; }

    [Required]
    public DateOnly date { get; set; }

    public string? name { get; set; }

    [Required]
    public DateTime startDate { get; set; }

    [Required]
    public DateTime endDate { get; set; }
}
