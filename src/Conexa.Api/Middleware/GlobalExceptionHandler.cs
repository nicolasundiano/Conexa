using Conexa.Application.Common.Exceptions;
using Conexa.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Conexa.Api.Middleware;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = Map(exception);

        if (problemDetails.Status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails Map(Exception exception) => exception switch
    {
        ValidationException validation => new ValidationProblemDetails(validation.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed"
        },
        NotFoundException => Problem(StatusCodes.Status404NotFound, "Not Found", exception.Message),
        ConflictException => Problem(StatusCodes.Status409Conflict, "Conflict", exception.Message),
        InvalidCredentialsException => Problem(StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
        ExternalServiceException => Problem(StatusCodes.Status502BadGateway, "Bad Gateway", exception.Message),
        DomainException => Problem(StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
        _ => Problem(StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred.")
    };

    private static ProblemDetails Problem(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail
    };
}
