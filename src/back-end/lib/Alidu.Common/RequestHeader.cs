using Alidu.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common
{
    public class RequestHeader : IRequestHeader
    {
        public RequestHeader()
        {

        }
        public IHeaderCredential Credential { get; }

        public IHeaderCommand Command { get; }

        public IHeaderChannel Channel { get; }

        public void SetCredential(string claims)
        {

        }
        public void SetCommand(string commandHeader)
        {

        }
        public void SetChannel(string channelHeader)
        {

        }
    }
}
