using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Player;

public class PlayerUpdateRequest
{
    [Required]
    public int fk_teams_id { get; set; }

    [Required]
    public int fk_positions_id { get; set; }

    [Required]
    [MaxLength(50)]
    public string nom { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string prenom { get; set; } = string.Empty;

    public int? numeroMaillot { get; set; }

    [MaxLength(100)]
    public string? email { get; set; }

    [MaxLength(15)]
    public string? phone { get; set; }

    public int? anneeExp { get; set; }

    public decimal? poids { get; set; }

    public decimal? taille { get; set; }

    public DateOnly? dateNaiss { get; set; }

    public DateOnly? dateArrivee { get; set; }

    [MaxLength(200)]
    public string? photoUrl { get; set; }
}
