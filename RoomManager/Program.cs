var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/api-docs/{name}.json");
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/api-docs/v1.json", "v1"); });
}
app.Run();