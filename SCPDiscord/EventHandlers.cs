using Exiled.API.Features;
using MEC;
using Exiled.Events.EventArgs;
using SCPDiscord.DataObjects;
using SCPDiscord.DataObjects.Events;
using System.Linq;
using System.Collections.Generic;

namespace SCPDiscord
{
	partial class EventHandlers
	{
		public static Tcp tcp;

		private static bool silentRestart;

		private Dictionary<Exiled.API.Features.Player, RoleType> roles = new Dictionary<Exiled.API.Features.Player, RoleType>();

		public EventHandlers()
		{
			//Configs.ReloadConfigs();

			tcp = new Tcp("127.0.0.1", SCPDiscord.instance.Config.Port);
			tcp.Init();
		}

		public void OnWaitingForPlayers()
		{
			//Configs.ReloadConfigs();

			tcp.SendData(new Generic
			{
				eventName = "WaitingForPlayers"
			});
		}

		public void OnRoundStart()
		{
			roles.Clear();

			tcp.SendData(new Generic
			{
				eventName = "RoundStart",
				param = Exiled.API.Features.Player.List.Where(x => x.UserId != null).Count().ToString()
			});
		}

		public void OnRoundEnd(RoundEndedEventArgs ev)
		{
			tcp.SendData(new Generic
			{
				eventName = "RoundEnd",
				param = ((int)(Round.ElapsedTime.TotalSeconds / 60)).ToString()
			});
		}

