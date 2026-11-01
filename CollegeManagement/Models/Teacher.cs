namespace CollegeManagement.Models;

/// <summary>Задачи 9, 10, 18, 19.</summary>
public class Teacher : Person
{
    private int _experience;

    public string Specialization { get; private set; } = "";

    // Задача 10: стаж читается свободно, а менять его можно только через UpdateExperience()
    public int Experience { get => _experience; private set => _experience = value; }

    protected override int MinAge => 21;
    protected override int MaxAge => 80;

    private Teacher() { }

    public Teacher(string name, string surname, int age, string specialization, int experience)
        : base(name, surname, age)
    {
        Specialization = RequireText(specialization, "Предмет");
        UpdateExperience(experience);
    }

    public void UpdateExperience(int years)
    {
        if (years < 0 || years > Math.Max(0, Age - 18))
            throw new ArgumentOutOfRangeException(nameof(years), "Недопустимый стаж.");
        _experience = years;
    }

    public void UpdateSpecialization(string specialization) =>
        Specialization = RequireText(specialization, "Предмет");

    // Задача 18
    public string GetTeacherInfo() =>
        $"{DisplayBasicInfo()}; предмет: {Specialization}; стаж: {Experience} лет; в колледже {CalculateStatistics()} дн.";
}
