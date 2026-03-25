namespace CoachManagement_Api.Entity;

public class Season
{
    public int id_seasons { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public string? name { get; set; }
    public string? notes { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
