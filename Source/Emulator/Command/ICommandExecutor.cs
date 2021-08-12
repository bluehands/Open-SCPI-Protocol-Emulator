using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FunicularSwitch;

namespace Emulator.Command
{
	public interface ICommandExecutor<TCommand, TOutputQueue, TExecutionResult>
	where TCommand : ICommand
	{
		Task<Result<(TExecutionResult, CommandExecutionContext)>> Execute(
			TCommand command,
			Func<TCommand, ConcurrentQueue<TOutputQueue>, CommandExecutionContext,
				Task<Result<TExecutionResult>>> commandProcessor,
			CommandExecutionContext commandExecutionContext);

		ConcurrentQueue<TOutputQueue> GetOutputQueue();
	}
}