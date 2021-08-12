using System.Collections.Concurrent;
using System.Threading.Tasks;
using Domain.Abstractions;
using Emulator.Command;
using FunicularSwitch;

namespace Emulator.Controller
{
	public interface IDeviceController<TCommand>
	where TCommand : ICommand
	{
		Task<Result<TCommand>> CommandProcessor(
			TCommand command,
			ConcurrentQueue<IByteArrayConvertible> queue,
			CommandExecutionContext executionContext);
		//todo add cancelation token
	}
}