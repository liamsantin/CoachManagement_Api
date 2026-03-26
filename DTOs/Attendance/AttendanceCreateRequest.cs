using System.ComponentModel.DataAnnotations;

namespace CoachManagement_Api.DTOs.Attendance;

public class AttendanceCreateRequest
{
    [Required]
    public int id_players { get; set; }

    /// <summary>Identifiant du training (colonne Trainings.id_events).</summary>
    [Required]
    public int id_trainings { get; set; }

    public string? notes { get; set; }

    public bool retard { get; set; }

    public string? motif { get; set; }
}
