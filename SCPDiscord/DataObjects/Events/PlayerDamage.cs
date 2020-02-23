using System;

namespace SCPDiscord.DataObjects.Events
{
	class PlayerDamage
	{
		public string type = "EVENT";
		public string eventName;
		public string time = DateTime.Now.ToString("HH:mm:ss");
		public User victim;
		public User attacker;
		public int damage;
		public string weapon;
	}
}
