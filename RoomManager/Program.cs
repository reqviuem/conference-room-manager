using Microsoft.EntityFrameworkCore;
using RoomManager.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/api-docs/{name}.json");
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/api-docs/v1.json", "v1"); });
}
app.Run();