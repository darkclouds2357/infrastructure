using Alidu.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Alidu.CQRS.ExecptionHandler
{
    public class CQRSExceptionMiddleware : ExceptionMiddleware
    {
        private readonly IMediator _mediator;

        public CQRSExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IMediator mediator) : base(next, logger)
        {
            _mediator = mediator;
        }

        protected override async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errorMessage) = GetErrorMessage(exception);
            var serviceExceptionEvent = new ServiceExceptionEvent()
            {
                StatusCode = statusCode,
                ErrorMessage = errorMessage,
            };

            await _mediator.Publish(serviceExceptionEvent);
        }
    }
}