using CommunityBoard.Common;
using System.Net;
using System.Text.Json;

namespace CommunityBoard.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "NotFound");
            await Write(ctx, HttpStatusCode.NotFound, Result<object>.Fail("not_found", ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized");
            await Write(ctx, HttpStatusCode.Unauthorized, Result<object>.Fail("unauthorized", ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled");   
            await Write(ctx, HttpStatusCode.InternalServerError, Result<object>.Fail("server_error", "Unexpected server error"));
        }
    }

    private static async Task Write(HttpContext ctx, HttpStatusCode code, object payload)
    {
        ctx.Response.StatusCode = (int)code;
        ctx.Response.ContentType = "application/json; charset=utf-8";
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}