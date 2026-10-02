using System.Text.Json;
using CampusFlow.WebAPI.Data;
using CampusFlow.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<CampusFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CampusFlow")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusFlowDbContext>();

    if (db.Database.EnsureCreated())
    {
        var path = Path.Combine(app.Environment.ContentRootPath, "Data", "seed-data.json");
        var seed = JsonSerializer.Deserialize<SeedData>(File.ReadAllText(path), JsonSerializerOptions.Web)!;

        db.Courses.AddRange(seed.Courses);
        db.Students.AddRange(seed.Students);
        db.Enrollments.AddRange(seed.Enrollments);
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapGet("/swagger", () => Results.Content("""
        <!doctype html>
        <html>
        <head>
          <title>CampusFlow API</title>
          <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5.33.1/swagger-ui.css" />
        </head>
        <body>
          <div id="swagger-ui"></div>
          <script src="https://unpkg.com/swagger-ui-dist@5.33.1/swagger-ui-bundle.js"></script>
          <script>
            SwaggerUIBundle({ url: "/openapi/v1.json", dom_id: "#swagger-ui" });
          </script>
        </body>
        </html>
        """, "text/html"))
        .ExcludeFromDescription();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
