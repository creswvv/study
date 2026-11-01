# CollegeManagement — ASP.NET Core MVC + SQLite

## Запуск
1. Установите .NET SDK (проект рассчитан на .NET 8; для другой версии поменяйте
   `TargetFramework` и версию пакета EF Core в `CollegeManagement.csproj` под вывод `dotnet --version`).
2. В папке проекта:
       dotnet restore
       dotnet run
3. Откройте адрес из консоли (обычно http://localhost:5000 или https://localhost:7xxx).
   База `college.db` (SQLite) создаётся и заполняется тестовыми данными автоматически.

## Тестовые аккаунты
| Роль         | Логин   | Пароль     |
|--------------|---------|------------|
| Администратор| admin   | admin123   |
| Преподаватель| teacher | teacher123 |
| Студент      | student | student123 |

Чтобы пересоздать базу — остановите приложение и удалите файл `college.db`.

## Где какие задачи
| Задачи | Файл |
|---|---|
| 1-8 (Student, private/public, валидация, конструктор, StudentId только чтение) | Models/Student.cs |
| 9-10 (Teacher, UpdateExperience) | Models/Teacher.cs |
| 11-13 (Group, AddStudent / RemoveStudent, закрытый список) | Models/Group.cs |
| 14-15 (Subject, AssignTeacher) | Models/Subject.cs |
| 16, 19, 20, 23, 25 (Person: protected, protected internal, private protected) | Models/Person.cs |
| 17-18 (наследование Student / Teacher) | Models/Student.cs, Models/Teacher.cs |
| 21-22 (internal CollegeSettings) | Models/CollegeSettings.cs, Controllers/AdminController.cs |
| 24 (ReportManager, protected internal GenerateReport) | Services/ReportManager.cs |
| 26-27 (UserAccount, User, Admin/Teacher/Student) | Models/User.cs |
| 28 (GradeBook, оценки 2-5) | Models/Grade.cs |
| 29 (AttendanceManager) | Services/AttendanceManager.cs |
| 30 (веб-приложение) | Controllers/*, Views/* |

## Страницы
Вход · Обзор (дашборд с графиками) · Студенты · Преподаватели · Группы · Журнал оценок · Посещаемость (+ отчёт) · Админ-панель.
Права: админ — всё; преподаватель — оценки, посещаемость, студенты; студент — только просмотр (в журнале видит только себя).
