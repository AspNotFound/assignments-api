using Assignment.Api.EndPoints;
using Assignment.Api.OpenApi;
using Assignment.Api.Services;
using Assignment.Application.Abstractions;
using Assignment.Application.Abstractions.Services;
using Assignment.Application.Extensions;
using Assignment.Infrastructure.Extensions;
using Microsoft.IdentityModel.Protocols.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiConfiguration();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUser, User>();
builder.Services.AddScoped<ICourseService, Course>();
builder.Services.AddScoped<IEnrollmentService, Enrollment>();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddCors(options =>
{
    options.AddPolicy("All", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.ClaimsIssuer = builder.Configuration["Jwt:Issuer"];
        options.Audience = builder.Configuration["Jwt:Audience"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateLifetime = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidConfigurationException())),
            ValidateIssuerSigningKey = true
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("All");
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApiEndpoints();

app.MapAssignmentEndPoints();
app.MapGradingSystemEndPoints();
app.MapSubmissionEndPoints();

app.Run();