using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WarehouseManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5180");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Warehouse Management API",
        Version = "v1"
    });
});

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Warehouse")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse Management API v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"));

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    database.Database.Migrate();
}

app.MapControllers();
app.Run();