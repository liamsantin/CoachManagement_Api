namespace CoachManagement_Api.DTOs.Match;

public class MatchResponse
{
    public int id_events { get; set; }
    public int fk_teams_id { get; set; }
    public int fk_opponents_id { get; set; }
    public int fk_status_id { get; set; }
    public int fk_types_id { get; set; }
    public int fk_seasons_id { get; set; }
    public int fk_lineup_id { get; set; }
    public int fk_formations_id { get; set; }
    public string lineup_name { get; set; } = string.Empty;
    public string? lineup_notes { get; set; }
    public int fk_localites_id { get; set; }
    public int? scoreEquipe { get; set; }
    public int? scoreOpponent { get; set; }
    public decimal? temps { get; set; }
    public string? stade { get; set; }
    public string? arbitre { get; set; }
    public DateOnly date { get; set; }
    public string? name { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public DateTime event_created_at { get; set; }
    public DateTime event_updated_at { get; set; }
    public DateTime match_created_at { get; set; }
    public DateTime match_updated_at { get; set; }
}
