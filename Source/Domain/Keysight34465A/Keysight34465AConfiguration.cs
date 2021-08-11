using System.Collections.Generic;
using Domain.Abstractions;

namespace Domain.Keysight34465A
{
	public class Keysight34465AConfiguration : IDeviceConfiguration
	{
		public string Ip { get; init; }
		public int Port { get; init; }
		public string Identification { get; init; }
		public List<double> VoltageInterferenceFactors { get; init; }
		public List<double> CurrentInterferenceFactors { get; init; }
		public double LowImpedanceInterferenceMultiplier { get; init; }
		public double HighImpedanceInterferenceMultiplier { get; init; }

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