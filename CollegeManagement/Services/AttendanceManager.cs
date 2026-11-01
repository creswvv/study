using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Services;

public class AttendanceStats
{
    public int Present { get; init; }
    public int Absent { get; init; }
    public int Late { get; init; }
    public int Total => Present + Absent + Late;
    public double Percent => Total == 0 ? 0 : Math.Round((Present + Late) * 100.0 / Total, 1);
}

/// <summary>Задача 29. Данные о посещаемости (контекст БД) закрыты, наружу - только методы.</summary>
public class AttendanceManager
{
    private readonly CollegeDbContext _db;
    public AttendanceManager(CollegeDbContext db) => _db = db;

    // добавление или изменение отметки
    public void Mark(int studentId, DateTime date, AttendanceStatus status, string? note = null)
    {
        date = date.Date;
        var rec = _db.Attendance.FirstOrDefault(a => a.PersonId == studentId && a.Date == date);
        if (rec == null)
            _db.Attendance.Add(new AttendanceRecord { PersonId = studentId, Date = date, Status = status, Note = note });
        else
        {
            rec.Status = status;
            rec.Note = note ?? rec.Note;
        }
    }

    public bool Change(int recordId, AttendanceStatus status)
    {
        var rec = _db.Attendance.Find(recordId);
        if (rec == null) return false;
        rec.Status = status;
        _db.SaveChanges();
        return true;
    }

    public void Save() => _db.SaveChanges();

    public Dictionary<int, AttendanceStatus> GetDay(int groupId, DateTime date)
    {
        date = date.Date;
        return _db.Attendance
            .Where(a => a.Date == date && a.Student!.GroupId == groupId)
            .ToDictionary(a => a.PersonId, a => a.Status);
    }

    public AttendanceStats GetStats(int? groupId, DateTime from, DateTime to, int? studentId = null)
    {
        var list = _db.Attendance
            .Where(a => a.Date >= from.Date && a.Date <= to.Date)
            .Where(a => !groupId.HasValue || a.Student!.GroupId == groupId)
            .Where(a => !studentId.HasValue || a.PersonId == studentId)
            .Select(a => a.Status)
            .ToList();
        return new AttendanceStats
        {
            Present = list.Count(x => x == AttendanceStatus.Present),
            Absent = list.Count(x => x == AttendanceStatus.Absent),
            Late = list.Count(x => x == AttendanceStatus.Late)
        };
    }

    public List<(Student Student, AttendanceStats Stats)> GetGroupReport(int groupId, DateTime from, DateTime to)
    {
        var students = _db.Students.Where(s => s.GroupId == groupId).OrderBy(s => s.Surname).ToList();
        var recs = _db.Attendance.Where(a => a.Date >= from.Date && a.Date <= to.Date && a.Student!.GroupId == groupId).ToList();
        return students.Select(s =>
        {
            var mine = recs.Where(r => r.PersonId == s.Id).ToList();
            return (s, new AttendanceStats
            {
                Present = mine.Count(r => r.Status == AttendanceStatus.Present),
                Absent = mine.Count(r => r.Status == AttendanceStatus.Absent),
                Late = mine.Count(r => r.Status == AttendanceStatus.Late)
            });
        }).ToList();
    }
}
