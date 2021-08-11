using System;
using System.IO;
using System.Threading.Tasks;
using Domain.Keysight34465A;
using Domain.Keysight3458A;
using Emulator.Command;
using Emulator.Controller;
using EmulatorHost.Configuration;
using EmulatorHost.Network;
using FunicularSwitch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Protocol.Interpreter;

namespace EmulatorHost
{
	class Program
	{
		private const string DeviceSettingsJsonFileName = "devicesettings.json";

		static Task Main(string[] args)
		{
			return LoadDeviceConfigurations(args)
				.Map(deviceConfigurations =>
					CreateHostBuilder(deviceConfigurations)
						.Build()
						.RunAsync())
				.Match(
					ok => ok,
					error =>
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(error);
						Console.ForegroundColor = ConsoleColor.White;
						return Task.CompletedTask;
					});
		}

		private static IHostBuilder CreateHostBuilder(DeviceConfigurations deviceConfigurations)
		{
			var hostBuilder = Host.CreateDefaultBuilder( /*args*/);

			hostBuilder.ConfigureServices(services =>
				services.AddTransient<TcpServer>());
			deviceConfigurations
				.Keysight34465AConfiguration
				.Match(keysight34465AConfiguration =>
				{
					hostBuilder.ConfigureServices(services =>
					{
						services.AddSingleton<Func<Keysight34465AConfiguration>>(() => keysight34465AConfiguration);
						services.AddSingleton<Keysight34465AProtocolInterpreter>();
						services.AddTransient<Keysight34465A>();
						services.AddTransient<Keysight34465AController>();
						services.AddHostedService<HostedDeviceService<
								Keysight34465AController,
								Keysight34465ACommand,
								Keysight34465AConfiguration,
								Keysight34465AProtocolInterpreter>>();
					});
				});
			deviceConfigurations
				.Keysight3458AConfiguration
				.Match(keysight3458AConfiguration =>
				{
					hostBuilder.ConfigureServices(services =>
					{
						services.AddSingleton<Func<Keysight3458AConfiguration>>(() => keysight3458AConfiguration);
						services.AddSingleton<Keysight3458AProtocolInterpreter>();
						services.AddTransient<Keysight3458A>();
						services.AddTransient<Keysight3458AController>();
						services.AddHostedService<HostedDeviceService<
								Keysight3458AController,
								Keysight3458ACommand,
								Keysight3458AConfiguration,
								Keysight3458AProtocolInterpreter>>();
					});
				});
			return hostBuilder;
		}

		private static Result<DeviceConfigurations> LoadDeviceConfigurations(string[] args)
		{
			var configurationSerializer = new DeviceConfigurationSerializer();

			if (args.Length > 0 && args[0] != null)
			{
				return configurationSerializer.DeserializeDeviceConfigurations(args[0]);
			}

			var configFilePath = Path.Combine(Environment.CurrentDirectory, DeviceSettingsJsonFileName);
			return File.Exists(configFilePath)
				? configurationSerializer.DeserializeDeviceConfigurations(File.ReadAllText(configFilePath))
				: Result.Error<DeviceConfigurations>(
					$"Missing device configuration file {configFilePath}. Pass configuration as start parameters or make sure the config file is present.");
		}
	}
}