using Assignment.Application.Security.Permissions;

namespace Assignment.Application.Dtos;

public record Dto<T, P>(T Value, P Permissions) where P : Permissions
{
    public static Dto<T, P> Create(T value, P permissions)
    {
        return new Dto<T, P>(value, permissions);
    }
}