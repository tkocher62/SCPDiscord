using System;

namespace SCPDiscord.DataObjects.Events
{
	class Generic
	{
		public string type = "EVENT";
		public string eventName;
		public string time = DateTime.Now.ToString("HH:mm:ss");
	}
}
