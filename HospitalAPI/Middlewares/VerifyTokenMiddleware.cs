using HospitalAPI.Features.Utils.IServices;

namespace HospitalAPI.Middlewares
{
    public class VerifyTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IResponseStatus _response;
        private readonly ITokenService _token;

        public VerifyTokenMiddleware(RequestDelegate next, IResponseStatus response, ITokenService token)
        {
            _next = next;
            _response = response;
            _token = token;
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
            var user = _token.VerifyToken(token!, "KeyAccessToken");
            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
            await _next(context);
        }
    }
}
