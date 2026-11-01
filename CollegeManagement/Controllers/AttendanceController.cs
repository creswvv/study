using CollegeManagement.Data;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.Controllers;

[Authorize]
public class AttendanceController : Controller
{
    private readonly CollegeDbContext _db;
    private readonly AttendanceManager _manager;

    public AttendanceController(CollegeDbContext db, AttendanceManager manager)
    {
        _db = db;
        _manager = manager;
    }

    public IActionResult Index(int? groupId, DateTime? date)
    {
        var day = (date ?? DateTime.Today).Date;
        var vm = new AttendanceIndexVm { Groups = _db.Groups.OrderBy(g => g.Name).ToList(), Date = day };
        vm.GroupId = groupId ?? vm.Groups.FirstOrDefault()?.Id;
        if (vm.GroupId.HasValue)
        {
            vm.Students = _db.Students.Where(s => s.GroupId == vm.GroupId).OrderBy(s => s.Surname).ToList();
            vm.Marks = _manager.GetDay(vm.GroupId.Value, day);
            vm.Stats = _manager.GetStats(vm.GroupId, day, day);
        }
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Teacher")]
    public IActionResult Save(int groupId, DateTime date, Dictionary<int, AttendanceStatus> status)
    {
        foreach (var (studentId, st) in status)
            _manager.Mark(studentId, date, st);
        _manager.Save();
        TempData["Ok"] = "Посещаемость сохранена";
        return RedirectToAction(nameof(Index), new { groupId, date = date.ToString("yyyy-MM-dd") });
    }

    public IActionResult Report(int? groupId, DateTime? from, DateTime? to)
    {
        var vm = new AttendanceReportVm
        {
            Groups = _db.Groups.OrderBy(g => g.Name).ToList(),
            From = (from ?? DateTime.Today.AddDays(-30)).Date,
            To = (to ?? DateTime.Today).Date
        };
        vm.GroupId = groupId ?? vm.Groups.FirstOrDefault()?.Id;
        if (vm.GroupId.HasValue)
        {
            vm.Rows = _manager.GetGroupReport(vm.GroupId.Value, vm.From, vm.To);
            vm.Total = _manager.GetStats(vm.GroupId, vm.From, vm.To);
        }
        return View(vm);
    }
}
