using Assignment.Domain.Entities;
using Assignment.Domain.Exceptions;

namespace Assignment.Domain.Aggregates;

public class GradingSystem
{
    private GradingSystem(string name, List<GradingSystemGrade> grades)
    {
        if (!IsValidName(name, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        if (!IsValidGrades(grades, out var validationErrorMessageGrades))
        {
            throw new DomainException(validationErrorMessageGrades);
        }

        Id = Guid.CreateVersion7();
        Name = name;
        _grades = [.. grades];
    }

    private GradingSystem(Guid id, string name, IReadOnlyCollection<GradingSystemGrade> grades)
    {
        Id = id;
        Name = name;
        _grades = [.. grades];
    }

    public Guid Id { get; }
    public string Name { get; private set; }

    private readonly List<GradingSystemGrade> _grades;
    public IReadOnlyCollection<GradingSystemGrade> Grades => _grades.AsReadOnly();

    public void Rename(string newName)
    {
        if (!IsValidName(newName, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        Name = newName;
    }

    public void AddGrade(GradingSystemGrade grade)
    {
        if (!IsValidGrade(_grades, grade, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        _grades.Add(grade);
    }

    public void RemoveGrade(Guid gradeId)
    {
        var gradeToRemove = _grades.FirstOrDefault(g => g.Id == gradeId);
        if (gradeToRemove == null)
        {
            return;
        }
        var gradesCopy = _grades.Where(g => g.Id != gradeId).ToList();
        if (!IsValidGrades(gradesCopy, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        _grades.Remove(gradeToRemove);
    }

    public void RenameGrade(Guid gradeId, string newName)
    {
        var gradeToRename = _grades.FirstOrDefault(g => g.Id == gradeId) ?? throw new DomainException($"No grade with ID '{gradeId}' found in the grading system.");
        var gradeCopy = gradeToRename.Copy();
        gradeCopy.Rename(newName);
        if (!IsValidGrade([.. _grades.Where(g => g.Id != gradeId)], gradeCopy, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        gradeToRename.Rename(newName);
    }

    public void ChangeGradePassingStatus(Guid gradeId, bool isPassingGrade)
    {
        var gradeToChange = _grades.FirstOrDefault(g => g.Id == gradeId) ?? throw new DomainException($"No grade with ID '{gradeId}' found in the grading system.");

        var gradeCopy = gradeToChange.Copy();
        gradeCopy.ChangePassingStatus(isPassingGrade);

        if (!IsValidGrade([.. _grades.Where(g => g.Id != gradeId)], gradeCopy, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        gradeToChange.ChangePassingStatus(isPassingGrade);
    }

    private static bool IsValidName(string name, out string? validationErrorMessage)
    {
        validationErrorMessage = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            validationErrorMessage = "Grading system name cannot be null or whitespace.";
            return false;
        }

        return true;
    }

    private static bool IsValidGrade(List<GradingSystemGrade> existingGrades, GradingSystemGrade newGrade, out string? validationErrorMessage)
    {
        validationErrorMessage = null;
        if (existingGrades.Any(g => g.Id == newGrade.Id))
        {
            validationErrorMessage = $"A grade with the ID '{newGrade.Id}' already exists in the grading system.";
            return false;
        }
        if (existingGrades.Any(g => g.Name.Equals(newGrade.Name, StringComparison.OrdinalIgnoreCase)))
        {
            validationErrorMessage = $"A grade with the name '{newGrade.Name}' already exists in the grading system.";
            return false;
        }
        if (existingGrades.Any(g => g.Order == newGrade.Order))
        {
            validationErrorMessage = $"A grade with the order value '{newGrade.Order}' already exists in the grading system.";
            return false;
        }
        if (newGrade.IsPassingGrade)
        {
            var failingGrades = existingGrades.Where(g => !g.IsPassingGrade).ToList();
            var maxFailingGradeOrder = failingGrades.Count > 0 ? failingGrades.Max(g => g.Order) : int.MinValue;
            if (newGrade.Order <= maxFailingGradeOrder)
            {
                validationErrorMessage = "All passing grades must have a higher order value than all failing grades.";
                return false;
            }
        }
        else
        {
            var passingGrades = existingGrades.Where(g => g.IsPassingGrade).ToList();
            var minPassingGradeOrder = passingGrades.Count > 0 ? passingGrades.Min(g => g.Order) : int.MaxValue;
            if (newGrade.Order >= minPassingGradeOrder)
            {
                validationErrorMessage = "All failing grades must have a lower order value than all passing grades.";
                return false;
            }
        }
        return true;
    }

    private static bool IsValidGrades(List<GradingSystemGrade> arg0, out string? validationErrorMessage)
    {
        validationErrorMessage = null;

        var grades = new List<GradingSystemGrade>();
        foreach (var grade in arg0)
        {
            if (!IsValidGrade(grades, grade, out var validationErrorMessageGrade))
            {
                validationErrorMessage = $"Invalid grade with name '{grade.Name}': {validationErrorMessageGrade}";
                return false;
            }
            grades.Add(grade);
        }

        if (grades.Count < 2)
        {
            validationErrorMessage = "Grading system must have at least two grades.";
            return false;
        }

        var hasPassingGrade = grades.Any(g => g.IsPassingGrade);
        if (!hasPassingGrade)
        {
            validationErrorMessage = "Grading system must have at least one passing grade.";
            return false;
        }

        var countFailingGrades = grades.Count(g => !g.IsPassingGrade);
        if (countFailingGrades != 1)
        {
            validationErrorMessage = "Grading system must have exactly one failing grade.";
            return false;
        }

        return true;
    }

    public static GradingSystem Create(string name, List<GradingSystemGrade> grades)
    {
        return new GradingSystem(name, grades);
    }

    public static GradingSystem Hydrate(Guid id, string name, IReadOnlyCollection<GradingSystemGrade> grades)
    {
        return new GradingSystem(id, name, grades);
    }
}