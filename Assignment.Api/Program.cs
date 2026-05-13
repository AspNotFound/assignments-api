using Assignment.Api.EndPoints;
using Assignment.Api.Security;
using Assignment.Api.Services;
using Assignment.Application.Abstractions;
using Assignment.Application.Extensions;
using Assignment.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUser, User>();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, builder.Environment);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAuthentication("Development")
        .AddScheme<AuthenticationSchemeOptions, DevAuthenticationHandler>("Development", options => { });
}
else
{
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
}

builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthentication();

app.MapAssignmentEndPoints();
app.MapGradingSystemEndPoints();
app.MapSubmissionEndPoints();

app.Run();