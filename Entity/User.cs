namespace CoachManagement_Api.Entity;

public class User
{
    public int id_users { get; set; }
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string? email { get; set; }
    public string? phone { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
