using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.ViewModels.ResponseStatus;

namespace HospitalAPI.Middlewares
{
    public class VerifyTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IResponseStatus _response;
        private readonly IUtilitiesService _utilities;

        public VerifyTokenMiddleware(RequestDelegate next, IResponseStatus response, IUtilitiesService utilities)
        {
            _next = next;
            _response = response;
            _utilities = utilities;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            var token = authHeader.Split(" ")[1];
            bool isVerified = _utilities.VerifyToken(token!);
            if (!isVerified)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
            await _next(context);
        }
    }
}
