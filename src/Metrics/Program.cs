using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetry().WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddPrometheusExporter());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var random = new Random();

app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "ok" });
});

app.MapGet("/fast", () =>
{
    return Results.Ok("fast response");
});

app.MapGet("/slow", async () =>
{
    await Task.Delay(500); // имитация медленного запроса
    return Results.Ok("slow response");
});

app.MapGet("/random-delay", async () =>
{
    var delay = random.Next(50, 1500);
    await Task.Delay(delay);

    return Results.Ok(new { delayMs = delay });
});

app.MapGet("/error", () =>
{
    return Results.Problem("Something went wrong", statusCode: 500);
});

app.MapGet("/random-status", () =>
{
    var value = random.Next(0, 3);

    return value switch
    {
        0 => Results.Ok("200 OK"),
        1 => Results.BadRequest("400 Bad Request"),
        _ => Results.Problem("500 Internal Server Error")
    };
});

app.MapGet("/cpu-bound", () =>
{
    // имитация CPU-bound работы
    var sum = 0L;
    for (var i = 0; i < 50_000_000; i++)
    {
        sum += i;
    }

    return Results.Ok(new { sum });
});

app.UseOpenTelemetryPrometheusScrapingEndpoint("/metrics");
app.Run();