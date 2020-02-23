using EXILED;
using EXILED.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCPDiscord.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SCPDiscord
{
	class Tcp
	{
		private string ip;
		private int port;
		private Socket socket;

		public Tcp(string ip, int port)
		{
			this.ip = ip;
			this.port = port;
		}

		public void Init()
		{
			try
			{
				new Thread(AttemptConnection).Start();
			}
			catch (Exception x)
			{
				Log.Warn("Failed to connect to bot.");
			}
		}

		private void AttemptConnection()
		{
			while (!IsConnected())
			{
				try
				{
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Connect(ip, port);

					new Thread(Listen).Start();
					SendData(new Identify());
				}
				catch (Exception x)
				{
					// Failed to connect
					Log.Warn("Failed to connect to SCPDiscord bot, retrying in 10 seconds...");
				}
				Thread.Sleep(10000);
			}
		}

		private void Listen()
		{
			while (IsConnected())
			{
				try
				{
					byte[] a = new byte[1000];
					Log.Warn("listening");
					socket.Receive(a);
					Log.Warn("got data");
					JObject o = (JObject)JToken.FromObject(JsonConvert.DeserializeObject(Encoding.UTF8.GetString(a)));
					Log.Warn(o.ToString());

					string type = (string)o["type"];
					if (type == "IDENT")
					{
						if ((string)o["data"] == "PASS") Log.Debug($"Server {ServerConsole.Port} passed identification.");
						else if ((string)o["data"] == "FAIL") Log.Warn($"Server {ServerConsole.Port} failed identification.");
					}
					else if (type == "UPDATE")
					{
						Log.Warn("updating");
						SendData(new Update());
					}
					else if (type == "ROLESYNC")
					{
						string userid = (string)o["userid"];

						if (o["group"] == null)
						{
							Log.Info($"No role sync found for {userid}");
							Plugin.VerifyReservedSlot(userid);
							continue;
						}

						string group = (string)o["group"];

						UserGroup userGroup = ServerStatic.PermissionsHandler.GetGroup(group);
						if (userGroup == null)
						{
							Log.Error($"Attempted to assign invalid user group {group} to {userid}");
							continue;
						}

						ReferenceHub player = Player.GetPlayer(userid);
						if (player == null)
						{
							Log.Error($"Error assigning user group to {userid}, player not found.");
							continue;
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
						GameCore.Console.singleton.TypeCommand($"/{(string)o["command"]}");
					}
				}
				catch (Exception x)
				{
					Log.Error("SCPDiscord listener error: " + x.Message);
				}
			}
			new Thread(AttemptConnection).Start();
		}

		public void SendData(object data)
		{
			SendData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
		}

		public void SendData(byte[] data)
		{
			if (IsConnected())
			{
				socket.Send(data);
			}
		}

		public bool IsConnected()
		{
			if (socket == null)
			{
				return false;
			}
			try
			{
				return !((socket.Poll(1000, SelectMode.SelectRead) && (socket.Available == 0)) || !socket.Connected);
			}
			catch (Exception x)
			{
				return false;
			}
		}
	}
}
