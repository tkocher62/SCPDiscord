using SCPDiscord.DataObjects;

namespace SCPDiscord
{
	partial class EventHandlers
	{
		private User PlyToUser(Exiled.API.Features.Player player)
		{
			return new User
			{
				name = player.Nickname,
				userid = player.UserId
			};
		}
	}
}
