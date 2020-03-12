namespace SCPDiscord
{
	class Configs
	{
		internal static string localAdminPath;
		internal static string serverPrefix;
		internal static string steamAPIKey;

		internal static int port;

		internal static void ReloadConfigs()
		{
			localAdminPath = Plugin.Config.GetString("scpd_localadmin_path", string.Empty);
			serverPrefix = Plugin.Config.GetString("scpd_server_prefix", string.Empty);
			steamAPIKey = Plugin.Config.GetString("scpd_steamapi_key", string.Empty);

			port = Plugin.Config.GetInt("scpd_port", 8080);
		}
	}
}
