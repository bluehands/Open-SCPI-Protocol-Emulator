using System;
using System.Collections.Generic;
using Domain.Keysight34465A;
using Domain.Keysight3458A;
using EmulatorHost.Configuration;
using FluentAssertions;
using FunicularSwitch;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmulatorHostTests.Configuration
{
	[TestClass]
	public class ConfigurationSerializerTests
	{
		[TestMethod]
		public void TestSerializationWithNone()
		{
			var configurationSerializer = new DeviceConfigurationSerializer();
			var deviceConfigurations = new DeviceConfigurations()
			{
				Keysight34465AConfiguration = Option<Keysight34465AConfiguration>.None
			};

			var json = configurationSerializer.SerializeDeviceConfigurations(deviceConfigurations);
			var result = configurationSerializer.DeserializeDeviceConfigurations(json);

			result.Should().BeOfType<Ok<DeviceConfigurations>>().Which.Value.Keysight34465AConfiguration.Should()
				.BeOfType<None<Keysight34465AConfiguration>>();
		}

		[TestMethod]
		public void TestSerializationNotPresent()
		{
			var configurationSerializer = new DeviceConfigurationSerializer();

			var result = configurationSerializer.DeserializeDeviceConfigurations("{}");
			result.Should().BeOfType<Error<DeviceConfigurations>>();
		}

		[TestMethod]
		public void TestSerializationWithNull()
		{
			var configurationSerializer = new DeviceConfigurationSerializer();
			var deviceConfigurations = new DeviceConfigurations()
			{
				Keysight34465AConfiguration = null
			};

			var expectedDeviceConfigurations = new DeviceConfigurations()
			{
				Keysight34465AConfiguration = Option<Keysight34465AConfiguration>.None
			};


			var json = configurationSerializer.SerializeDeviceConfigurations(deviceConfigurations);
			var result = configurationSerializer.DeserializeDeviceConfigurations(json);

			result.Should().BeOfType<Ok<DeviceConfigurations>>().Which.Value.Keysight34465AConfiguration.Should()
				.BeOfType<None<Keysight34465AConfiguration>>();
		}

		[TestMethod]
		public void TestSerialization()
		{
			var configurationSerializer = new DeviceConfigurationSerializer();
			var deviceConfigurations = new DeviceConfigurations()
			{
				Keysight34465AConfiguration = new Keysight34465AConfiguration
				{
					Ip = "127.0.0.1",
					Port = 5025,
					Identification =
						"Keysight Technologies,34465A,21-BH-34465-EMULATOR,0.01.00-01.00-01.00-01.00-01-01",
					VoltageInterferenceFactors = new List<double>
						{0, 0.001, 0.002, 0.001, 0, -0.001, -0.002, -0.001},
					CurrentInterferenceFactors = new List<double>
						{0, 0.0001, 0.0002, 0.0001, 0, -0.0001, -0.0002, -0.0001},
					LowImpedanceInterferenceMultiplier = 2,
					HighImpedanceInterferenceMultiplier = 1,

					VoltageRangeAuto = 300,
					VoltageRangeMin = 100,
					VoltageRangeMax = 1000,
					VoltageRangeDef = 500,

					CurrentRangeAuto = 0.00001,
					CurrentRangeMin = 0.00001,
					CurrentRangeMax = 0.00001,
					CurrentRangeDef = 0.00001,
				},
				Keysight3458AConfiguration = new Keysight3458AConfiguration
				{
					Ip = "127.0.0.1",
					Port = 5026,
					Identification =
						"Keysight Technologies, 3458A-EMULATOR,0.01.00-01.00-01.00-01.00-01-01",
					VoltageInterferenceFactors = new List<double>
						{0, 0.001, 0.002, 0.001, 0, -0.001, -0.002, -0.001},
					CurrentInterferenceFactors = new List<double>
						{0, 0.0001, 0.0002, 0.0001, 0, -0.0001, -0.0002, -0.0001},
					VoltageRangeAuto = 300,
					VoltageRangeMin = 100,
					VoltageRangeMax = 1000,
					VoltageRangeDef = 500,

					CurrentRangeAuto = 0.00001,
					CurrentRangeMin = 0.00001,
					CurrentRangeMax = 0.00001,
					CurrentRangeDef = 0.00001,
				}
			};

			var json = configurationSerializer.SerializeDeviceConfigurations(deviceConfigurations);

			Console.WriteLine(json);

			var result = configurationSerializer.DeserializeDeviceConfigurations(json);

			result.Should().BeOfType<Ok<DeviceConfigurations>>().Which.Value.Keysight34465AConfiguration.Should()
				.BeOfType<Some<Keysight34465AConfiguration>>();
		}
	}
}