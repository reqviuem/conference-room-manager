using Microsoft.EntityFrameworkCore;
using RoomManager.Data;
using RoomManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMainService, MainService>();
builder.Services.AddScoped<BookingPriceCalculator>();
builder.Services.AddScoped<DateSerializer>();

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/api-docs/{name}.json");
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/api-docs/v1.json", "v1"); });
}
app.Run();