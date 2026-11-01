namespace CollegeManagement.Models;

/// <summary>Задачи 1-8, 17, 19. public-класс, наследник Person.</summary>
public class Student : Person
{
    private readonly List<Grade> _grades = new();

    // Задача 8: StudentId можно прочитать, но нельзя изменить после создания.
    public string StudentId { get; private set; } = "";
    public int? GroupId { get; private set; }
    public Group? Group { get; private set; }
    public IReadOnlyCollection<Grade> Grades => _grades.AsReadOnly();
    public double Average => OpenGradeBook().Average;

    // Задача 4: SetAge() из Person проверяет диапазон, для студента это 15-70.
    protected override int MinAge => 15;
    protected override int MaxAge => 70;

    private Student() { }                                   // для EF Core

    // Задача 5: публичный конструктор
    public Student(string name, string surname, int age, Group? group) : base(name, surname, age)
    {
        ValidateStudentData();                              // задача 6
        StudentId = "AD" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        group?.AddStudent(this);
    }

    // Задача 2: публичные методы для получения закрытых данных
    public string GetName() => _name;
    public string GetSurname() => _surname;
    public int GetAge() => _age;

    // Задача 6: приватный метод проверки
    private void ValidateStudentData()
    {
        if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_surname))
            throw new ArgumentException("Имя и фамилия обязательны.");
        if (_age < MinAge || _age > MaxAge)
            throw new ArgumentOutOfRangeException(nameof(Age), "Недопустимый возраст студента.");
    }

    // Задача 7
    public string GetStudentInfo() =>
        $"{DisplayBasicInfo()}; группа: {Group?.Name ?? "не назначена"}; ID: {StudentId}; в колледже {CalculateStatistics()} дн.";

    public void Update(string name, string surname, int age)
    {
        Name = name;
        Surname = surname;
        SetAge(age);
    }

    public GradeBook OpenGradeBook() => new GradeBook(_grades);

    internal void SetGroup(Group? group)
    {
        Group = group;
        if (group == null) GroupId = null;
    }
}
