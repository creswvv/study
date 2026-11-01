namespace CollegeManagement.Models;

/// <summary>Задачи 11-13. Список студентов закрыт (private), снаружи он доступен только для чтения.</summary>
public class Group
{
    private readonly List<Student> _students = new();

    public int Id { get; private set; }
    public string Name { get; private set; } = "";
    public int Course { get; private set; }
    public IReadOnlyCollection<Student> Students => _students.AsReadOnly();

    private Group() { }

    public Group(string name, int course)
    {
        Rename(name, course);
    }

    public void Rename(string name, int course)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Название группы обязательно.");
        if (course < 1 || course > 4) throw new ArgumentOutOfRangeException(nameof(course), "Курс от 1 до 4.");
        Name = name.Trim();
        Course = course;
    }

    // Задача 12: добавляем, только если студента ещё нет в группе
    public bool AddStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        bool exists = _students.Any(s => ReferenceEquals(s, student) || (s.Id != 0 && s.Id == student.Id));
        if (exists) return false;
        _students.Add(student);
        student.SetGroup(this);
        return true;
    }

    // Задача 13: удаление только через этот метод
    public bool RemoveStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        var found = _students.FirstOrDefault(s => ReferenceEquals(s, student) || (s.Id != 0 && s.Id == student.Id));
        if (found == null) return false;
        _students.Remove(found);
        found.SetGroup(null);
        return true;
    }
}
