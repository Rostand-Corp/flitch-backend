using System.Net;
using System.Text.Json;

namespace Web.Middlewares;

public class UnauthorizedResponseMiddleware // not being used atm
{
    private readonly RequestDelegate _next;

    public UnauthorizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
        {
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                StatusCode = 401,
                Message = "You are not authorized to access this resource",
                Links = new[] { "/auth/login", "/auth/register" }
            };
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
        else
        {
            await _next(context);
        }
    }
}