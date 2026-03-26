using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Replacement;

public class ReplacementCreateRequest
{
    [Required] public decimal minute { get; set; }
    [Required] public int fk_matchs_id { get; set; }
    [Required] public int fk_play_entering { get; set; }
    [Required] public int fk_play_outgoing { get; set; }
}
