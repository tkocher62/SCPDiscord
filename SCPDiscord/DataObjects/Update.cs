using EXILED.Extensions;
using System.Linq;

namespace SCPDiscord.DataObjects
{
	public class Update
	{
		public string type = "UPDATE";
		public int playerCount = Player.GetHubs().Count();
	}
}
