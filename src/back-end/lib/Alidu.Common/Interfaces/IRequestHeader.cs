using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common.Interfaces
{
    public interface IRequestHeader
    {
        IHeaderCredential Credential { get; }
        IHeaderCommand Command { get; }
        IHeaderChannel Channel { get; }

        void SetCredential(string claims);
        void SetCommand(string commandHeader);
        void SetChannel(string channelHeader);
    }
}
