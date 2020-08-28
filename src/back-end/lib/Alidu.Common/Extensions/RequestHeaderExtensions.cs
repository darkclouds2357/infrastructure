using Alidu.Common.Constants;
using Alidu.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Alidu.Common
{
    public static class RequestHeaderExtensions
    {
        public static IServiceCollection AddRequestHeader(this IServiceCollection services)
        {
            services.AddScoped<IRequestHeader, RequestHeader>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return new RequestHeader();
                return httpContextAccessor.HttpContext.GetRequestHeader();
            });
            return services;
        }

        public static RequestHeader GetRequestHeader(this HttpContext context)
        {
            var requestHeaders = context.Request.Headers;
            var result = new RequestHeader();

            if (requestHeaders.ContainsKey(TraefikDefault.Claims))
                result.SetCredential(requestHeaders[TraefikDefault.Claims]);

            if (requestHeaders.ContainsKey(TraefikDefault.Channel))
                result.SetChannel(requestHeaders[TraefikDefault.Channel]);

            if (requestHeaders.ContainsKey(TraefikDefault.CommandId))
                result.SetCommand(requestHeaders[TraefikDefault.CommandId]);

            return result;
        }

        public static IRequestHeader RefreshRequestCredential(this IRequestHeader requestHeader, HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(TraefikDefault.Claims))
                requestHeader.SetCredential(context.Request.Headers[TraefikDefault.Claims]);
            return requestHeader;
        }
    }
}