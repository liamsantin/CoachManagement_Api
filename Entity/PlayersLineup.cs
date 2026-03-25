namespace CoachManagement_Api.Entity;

public class PlayersLineup
{
    public int id_playersLineup { get; set; }
    public int fk_players_id { get; set; }
    public int fk_lineup_id { get; set; }
    public int fk_positions_id { get; set; }
    public bool titulaire { get; set; }
    public int? numMaillot { get; set; }
    public bool capitaine { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
