namespace CoachManagement_Api.DTOs.Attendance;

public class AttendanceUpdateRequest
{
    public string? notes { get; set; }

    public bool retard { get; set; }

    public string? motif { get; set; }
}
