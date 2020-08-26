using Alidu.Common.Interfaces;

namespace Alidu.Common
{
    public class RequestCommand : IRequestCommand
    {
        public RequestCommand(string commandId)
        {
            CommandId = commandId;
        }

        public string CommandId { get; private set; }

        public void SetCommandId(string commandId) => CommandId = commandId;
    }
}