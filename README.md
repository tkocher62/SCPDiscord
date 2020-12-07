# SCPDiscord

This is a plugin I made in collaboration with a friend to monitor activity on my game server for [SCP: Secret Laboratory.](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory/) In this game, many players are assigned various roles to play out. In order to monitor player counts and issues it's helpful to be able to find statistics and logs quickly. This plugin communicates with a [Discord](https://discord.com/) bot over TCP networking, sending it live player counts, logs, syncing with [Patreon](https://www.patreon.com/), and allowing for remote command execution.

### Live Player Counts
This plugin displays live player counts that update every 5 seconds to the Discord server, allowing for quick viewing of the status of all four game servers.

![](https://github.com/tkocher62/SCPDiscord/blob/master/playercount.png)

### Real-Time Logs
The plugin informs the Discord bot of everything that happens in the server, allowing for real-time logs to be kept for each server.

![](https://github.com/tkocher62/SCPDiscord/blob/master/logs.png)

### Admin Logs
Alongside game logs, the plugin also keeps logs of every administrative command run in the server as well as special group assignments. This is especially useful in monitoring server staff's actions on the server, ensuring they're not abusing their power.

![](https://github.com/tkocher62/SCPDiscord/blob/master/adminlog.png)

### Patreon Integration
Patreon is a third party service that allows individuals to purchase support tiers from creators in exchange for certain rewards. I have setup this system for my servers to give players in game roles based on the tier they purchased. In order to do this, users must first sync their Steam accounts with their Discord account through the bot.

![](https://github.com/tkocher62/SCPDiscord/blob/master/sync.png)

Then, when players join the server, they will be granted the role corresponding with their purchased tier along with the in game benefits that come with it.

![](https://github.com/tkocher62/SCPDiscord/blob/master/ingamesync.png)

### Remote Command Execution
Authorized users have permission to execute in game commands through the Discord server. This is very useful for performing quick actions such as kicking a troublemaker without the hassle of loading up the game and connecting to the servers.

![](https://github.com/tkocher62/SCPDiscord/blob/master/requestkick.png)

![](https://github.com/tkocher62/SCPDiscord/blob/master/kicked.png)
