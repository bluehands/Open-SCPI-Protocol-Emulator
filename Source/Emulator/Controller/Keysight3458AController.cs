using System.Collections.Concurrent;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Keysight3458A;
using Emulator.Command;
using FunicularSwitch;

namespace Emulator.Controller
{
	public class Keysight3458AController : IDeviceController<Keysight3458ACommand>
	{
		private Keysight3458A Device { get; }

		public Keysight3458AController(Keysight3458A device)
		{
			Device = device;
		}
		public Task<Result<Keysight3458ACommand>> CommandProcessor(
			Keysight3458ACommand command,
			ConcurrentQueue<IByteArrayConvertible> queue,
			CommandExecutionContext executionContext)
		{
			executionContext.ExecutedCommands.Add(command);

			Task<Result<Keysight3458ACommand>> Identification(Keysight3458ACommand.Identification_ identification) =>
				Device.GetIdentification()
					.Map(result =>
					{
						queue.Enqueue(result);
						return command;
					});

			Task<Result<Keysight3458ACommand>> Read(Keysight3458ACommand.Read_ read) =>
				CommandProcessor(Keysight3458ACommand.Initiate, queue, executionContext)
					.Bind(_ => CommandProcessor(Keysight3458ACommand.Fetch, queue, executionContext))
					.Map(_ => command);

			Task<Result<Keysight3458ACommand>> Abort(Keysight3458ACommand.Abort_ abort) =>
				Device.Abort()
					.Map(_ => command);

			Task<Result<Keysight3458ACommand>> Initiate(Keysight3458ACommand.Initiate_ initiate) =>
				Device.Initiate()
					.Map(_ => command);

			Task<Result<Keysight3458ACommand>> Fetch(Keysight3458ACommand.Fetch_ fetch) =>
				Device.Fetch(queue)
					.Map(_ => command);

			Task<Result<Keysight3458ACommand>> ConfigureCurrent(Keysight3458ACommand.ConfigureCurrent_ configureCurrent) =>
				Device.ConfigureCurrent(configureCurrent.ElectricityType, configureCurrent.Range, configureCurrent.Resolution)
					.Map(_ => command);

			Task<Result<Keysight3458ACommand>> MeasureCurrent(Keysight3458ACommand.MeasureCurrent_ measureCurrent) =>
				CommandProcessor(Keysight3458ACommand.ConfigureCurrent(measureCurrent.ElectricityType, measureCurrent.Range, measureCurrent.Resolution), queue, executionContext)
					.Bind(_ => CommandProcessor(Keysight3458ACommand.Read, queue, executionContext))
					.Map(_ => command);

			Task<Result<Keysight3458ACommand>> ConfigureVoltage(Keysight3458ACommand.ConfigureVoltage_ configureVoltage) =>
				Device.ConfigureVoltage(configureVoltage.ElectricityType, configureVoltage.Range, configureVoltage.Resolution)
					.Map(_ => command);

			Task<Result<Keysight3458ACommand>> MeasureVoltage(Keysight3458ACommand.MeasureVoltage_ measureVoltage) =>
				CommandProcessor(Keysight3458ACommand.ConfigureVoltage(measureVoltage.ElectricityType, measureVoltage.Range, measureVoltage.Resolution), queue, executionContext)
					.Bind(_ => CommandProcessor(Keysight3458ACommand.Read, queue, executionContext))
					.Map(_ => command);

			return command.Match(
				Identification,
				Read,
				Abort,
				Initiate,
				Fetch,
				ConfigureCurrent,
				MeasureCurrent,
				ConfigureVoltage,
				MeasureVoltage);
		}
	}
}