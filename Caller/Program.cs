using System.Threading.RateLimiting;
using Caller;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient<CounterGateway>(client => 
    {
        client.BaseAddress = new Uri("https://localhost:7234");
    })
    .AddStandardResilienceHandler(options =>
    {
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 2,
            Window = TimeSpan.FromSeconds(1)
        });

        options.RateLimiter.RateLimiter = 
            args => limiter.AcquireAsync(cancellationToken: args.Context.CancellationToken);
    });
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();