namespace CoachManagement_Api.Entity;

public class Match
{
    public int id_events { get; set; }
    public int fk_teams_id { get; set; }
    public int fk_opponents_id { get; set; }
    public int fk_status_id { get; set; }
    public int fk_types_id { get; set; }
    public int fk_seasons_id { get; set; }
    public int fk_lineup_id { get; set; }
    public int fk_localites_id { get; set; }
    public int? scoreEquipe { get; set; }
    public int? scoreOpponent { get; set; }
    public decimal? temps { get; set; }
    public string? stade { get; set; }
    public string? arbitre { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
