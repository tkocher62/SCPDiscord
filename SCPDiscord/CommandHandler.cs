using EXILED;
using EXILED.Extensions;
using Newtonsoft.Json.Linq;
using SCPDiscord.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCPDiscord
{
	class CommandHandler
	{
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
					ReferenceHub player = Player.GetPlayer((string)o["user"]);
					int min = (int)o["min"];
					if (player != null)
					{
						PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(player.gameObject, min, (string)o["reason"], "Server");
						EventHandlers.tcp.SendData(new Ban
						{
							player = new User
							{
								name = player.nicknameSync.Network_myNickSync,
								userid = player.characterClassManager.UserId
							},
							duration = min,
							success = true
						});
					}
					else
					{
						EventHandlers.tcp.SendData(new Ban
						{
							player = null,
							duration = min,
							success = false
						});
					}
				}
			}
			catch (Exception x)
			{
				Log.Error("SCPDiscord handle command error: " + x.Message);
			}
		}
	}
}