using System.Net;
using DAL.Exceptions;

namespace QuickXe_Api.Middleware
{
    
        public class ExceptionHandlingMiddleware : IMiddleware
        {
            public async Task InvokeAsync(HttpContext context, RequestDelegate next)
            {
                try
                {
                    await next(context);
                }
                catch (DataNotFoundException e)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync(e.Message);

                }
                catch (DataValidationException e)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync(e.Message);
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync(e.Message);
                }
            }
        }

    
}
