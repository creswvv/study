using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace CollegeManagement.Models;

/// <summary>
/// Задача 26. Логин меняется только внутри класса (private set),
/// хеш пароля - private-свойство, наружу не выводится нигде (в том числе в ToString()).
/// </summary>
public abstract class UserAccount
{
    public int Id { get; protected set; }
    public string LoginName { get; private set; } = "";
    private string PasswordHash { get; set; } = "";

    [NotMapped] public bool IsAuthenticated { get; private set; }

    protected UserAccount() { }

    protected UserAccount(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login)) throw new ArgumentException("Логин обязателен.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            throw new ArgumentException("Пароль должен содержать минимум 4 символа.");
        LoginName = login.Trim().ToLowerInvariant();
        PasswordHash = Hash(password);
    }

    public bool Login(string login, string password)
    {
        IsAuthenticated = !string.IsNullOrWhiteSpace(login) && !string.IsNullOrEmpty(password)
            && LoginName == login.Trim().ToLowerInvariant()
            && PasswordHash == Hash(password);
        return IsAuthenticated;
    }

    public void Logout() => IsAuthenticated = false;

    public bool ChangePassword(string oldPassword, string newPassword)
    {
        if (PasswordHash != Hash(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 4)
            return false;
        PasswordHash = Hash(newPassword);
        return true;
    }

    private static string Hash(string password) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("college::" + password)));

    public override string ToString() => $"Аккаунт: {LoginName}, пароль: ******";
}

/// <summary>Задача 27. protected - общие данные, private (в UserAccount) - конфиденциальные.</summary>
public abstract class User : UserAccount
{
    protected string _fullName = "";

    public string FullName { get => _fullName; protected set => _fullName = value; }
    public int? PersonId { get; protected set; }
    public abstract string Role { get; }

    protected User() { }

    protected User(string login, string fullName, string password, int? personId) : base(login, password)
    {
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Имя пользователя обязательно.");
        _fullName = fullName.Trim();
        PersonId = personId;
    }

    public override string ToString() => $"{Role}: {FullName} ({LoginName})";
}

public class AdminUser : User
{
    private AdminUser() { }
    public AdminUser(string login, string fullName, string password)
        : base(login, fullName, password, null) { }
    public override string Role => "Admin";
}

public class TeacherUser : User
{
    private TeacherUser() { }
    public TeacherUser(string login, string fullName, string password, int? personId)
        : base(login, fullName, password, personId) { }
    public override string Role => "Teacher";
}

public class StudentUser : User
{
    private StudentUser() { }
    public StudentUser(string login, string fullName, string password, int? personId)
        : base(login, fullName, password, personId) { }
    public override string Role => "Student";
}
