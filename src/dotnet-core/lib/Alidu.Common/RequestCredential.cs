using Alidu.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common
{
    public class RequestCredential : IRequestCredential
    {
        public RequestCredential()
        {

        }

        public RequestCredential(string claims) : this() => SetClaims(claims);

        public string OwnerId => throw new NotImplementedException();

        public string OrgId => throw new NotImplementedException();

        public void SetClaims(string claims)
        {
        }
    }
}
