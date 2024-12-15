var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<RequestCounter>();
var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/count", (RequestCounter requestCounter) =>
{
    requestCounter.Count();
    return Results.Ok();
});

app.MapGet("/stats", (RequestCounter requestCounter) =>
{
    var result = new
    {
        TotalRequests = requestCounter.TotalRequests,
        RequestsPerSecond = requestCounter.RequestsPerSecond()
    };
    requestCounter.Reset();
    
    return Results.Ok(result);
});

app.Run();



public class RequestCounter
{
    private readonly List<DateTime> _requests = new();

    public int TotalRequests => _requests.Count;

    public double RequestsPerSecond()
    {
        var elapsedTime = _requests.Last() - _requests.First();
        return TotalRequests / elapsedTime.TotalSeconds;
    }
    
    public void Count()
    {
        _requests.Add(DateTime.Now);
    }
    
    public void Reset()
    {
        _requests.Clear();
    }
}