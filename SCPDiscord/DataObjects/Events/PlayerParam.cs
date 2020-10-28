using System;

namespace SCPDiscord.DataObjects.Events
{
	class PlayerParam
	{
		public string type = "EVENT";
		public string eventName;
		public string time = DateTime.Now.ToString("HH:mm:ss");
		public User player;
		public object param;
	}
}
