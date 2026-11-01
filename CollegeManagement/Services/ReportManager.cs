using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Services;

public record GroupRow(string Name, int Course, int Students, double Average);
public record TopStudent(string Name, string Group, double Average);
public record AttendantRow(string Name, double Percent, int Days);
public record RecentGrade(string Student, string StudentId, string Group, string Subject, int Value, string Initials);

public class CollegeReport
{
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
    public int GroupCount { get; set; }
    public int SubjectCount { get; set; }
    public double AverageGrade { get; set; }
    public double AttendancePercent { get; set; }
    public List<GroupRow> Groups { get; set; } = new();
    public List<TopStudent> Top { get; set; } = new();
}

public class DashboardData
{
    public CollegeReport Report { get; set; } = new();
    public DateTime Day { get; set; }
    public int Total { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public List<string> TrendLabels { get; set; } = new();
    public List<int> TrendValues { get; set; } = new();
    public List<string> GroupLabels { get; set; } = new();
    public List<int> GroupCounts { get; set; } = new();
    public int[] WeeklyAbsent { get; set; } = new int[5];
    public List<AttendantRow> TopAttendants { get; set; } = new();
    public List<RecentGrade> Recent { get; set; } = new();
}

/// <summary>Задача 24. GenerateReport() - protected internal: доступен всей сборке и наследникам.</summary>
public class ReportManager
{
    private readonly CollegeDbContext _db;
    public ReportManager(CollegeDbContext db) => _db = db;

    protected internal CollegeReport GenerateReport()
    {
        var groups = _db.Groups.AsNoTracking().Include(g => g.Students).ThenInclude(s => s.Grades)
            .OrderBy(g => g.Name).ToList();
        var all = groups.SelectMany(g => g.Students).ToList();

        var report = new CollegeReport
        {
            StudentCount = _db.Students.Count(),
            TeacherCount = _db.Teachers.Count(),
            GroupCount = groups.Count,
            SubjectCount = _db.Subjects.Count()
        };

        var values = _db.Grades.Select(g => g.Value).ToList();
        report.AverageGrade = values.Count == 0 ? 0 : Math.Round(values.Average(), 2);

        var from = DateTime.Today.AddDays(-30);
        var st = _db.Attendance.Where(a => a.Date >= from).Select(a => a.Status).ToList();
        report.AttendancePercent = st.Count == 0 ? 0 : Math.Round(st.Count(x => x != AttendanceStatus.Absent) * 100.0 / st.Count, 1);

        report.Groups = groups.Select(g =>
        {
            var gv = g.Students.SelectMany(s => s.Grades).Select(x => x.Value).ToList();
            return new GroupRow(g.Name, g.Course, g.Students.Count, gv.Count == 0 ? 0 : Math.Round(gv.Average(), 2));
        }).ToList();

        report.Top = all.Where(s => s.Grades.Count > 0)
            .Select(s => new TopStudent(s.FullName, groups.First(g => g.Students.Any(x => x.Id == s.Id)).Name, Math.Round(s.Average, 2)))
            .OrderByDescending(t => t.Average).Take(5).ToList();

        return report;
    }

    // публичная обёртка для контроллеров
    public CollegeReport GetReport() => GenerateReport();

    public DashboardData GetDashboard()
    {
        var d = new DashboardData { Report = GenerateReport() };
        d.Total = d.Report.StudentCount;

        var from = DateTime.Today.AddDays(-30);
        var recs = _db.Attendance.AsNoTracking().Include(a => a.Student).Where(a => a.Date >= from).ToList();

        d.Day = recs.Count > 0 ? recs.Max(r => r.Date) : DateTime.Today;
        var today = recs.Where(r => r.Date == d.Day).ToList();
        d.Present = today.Count(r => r.Status == AttendanceStatus.Present);
        d.Absent = today.Count(r => r.Status == AttendanceStatus.Absent);
        d.Late = today.Count(r => r.Status == AttendanceStatus.Late);

        var byDay = recs.GroupBy(r => r.Date).OrderBy(g => g.Key).ToList();
        d.TrendLabels = byDay.Select(g => g.Key.ToString("dd.MM")).ToList();
        d.TrendValues = byDay.Select(g => g.Count(r => r.Status != AttendanceStatus.Absent)).ToList();

        foreach (var r in recs.Where(r => r.Status == AttendanceStatus.Absent))
        {
            int dow = (int)r.Date.DayOfWeek;
            if (dow >= 1 && dow <= 5) d.WeeklyAbsent[dow - 1]++;
        }

        d.TopAttendants = recs.GroupBy(r => r.PersonId)
            .Select(g => new AttendantRow(
                g.First().Student!.FullName,
                Math.Round(g.Count(x => x.Status != AttendanceStatus.Absent) * 100.0 / g.Count(), 1),
                g.Count(x => x.Status != AttendanceStatus.Absent)))
            .OrderByDescending(a => a.Percent).ThenBy(a => a.Name).Take(6).ToList();

        var groups = _db.Groups.AsNoTracking().Include(g => g.Students).OrderBy(g => g.Name).ToList();
        d.GroupLabels = groups.Select(g => g.Name).ToList();
        d.GroupCounts = groups.Select(g => g.Students.Count).ToList();

        d.Recent = _db.Grades.AsNoTracking()
            .Include(g => g.Student).ThenInclude(s => s!.Group)
            .Include(g => g.Subject)
            .OrderByDescending(g => g.Date).ThenByDescending(g => g.Id)
            .Take(5).ToList()
            .Select(g => new RecentGrade(g.Student!.FullName, g.Student.StudentId, g.Student.Group?.Name ?? "-",
                g.Subject!.Name, g.Value, g.Student.Initials))
            .ToList();

        return d;
    }
}
