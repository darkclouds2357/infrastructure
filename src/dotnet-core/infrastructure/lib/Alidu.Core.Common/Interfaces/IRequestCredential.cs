using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Common.Interfaces
{
    public interface IRequestCredential
    {
        void SetClaims(string claims);
    }
}
