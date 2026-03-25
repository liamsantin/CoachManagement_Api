namespace CoachManagement_Api.Entity;

public class Lineup
{
    public int id_lineup { get; set; }
    public int? fk_matchs_id { get; set; }
    public int fk_formations_id { get; set; }
    public string name { get; set; } = string.Empty;
    public string? notes { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
