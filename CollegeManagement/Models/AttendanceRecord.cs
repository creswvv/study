namespace CollegeManagement.Models;

public enum AttendanceStatus { Present = 0, Absent = 1, Late = 2 }

public class AttendanceRecord
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public Student? Student { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
}
