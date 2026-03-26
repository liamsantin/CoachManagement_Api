using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.PlayersLineup;

public class PlayersLineupCreateRequest
{
    [Required] public int fk_players_id { get; set; }
    [Required] public int fk_lineup_id { get; set; }
    [Required] public int fk_positions_id { get; set; }
    public bool titulaire { get; set; } = true;
    public int? numMaillot { get; set; }
    public bool capitaine { get; set; } = false;
}
