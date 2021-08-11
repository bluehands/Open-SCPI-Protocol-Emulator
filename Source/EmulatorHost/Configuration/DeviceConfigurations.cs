using Domain.Keysight34465A;
using Domain.Keysight3458A;
using FunicularSwitch;
using Newtonsoft.Json;

namespace EmulatorHost.Configuration
{
	public sealed class DeviceConfigurations
	{
		[JsonProperty(Required = Required.AllowNull)]
		public Option<Keysight34465AConfiguration> Keysight34465AConfiguration { get; set; }

		[JsonProperty(Required = Required.AllowNull)]
		public Option<Keysight3458AConfiguration> Keysight3458AConfiguration { get; set; }
	}
}