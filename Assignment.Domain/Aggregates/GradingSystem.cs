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
    public IReadOnlyCollection<GradingSystemGrade> Grades => _grades;

    public void Rename(string newName)
    {
        if (!IsValidName(newName, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        Name = newName;
    }

    public void ChangeGrades(List<IGradingSystemGradeModification> modification)
    {
        var gradesCopy = _grades.Select(g => g.Copy()).ToList();
        foreach (var gradeModification in modification)
        {
            gradeModification.Apply(gradesCopy);
        }

        if (!IsValidGrades(gradesCopy, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        modification.ForEach(m => m.Apply(_grades));
    }

    public void AddGrade(GradingSystemGrade grade)
    {
        if (!IsValidGrade(_grades, grade, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        var gradesCopy = new List<GradingSystemGrade>(_grades) { grade };
        if (!IsValidGrades(gradesCopy, out var validationErrorMessageGrades))
        {
            throw new DomainException(validationErrorMessageGrades);
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
        var gradesCopy = _grades.Select(g => g.Copy()).ToList();
        var gradeToRename = gradesCopy.FirstOrDefault(g => g.Id == gradeId) ?? throw new DomainException($"No grade with ID '{gradeId}' found in the grading system.");
        gradeToRename.Rename(newName);
        if (!IsValidGrades(gradesCopy, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        _grades.Single(g => g.Id == gradeId).Rename(newName);
    }

    public void ChangeGradePassingStatus(Guid gradeId, bool isPassingGrade)
    {
        var gradesCopy = _grades.Select(g => g.Copy()).ToList();
        var gradeToChange = gradesCopy.FirstOrDefault(g => g.Id == gradeId) ?? throw new DomainException($"No grade with ID '{gradeId}' found in the grading system.");
        gradeToChange.ChangePassingStatus(isPassingGrade);
        if (!IsValidGrades(gradesCopy, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }
        _grades.Single(g => g.Id == gradeId).ChangePassingStatus(isPassingGrade);
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

public interface IGradingSystemGradeModification
{
    void Apply(List<GradingSystemGrade> grades);
}

public class AddGradingSystemGradeModification(GradingSystemGrade grade) : IGradingSystemGradeModification
{
    public GradingSystemGrade Grade { get; } = grade;
    public void Apply(List<GradingSystemGrade> grades)
    {
        grades.Add(Grade);
    }
}

public class RemoveGradingSystemGradeModification(Guid gradeId) : IGradingSystemGradeModification
{
    public Guid GradeId { get; } = gradeId;
    public void Apply(List<GradingSystemGrade> grades)
    {
        var gradeToRemove = grades.FirstOrDefault(g => g.Id == GradeId);
        if (gradeToRemove != null)
        {
            grades.Remove(gradeToRemove);
        }
    }
}

public class UpdateGradingSystemGradeModification(Guid gradeId, string newName, bool newIsPassingGrade, int newOrder) : IGradingSystemGradeModification
{
    public Guid GradeId { get; } = gradeId;
    public string NewName { get; } = newName;
    public bool NewIsPassingGrade { get; } = newIsPassingGrade;
    public int NewOrder { get; } = newOrder;

    public void Apply(List<GradingSystemGrade> grades)
    {
        var gradeToUpdate = grades.FirstOrDefault(g => g.Id == GradeId) ?? throw new DomainException($"No grade with ID '{GradeId}' found in the grading system.");
        gradeToUpdate.Rename(NewName);
        gradeToUpdate.ChangePassingStatus(NewIsPassingGrade);
        gradeToUpdate.ChangeOrder(NewOrder);
    }
}