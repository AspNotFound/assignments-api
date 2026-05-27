using System.ComponentModel.DataAnnotations;
using Assignment.Domain.Exceptions;

namespace Assignment.Domain.Entities;

public class GradingSystemGrade
{
    private GradingSystemGrade(string name, bool isPassingGrade, int order)
    {
        if (!IsValidName(name, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        Id = Guid.CreateVersion7();
        Name = name;
        IsPassingGrade = isPassingGrade;

        Order = order;
    }

    private GradingSystemGrade(Guid id, string name, bool isPassingGrade, int order)
    {
        Id = id;
        Name = name;
        IsPassingGrade = isPassingGrade;
        Order = order;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public bool IsPassingGrade { get; private set; }
    public int Order { get; private set; }

    internal void Rename(string newName)
    {
        if (!IsValidName(newName, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        Name = newName;
    }

    internal void ChangePassingStatus(bool isPassingGrade)
    {
        IsPassingGrade = isPassingGrade;
    }

    internal void ChangeOrder(int newOrder)
    {
        Order = newOrder;
    }

    private static bool IsValidName(string name, out string? validationErrorMessage)
    {
        validationErrorMessage = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            validationErrorMessage = "Grade name cannot be null or whitespace.";
            return false;
        }

        return true;
    }

    public static GradingSystemGrade Create(string name, bool isPassingGrade, int order)
    {
        return new GradingSystemGrade(name, isPassingGrade, order);
    }

    public static GradingSystemGrade Hydrate(Guid id, string name, bool isPassingGrade, int order)
    {
        return new GradingSystemGrade(id, name, isPassingGrade, order);
    }

    public GradingSystemGrade Copy()
    {
        return new GradingSystemGrade(Id, Name, IsPassingGrade, Order);
    }
}