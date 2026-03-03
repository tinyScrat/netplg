using Lpc.Application;
using Lpc.Infrastructure;
using Lpc.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("https://localhost:8000")
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services
    .AddInfrastructure()
    .AddApplication()
    .AddWebApi();

var app = builder.Build();

app.UseCors();

app.MapGet("/hello", () => "Hello World!");

app.Run();
