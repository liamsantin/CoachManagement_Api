using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Match;

public class MatchCreateRequest
{
    [Required] public int fk_teams_id { get; set; }
    [Required] public int fk_opponents_id { get; set; }
    [Required] public int fk_status_id { get; set; }
    [Required] public int fk_types_id { get; set; }
    [Required] public int fk_seasons_id { get; set; }
    [Required] public int fk_formations_id { get; set; }
    [Required] public int fk_localites_id { get; set; }

    public int? scoreEquipe { get; set; }
    public int? scoreOpponent { get; set; }
    public decimal? temps { get; set; }
    public string? stade { get; set; }
    public string? arbitre { get; set; }

    [Required] public DateOnly date { get; set; }
    public string? name { get; set; }
    [Required] public DateTime startDate { get; set; }
    [Required] public DateTime endDate { get; set; }

    [Required, MaxLength(50)] public string lineup_name { get; set; } = string.Empty;
    public string? lineup_notes { get; set; }
}
