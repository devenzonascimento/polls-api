using MediatR;
using Microsoft.Extensions.Logging;

namespace PollsApp.Application.Behaviors;

public class RequestLoggingPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger;

    public RequestLoggingPipelineBehavior(
        ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger
    )
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        string requestName = typeof(TRequest).Name;

        logger.LogInformation("Processing request {RequestName}", requestName);

        TResponse result = await next(cancellationToken).ConfigureAwait(false);

        if (result is not null)
        {
            logger.LogInformation("Completed request {RequestName}", requestName);
        }

        return result;
    }
}
