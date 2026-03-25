namespace CoachManagement_Api.Entity;

public class Club
{
    public int id_clubs { get; set; }
    public int fk_users_id { get; set; }
    public string name { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
