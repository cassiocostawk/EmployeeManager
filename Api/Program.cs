using Api.Extensions;
using Api.Middlewares;
using Api.Services;
using Application.Behaviors;
using Application.Commands;
using Application.Mappings;
using Application.Validators;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

#region Configuration Setup
builder.Services.AddDatabaseConfiguration(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
#endregion

#region DI
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
#endregion

#region Pipelines
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<RequestsProfile>();
    config.AddProfile<ResponsesProfile>();
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeRequestValidator>();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(typeof(CreateEmployeeCommand).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddHttpContextAccessor();
#endregion

#region CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
#endregion

#region Health Checks
builder.Services.AddHealthCheckConfiguration();
#endregion

#region API
builder.Services.AddControllers();

builder.Services.AddSwaggerConfiguration();
#endregion

#region Build App
var app = builder.Build();
#endregion

#region Middlewares 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

#region Health Check Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});
#endregion

#region Database Initialization
await app.Services.InitializeDatabaseAsync();
#endregion

app.Run();
