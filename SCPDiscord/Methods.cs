using SCPDiscord.DataObjects;

namespace SCPDiscord
{
	partial class EventHandlers
	{
		private User HubToUser(ReferenceHub hub)
		{
			return new User
			{
				name = hub.nicknameSync.Network_myNickSync,
				userid = hub.characterClassManager.UserId
			};
		}
	}
}
