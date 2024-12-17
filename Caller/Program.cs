using System.Threading.RateLimiting;
using Caller;
using Polly.RateLimiting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient<CounterGateway>(client => { client.BaseAddress = new Uri("https://localhost:7234"); })
    .AddStandardResilienceHandler(options =>
    {
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 2,
            Window = TimeSpan.FromSeconds(1)
        });
        options.RateLimiter.RateLimiter =
            args => limiter.AcquireAsync(cancellationToken: args.Context.CancellationToken);

        var standardRetryPredicate = options.Retry.ShouldHandle;
        options.Retry.ShouldHandle = async args =>
            await standardRetryPredicate(args) || args.Outcome.Exception is RateLimiterRejectedException;
    });
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
