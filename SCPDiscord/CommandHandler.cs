using Exiled.API.Features;
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
					Log.Warn(o);

					string userid = (string)o["userid"];

					if (o["group"] == null)
					{
						Log.Debug($"No role sync found for {userid}");
						SCPDiscord.VerifyReservedSlot(userid);
						return;
					}

					string group = (string)o["group"];

					UserGroup userGroup = ServerStatic.PermissionsHandler.GetGroup(group);
					if (userGroup == null)
					{
						Log.Error($"Attempted to assign invalid user group {group} to {userid}");
						return;
					}

					Player player = Player.Get(userid);
					if (player == null)
					{
						Log.Error($"Error assigning user group to {userid}, player not found.");
						return;
					}

					if (SCPDiscord.setRoleGroups.Contains(group))
					{
						Log.Debug($"Assigning role: {userGroup} to {userid}.");
						player.Group = userGroup;
					}

					string tag = (string)o["tag"];
					if (SCPDiscord.setTagGroups.Contains(group) && tag != null)
					{
						Log.Debug($"Changing tag of {userid} to {tag}.");
						player.RankName = tag;
					}

					if (SCPDiscord.reservedSlotGroups.Contains(group))
					{
						// grant reserved slot
						Log.Debug("Player has necessary rank for reserved slot, checking...");
						List<string> lines = File.ReadAllLines(SCPDiscord.reservedSlots).ToList();
						if (!lines.Contains(userid))
						{
							Log.Debug("Reserved slot not found, adding player...");
							lines.Add(userid);
							File.WriteAllLines(SCPDiscord.reservedSlots, lines);
							// This only reloads the slots on the current server, change this to reload on every server?
							// Might not work
							ReservedSlot.Reload();
						}
					}
					else
					{
						SCPDiscord.VerifyReservedSlot(userid);
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
					Player player = Player.Get(uid);
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
						PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(player.GameObject, min, reason, "Server");

						ban.player = new User
						{
							name = player.Nickname,
							userid = player.UserId
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

							if (SCPDiscord.instance.Config.SteamApiKey != string.Empty)
							{
								string data = null;
								try
								{
									data = webclient.DownloadString($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={SCPDiscord.instance.Config.SteamApiKey}&format=json&steamids={uid.Replace("@steam", "")}");
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
					Player player = Player.Get(uid);

					Kick kick = new Kick
					{
						player = null
					};

					if (player != null)
					{
						kick.player = new User
						{
							name = player.Nickname,
							userid = player.UserId
						};

						ServerConsole.Disconnect(player.GameObject, (string)o["reason"]);
					}
					EventHandlers.tcp.SendData(kick);
				}
				else if (type == "UNBAN")
				{
					Unban unban = new Unban();

					List<string> ipBans = File.ReadAllLines(SCPDiscord.ipBans).ToList();
					List<string> userIDBans = File.ReadAllLines(SCPDiscord.useridBans).ToList();

					string id = (string)o["user"];
					if (!id.Contains("."))
					{
						if (!id.Contains("@steam") && !id.Contains("@discord"))
						{
							id += "@steam";
						}
					}
					List<string> matchingIPBans = ipBans.FindAll(s => s.Contains(id));
					List<string> matchingSteamIDBans = userIDBans.FindAll(s => s.Contains(id));

					if (matchingIPBans.Count == 0 && matchingSteamIDBans.Count == 0)
					{
						unban.success = false;
						EventHandlers.tcp.SendData(unban);
						return;
					}

					ipBans.RemoveAll(s => s.Contains(id));
					userIDBans.RemoveAll(s => s.Contains(id));

					foreach (var row in matchingIPBans) userIDBans.RemoveAll(s => s.Contains(row.Split(';').Last()));
					foreach (var row in matchingSteamIDBans) ipBans.RemoveAll(s => s.Contains(row.Split(';').Last()));

					File.WriteAllLines(SCPDiscord.ipBans, ipBans);
					File.WriteAllLines(SCPDiscord.useridBans, userIDBans);

					EventHandlers.tcp.SendData(unban);
				}
			}
			catch (Exception x)
			{
				Log.Error("SCPDiscord handle command error: " + x.Message);
			}
		}
	}
}