using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("fixed", x =>
    {
        x.PermitLimit = 100;
        x.QueueLimit = 100;
        x.Window = TimeSpan.FromSeconds(1);
        x.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

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
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("require-authentication", policy => policy.RequireAuthenticatedUser());
    x.AddPolicy("UserType", policy => policy.RequireClaim("UserType"));
});


var app = builder.Build();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .SetIsOriginAllowed(x => true)
    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.MapGet("/", () => "Hello World!");

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
