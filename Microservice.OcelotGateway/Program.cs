using Microservice.OcelotGateway;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services
    .AddOcelot(builder.Configuration)
    .AddConsul<MyConsulServiceBuilder>();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod());

await app.UseOcelot();

await app.RunAsync();