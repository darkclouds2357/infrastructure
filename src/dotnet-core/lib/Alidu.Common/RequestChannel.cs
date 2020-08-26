using Alidu.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common
{
    public class RequestChannel : IRequestChannel
    {
        public RequestChannel()
        {

        }
        public RequestChannel(string requestChannel) : this() => SetChannel(requestChannel);

        public void SetChannel(string requestChannel)
        {

        }
    }
}
