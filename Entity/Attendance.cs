namespace CoachManagement_Api.Entity;

public class Attendance
{
    public int id_players { get; set; }
    public int id_trainings { get; set; }
    public string? notes { get; set; }
    public bool retard { get; set; }
    public string? motif { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
