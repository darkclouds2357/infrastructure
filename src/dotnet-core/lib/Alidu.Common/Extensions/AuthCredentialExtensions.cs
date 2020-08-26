using Alidu.Common.Constants;
using Alidu.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace Alidu.Common
{
    public static class AuthCredentialExtensions
    {
        public static IRequestCredential GetRequestClaim(this HttpContext context)
        {
            var requestHeaders = context.Request.Headers;
            if (!requestHeaders.ContainsKey(TraefikDefault.Claims))
                return new RequestCredential();
            var requestClaim = requestHeaders[TraefikDefault.Claims];
            return new RequestCredential(requestClaim);
        }

        public static IRequestChannel GetRequestChannel(this HttpContext context)
        {
            var requestHeaders = context.Request.Headers;
            if (!requestHeaders.ContainsKey(TraefikDefault.Channel))
                return new RequestChannel();
            var requestChannel = requestHeaders[TraefikDefault.Channel];
            return new RequestChannel(requestChannel);
        }

        public static IRequestCommand GetRequestCommand(this HttpContext context)
        {
            var requestHeaders = context.Request.Headers;
            if (!requestHeaders.ContainsKey(TraefikDefault.CommandId))
                return new RequestCommand(Guid.NewGuid().ToString());
            var commandId = requestHeaders[TraefikDefault.CommandId];
            return new RequestCommand(commandId);
        }

        public static IRequestCredential RefreshRequestCredential(this IRequestCredential requestCredential, HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(TraefikDefault.Claims))
                requestCredential.SetClaims(context.Request.Headers[TraefikDefault.Claims]);
            return requestCredential;
        }
    }
}