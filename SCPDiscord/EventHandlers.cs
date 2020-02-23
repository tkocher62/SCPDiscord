using EXILED;
using SCPDiscord.DataObjects;
using SCPDiscord.DataObjects.Events;

namespace SCPDiscord
{
	class EventHandlers
	{
		Tcp tcp;

		public EventHandlers()
		{
			//tcp = new Tcp("127.0.0.1", 1111);
		}

		public void OnWaitingForPlayers()
		{
			tcp.SendData(new Generic
			{
				eventName = "WaitingForPlayers"
			});
		}

		public void OnRoundStart()
		{
			tcp.SendData(new Generic
			{
				eventName = "RoundStart"
			});
		}

		public void OnRoundEnd()
		{
			tcp.SendData(new Generic
			{
				eventName = "RoundEnd"
			});
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			tcp.SendData(new Player
			{
				eventName = "PlayerJoin",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					steamid = ev.Player.characterClassManager.UserId.Replace("@steam", "")
				}
			});
		}

		public void OnSetClass(SetClassEvent ev)
		{
			tcp.SendData(new SetClass
			{
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					steamid = ev.Player.characterClassManager.UserId.Replace("@steam", "")
				},
				role = ev.Role
			});
		}

		public void OnDropItem(ref DropItemEvent ev)
		{
			tcp.SendData(new ItemInteract
			{
				eventName = "DropItem",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					steamid = ev.Player.characterClassManager.UserId.Replace("@steam", "")
				},
				item = ev.Item.id
			});
		}

		public void OnPickupItem(ref PickupItemEvent ev)
		{
			tcp.SendData(new ItemInteract
			{
				eventName = "PickupItem",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					steamid = ev.Player.characterClassManager.UserId.Replace("@steam", "")
				},
				item = ev.Item.info.itemId
			});
		}

		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			tcp.SendData(new Player
			{
				eventName = "PlayerLeave",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					steamid = ev.Player.characterClassManager.UserId.Replace("@steam", "")
				}
			});
		}

		public void OnPlayerHurt(ref PlayerHurtEvent ev)
		{
			tcp.SendData(new PlayerDamage
			{
				eventName = "PlayerHurt",
				victim = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					steamid = ev.Player.characterClassManager.UserId.Replace("@steam", "")
				},
				attacker = new User
				{
					name = ev.Attacker.nicknameSync.Network_myNickSync,
					steamid = ev.Attacker.characterClassManager.UserId.Replace("@steam", "")
				},
				damage = (int)ev.Info.Amount,
				weapon = ev.Info.GetDamageName().ToString()
			});
		}

		public void OnPlayerDeath(ref PlayerDeathEvent ev)
		{
			tcp.SendData(new PlayerDamage
			{
				eventName = "PlayerDeath",
				victim = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					steamid = ev.Player.characterClassManager.UserId.Replace("@steam", "")
				},
				attacker = new User
				{
					name = ev.Killer.nicknameSync.Network_myNickSync,
					steamid = ev.Killer.characterClassManager.UserId.Replace("@steam", "")
				},
				damage = (int)ev.Info.Amount,
				weapon = ev.Info.GetDamageName().ToString()
			});
		}
	}
}
