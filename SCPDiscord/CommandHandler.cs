using EXILED;
using EXILED.Extensions;
using Newtonsoft.Json.Linq;
using SCPDiscord.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SCPDiscord
{
	class CommandHandler
	{
		private static WebClient webclient = new WebClient();

		public static void HandleCommand(JObject o)
		{
			try
			{
				string type = (string)o["type"];
				if (type == "IDENT")
				{
					if ((string)o["data"] == "PASS") Log.Debug($"Server {ServerConsole.Port} passed identification.");
					else if ((string)o["data"] == "FAIL") Log.Warn($"Server {ServerConsole.Port} failed identification.");
				}
				else if (type == "UPDATE")
				{
					EventHandlers.tcp.SendData(new Update());
				}
				else if (type == "ROLESYNC")
				{
					string userid = (string)o["userid"];

					if (o["group"] == null)
					{
						Log.Debug($"No role sync found for {userid}");
						Plugin.VerifyReservedSlot(userid);
						return;
					}

					string group = (string)o["group"];

					UserGroup userGroup = ServerStatic.PermissionsHandler.GetGroup(group);
					if (userGroup == null)
					{
						Log.Error($"Attempted to assign invalid user group {group} to {userid}");
						return;
					}

					ReferenceHub player = Player.GetPlayer(userid);
					if (player == null)
					{
						Log.Error($"Error assigning user group to {userid}, player not found.");
						return;
					}

					if (Plugin.setRoleGroups.Contains(group))
					{
						Log.Debug($"Assigning role: {userGroup} to {userid}.");
						player.serverRoles.SetGroup(userGroup, false);
					}
					if (Plugin.reservedSlotGroups.Contains(group))
					{
						// grant reserved slot
						Log.Debug("Player has necessary rank for reserved slot, checking...");
						List<string> lines = File.ReadAllLines(Plugin.reservedSlots).ToList();
						if (!lines.Contains(userid))
						{
							Log.Debug("Reserved slot not found, adding player...");
							lines.Add(userid);
							File.WriteAllLines(Plugin.reservedSlots, lines);
							// This only reloads the slots on the current server, change this to reload on every server?
							// Might not work
							ReservedSlot.Reload();
						}
					}
					else
					{
						Plugin.VerifyReservedSlot(userid);
					}
				}
				else if (type == "COMMAND")
				{
					GameCore.Console.singleton.TypeCommand((string)o["command"]);
				}
				else if (type == "BAN")
				{
					bool isuid = false;
					string uid = (string)o["user"];
					if (!uid.Contains("@steam") && !uid.Contains("@discord"))
					{
						if (!uid.Contains("."))
						{
							isuid = true;
							uid += "@steam";
						}
					}
					else
					{
						isuid = true;
					}
					ReferenceHub player = Player.GetPlayer(uid);
					int min = (int)o["min"];
					string reason = (string)o["reason"];

					Ban ban = new Ban
					{
						player = null,
						duration = min,
						success = true,
						offline = false
					};

					if (player != null)
					{
						PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(player.gameObject, min, reason, "Server");

						ban.player = new User
						{
							name = player.nicknameSync.Network_myNickSync,
							userid = player.characterClassManager.UserId
						};
					}
					else
					{
						if (isuid)
						{
							ban.offline = true;

							ban.player = new User
							{
								name = "Offline Player",
								userid = uid
							};

							if (Configs.steamAPIKey != string.Empty)
							{
								string data = null;
								try
								{
									data = webclient.DownloadString($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={Configs.steamAPIKey}&format=json&steamids={uid.Replace("@steam", "")}");
								}
								catch
								{
									Log.Debug("Failed to get profile data from SteamAPI.");
								}
								JObject o2 = JObject.Parse(data);

								if (o2 != null)
								{
									ban.player.name = (string)o2["response"]["players"][0]["personaname"];
								}
							}

							BanHandler.IssueBan(new BanDetails()
							{
								OriginalName = ban.player.name,
								Id = uid,
								IssuanceTime = TimeBehaviour.CurrentTimestamp(),
								Expires = DateTime.UtcNow.AddMinutes((double)min).Ticks,
								Reason = reason,
								Issuer = "Server"
							}, BanHandler.BanType.UserId);
						}
						else if (uid.Contains("."))
						{
							ban.offline = true;

							BanHandler.IssueBan(new BanDetails()
							{
								OriginalName = "IP Address",
								Id = uid,
								IssuanceTime = TimeBehaviour.CurrentTimestamp(),
								Expires = DateTime.UtcNow.AddMinutes((double)min).Ticks,
								Reason = reason,
								Issuer = "Server"
							}, BanHandler.BanType.IP);
						}
						else
						{
							ban.success = false;
						}
					}
					EventHandlers.tcp.SendData(ban);
				}
				else if (type == "KICK")
				{
					string uid = (string)o["user"];
					if (!uid.Contains("@steam") && !uid.Contains("@discord"))
					{
						uid += "@steam";
					}
					ReferenceHub player = Player.GetPlayer(uid);

					Kick kick = new Kick
					{
						player = null
					};

					if (player != null)
					{
						ServerConsole.Disconnect(player.gameObject, (string)o["reason"]);

						kick.player = new User
						{
							name = player.nicknameSync.Network_myNickSync,
							userid = player.characterClassManager.UserId
						};
					}
					EventHandlers.tcp.SendData(kick);
				}
			}
			catch (Exception x)
			{
				Log.Error("SCPDiscord handle command error: " + x.Message);
			}
		}
	}
}