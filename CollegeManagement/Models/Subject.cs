namespace CollegeManagement.Models;

/// <summary>Задачи 14, 15. Поля названия, часов и преподавателя закрыты.</summary>
public class Subject
{
    private string _name = "";
    private int _hours;

    public int Id { get; private set; }
    public string Name { get => _name; private set => _name = value; }
    public int Hours { get => _hours; private set => _hours = value; }
    public int? TeacherId { get; private set; }
    public Teacher? Teacher { get; private set; }

    private Subject() { }

    public Subject(string name, int hours) => Update(name, hours);

    public void Update(string name, int hours)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Название предмета обязательно.");
        if (hours <= 0) throw new ArgumentOutOfRangeException(nameof(hours), "Часы должны быть больше нуля.");
        _name = name.Trim();
        _hours = hours;
    }

    // Задача 15
    public void AssignTeacher(Teacher teacher)
    {
        if (teacher == null) throw new ArgumentNullException(nameof(teacher), "Преподаватель не может быть null.");
        Teacher = teacher;
    }
}
