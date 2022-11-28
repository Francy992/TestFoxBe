using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json;
using TestFoxBe.Dtos;

namespace TestFoxBe.Middlewares;

[ExcludeFromCodeCoverage]
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log error here.
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int) HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(new ErrorDto() { Message = ex.Message });
            await response.WriteAsync(result);
        }
    }
}