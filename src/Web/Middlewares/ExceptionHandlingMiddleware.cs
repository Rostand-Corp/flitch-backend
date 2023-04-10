using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FlitchException ex)
        {
            await HandleExpectedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnexpectedExceptionAsync(context, ex);
        }
    }

    private async Task HandleExpectedExceptionAsync(HttpContext context, FlitchException e)
    {
        _logger.LogError(e, e.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)e.HttpStatusCode;

        if (e is ValidationException ve)
        {
            var validationProblemDetails = new ValidationProblemDetails(ve.Errors)
            {
                Type = e.ErrorType,
                Title = "Validation exception",
                Status = (int) e.HttpStatusCode,
                Detail = "One or more validation problems have occurred.",
            };

            var json = JsonSerializer.Serialize(validationProblemDetails);
            await context.Response.WriteAsync(json);
        }
        else
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int) e.HttpStatusCode,
                Type = e.ErrorType,
                Title = "Flitch exception has occurred.",
                Detail = e.Message,
            };

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
    private async Task HandleUnexpectedExceptionAsync(HttpContext context, Exception e)
    {
        _logger.LogError(e, e.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

        ProblemDetails problem = new()
        {
            Status = (int) HttpStatusCode.InternalServerError,
            Type = "InternalServerError",
            Title = "Internal server error",
            Detail = "A critical internal server error occurred"
        };

        var json = JsonSerializer.Serialize(problem);

        await context.Response.WriteAsync(json);
    }
}