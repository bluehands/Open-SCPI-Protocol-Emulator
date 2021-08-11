using System.Collections.Generic;
using Domain.Abstractions;

namespace Domain.Keysight3458A
{
	public class Keysight3458AConfiguration : IDeviceConfiguration
	{
		public string Ip { get; init; }
		public int Port { get; init; }
		public string Identification { get; init; }
		public List<double> VoltageInterferenceFactors { get; init; }
		public List<double> CurrentInterferenceFactors { get; init; }

		public double CurrentRangeAuto { get; init; }
		public double CurrentRangeMin { get; init; }
		public double CurrentRangeMax { get; init; }
		public double CurrentRangeDef { get; init; }

		public double VoltageRangeAuto { get; init; }
		public double VoltageRangeMin { get; init; }
		public double VoltageRangeMax { get; init; }
		public double VoltageRangeDef { get; init; }
	}
}