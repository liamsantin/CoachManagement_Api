namespace CoachManagement_Api.Entity;

public class Replacement
{
    public int id_replacements { get; set; }
    public decimal minute { get; set; }
    public int fk_matchs_id { get; set; }
    public int fk_play_entering { get; set; }
    public int fk_play_outgoing { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
