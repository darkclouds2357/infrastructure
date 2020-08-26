using Alidu.Common.Interfaces;

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