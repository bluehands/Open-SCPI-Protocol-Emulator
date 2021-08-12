using System.IO;
using System.Text;
using Domain.Keysight34465A;
using Domain.Keysight3458A;
using FunicularSwitch.Extensions;
using System;
using FunicularSwitch;
using Newtonsoft.Json;

namespace EmulatorHost.Configuration
{
	

	public class DeviceConfigurationSerializer
	{
		private JsonSerializer Serializer { get; set; }

		public DeviceConfigurationSerializer()
		{
			var jsonSerializerSettings = new JsonSerializerSettings()
			{
				Converters =
				{
					new OptionConverter<Keysight34465AConfiguration>(),
					new OptionConverter<Keysight3458AConfiguration>(),
				},
				// Formatting = Formatting.Indented
			};
			Serializer = JsonSerializer.Create(jsonSerializerSettings);
		}

		public string SerializeDeviceConfigurations(DeviceConfigurations deviceConfigurations)
		{
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			using JsonWriter writer = new JsonTextWriter(stringWriter);
			Serializer.Serialize(writer, deviceConfigurations);
			return stringBuilder.ToString();
		}

		public Result<DeviceConfigurations> DeserializeDeviceConfigurations(string json)
		{
			try
			{
				var reader = new JsonTextReader(new StringReader(json));
				return Serializer.Deserialize<DeviceConfigurations>(reader);
			}
			catch (Exception e)
			{
				return Result.Error<DeviceConfigurations>(
					$"{typeof(DeviceConfigurationSerializer).BeautifulName()}: {e.Message}");
			}
		}
	}

	public class OptionConverter<T> : JsonConverter
	{
		public override bool CanConvert(Type objectType) => typeof(Option<T>).IsAssignableFrom(objectType);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			var value = serializer.Deserialize<T>(reader);
			return value == null
				? Option<T>.None
				: Option.Some(value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is Option<T> option)
			{
				option.Match(
					some: optionValue => serializer.Serialize(writer, optionValue),
					none: writer.WriteNull);
			}
			else
			{
				writer.WriteNull();
			}
		}
	}
}