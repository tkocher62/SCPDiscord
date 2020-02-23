using System;

namespace SCPDiscord.DataObjects.Events
{
	class SetClass
	{
		public string type = "EVENT";
		public string eventName = "SetClass";
		public string time = DateTime.Now.ToString("HH:mm:ss");
		public User player;
		public RoleType role;
	}
}
