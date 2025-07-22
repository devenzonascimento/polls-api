using PollsApp.Domain.Exceptions;

namespace PollsApp.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ErrorHandlingMiddleware> log;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> log)
    {
        this.next = next;
        this.log = log;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx).ConfigureAwait(false);
        }
        catch (DomainException e)
        {
            ctx.Response.StatusCode = e switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                InvalidStateException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };
            await ctx.Response.WriteAsJsonAsync(new
            {
                type = e.GetType().Name,
                title = e.Message,
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "InternalServerError");
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await ctx.Response.WriteAsJsonAsync(new
            {
                type = "InternalServerError",
                title = "An internal error occurred. Try again later.",
            }).ConfigureAwait(false);
        }
    }
}
