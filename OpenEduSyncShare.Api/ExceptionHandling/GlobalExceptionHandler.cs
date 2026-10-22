using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenEduSyncShare.Application.Exceptions;

namespace OpenEduSyncShare.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var isDatabaseUnavailable =
            exception is DatabaseUnavailableException;

        var statusCode = isDatabaseUnavailable
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status500InternalServerError;

        logger.LogError(
            exception,
            "Request failed. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = isDatabaseUnavailable
                ? "База данных временно недоступна"
                : "Внутренняя ошибка сервера",
            Detail = isDatabaseUnavailable
                ? "Не удалось выполнить операцию с базой данных. Попробуйте позже."
                : "При обработке запроса произошла ошибка. Попробуйте позже.",
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] =
            Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}