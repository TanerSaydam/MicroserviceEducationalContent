using Microservice.OcelotGateway;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddAuthentication().AddJwtBearer(opt =>
{
    var secretKey = "my secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret keymy secret key";
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    opt.TokenValidationParameters.ValidateIssuer = true;
    opt.TokenValidationParameters.ValidateAudience = true;
    opt.TokenValidationParameters.ValidateLifetime = true;
    opt.TokenValidationParameters.ValidateIssuerSigningKey = true;
    opt.TokenValidationParameters.ValidIssuer = "Issuer";
    opt.TokenValidationParameters.ValidAudience = "Audience";
    opt.TokenValidationParameters.IssuerSigningKey = securityKey;
});
builder.Services.AddAuthorization();

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