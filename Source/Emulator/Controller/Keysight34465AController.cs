using System.Collections.Concurrent;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Keysight34465A;
using Emulator.Command;
using FunicularSwitch;

namespace Emulator.Controller
{
	public class Keysight34465AController : IDeviceController<Keysight34465ACommand>
	{
		private Keysight34465A Device { get; }

		public Keysight34465AController(Keysight34465A device)
		{
			Device = device;
		}
		
		public Task<Result<Keysight34465ACommand>> CommandProcessor(
			Keysight34465ACommand command,
			ConcurrentQueue<IByteArrayConvertible> queue,
			CommandExecutionContext executionContext)
		{
			executionContext.ExecutedCommands.Add(command);

			Task<Result<Keysight34465ACommand>> Identification(Keysight34465ACommand.Identification_ identification) =>
				Device.GetIdentification()
					.Map(result =>
					{
						queue.Enqueue(result);
						return command;
					});

			Task<Result<Keysight34465ACommand>> Read(Keysight34465ACommand.Read_ read) =>
				CommandProcessor(Keysight34465ACommand.Initiate, queue, executionContext)
					.Bind(_ => CommandProcessor(Keysight34465ACommand.Fetch, queue, executionContext))
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> Abort(Keysight34465ACommand.Abort_ abort) =>
				Device.Abort()
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> Initiate(Keysight34465ACommand.Initiate_ initiate) =>
				Device.Initiate()
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> Fetch(Keysight34465ACommand.Fetch_ fetch) =>
				Device.Fetch(queue)
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> ConfigureCurrent(Keysight34465ACommand.ConfigureCurrent_ configureCurrent) =>
				Device.ConfigureCurrent(configureCurrent.ElectricityType, configureCurrent.Range, configureCurrent.Resolution)
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> MeasureCurrent(Keysight34465ACommand.MeasureCurrent_ measureCurrent) =>
				CommandProcessor(Keysight34465ACommand.ConfigureCurrent(measureCurrent.ElectricityType, measureCurrent.Range, measureCurrent.Resolution), queue, executionContext)
					.Bind(_ => CommandProcessor(Keysight34465ACommand.Read, queue, executionContext))
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> ConfigureVoltage(Keysight34465ACommand.ConfigureVoltage_ configureVoltage) =>
				Device.ConfigureVoltage(configureVoltage.ElectricityType, configureVoltage.Range, configureVoltage.Resolution)
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> MeasureVoltage(Keysight34465ACommand.MeasureVoltage_ measureVoltage) =>
				CommandProcessor(Keysight34465ACommand.ConfigureVoltage(measureVoltage.ElectricityType, measureVoltage.Range, measureVoltage.Resolution), queue, executionContext)
					.Bind(_ => CommandProcessor(Keysight34465ACommand.Read, queue, executionContext))
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> DisplayText(Keysight34465ACommand.DisplayText_ displayText) =>
				CommandProcessor(Keysight34465ACommand.ClearDisplay, queue, executionContext)
					.Bind(async _ =>
					{
						await Device.DisplayText(displayText.Text);
						return await CommandProcessor(Keysight34465ACommand.ClearDisplay, queue, executionContext)
							.ConfigureAwait(false);
					})
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> ClearDisplay(Keysight34465ACommand.ClearDisplay_ clearDisplay) =>
				Device.ClearDisplay()
					.Map(_ => command);

			Task<Result<Keysight34465ACommand>> SetImpedance(Keysight34465ACommand.SetImpedance_ setImpedance) =>
				Device.SetImpedance(setImpedance.Impedance)
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
				MeasureVoltage,
				DisplayText,
				ClearDisplay,
				SetImpedance);
		}
	}
}