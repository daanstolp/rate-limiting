# Throttling outgoing HTTP requests

You want to consume an external web service. This web service is rate-limited to 100 requests per minute. 

You want your application to proactively avoid hitting this limit.

This solution experiments with a possible solution.

## Counter service
This project represents the rate-limited external web service. It exposes two endpoints:
- `/count`: increments an internal counter each time it is called.
- `/stats`: returns the total number of requests, and the number of requests per second.

The counter service resets after each call to `/stats`, and uses the timestamps of the first and last call to `/count` to calculate the number of requests per second.

## Caller
This is simple worker application that calls the Counter web service. 
- Calls the counter service's `/count` endpoint in a tight loop for 5 seconds.
- Then calls the `/stats` end point and prints the results.
- It uses the `Microsoft.Extensions.Http.Resilience` package to add best-practice resilience policies to the HttpClient, using `AddStandardResilienceHandler()`.

