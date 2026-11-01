namespace CollegeManagement.Models;

public class Grade
{
    public int Id { get; private set; }
    public int PersonId { get; private set; }
    public Student? Student { get; private set; }
    public int SubjectId { get; private set; }
    public Subject? Subject { get; private set; }
    public int Value { get; private set; }
    public DateTime Date { get; private set; }
    public string? Comment { get; private set; }

    private Grade() { }

    public Grade(Subject subject, int value, DateTime date, string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(subject);
        if (!IsValid(value)) throw new ArgumentOutOfRangeException(nameof(value), "Оценка должна быть от 2 до 5.");
        Subject = subject;
        SubjectId = subject.Id;
        Value = value;
        Date = date.Date;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }

    public static bool IsValid(int value) => value is >= 2 and <= 5;
}

/// <summary>Задача 28. Список оценок закрыт, добавление и удаление - только через публичные методы.</summary>
public class GradeBook
{
    private readonly List<Grade> _grades;

    internal GradeBook(List<Grade> source) => _grades = source;

    public IReadOnlyList<Grade> Grades => _grades.AsReadOnly();
    public double Average => _grades.Count == 0 ? 0 : _grades.Average(g => g.Value);

    public double AverageFor(int subjectId)
    {
        var list = _grades.Where(g => g.SubjectId == subjectId).ToList();
        return list.Count == 0 ? 0 : list.Average(g => g.Value);
    }

    public void AddGrade(Grade grade)
    {
        ArgumentNullException.ThrowIfNull(grade);
        if (!Grade.IsValid(grade.Value))
            throw new ArgumentOutOfRangeException(nameof(grade), "Оценка должна быть от 2 до 5.");
        _grades.Add(grade);
    }

    public bool RemoveGrade(Grade grade) => _grades.Remove(grade);
}
