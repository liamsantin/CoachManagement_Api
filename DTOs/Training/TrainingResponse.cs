namespace CoachManagement_Api.DTOs.Training;

public class TrainingResponse
{
    public int id_events { get; set; }
    public DateOnly date { get; set; }
    public string? name { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public int fk_teams_id { get; set; }
    public int fk_localites_id { get; set; }
    public int? fk_types_id { get; set; }
    public string? description { get; set; }
    public int nbrPlayer { get; set; }
    public DateTime event_created_at { get; set; }
    public DateTime event_updated_at { get; set; }
    public DateTime training_created_at { get; set; }
    public DateTime training_updated_at { get; set; }
}
