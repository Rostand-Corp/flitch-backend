using System.Net;
using System.Text.Json;
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception e)
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