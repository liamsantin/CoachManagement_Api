namespace CoachManagement_Api.DTOs.Replacement;

public class ReplacementUpdateRequest
{
    public decimal minute { get; set; }
    public int fk_play_entering { get; set; }
    public int fk_play_outgoing { get; set; }
}
