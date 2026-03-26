namespace CoachManagement_Api.Entity;

public class Event
{
    public int id_events { get; set; }
    public int fk_teams_id { get; set; }
    public DateOnly date { get; set; }
    public string? name { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
