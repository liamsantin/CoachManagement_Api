namespace CoachManagement_Api.DTOs.PlayersLineup;

public class PlayersLineupUpdateRequest
{
    public int fk_positions_id { get; set; }
    public bool titulaire { get; set; } = true;
    public int? numMaillot { get; set; }
    public bool capitaine { get; set; } = false;
}
