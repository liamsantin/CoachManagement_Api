namespace CoachManagement_Api.DTOs.Participation;

public class ParticipationUpdateRequest
{
    public string? noteOn10 { get; set; }
    public string? notes { get; set; }
    public decimal? tempsJeu { get; set; }
    public int but { get; set; }
    public int passeD { get; set; }
    public int cartonJaune { get; set; }
    public int cartonRouge { get; set; }
}
