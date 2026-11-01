namespace CollegeManagement.Models;

/// <summary>
/// Задачи 16, 19, 20, 23, 25. Базовый класс для Student и Teacher.
/// protected  - поля доступны наследникам;
/// private    - _internalNote виден ТОЛЬКО внутри Person (наследники его не видят);
/// protected internal - GetFullName(): наследникам и всей сборке;
/// private protected  - CalculateStatistics(): только наследникам и только внутри этой сборки.
/// </summary>
public abstract class Person
{
    protected string _name = "";
    protected string _surname = "";
    protected int _age;

    private string _internalNote = "";               // задача 20: доступно только в Person

    public int Id { get; protected set; }
    public DateTime RegisteredAt { get; protected set; } = DateTime.Today;

    public string Name { get => _name; set => _name = RequireText(value, "Имя"); }
    public string Surname { get => _surname; set => _surname = RequireText(value, "Фамилия"); }
    public int Age { get => _age; protected set => _age = value; }

    public string FullName => GetFullName();
    public int DaysInCollege => CalculateStatistics();

    protected virtual int MinAge => 15;
    protected virtual int MaxAge => 100;

    protected Person() { }

    protected Person(string name, string surname, int age)
    {
        Name = name;
        Surname = surname;
        SetAge(age);
        _internalNote = "создано через конструктор";
    }

    public virtual void SetAge(int age)
    {
        if (age < MinAge || age > MaxAge)
            throw new ArgumentOutOfRangeException(nameof(age), $"Возраст должен быть от {MinAge} до {MaxAge}.");
        _age = age;
    }

    protected internal string GetFullName() => $"{Surname} {Name}";

    protected string DisplayBasicInfo() => $"{GetFullName()}, {Age} лет";

    private protected int CalculateStatistics() => Math.Max(0, (DateTime.Today - RegisteredAt.Date).Days);

    protected static string RequireText(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Поле «{field}» не может быть пустым.");
        return value.Trim();
    }

    public string Initials => $"{Name.FirstOrDefault()}{Surname.FirstOrDefault()}".ToUpperInvariant();
}
