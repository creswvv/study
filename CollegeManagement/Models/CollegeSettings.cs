namespace CollegeManagement.Models;

/// <summary>Публичный «только для чтения» интерфейс - его используют контроллеры и представления.</summary>
public interface ICollegeInfo
{
    string Name { get; }
    string AcademicYear { get; }
}

/// <summary>
/// Задачи 21, 22. Класс internal - виден только внутри сборки проекта.
/// Методы изменения настроек тоже internal.
/// </summary>
internal class CollegeSettings : ICollegeInfo
{
    public string Name { get; private set; }
    public string AcademicYear { get; private set; }

    public CollegeSettings(string name, string academicYear)
    {
        Name = name;
        AcademicYear = academicYear;
    }

    internal void UpdateName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
    }

    internal void UpdateAcademicYear(string year)
    {
        if (!string.IsNullOrWhiteSpace(year)) AcademicYear = year.Trim();
    }
}
