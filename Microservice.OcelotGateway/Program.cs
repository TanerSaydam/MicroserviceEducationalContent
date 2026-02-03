using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod());

await app.UseOcelot();

await app.RunAsync();