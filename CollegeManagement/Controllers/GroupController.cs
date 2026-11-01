using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

[Authorize]
public class GroupController : Controller
{
    private readonly CollegeDbContext _db;
    public GroupController(CollegeDbContext db) => _db = db;

    public IActionResult Index() =>
        View(_db.Groups.Include(g => g.Students).ThenInclude(s => s.Grades).OrderBy(g => g.Name).ToList());

    public IActionResult Details(int id)
    {
        var g = _db.Groups.Include(x => x.Students).ThenInclude(s => s.Grades).FirstOrDefault(x => x.Id == id);
        if (g == null) return NotFound();
        ViewBag.Free = _db.Students.Where(s => s.GroupId == null).OrderBy(s => s.Surname).ToList();
        return View(g);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new GroupForm());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public IActionResult Create(GroupForm f)
    {
        if (!ModelState.IsValid) return View(f);
        try
        {
            _db.Groups.Add(new Group(f.Name, f.Course));
            _db.SaveChanges();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(f);
        }
        TempData["Ok"] = "Группа создана";
        return RedirectToAction(nameof(Index));
    }

    // Задача 12
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Teacher")]
    public IActionResult AddStudent(int id, int studentId)
    {
        var g = _db.Groups.Include(x => x.Students).FirstOrDefault(x => x.Id == id);
        var s = _db.Students.Find(studentId);
        if (g == null || s == null) return NotFound();
        bool added = g.AddStudent(s);
        TempData[added ? "Ok" : "Error"] = added ? "Студент добавлен в группу" : "Студент уже состоит в группе";
        _db.SaveChanges();
        return RedirectToAction(nameof(Details), new { id });
    }

    // Задача 13
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Teacher")]
    public IActionResult RemoveStudent(int id, int studentId)
    {
        var g = _db.Groups.Include(x => x.Students).FirstOrDefault(x => x.Id == id);
        var s = g?.Students.FirstOrDefault(x => x.Id == studentId);
        if (g == null || s == null) return NotFound();
        g.RemoveStudent(s);
        _db.SaveChanges();
        TempData["Ok"] = "Студент исключён из группы";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var g = _db.Groups.Find(id);
        if (g != null)
        {
            _db.Groups.Remove(g);
            _db.SaveChanges();
            TempData["Ok"] = "Группа удалена";
        }
        return RedirectToAction(nameof(Index));
    }
}
