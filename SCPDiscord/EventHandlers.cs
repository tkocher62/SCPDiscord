using EXILED;
using MEC;
using EXILED.Extensions;
using SCPDiscord.DataObjects;
using SCPDiscord.DataObjects.Events;
using System.Linq;
using System.Collections.Generic;

namespace SCPDiscord
{
	class EventHandlers
	{
		public static Tcp tcp;

		private static bool silentRestart;

		private Dictionary<ReferenceHub, RoleType> roles = new Dictionary<ReferenceHub, RoleType>();

		public EventHandlers()
		{
			Configs.ReloadConfigs();

			tcp = new Tcp("127.0.0.1", Configs.port);
			tcp.Init();
		}

		public void OnWaitingForPlayers()
		{
			Configs.ReloadConfigs();

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
				param = EXILED.Extensions.Player.GetHubs().Count().ToString()
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
			Timing.CallDelayed(1f, () => tcp.SendData(new RoleSync
			{
				userid = ev.Player.characterClassManager.UserId
			}));

			tcp.SendData(new SCPDiscord.DataObjects.Events.Player
			{
				eventName = "PlayerJoin",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				}
			});
		}

		public void OnSetClass(SetClassEvent ev)
		{
			if (!roles.ContainsKey(ev.Player)) roles.Add(ev.Player, RoleType.None);
			if (roles[ev.Player] == ev.Role || ev.Role == RoleType.Spectator) return;
			roles[ev.Player] = ev.Role;

			if (ev.Player.characterClassManager.UserId != null)
			{
				tcp.SendData(new PlayerParam
				{
					eventName = "SetClass",
					player = new User
					{
						name = ev.Player.nicknameSync.Network_myNickSync,
						userid = ev.Player.characterClassManager.UserId
					},
					param = Conversions.roles[ev.Role]
				});
			}
		}

