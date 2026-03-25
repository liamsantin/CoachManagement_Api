namespace CoachManagement_Api.Entity;

public class Player
{
    public int id_players { get; set; }
    public int fk_teams_id { get; set; }
    public int fk_positions_id { get; set; }
    public string nom { get; set; } = string.Empty;
    public string prenom { get; set; } = string.Empty;
    public int? numeroMaillot { get; set; }
    public string? email { get; set; }
    public string? phone { get; set; }
    public int? anneeExp { get; set; }
    public decimal? poids { get; set; }
    public decimal? taille { get; set; }
    public DateOnly? dateNaiss { get; set; }
    public DateOnly? dateArrivee { get; set; }
    public string? photoUrl { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
