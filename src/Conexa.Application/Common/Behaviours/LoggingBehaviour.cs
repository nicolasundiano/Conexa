using Conexa.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Conexa.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(
    ILogger<TRequest> logger,
    ICurrentUserService currentUser) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Request {RequestName} handled for user {UserId}",
            typeof(TRequest).Name,
            currentUser.UserId);

        return Task.CompletedTask;
    }
}
