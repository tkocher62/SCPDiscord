using System;

namespace SCPDiscord.DataObjects.Events
{
	class UserId
	{
		public string type = "EVENT";
		public string eventName;
		public string time = DateTime.Now.ToString("HH:mm:ss");
		public string userid;
		public string ip;
	}
}
