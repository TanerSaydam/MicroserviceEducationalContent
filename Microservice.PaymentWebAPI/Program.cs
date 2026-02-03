var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/pay", () =>
{
    return Results.Ok(new { Message = "Payment was successful" });
});

app.Run();