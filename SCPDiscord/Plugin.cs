using EXILED;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCPDiscord
{
	public class Plugin : EXILED.Plugin
	{
		public static List<string> setRoleGroups = new List<string>()
		{
			"patron1",
			"patron2",
			"patron3"
		};

		public static List<string> reservedSlotGroups = new List<string>()
		{
			"patron3",
			"patron4",
			"patron5",
			"patron6",
			"patron7"
		};

		public static void VerifyReservedSlot(string userid)
		{
			Log.Debug($"Verifying reserved slots statusfor {userid}...");
			List<string> lines = File.ReadAllLines(reservedSlots).ToList();
			for (int i = 0; i < lines.Count; i++)
			{
				if (lines[i] == userid)
				{
					Log.Debug("Reserved slot found, removing...");
					lines.RemoveAt(i);
					File.WriteAllLines(reservedSlots, lines);
					// Send message to server to reload reserved slots?
					return;
				}
			}
		}

		public static string reservedSlots = 
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
			+ Path.DirectorySeparatorChar + "SCP Secret Laboratory"
			+ Path.DirectorySeparatorChar + "config"
			+ Path.DirectorySeparatorChar + "global"
			+ Path.DirectorySeparatorChar + "UserIDReservedSlots.txt";

		private EventHandlers ev;

		public override void OnEnable()
		{
			ev = new EventHandlers();
			Events.WaitingForPlayersEvent += ev.OnWaitingForPlayers;
			Events.PlayerJoinEvent += ev.OnPlayerJoin;
			Events.SetClassEvent += ev.OnSetClass;
			Events.DropItemEvent += ev.OnDropItem;
			Events.PickupItemEvent += ev.OnPickupItem;
			Events.PlayerLeaveEvent += ev.OnPlayerLeave;
			Events.PlayerHurtEvent += ev.OnPlayerHurt;
			Events.PlayerDeathEvent += ev.OnPlayerDeath;
			Events.DecontaminationEvent += ev.OnDecontamination;
			Events.GrenadeThrownEvent += ev.OnGrenadeThrown;
			Events.RemoteAdminCommandEvent += ev.OnRACommand;
			Events.ConsoleCommandEvent += ev.OnConsoleCommand;
			Events.PreAuthEvent += ev.OnPreAuth;
			Events.RoundStartEvent += ev.OnRoundStart;
			Events.RoundEndEvent += ev.OnRoundEnd;
			Events.RoundRestartEvent += ev.OnRoundRestart;
			Events.Scp079TriggerTeslaEvent += ev.OnScp079TriggerTesla;
			Events.Scp914KnobChangeEvent += ev.OnScp914ChangeKnob;
			Events.TeamRespawnEvent += ev.OnTeamRespawn;
			Events.Scp106ContainEvent += ev.OnScp106Contain;
			Events.Scp914ActivationEvent += ev.OnScp914Activation;
			Events.SetGroupEvent += ev.OnSetGroup;
			Events.PocketDimEnterEvent += ev.OnPocketDimensionEnter;
			Events.PocketDimEscapedEvent += ev.OnPocketDimensionEscape;
		}

		public override void OnDisable()
		{
			Events.WaitingForPlayersEvent -= ev.OnWaitingForPlayers;
			Events.PlayerJoinEvent -= ev.OnPlayerJoin;
			ev = null;
		}

		public override void OnReload() { }

		public override string getName { get; } = "SCPDiscord";
	}
}
