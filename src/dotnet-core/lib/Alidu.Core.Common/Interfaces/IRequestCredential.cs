using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Common.Interfaces
{
    public interface IRequestCredential
    {
        string OwnerId { get; }
        string OrgId { get; }

        void SetClaims(string claims);
    }
}
