using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common.Interfaces
{
    public interface IRequestCredential
    {
        string OwnerId { get; }
        string OrgId { get; }
        string WorkingOrgId { get; }

        void SetClaims(string claims);
    }
}
