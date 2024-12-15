using System.Net.Http.Json;

namespace Caller;

public record Stats(int TotalRequests, double RequestsPerSecond);

public class CounterGateway(HttpClient http)
{
    public async Task Count()
    {
        await http.PostAsync("/count", null);
    }

    public async Task<Stats> GetStats()
    {
        var response = await http.GetFromJsonAsync<Stats>("/stats");
        return response!;
    }
}