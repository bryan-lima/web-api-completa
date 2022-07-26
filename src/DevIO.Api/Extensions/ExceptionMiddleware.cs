using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DevIO.Api.Extensions
{
    public class ExceptionMiddleware
    {
        #region Private Fields

        private readonly RequestDelegate _next;

        #endregion Private Fields

        #region Public Constructors

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(httpContext, ex);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void HandleExceptionAsync(HttpContext context, Exception exception)
        {
            exception.Ship(context);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        #endregion Private Methods
    }
}