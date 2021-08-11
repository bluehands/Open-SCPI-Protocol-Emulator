using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FunicularSwitch;

namespace Emulator.Command
{
	public class
		CommandExecutor<TCommand, TOutputQueue, TExecutionResult> : ICommandExecutor<TCommand, TOutputQueue,
			TExecutionResult>
		where TCommand : ICommand
	{
		private readonly ConcurrentQueue<TOutputQueue> outputQueue = new ConcurrentQueue<TOutputQueue>();

		public Task<Result<(TExecutionResult, CommandExecutionContext)>> Execute(
			TCommand command,
			Func<TCommand, ConcurrentQueue<TOutputQueue>, CommandExecutionContext,
				Task<Result<TExecutionResult>>> commandProcessor,
			CommandExecutionContext commandExecutionContext)
		{
			return commandProcessor(command, outputQueue, commandExecutionContext)
				.Map(executionResult => (executionResult, commandExecutionContext));
		}

		public ConcurrentQueue<TOutputQueue> GetOutputQueue() => outputQueue;
	}
}