using System;

namespace SCPDiscord.DataObjects.Events
{
	class Command
	{
		public string type = "EVENT";
		public string eventName;
		public string time = DateTime.Now.ToString("HH:mm:ss");
		public User sender;
		public string command;
	}
}
