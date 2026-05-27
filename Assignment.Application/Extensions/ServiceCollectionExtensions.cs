using Assignment.Application.Commands.Assignments.Handlers;
using Assignment.Application.Commands.GradingSystem.Handlers;
using Assignment.Application.Commands.Submissions.Handlers;
using Assignment.Application.Queries.Assignments.Handlers;
using Assignment.Application.Queries.GradingSystems.Handlers;
using Assignment.Application.Queries.Submissions.Handlers;
using Assignment.Application.Security.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateAssignmentHandler>();
        services.AddScoped<UpdateAssignmentHandler>();
        services.AddScoped<DeleteAssignmentHandler>();

        services.AddScoped<CreateGradingSystemHandler>();
        services.AddScoped<UpdateGradingSystemHandler>();
        services.AddScoped<DeleteGradingSystemHandler>();

        services.AddScoped<CreateSubmissionHandler>();
        services.AddScoped<UpdateSubmissionHandler>();
        services.AddScoped<DeleteSubmissionHandler>();
        services.AddScoped<JudgeSubmissionHandler>();

        services.AddScoped<GetAllAssignmentsHandler>();
        services.AddScoped<GetAssignmentByIdHandler>();

        services.AddScoped<GetAllGradingSystemsHandler>();
        services.AddScoped<GetGradingSystemByIdHandler>();

        services.AddScoped<GetAuthorAssignmentSubmissionHandler>();
        services.AddScoped<GetSubmissionByIdHandler>();
        services.AddScoped<GetSubmissionsByAssignmentHandler>();

        services.AddScoped<AssignmentAuthorizationPolicy>();
        services.AddScoped<SubmissionAuthorizationPolicy>();
        services.AddScoped<GradingSystemAuthorizationPolicy>();

        return services;
    }
}