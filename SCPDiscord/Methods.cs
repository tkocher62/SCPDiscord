using SCPDiscord.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
