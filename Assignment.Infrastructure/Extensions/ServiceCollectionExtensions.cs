using Assignment.Application.Abstractions.ReadRepositories;
using Assignment.Application.Abstractions.Repositories;
using Assignment.Infrastructure.Ef.Contexts;
using Assignment.Infrastructure.Ef.ReadRepositories;
using Assignment.Infrastructure.Ef.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Assignment.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<AssignmentContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IGradingSystemRepository, GradingSystemRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();

        services.AddScoped<IAssignmentReadRepository, AssignmentReadRepository>();
        services.AddScoped<IGradingSystemReadRepository, GradingSystemReadRepository>();
        services.AddScoped<ISubmissionReadRepository, SubmissionReadRepository>();

        return services;
    }
}