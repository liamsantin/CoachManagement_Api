using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Participation;

public class ParticipationCreateRequest
{
    [Required] public int id_players { get; set; }
    [Required] public int id_matchs { get; set; }
    public string? noteOn10 { get; set; }
    public string? notes { get; set; }
    public decimal? tempsJeu { get; set; }
    public int but { get; set; }
    public int passeD { get; set; }
    public int cartonJaune { get; set; }
    public int cartonRouge { get; set; }
}
