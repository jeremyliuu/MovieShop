using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieShop.Core.Exceptions;
using Newtonsoft.Json;
namespace MovieShop.API.Infrastructure
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            // Handle when any exception happens
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // handle exceptions
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            // get the exception details from ex, such as exception date, stacktrace, inner exception, error message etc
            // return exception information in a friendly JSON message so that Angular can use that message to display
            // proper Error message in the Browser
            // by default set the HttpStatusCode to 500 as its a internal server error
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var env = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
            string result;
            if (env.IsDevelopment())
            {
                // contrsuct a error object so that we can send that error object as Json to Angular or client
                var errorDetails = new ErrorResponseModel
                {
                    ErrorMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                };
                // convert C# errordetails object to JSON using NewtonSoft.Json library so that Angular will recieve Json object
                result = JsonConvert.SerializeObject(new { erros = errorDetails });
            }
            else
            {
                result = JsonConvert.SerializeObject(new { erros = ex.Message });
            }
            // we can catch for specific exception and handle them and add our custom messages
            switch (ex)
            {
                case UnauthorizedAccessException _:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case DivideByZeroException _:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                case Exception _:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                default:
                    break;
            }
            // write the json object to the http response
            await httpContext.Response.WriteAsync(result);
        }
    }
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}