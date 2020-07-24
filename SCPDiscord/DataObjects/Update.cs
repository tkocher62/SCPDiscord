using Exiled.API.Features;
using System.Linq;

namespace SCPDiscord.DataObjects
{
	public class Update
	{
		public string type = "UPDATE";
		public int playerCount = Player.List.Count();
	}
}
