using Microservice.AuthWebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<JwtProvider>();

var app = builder.Build();

app.MapPost("/login", (JwtProvider jwtProvider) =>
{
    var token = jwtProvider.CreateToken();

    return token;
});

app.Run();
