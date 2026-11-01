using CollegeManagement.Models;

namespace CollegeManagement.Data;

public static class DbSeeder
{
    public static void Seed(CollegeDbContext db)
    {
        db.Database.EnsureCreated();
        if (db.Groups.Any()) return;

        var rnd = new Random(42);

        var groups = new[]
        {
            new Group("ИС-11", 1), new Group("ИС-21", 2), new Group("ПО-31", 3), new Group("ПО-41", 4)
        };
        db.Groups.AddRange(groups);

        var teachers = new[]
        {
            new Teacher("Мария", "Иванова", 38, "Программирование", 14),
            new Teacher("Ержан", "Смагулов", 45, "Базы данных", 20),
            new Teacher("Алексей", "Петров", 33, "Веб-разработка", 8),
            new Teacher("Айгуль", "Касенова", 41, "Математика", 17),
            new Teacher("Елена", "Браун", 36, "Английский язык", 11)
        };
        db.Teachers.AddRange(teachers);

        var subjects = new[]
        {
            new Subject("Программирование", 120), new Subject("Базы данных", 96),
            new Subject("Веб-разработка", 108), new Subject("Математика", 80), new Subject("Английский язык", 64)
        };
        for (int i = 0; i < subjects.Length; i++) subjects[i].AssignTeacher(teachers[i]);
        db.Subjects.AddRange(subjects);

        string[] surnames = { "Ахметов", "Серикова", "Иванов", "Нурланова", "Смирнов", "Касымов", "Петрова", "Оспанов",
            "Ким", "Жумабаева", "Соколов", "Байжанов", "Лебедева", "Токаев", "Морозова", "Темирбеков", "Кузнецова",
            "Абдрахманов", "Попова", "Мустафин", "Волкова", "Еримбетов", "Орлова", "Сарсенов" };
        string[] names = { "Данияр", "Алия", "Дмитрий", "Айдана", "Максим", "Ерлан", "Анна", "Тимур",
            "Денис", "Мадина", "Артём", "Нурсултан", "Ольга", "Бауыржан", "Дарья", "Расул", "Ксения",
            "Алихан", "Виктория", "Ренат", "Полина", "Арман", "Ева", "Азамат" };

        var students = new List<Student>();
        for (int i = 0; i < surnames.Length; i++)
        {
            var g = groups[i % groups.Length];
            students.Add(new Student(names[i], surnames[i], 15 + g.Course + rnd.Next(0, 2), g));
        }
        db.Students.AddRange(students);
        db.SaveChanges();

        // оценки
        foreach (var s in students)
        {
            var book = s.OpenGradeBook();
            for (int k = 0; k < 6; k++)
            {
                int v = rnd.Next(100) < 8 ? 2 : rnd.Next(3, 6);
                book.AddGrade(new Grade(subjects[rnd.Next(subjects.Length)], v, DateTime.Today.AddDays(-rnd.Next(0, 28))));
            }
        }
        db.SaveChanges();

        // посещаемость за 30 дней (будни)
        for (int d = 29; d >= 0; d--)
        {
            var day = DateTime.Today.AddDays(-d);
            if (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
            foreach (var s in students)
            {
                int r = rnd.Next(100);
                var st = r < 87 ? AttendanceStatus.Present : r < 93 ? AttendanceStatus.Late : AttendanceStatus.Absent;
                db.Attendance.Add(new AttendanceRecord { PersonId = s.Id, Date = day, Status = st });
            }
        }

        // пользователи
        db.Accounts.Add(new AdminUser("admin", "Администратор колледжа", "admin123"));
        db.Accounts.Add(new TeacherUser("teacher", teachers[0].FullName, "teacher123", teachers[0].Id));
        db.Accounts.Add(new StudentUser("student", students[0].FullName, "student123", students[0].Id));
        db.SaveChanges();
    }
}
