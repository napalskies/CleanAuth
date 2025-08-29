
using System.Security.Authentication;

namespace WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {

        public readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidCredentialException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid credentials provided.");
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("User not authorised to access resource.");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(e.Message);
            }
        }
    }
}
