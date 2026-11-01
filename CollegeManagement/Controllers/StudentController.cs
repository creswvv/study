using CollegeManagement.Data;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

[Authorize]
public class StudentController : Controller
{
    private readonly StudentService _students;
    private readonly AttendanceManager _attendance;
    private readonly CollegeDbContext _db;

    public StudentController(StudentService students, AttendanceManager attendance, CollegeDbContext db)
    {
        _students = students;
        _attendance = attendance;
        _db = db;
    }

    private void FillGroups(int? selected)
    {
        ViewBag.Groups = new SelectList(_db.Groups.OrderBy(g => g.Name).ToList(), "Id", "Name", selected);
    }

    public IActionResult Index(string? search, int? groupId)
    {
        FillGroups(groupId);
        ViewBag.Search = search;
        return View(_students.GetAll(search, groupId));
    }

    public IActionResult Details(int id)
    {
        var s = _students.Get(id);
        if (s == null) return NotFound();
        ViewBag.Stats = _attendance.GetStats(null, DateTime.Today.AddDays(-30), DateTime.Today, id);
        return View(s);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public IActionResult Create()
    {
        FillGroups(null);
        return View("Form", new StudentForm());
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Teacher")]
    public IActionResult Create(StudentForm f)
    {
        FillGroups(f.GroupId);
        if (!ModelState.IsValid) return View("Form", f);
        try
        {
            _students.Create(f.Name, f.Surname, f.Age, f.GroupId);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Form", f);
        }
        TempData["Ok"] = "Студент добавлен";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Teacher")]
    public IActionResult Edit(int id)
    {
        var s = _students.Get(id);
        if (s == null) return NotFound();
        FillGroups(s.GroupId);
        return View("Form", new StudentForm { Id = s.Id, Name = s.GetName(), Surname = s.GetSurname(), Age = s.GetAge(), GroupId = s.GroupId });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Teacher")]
    public IActionResult Edit(StudentForm f)
    {
        FillGroups(f.GroupId);
        if (!ModelState.IsValid) return View("Form", f);
        try
        {
            if (!_students.Update(f.Id, f.Name, f.Surname, f.Age, f.GroupId)) return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Form", f);
        }
        TempData["Ok"] = "Данные студента обновлены";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        _students.Delete(id);
        TempData["Ok"] = "Студент удалён";
        return RedirectToAction(nameof(Index));
    }
}
