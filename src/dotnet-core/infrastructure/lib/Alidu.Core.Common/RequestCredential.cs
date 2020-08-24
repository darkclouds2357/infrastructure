using Alidu.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Common
{
    public class RequestCredential : IRequestCredential
    {
        public RequestCredential()
        {

        }

        public RequestCredential(string claims) : this() => SetClaims(claims);
        public void SetClaims(string claims)
        {
        }
    }
}
