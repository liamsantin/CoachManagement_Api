namespace CoachManagement_Api.Entity;

public class Opponent
{
    public int id_opponents { get; set; }
    public int fk_leagues_id { get; set; }
    public string name { get; set; } = string.Empty;
    public string? club { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
