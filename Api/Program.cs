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

#region API
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

#region Database Initialization
await app.Services.InitializeDatabaseAsync();
#endregion

app.Run();
