using LMS.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace LMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next; _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(ctx, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        int status = ex switch
        {
            AppException ae => ae.StatusCode,
            FluentValidation.ValidationException => 422,
            _ => 500
        };

        string message = ex switch
        {
            FluentValidation.ValidationException ve =>
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage)),
            _ => ex.Message
        };

        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = status;

        var body = JsonSerializer.Serialize(new { status, message });
        return ctx.Response.WriteAsync(body);
    }
}