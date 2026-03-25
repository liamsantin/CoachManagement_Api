namespace CoachManagement_Api.Entity;

public class Team
{
    public int id_teams { get; set; }
    public int fk_users_id { get; set; }
    public int? fk_clubs_id { get; set; }
    public int fk_leagues_id { get; set; }
    public string name { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
