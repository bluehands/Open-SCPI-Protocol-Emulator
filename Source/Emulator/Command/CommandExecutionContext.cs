using System.Collections.Generic;

namespace Emulator.Command
{
	public class CommandExecutionContext
	{
		public List<ICommand> ExecutedCommands { get; } = new List<ICommand>();
	}
}