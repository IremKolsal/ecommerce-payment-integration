using System.Net;
using System.Text.Json;

namespace ECommerce.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> log)
{
    public async Task Invoke(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (KeyNotFoundException ex)
        {
            log.LogWarning(ex, "NotFound");
            await Write(ctx, HttpStatusCode.NotFound, "NOT_FOUND", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            log.LogWarning(ex, "InvalidState");
            await Write(ctx, HttpStatusCode.Conflict, "INVALID_STATE", ex.Message);
        }
        catch (HttpRequestException ex)
        {
            log.LogError(ex, "Upstream");
            await Write(ctx, HttpStatusCode.BadGateway, "BALANCE_UPSTREAM_ERROR", ex.Message);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled");
            await Write(ctx, HttpStatusCode.InternalServerError, "UNEXPECTED", "Unexpected error");
        }
    }

    static async Task Write(HttpContext ctx, HttpStatusCode code, string err, string msg)
    {
        ctx.Response.StatusCode = (int)code;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = err, message = msg }));
    }
}

public static class ExceptionMiddlewareExt
{
    public static IApplicationBuilder UseGlobalExceptions(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
