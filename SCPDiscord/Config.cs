using Exiled.API.Interfaces;

namespace SCPDiscord
{
	public class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

		public string LocalAdminPath { get; set; } = string.Empty;
		public string ServerPrefix { get; set; } = string.Empty;
		public string SteamApiKey { get; set; } = string.Empty;

		public int Port { get; set; } = 8080;
	}
}
