using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace HospitalAPI.Features.GlobalException
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.StatusCode = statusCode;
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Server Error",
                Detail = exception.Message,
            };
            var json = JsonSerializer.Serialize(problem);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json, cancellationToken);
            return true;
        }
    }
}
