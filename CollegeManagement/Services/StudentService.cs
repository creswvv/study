using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Services;

public class StudentService
{
    private readonly CollegeDbContext _db;
    public StudentService(CollegeDbContext db) => _db = db;

    public List<Student> GetAll(string? search = null, int? groupId = null)
    {
        var q = _db.Students.Include(s => s.Group).Include(s => s.Grades).AsQueryable();
        if (groupId.HasValue) q = q.Where(s => s.GroupId == groupId);
        var list = q.OrderBy(s => s.Surname).ThenBy(s => s.Name).ToList();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var t = search.Trim().ToLowerInvariant();
            list = list.Where(s => s.FullName.ToLowerInvariant().Contains(t) || s.StudentId.ToLowerInvariant().Contains(t)).ToList();
        }
        return list;
    }

    public Student? Get(int id) =>
        _db.Students.Include(s => s.Group).Include(s => s.Grades).ThenInclude(g => g.Subject).FirstOrDefault(s => s.Id == id);

    public Student Create(string name, string surname, int age, int? groupId)
    {
        var group = groupId.HasValue ? _db.Groups.Include(g => g.Students).FirstOrDefault(g => g.Id == groupId) : null;
        var student = new Student(name, surname, age, group);
        _db.Students.Add(student);
        _db.SaveChanges();
        return student;
    }

    public bool Update(int id, string name, string surname, int age, int? groupId)
    {
        var student = _db.Students.Include(s => s.Group).FirstOrDefault(s => s.Id == id);
        if (student == null) return false;
        student.Update(name, surname, age);

        if (student.GroupId != groupId)
        {
            if (groupId.HasValue)
            {
                var g = _db.Groups.Include(x => x.Students).FirstOrDefault(x => x.Id == groupId);
                g?.AddStudent(student);
            }
            else student.SetGroup(null);
        }
        _db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var s = _db.Students.Find(id);
        if (s == null) return false;
        _db.Students.Remove(s);
        _db.SaveChanges();
        return true;
    }
}
