namespace CoachManagement_Api.Entity;

public class Training
{
    public int id_events { get; set; }
    public int fk_localites_id { get; set; }
    public int? fk_types_id { get; set; }
    public string? description { get; set; }
    public int nbrPlayer { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
