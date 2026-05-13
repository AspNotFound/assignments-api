namespace Assignment.Application.Abstractions;

public interface IUser
{
    public string UserId { get; }
    public bool IsTeacher();
    public bool IsStudent();
    public bool IsAdmin();
}