using HospitalAPI.Features.Utils.IServices;

namespace HospitalAPI.Middlewares
{
    public class VerifyTokenMiddleware : IMiddleware
    {
        private readonly IResponseStatus _response;
        private readonly ITokenService _token;

        public VerifyTokenMiddleware(IResponseStatus response, ITokenService token)
        {
            _response = response;
            _token = token;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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
            await next(context);
        }
    }
}
