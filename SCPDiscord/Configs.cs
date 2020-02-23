using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPDiscord
{
	class Configs
	{
		internal static int port;

		internal static void ReloadConfigs()
		{
			port = Plugin.Config.GetInt("scpd_port", 8080);
		}
	}
}
