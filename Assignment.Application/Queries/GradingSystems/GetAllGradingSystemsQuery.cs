namespace Assignment.Application.Queries.GradingSystems;

public record GetAllGradingSystemsQuery
{
    public readonly static GetAllGradingSystemsQuery Instance = new();
    private GetAllGradingSystemsQuery() { }
}