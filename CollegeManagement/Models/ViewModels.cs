using System.ComponentModel.DataAnnotations;
using CollegeManagement.Services;

namespace CollegeManagement.Models;

public class LoginVm
{
    [Required(ErrorMessage = "Введите логин")] public string Login { get; set; } = "";
    [Required(ErrorMessage = "Введите пароль"), DataType(DataType.Password)] public string Password { get; set; } = "";
    public string? ReturnUrl { get; set; }
}

public class StudentForm
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите имя"), StringLength(50)] public string Name { get; set; } = "";
    [Required(ErrorMessage = "Введите фамилию"), StringLength(50)] public string Surname { get; set; } = "";
    [Range(15, 70, ErrorMessage = "Возраст студента от 15 до 70 лет")] public int Age { get; set; } = 17;
    public int? GroupId { get; set; }
}

public class TeacherForm
{
    [Required(ErrorMessage = "Введите имя")] public string Name { get; set; } = "";
    [Required(ErrorMessage = "Введите фамилию")] public string Surname { get; set; } = "";
    [Range(21, 80, ErrorMessage = "Возраст от 21 до 80")] public int Age { get; set; } = 35;
    [Required(ErrorMessage = "Укажите предмет")] public string Specialization { get; set; } = "";
    [Range(0, 60, ErrorMessage = "Стаж от 0 до 60")] public int Experience { get; set; }
}

public class GroupForm
{
    [Required(ErrorMessage = "Введите название")] public string Name { get; set; } = "";
    [Range(1, 4, ErrorMessage = "Курс от 1 до 4")] public int Course { get; set; } = 1;
}

public class SubjectForm
{
    [Required(ErrorMessage = "Введите название")] public string Name { get; set; } = "";
    [Range(1, 1000, ErrorMessage = "Часы от 1 до 1000")] public int Hours { get; set; } = 72;
    public int? TeacherId { get; set; }
}

public class UserForm
{
    [Required] public string Login { get; set; } = "";
    [Required] public string FullName { get; set; } = "";
    [Required, MinLength(4)] public string Password { get; set; } = "";
    [Required] public string Role { get; set; } = "Student";
    public int? PersonId { get; set; }
}

public class GradeIndexVm
{
    public List<Group> Groups { get; set; } = new();
    public List<Subject> Subjects { get; set; } = new();
    public List<Student> Students { get; set; } = new();
    public int? GroupId { get; set; }
    public int? SubjectId { get; set; }
}

public class AttendanceIndexVm
{
    public List<Group> Groups { get; set; } = new();
    public List<Student> Students { get; set; } = new();
    public Dictionary<int, AttendanceStatus> Marks { get; set; } = new();
    public int? GroupId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStats Stats { get; set; } = new();
}

public class AttendanceReportVm
{
    public List<Group> Groups { get; set; } = new();
    public int? GroupId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public List<(Student Student, AttendanceStats Stats)> Rows { get; set; } = new();
    public AttendanceStats Total { get; set; } = new();
}

public class AdminVm
{
    public CollegeReport Report { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<Subject> Subjects { get; set; } = new();
    public List<Teacher> Teachers { get; set; } = new();
    public List<Student> Students { get; set; } = new();
}
