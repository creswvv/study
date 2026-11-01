using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

[Authorize]
public class GradeController : Controller
{
    private readonly CollegeDbContext _db;
    public GradeController(CollegeDbContext db) => _db = db;

    public IActionResult Index(int? groupId, int? subjectId)
    {
        var vm = new GradeIndexVm
        {
            Groups = _db.Groups.OrderBy(g => g.Name).ToList(),
            Subjects = _db.Subjects.OrderBy(s => s.Name).ToList()
        };
        vm.GroupId = groupId ?? vm.Groups.FirstOrDefault()?.Id;
        vm.SubjectId = subjectId ?? vm.Subjects.FirstOrDefault()?.Id;
        vm.Students = _db.Students.Include(s => s.Grades)
            .Where(s => s.GroupId == vm.GroupId).OrderBy(s => s.Surname).ToList();

        // студент видит только себя
        if (User.IsInRole("Student"))
        {
            var pid = User.FindFirst("PersonId")?.Value;
            vm.Students = vm.Students.Where(s => s.Id.ToString() == pid).ToList();
        }
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Teacher")]
    public IActionResult Add(int studentId, int subjectId, int value, string? comment, int groupId)
    {
        var student = _db.Students.Include(s => s.Grades).FirstOrDefault(s => s.Id == studentId);
        var subject = _db.Subjects.Find(subjectId);
        if (student == null || subject == null) return NotFound();
        try
        {
            student.OpenGradeBook().AddGrade(new Grade(subject, value, DateTime.Today, comment));
            _db.SaveChanges();
            TempData["Ok"] = "Оценка добавлена";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { groupId, subjectId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Teacher")]
    public IActionResult Remove(int id, int groupId, int subjectId)
    {
        var grade = _db.Grades.Find(id);
        if (grade != null)
        {
            var student = _db.Students.Include(s => s.Grades).First(s => s.Id == grade.PersonId);
            var tracked = student.Grades.First(g => g.Id == id);
            student.OpenGradeBook().RemoveGrade(tracked);
            _db.Grades.Remove(tracked);
            _db.SaveChanges();
            TempData["Ok"] = "Оценка удалена";
        }
        return RedirectToAction(nameof(Index), new { groupId, subjectId });
    }
}