		public void OnDropItem(ref DropItemEvent ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "DropItem",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				},
				param = Conversions.items[ev.Item.id]
			});
		}

		public void OnPickupItem(ref PickupItemEvent ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "PickupItem",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				},
				param = Conversions.items[ev.Item.ItemId]
			});
		}

		public void OnPlayerLeave(PlayerLeaveEvent ev)
		{
			if (ev.Player.characterClassManager.UserId != null)
			{
				tcp.SendData(new DataObjects.Events.Player
				{
					eventName = "PlayerLeave",
					player = new User
					{
						name = ev.Player.nicknameSync.Network_myNickSync,
						userid = ev.Player.characterClassManager.UserId
					}
				});
			}
		}

		public void OnPlayerHurt(ref PlayerHurtEvent ev)
		{
			tcp.SendData(new PlayerDamage
			{
				eventName = "PlayerHurt",
				victim = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				},
				attacker = new User
				{
					name = ev.Attacker.nicknameSync.Network_myNickSync,
					userid = ev.Attacker.characterClassManager.UserId
				},
				damage = (int)ev.Info.Amount,
				weapon = ev.Info.GetDamageName().ToString()
			});
		}

		public void OnPlayerDeath(ref PlayerDeathEvent ev)
		{
			if (ev.Player.GetRole() != RoleType.Spectator)
			{
				PlayerDamage data = new PlayerDamage
				{
					eventName = "PlayerDeath",
					victim = new User
					{
						name = ev.Player.nicknameSync.Network_myNickSync,
						userid = ev.Player.characterClassManager.UserId
					},
					attacker = new User
					{
						name = ev.Killer.nicknameSync.Network_myNickSync,
						userid = ev.Killer.characterClassManager.UserId
					},
					damage = (int)ev.Info.Amount,
					weapon = ev.Info.GetDamageName().ToString()
				};

				DamageTypes.DamageType type = ev.Info.GetDamageType();
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

		public void OnDecontamination(ref DecontaminationEvent ev)
		{
			tcp.SendData(new Generic
			{
				eventName = "Decontamination"
			});
		}

		public void OnGrenadeThrown(ref GrenadeThrownEvent ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "GrenadeThrown",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				},
				param = Conversions.grenades[ev.Id]
			});
		}

		public void OnRACommand(ref RACommandEvent ev)
		{
			ReferenceHub ply = EXILED.Extensions.Player.GetPlayer(ev.Sender.SenderId);

			tcp.SendData(new Command
			{
				eventName = "RACommand",
				sender = new User
				{
					name = ply != null ? ply.nicknameSync.Network_myNickSync : "Server",
					userid = ply != null ? ply.characterClassManager.UserId : ""
				},
				command = ev.Command
			});

			string cmd = ev.Command.ToLower();

			if ((cmd == "silentrestart" || cmd == "sr") && ply.CheckPermission("scpd.sr"))
			{
				ev.Allow = false;
				silentRestart = !silentRestart;
				ev.Sender.RAMessage(silentRestart ? "Server set to silently restart next round." : "Server silent restart cancelled.", true);
			}
		}

		public void OnConsoleCommand(ConsoleCommandEvent ev)
		{
			tcp.SendData(new Command
			{
				eventName = "ConsoleCommand",
				sender = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				},
				command = ev.Command
			});
		}

		public void OnPreAuth(ref PreauthEvent ev)
		{
			string remote = ev.Request.RemoteEndPoint.ToString();
			tcp.SendData(new UserId
			{
				eventName = "PreAuth",
				userid = ev.UserId,
				ip = remote.Substring(0, remote.IndexOf(":"))
			});
		}

		public void OnRoundRestart()
		{
			tcp.SendData(new Generic
			{
				eventName = "RoundRestart"
			});

			if (silentRestart && Configs.localAdminPath != string.Empty && Configs.serverPrefix != string.Empty)
			{
				Timing.CallDelayed(2.5f, () =>
				{
					tcp.SendData(new Restart());
				});
				silentRestart = false;
			}
		}

		public void OnScp079TriggerTesla(ref Scp079TriggerTeslaEvent ev)
		{
			tcp.SendData(new SCPDiscord.DataObjects.Events.Player
			{
				eventName = "Scp079TriggerTesla",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				}
			});
		}

		public void OnScp914ChangeKnob(ref Scp914KnobChangeEvent ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "Scp914ChangeKnob",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				},
				param = Conversions.knobsettings[ev.KnobSetting]
			});
		}

		public void OnTeamRespawn(ref TeamRespawnEvent ev)
		{
			tcp.SendData(new TeamRespawn
			{
				eventName = "TeamRespawn",
				players = EXILED.Extensions.Player.GetHubs().Select(x =>
				{
					return new User
					{
						name = x.nicknameSync.Network_myNickSync,
						userid = x.characterClassManager.UserId
					};
				}).ToArray(),
				team = ev.IsChaos ? 0 : 1
			});
		}

		/*public void OnFemurEnter(FemurEnterEvent ev)
		{
			tcp.SendData(new SCPDiscord.DataObjects.Events.Player
			{
				eventName = "FemurEnter",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				}
			});
		}*/

		public void OnScp106Contain(Scp106ContainEvent ev)
		{
			// 'player' is the player who hit the button, not 106
			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "Scp106Contain",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				}
			});
		}

		public void OnScp914Activation(ref Scp914ActivationEvent ev)
		{
			tcp.SendData(new DataObjects.Events.Player
			{
				eventName = "Scp914Activation",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				}
			});
		}

		public void OnSetGroup(SetGroupEvent ev)
		{
			tcp.SendData(new PlayerParam
			{
				eventName = "SetGroup",
				player = new User
				{
					name = ev.Player.nicknameSync.Network_myNickSync,
					userid = ev.Player.characterClassManager.UserId
				},
				param = ev.Group.BadgeText
			});
		}
	}
}