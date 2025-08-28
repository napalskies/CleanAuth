
using System.Security.Authentication;

namespace WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
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
