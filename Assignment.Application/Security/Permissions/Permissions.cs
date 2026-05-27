namespace Assignment.Application.Security.Permissions;

public record Permissions
(
    bool Edit
);

public record AssignmentPermissions(bool CreateSubmission, bool Edit, bool ViewSubmissions) : Permissions(Edit);