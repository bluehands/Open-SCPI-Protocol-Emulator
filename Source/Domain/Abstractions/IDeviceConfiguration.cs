namespace Domain.Abstractions
{
	public interface IDeviceConfiguration
	{
		public string Ip { get; init; }
		public int Port { get; init; }
		public string Identification { get; init; }
	}
}