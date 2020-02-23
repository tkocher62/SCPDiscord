using EXILED;

namespace SCPDiscord
{
	public class Plugin : EXILED.Plugin
	{
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
			Events.RoundRestartEvent += ev.OnRoundRestart;
			Events.Scp079TriggerTeslaEvent += ev.OnScp079TriggerTesla;
			Events.Scp914KnobChangeEvent += ev.OnScp914ChangeKnob;
			Events.TeamRespawnEvent += ev.OnTeamRespawn;
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