		public void OnPlayerJoin(JoinedEventArgs ev)
		{
			Timing.CallDelayed(1f, () => tcp.SendData(new RoleSync
			{
				userid = ev.Player.UserId
			}));

			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "PlayerJoin",
				player = PlyToUser(ev.Player)
			});
		}

		public void OnSetClass(ChangingRoleEventArgs ev)
		{
			if (!roles.ContainsKey(ev.Player)) roles.Add(ev.Player, RoleType.None);
			if (roles[ev.Player] == ev.NewRole || ev.NewRole == RoleType.Spectator) return;
			roles[ev.Player] = ev.NewRole;

			if (ev.Player.UserId != null)
			{
				tcp.SendData(new PlayerParam
				{
					eventName = "SetClass",
					player = PlyToUser(ev.Player),
					param = Conversions.roles.ContainsKey(ev.NewRole) ? Conversions.roles[ev.NewRole] : ev.NewRole.ToString()
				});
			}
		}

		public void OnDropItem(DroppingItemEventArgs ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "DropItem",
				player = PlyToUser(ev.Player),
				param = Conversions.items.ContainsKey(ev.Item.id) ? Conversions.items[ev.Item.id] : ev.Item.id.ToString()
			});
		}

		public void OnPickupItem(PickingUpItemEventArgs ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "PickupItem",
				player = PlyToUser(ev.Player),
				param = Conversions.items.ContainsKey(ev.Pickup.ItemId) ? Conversions.items[ev.Pickup.ItemId] : ev.Pickup.ItemId.ToString()
			});
		}

		public void OnPlayerLeave(LeftEventArgs ev)
		{
			if (ev.Player.UserId != null)
			{
				tcp.SendData(new DataObjects.Events.Player
				{
					eventName = "PlayerLeave",
					player = PlyToUser(ev.Player)
				});
			}
		}

		public void OnPlayerHurt(HurtingEventArgs ev)
		{
			tcp.SendData(new PlayerDamage
			{
				eventName = "PlayerHurt",
				victim = PlyToUser(ev.Target),
				attacker = PlyToUser(ev.Attacker),
				damage = (int)ev.Amount,
				weapon = ev.DamageType.ToString()
			});
		}

		public void OnPlayerDeath(DiedEventArgs ev)
		{
			if (ev.Target.Role != RoleType.Spectator)
			{
				PlayerDamage data = new PlayerDamage
				{
					eventName = "PlayerDeath",
					victim = PlyToUser(ev.Target),
					attacker = PlyToUser(ev.Killer),
					damage = (int)ev.HitInformations.Amount,
					weapon = ev.HitInformations.GetDamageName().ToString()
				};

				DamageTypes.DamageType type = ev.HitInformations.GetDamageType();
				if (type == DamageTypes.Tesla) data.eventName += "Tesla";
				else if (type == DamageTypes.Decont) data.eventName += "Decont";
				else if (type == DamageTypes.Falldown) data.eventName += "Fall";
				else if (type == DamageTypes.Flying) data.eventName += "Flying";
				else if (type == DamageTypes.Lure) data.eventName += "Lure";
				else if (type == DamageTypes.Nuke) data.eventName += "Nuke";
				else if (type == DamageTypes.Pocket) data.eventName += "Pocket";
				else if (type == DamageTypes.Recontainment) data.eventName += "Recont";

				tcp.SendData(data);
			}
		}

		public void OnDecontamination(DecontaminatingEventArgs ev)
		{
			tcp.SendData(new Generic
			{
				eventName = "Decontamination"
			});
		}

		public void OnGrenadeThrown(ThrowingGrenadeEventArgs ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "GrenadeThrown",
				player = PlyToUser(ev.Player),
				param = Conversions.grenades[ev.Id]
			});
		}

		public void OnRACommand(SendingRemoteAdminCommandEventArgs ev)
		{
			string cmd = ev.Name;
			foreach (string arg in ev.Arguments) cmd += $" {arg}";
			tcp.SendData(new Command
			{
				eventName = "RACommand",
				sender = ev.Sender != null ? PlyToUser(ev.Sender) : new User
				{
					name = "Server",
					userid = ""
				},
				command = cmd
			});

			/*string cmd = ev.Command.ToLower();

			if ((cmd == "silentrestart" || cmd == "sr") && ev.Sender.CheckPermission("scpd.sr"))
			{
				ev.Allow = false;
				silentRestart = !silentRestart;
				ev.Sender.RAMessage(silentRestart ? "Server set to silently restart next round." : "Server silent restart cancelled.", true);
			}*/
		}

		public void OnConsoleCommand(SendingConsoleCommandEventArgs ev)
		{
			string cmd = ev.Name;
			foreach (string arg in ev.Arguments) cmd += $" {arg}";
			tcp.SendData(new Command
			{
				eventName = "ConsoleCommand",
				sender = PlyToUser(ev.Player),
				command = cmd
			});
		}

		public void OnPreAuth(PreAuthenticatingEventArgs ev)
		{
			tcp.SendData(new UserId
			{
				eventName = "PreAuth",
				userid = ev.UserId,
				ip = ev.Request.RemoteEndPoint.ToString()
			});
		}

		public void OnRoundRestart()
		{
			tcp.SendData(new Generic
			{
				eventName = "RoundRestart"
			});

			if (silentRestart && SCPDiscord.instance.Config.LocalAdminPath != string.Empty && SCPDiscord.instance.Config.ServerPrefix != string.Empty)
			{
				Timing.CallDelayed(2.5f, () =>
				{
					tcp.SendData(new Restart());
				});
				silentRestart = false;
			}
		}

		public void OnScp079TriggerTesla(InteractingTeslaEventArgs ev)
		{
			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "Scp079TriggerTesla",
				player = PlyToUser(ev.Player)
			});
		}

		public void OnScp914ChangeKnob(ChangingKnobSettingEventArgs ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "Scp914ChangeKnob",
				player = PlyToUser(ev.Player),
				param = Conversions.knobsettings[ev.KnobSetting]
			});
		}

		public void OnTeamRespawn(RespawningTeamEventArgs ev)
		{
			tcp.SendData(new TeamRespawn
			{
				eventName = "TeamRespawn",
				players = Exiled.API.Features.Player.List.Select(x =>
				{
					return PlyToUser(x);
				}).ToArray(),
				team = ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency ? 0 : 1
			});
		}

		public void OnScp106Contain(ContainingEventArgs ev)
		{
			// 'player' is the player who hit the button, not 106
			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "Scp106Contain",
				player = PlyToUser(ev.Player)
			});
		}

		public void OnScp914Activation(ActivatingEventArgs ev)
		{
			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "Scp914Activation",
				player = PlyToUser(ev.Player)
			});
		}

		public void OnSetGroup(ChangingGroupEventArgs ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "SetGroup",
				player = PlyToUser(ev.Player),
				param = ev.NewGroup.BadgeText
			});
		}

		public void OnPocketDimensionEnter(EnteringPocketDimensionEventArgs ev)
		{
			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "PocketDimensionEnter",
				player = PlyToUser(ev.Player)
			});
		}

		public void OnPocketDimensionEscape(EscapingPocketDimensionEventArgs ev)
		{
			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "PocketDimensionEscape",
				player = PlyToUser(ev.Player)
			});
		}
	}
}