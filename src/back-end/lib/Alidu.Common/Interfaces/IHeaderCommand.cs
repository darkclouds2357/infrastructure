namespace Alidu.Common.Interfaces
{
    public interface IHeaderCommand
    {
        string CommandId { get; }

        void SetCommandId(string commandId);
    }
}