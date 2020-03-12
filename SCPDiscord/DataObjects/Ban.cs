namespace SCPDiscord.DataObjects
{
	class Ban
	{
		public string type = "BAN";
		public User player;
		public int duration;
		public bool success;
		public bool offline;
	}
}
