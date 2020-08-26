using Alidu.Common.Interfaces;
using System;

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

        public string WorkingOrgId => throw new NotImplementedException();

        public void SetClaims(string claims)
        {
        }
    }
}