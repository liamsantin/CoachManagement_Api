namespace CoachManagement_Api.Entity;

public class Participation
{
    public int id_players { get; set; }
    public int id_matchs { get; set; }
    public string? noteOn10 { get; set; }
    public string? notes { get; set; }
    public decimal? tempsJeu { get; set; }
    public int but { get; set; }
    public int passeD { get; set; }
    public int cartonJaune { get; set; }
    public int cartonRouge { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
