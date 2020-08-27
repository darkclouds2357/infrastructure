using Alidu.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.Common.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong");
                await HandleExceptionAsync(context, ex);
            }
        }

        protected virtual Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
             
            var (statusCode, errorMessage) = GetErrorMessage(exception);

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(errorMessage);
        }

        protected (int statusCode, string errorMessage) GetErrorMessage(Exception exception)
        {
            int statusCode;
            string errorMessage;
            switch (exception)
            {
                case TokenException tokenException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    errorMessage = JsonConvert.SerializeObject(tokenException.Messages);
                    break;

                case PermissionException permissionException:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    errorMessage = JsonConvert.SerializeObject(permissionException.Messages);
                    break;

                case NotFoundException endpointException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    errorMessage = JsonConvert.SerializeObject(endpointException.Messages);
                    break;

                case BusinessRuleException businessRuleException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    errorMessage = JsonConvert.SerializeObject(businessRuleException.Messages);
                    break;
                     
                case HeaderException headerException:
                    statusCode = (int)HttpStatusCode.PreconditionFailed;
                    errorMessage = JsonConvert.SerializeObject(headerException.Messages);
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorMessage = exception.GetAllMessages();
                    break;
            }

            return (statusCode, errorMessage);
        }
    }
}
