using Serilog.Context;

namespace PollsApp.Api.Middlewares;

public class RequestLogContextMiddleware
{
    private readonly RequestDelegate next;

    public RequestLogContextMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        {
            return next(context);
        }
    }
}
