using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using zombie_defense.Application.useCases;
using zombie_defense.Domain.Ports;
using zombie_defense.Infraestructure.Adapters;
using zombie_defense.Infraestructure.Persistence;
using zombie_defense.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddScoped<IZombieTypeRepository, SqlZombieTypeRepository>();

// Uses Cases
builder.Services.AddScoped<CalculateOptimalStrategyUseCase>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, ct) =>
        {
            document.Info.Title = "Zombie Defense API";
            document.Info.Version = "v1";
            document.Info.Description = "API for managing zombie defense operations.";
            return Task.CompletedTask;
        });
    });
var allowedOrigins = builder.Configuration.GetSection("AllowedHosts").Get<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(allowedOrigins!)
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
}


app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();

app.UseMiddleware<ApiKeyMiddleware>();
app.MapControllers();

app.Run();
