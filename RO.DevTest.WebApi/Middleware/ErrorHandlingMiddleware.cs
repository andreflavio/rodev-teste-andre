using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace RO.DevTest.WebApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ArgumentException argEx:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = argEx.Message });
                break;
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new { error = "Acesso não autorizado. Faça login com credenciais válidas." });
                break;
            case Exception when exception.Message.Contains("forbidden", StringComparison.OrdinalIgnoreCase):
                code = HttpStatusCode.Forbidden;
                result = JsonSerializer.Serialize(new { error = "Acesso proibido: apenas administradores podem realizar esta ação." });
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new { error = "Ocorreu um erro interno no servidor. Tente novamente mais tarde." });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}