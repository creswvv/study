using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

[Authorize]
public class TeacherController : Controller
{
    private readonly CollegeDbContext _db;
    public TeacherController(CollegeDbContext db) => _db = db;

    public IActionResult Index()
    {
        ViewBag.Subjects = _db.Subjects.Include(s => s.Teacher).ToList();
        return View(_db.Teachers.OrderBy(t => t.Surname).ToList());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new TeacherForm());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public IActionResult Create(TeacherForm f)
    {
        if (!ModelState.IsValid) return View(f);
        try
        {
            _db.Teachers.Add(new Teacher(f.Name, f.Surname, f.Age, f.Specialization, f.Experience));
            _db.SaveChanges();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(f);
        }
        TempData["Ok"] = "Преподаватель добавлен";
        return RedirectToAction(nameof(Index));
    }

    // Задача 10: стаж меняется только через UpdateExperience()
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public IActionResult UpdateExperience(int id, int years)
    {
        var t = _db.Teachers.Find(id);
        if (t == null) return NotFound();
        try
        {
            t.UpdateExperience(years);
            _db.SaveChanges();
            TempData["Ok"] = "Стаж обновлён";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var t = _db.Teachers.Find(id);
        if (t != null)
        {
            _db.Teachers.Remove(t);
            _db.SaveChanges();
            TempData["Ok"] = "Преподаватель удалён";
        }
        return RedirectToAction(nameof(Index));
    }
}
