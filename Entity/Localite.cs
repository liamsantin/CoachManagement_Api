namespace CoachManagement_Api.Entity;

public class Localite
{
    public int id_localites { get; set; }
    public int fk_cantons_id { get; set; }
    public string npa { get; set; } = string.Empty;
    public string localite { get; set; } = string.Empty;
}
