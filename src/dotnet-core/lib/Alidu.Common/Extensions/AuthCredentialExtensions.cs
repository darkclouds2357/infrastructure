using Alidu.Common.Constants;
using Alidu.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace Alidu.Common
{
    public static class AuthCredentialExtensions
    {

        public static IRequestHeader GetRequestHeader(this HttpContext context)
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