using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCPDiscord
{
	public class SCPDiscord : Plugin<Config>
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
			Log.Debug($"Verifying reserved slots status for {userid}...");
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

		public static string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
			+ Path.DirectorySeparatorChar + "SCP Secret Laboratory"
			+ Path.DirectorySeparatorChar + "config"
			+ Path.DirectorySeparatorChar + "global";

		public static string reservedSlots = basePath + Path.DirectorySeparatorChar + "UserIDReservedSlots.txt";
		public static string useridBans = basePath + Path.DirectorySeparatorChar + "UserIdBans.txt";
		public static string ipBans = basePath + Path.DirectorySeparatorChar + "IpBans.txt";

		public static SCPDiscord plugin;

		private EventHandlers ev;

		public override void OnEnabled()
		{
			base.OnEnabled();

			if (!Config.IsEnabled) return;

			plugin = this;
			ev = new EventHandlers();

			Exiled.Events.Handlers.Server.WaitingForPlayers += ev.OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += ev.OnRACommand;
			Exiled.Events.Handlers.Server.SendingConsoleCommand += ev.OnConsoleCommand;
			Exiled.Events.Handlers.Server.RoundStarted += ev.OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded += ev.OnRoundEnd;
			Exiled.Events.Handlers.Server.RestartingRound += ev.OnRoundRestart;
			Exiled.Events.Handlers.Server.RespawningTeam += ev.OnTeamRespawn;

			Exiled.Events.Handlers.Map.Decontaminating += ev.OnDecontamination;

			Exiled.Events.Handlers.Player.Joined += ev.OnPlayerJoin;
			Exiled.Events.Handlers.Player.ChangingRole += ev.OnSetClass;
			Exiled.Events.Handlers.Player.DroppingItem += ev.OnDropItem;
			Exiled.Events.Handlers.Player.PickingUpItem += ev.OnPickupItem;
			Exiled.Events.Handlers.Player.Left += ev.OnPlayerLeave;
			Exiled.Events.Handlers.Player.Hurting += ev.OnPlayerHurt;
			Exiled.Events.Handlers.Player.Dying += ev.OnPlayerDeath;
			Exiled.Events.Handlers.Player.PreAuthenticating += ev.OnPreAuth;
			Exiled.Events.Handlers.Player.ThrowingGrenade += ev.OnGrenadeThrown;
			Exiled.Events.Handlers.Player.ChangingGroup += ev.OnSetGroup;
			Exiled.Events.Handlers.Player.EnteringPocketDimension += ev.OnPocketDimensionEnter;
			Exiled.Events.Handlers.Player.EscapingPocketDimension += ev.OnPocketDimensionEscape;

			Exiled.Events.Handlers.Scp914.ChangingKnobSetting += ev.OnScp914ChangeKnob;
			Exiled.Events.Handlers.Scp914.Activating += ev.OnScp914Activation;

			Exiled.Events.Handlers.Scp079.InteractingTesla += ev.OnScp079TriggerTesla;

			Exiled.Events.Handlers.Scp106.Containing += ev.OnScp106Contain;
		}

		public override void OnDisabled()
		{
			base.OnDisabled();

			Exiled.Events.Handlers.Server.WaitingForPlayers -= ev.OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.SendingRemoteAdminCommand -= ev.OnRACommand;
			Exiled.Events.Handlers.Server.SendingConsoleCommand -= ev.OnConsoleCommand;
			Exiled.Events.Handlers.Server.RoundStarted -= ev.OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded -= ev.OnRoundEnd;
			Exiled.Events.Handlers.Server.RestartingRound -= ev.OnRoundRestart;
			Exiled.Events.Handlers.Server.RespawningTeam -= ev.OnTeamRespawn;

			Exiled.Events.Handlers.Map.Decontaminating -= ev.OnDecontamination;

			Exiled.Events.Handlers.Player.Joined -= ev.OnPlayerJoin;
			Exiled.Events.Handlers.Player.ChangingRole -= ev.OnSetClass;
			Exiled.Events.Handlers.Player.DroppingItem -= ev.OnDropItem;
			Exiled.Events.Handlers.Player.PickingUpItem -= ev.OnPickupItem;
			Exiled.Events.Handlers.Player.Left -= ev.OnPlayerLeave;
			Exiled.Events.Handlers.Player.Hurting -= ev.OnPlayerHurt;
			Exiled.Events.Handlers.Player.Dying -= ev.OnPlayerDeath;
			Exiled.Events.Handlers.Player.PreAuthenticating -= ev.OnPreAuth;
			Exiled.Events.Handlers.Player.ThrowingGrenade -= ev.OnGrenadeThrown;
			Exiled.Events.Handlers.Player.ChangingGroup -= ev.OnSetGroup;
			Exiled.Events.Handlers.Player.EnteringPocketDimension -= ev.OnPocketDimensionEnter;
			Exiled.Events.Handlers.Player.EscapingPocketDimension -= ev.OnPocketDimensionEscape;

			Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= ev.OnScp914ChangeKnob;
			Exiled.Events.Handlers.Scp914.Activating -= ev.OnScp914Activation;

			Exiled.Events.Handlers.Scp079.InteractingTesla -= ev.OnScp079TriggerTesla;

			Exiled.Events.Handlers.Scp106.Containing -= ev.OnScp106Contain;
		}

		public override string Name => "ScpDiscord";
	}
}
