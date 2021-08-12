using Emulator.Command;
using FunicularSwitch;

namespace Protocol.Interpreter
{
    public interface IProtocolInterpreter<TCommand>
    where TCommand : ICommand
    {
        Result<TCommand> GetCommand(string input);
    }
}