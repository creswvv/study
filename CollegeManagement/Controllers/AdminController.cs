using CollegeManagement.Data;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUser = CollegeManagement.Models.User;

namespace CollegeManagement.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly CollegeDbContext _db;
    private readonly ReportManager _reports;
    private readonly ICollegeInfo _info;

    public AdminController(CollegeDbContext db, ReportManager reports, ICollegeInfo info)
    {
        _db = db;
        _reports = reports;
        _info = info;
    }

    public IActionResult Index()
    {
        return View(new AdminVm
        {
            Report = _reports.GetReport(),
            Users = _db.Accounts.OfType<AppUser>().OrderBy(u => u.LoginName).ToList(),
            Subjects = _db.Subjects.Include(s => s.Teacher).OrderBy(s => s.Name).ToList(),
            Teachers = _db.Teachers.OrderBy(t => t.Surname).ToList(),
            Students = _db.Students.OrderBy(s => s.Surname).ToList()
        });
    }

    // Задача 22: internal-методы CollegeSettings доступны только внутри сборки
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateSettings(string collegeName, string academicYear)
    {
        var settings = (CollegeSettings)_info;
        settings.UpdateName(collegeName);
        settings.UpdateAcademicYear(academicYear);
        TempData["Ok"] = "Настройки колледжа обновлены (до перезапуска приложения)";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateUser(UserForm f)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Заполните все поля пользователя (пароль от 4 символов)";
            return RedirectToAction(nameof(Index));
        }
        if (_db.Accounts.Any(u => u.LoginName == f.Login.Trim().ToLower()))
        {
            TempData["Error"] = "Такой логин уже существует";
            return RedirectToAction(nameof(Index));
        }
        try
        {
            AppUser user = f.Role switch
            {
                "Admin" => new AdminUser(f.Login, f.FullName, f.Password),
                "Teacher" => new TeacherUser(f.Login, f.FullName, f.Password, f.PersonId),
                _ => new StudentUser(f.Login, f.FullName, f.Password, f.PersonId)
            };
            _db.Accounts.Add(user);
            _db.SaveChanges();
            TempData["Ok"] = "Пользователь создан";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DeleteUser(int id)
    {
        var u = _db.Accounts.Find(id);
        if (u != null && u.LoginName != "admin")
        {
            _db.Accounts.Remove(u);
            _db.SaveChanges();
            TempData["Ok"] = "Пользователь удалён";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateSubject(SubjectForm f)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Проверьте название и часы предмета"; return RedirectToAction(nameof(Index)); }
        try
        {
            var subject = new Subject(f.Name, f.Hours);
            var teacher = f.TeacherId.HasValue ? _db.Teachers.Find(f.TeacherId) : null;
            if (teacher != null) subject.AssignTeacher(teacher);
            _db.Subjects.Add(subject);
            _db.SaveChanges();
            TempData["Ok"] = "Предмет добавлен";
        }
        catch (ArgumentException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    // Задача 15
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AssignTeacher(int subjectId, int? teacherId)
    {
        var subject = _db.Subjects.Find(subjectId);
        var teacher = teacherId.HasValue ? _db.Teachers.Find(teacherId) : null;
        if (subject == null) return NotFound();
        try
        {
            subject.AssignTeacher(teacher!);
            _db.SaveChanges();
            TempData["Ok"] = "Преподаватель назначен";
        }
        catch (ArgumentNullException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DeleteSubject(int id)
    {
        var s = _db.Subjects.Find(id);
        if (s != null) { _db.Subjects.Remove(s); _db.SaveChanges(); TempData["Ok"] = "Предмет удалён"; }
        return RedirectToAction(nameof(Index));
    }
}
