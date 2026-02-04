var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("SpaCors", policy =>
        policy.WithOrigins("https://localhost:8000")
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

app.UseCors("SpaCors");

app.MapGet("/hello", () => "Hello World!");

app.Run();
