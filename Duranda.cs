using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using static Duranda.DurandaConsole;
using static Duranda.Reflection;
using PrefabList = System.Collections.Generic.Dictionary<int, UnityEngine.GameObject>;

namespace Duranda
{
	public class Duranda : MonoBehaviour
	{
		public DurandaConsole Console;
		private PrefabList NamedPrefabs;
		private readonly Dictionary<KeyCode, string> Keybinds = new Dictionary<KeyCode, string>();

		private bool God = false;
		private bool Ghost = false;
		private bool NoCost = false;
		private bool InstaEquip = false;
		private bool UnlimitedStamina = false;
		private bool Durability = false;
		private bool Teleportable = false;
		private bool UnlockDLCs = false;
		private bool Roof = false;
		private bool NoGuardianCd = false;
		private bool DebugMode = false;
		private bool FlyMode = false;

		private int CarryWeight = 300;
		private int InventoryHeight = 4;
		private int CameraShake = 4;
		private int JumpForce = 10;
		private int PickupRange = 2;
		private int FOV = 60;
		private int ComfortLevel = 0;
		private int InteractDistance = 5;
		private int PlaceDistance = 5;

		private string GuardianPower = "";

		void Awake()
		{
			Console = new DurandaConsole();

			Console.RegisterCommand("commands", "Duranda console command list", new ConsoleCommandCallback((args) =>
			{
				Console.ListCommands();
			}));

			Console.RegisterCommand("echo", "Print to console", new ConsoleCommandCallback((args) =>
			{
				string buffer = "";
				foreach (var i in args)
					buffer += i + " ";

				Console.WriteLine(buffer);
			}));

			Console.RegisterCommand("debug_mode", "Toggle debug mode.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: debug_mode [value]\nCurrent value: {DebugMode}");
					return;
				}

				if (!bool.TryParse(args[0], out DebugMode))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Debug mode is now {DebugMode}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Debug mode is now {DebugMode}");
			}));

			Console.RegisterCommand("fly_toggle", "Toggle debug fly.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: fly_toggle [value]\nCurrent value: {FlyMode}");
					return;
				}

				if (!bool.TryParse(args[0], out FlyMode))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Fly mode is now {FlyMode}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Fly mode is now {FlyMode}");
			}));

			Console.RegisterCommand("text_echo", "Print text message", new ConsoleCommandCallback((args) =>
			{
				Player localPlayer = GetLocalPlayer();
				if(localPlayer == null)
				{
					Console.WriteLine("Not in-game");
					return;
				}

				string buffer = "";
				foreach (var i in args)
					buffer += i + " ";

				localPlayer.Message(MessageHud.MessageType.Center, buffer);
			}));

			Console.RegisterCommand("hud_echo", "Print to hud", new ConsoleCommandCallback((args) =>
			{
				string buffer = "";
				foreach (var i in args)
					buffer += i + " ";

				SendHudMessage(buffer);
			}));

			Console.RegisterCommand("killmobs", "Kill mobs in the area.", new ConsoleCommandCallback((args) =>
			{
				if(args.Count == 0)
				{
					Console.WriteLine("Usage: killmobs [amount] => 'all' for every mob");
					return;
				}

				int amount = -1;
				if (args[0] != "all" && !int.TryParse(args[0], out amount))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				int killed = KillMobs(amount);
				Console.WriteLine($"Killed {killed} mobs.");
			}));

			Console.RegisterCommand("player_kill", "Kill the given player.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine("Usage: killplayer [name]");
					return;
				}

				if(KillPlayer(args[0])) Console.WriteLine($"The player has been killed.");
				else Console.WriteLine($"No player found with name {args[0]}.");
			}));

			Console.RegisterCommand("camerashake", "Set the camera shake value.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: camerashake [value]\nCurrent value: {CameraShake}");
					return;
				}

				if (!int.TryParse(args[0], out CameraShake))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Camera shake has been changed to {CameraShake}");
			}));

			Console.RegisterCommand("interact_distance", "Set the interact distance value.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: interact_distance [value]\nCurrent value: {InteractDistance}");
					return;
				}

				if (!int.TryParse(args[0], out InteractDistance))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Interact distance has been changed to {InteractDistance}");
			}));

			Console.RegisterCommand("place_distance", "Set the place distance value.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: place_distance [value]\nCurrent value: {PlaceDistance}");
					return;
				}

				if (!int.TryParse(args[0], out PlaceDistance))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Interact distance has been changed to {PlaceDistance}");
			}));

			Console.RegisterCommand("pickuprange", "Set the pickup range. (aka loot magnet)", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: pickuprange [value]\nCurrent value: {PickupRange}");
					return;
				}

				if (!int.TryParse(args[0], out PickupRange))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Pickup range has been changed to {PickupRange}");
			}));

			Console.RegisterCommand("durability_toggle", "Disable durability for every item in your inventory.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: durability_toggle [value]\nCurrent value: {Durability}");
					return;
				}

				if (!bool.TryParse(args[0], out Durability))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"No durability is now {Durability}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"No durability is now {Durability}");
			}));

			Console.RegisterCommand("jumpheight", "Set the jump height.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: jumpheight [value]\nCurrent value: {JumpForce}");
					return;
				}

				if (!int.TryParse(args[0], out JumpForce))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Jump height has been changed to {JumpForce}");
			}));

			Console.RegisterCommand("unlockdlcs", "Unlock every DLC.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: unlockdlcs [value]\nCurrent value: {UnlockDLCs}");
					return;
				}

				if (!bool.TryParse(args[0], out UnlockDLCs))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Unlock every DLC is now {UnlockDLCs}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Unlock every DLC is now {UnlockDLCs}");
			}));

			Console.RegisterCommand("map_reveal", "Reveals the whole map.", new ConsoleCommandCallback((args) =>
			{
				Minimap.instance.ExploreAll();
				Console.WriteLine("Revealed the whole map!");
			}));

			Console.RegisterCommand("map_reset", "Resets the whole map.", new ConsoleCommandCallback((args) =>
			{
				Minimap.instance.Reset();
				Console.WriteLine("Reseted the whole map!");
			}));
			
			Console.RegisterCommand("map_merchant", "Discover vendor.", new ConsoleCommandCallback((args) =>
			{
				Player localPlayer = GetLocalPlayer();
				if(localPlayer == null)
				{
					Console.WriteLine("Not in-game");
					return;
				}

				ZoneSystem.LocationInstance locationInstance;
				ZoneSystem.instance.FindClosestLocation("Vendor_BlackForest", localPlayer.transform.position, out locationInstance);
				Minimap.instance.DiscoverLocation(locationInstance.m_position, Minimap.PinType.Icon3, "Merchant");
				Console.WriteLine($"Merchant position: X: {locationInstance.m_position} Y: {locationInstance.m_position.y} Z: {locationInstance.m_position.z}");
			}));

			Console.RegisterCommand("healself", "Heal to maxhealth.", new ConsoleCommandCallback((args) =>
			{
				Player localPlayer = GetLocalPlayer();

				if (localPlayer != null)
				{
					localPlayer.Heal(localPlayer.GetMaxHealth(), true);
					Console.WriteLine("You have been healed up!");
				}
			}));

			Console.RegisterCommand("setfov", "Change the player fov.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: setfov [value]\nCurrent value: {FOV}");
					return;
				}

				if (!int.TryParse(args[0], out FOV))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"FOV has been changed to {FOV}");
			}));

			Console.RegisterCommand("skill_setlevel", "Set the level of a skill.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					string skillNames = GetSkillNames();
					Console.WriteLine($"Usage: skill_setlevel [name] [value]\n{skillNames}");
					return;
				}

				Skills.SkillType skill = GetSkillFromName(args[0]);
				if(skill == Skills.SkillType.None)
				{
					string skillNames = GetSkillNames();
					Console.WriteLine($"Invalid skill name!\n{skillNames}");
					return;
				}

				Player localPlayer = GetLocalPlayer();
				if (args.Count == 1)
				{
					if(skill == Skills.SkillType.All)
					{
						foreach (var i in Enum.GetValues(typeof(Skills.SkillType)))
						{
							Skills.SkillType tempType = (Skills.SkillType)i;
							if (tempType == Skills.SkillType.None || tempType == Skills.SkillType.All)
								continue;

							Console.WriteLine($"Current level of the {i} skill: {GetSkillLevel(tempType)}");
						}
					} else Console.WriteLine($"Current level of the {skill} skill: {GetSkillLevel(skill)}");

					return;
				}

				if (!int.TryParse(args[1], out int skillLevel))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				SetSkillLevel(skill, skillLevel);

				string message = skill == Skills.SkillType.All ? $"Changed the level of every skill to {skillLevel}" : $"Changed the level of the {skill} to {skillLevel}";
				Console.WriteLine(message);
				localPlayer.Message(MessageHud.MessageType.Center, message);
			}));

			Console.RegisterCommand("god_toggle", "Toggle god mode.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: god_toggle [value]\nCurrent value: {God}");
					return;
				}

				if (!bool.TryParse(args[0], out God))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"God mode is now {God}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"God mode is now {God}");
			}));

			Console.RegisterCommand("ghost_toggle", "Toggle ghost mode.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: ghost_toggle [value]\nCurrent value: {Ghost}");
					return;
				}

				if (!bool.TryParse(args[0], out Ghost))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Ghost mode is now {Ghost}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Ghost mode is now {Ghost}");
			}));

			Console.RegisterCommand("food_puke", "Clear foods.", new ConsoleCommandCallback((args) =>
			{
				Player localPlayer = GetLocalPlayer();

				if(localPlayer != null)
				{
					localPlayer.ClearFood();
					localPlayer.Message(MessageHud.MessageType.Center, "Your foods cleared!");
					Console.WriteLine("Your foods cleared!");
				}
			}));

			Console.RegisterCommand("tame_all", "Tame every mob in your area.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: tame_all [radius]");
					return;
				}

				if (!int.TryParse(args[0], out int radius))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Player localPlayer = GetLocalPlayer();

				if(localPlayer != null)
				{
					Tameable.TameAllInArea(localPlayer.transform.position, radius);

					localPlayer.Message(MessageHud.MessageType.Center, "You have tamed every mob in the given area!");
					Console.WriteLine("You have tamed every mob in the given area!");
				}
			}));

			Console.RegisterCommand("getpos", "Get your position.", new ConsoleCommandCallback((args) =>
			{
				Player localPlayer = GetLocalPlayer();
				if(localPlayer == null)
				{
					Console.WriteLine("Couldn't get player position. (Not in-game?)");
					return;
				}

				string message = $"X: {localPlayer.transform.position.x:F0} Y: {localPlayer.transform.position.y:F0} Z: {localPlayer.transform.position.z:F0}";

				Console.WriteLine(message);
				localPlayer.Message(MessageHud.MessageType.Center, message);
			}));

			Console.RegisterCommand("wind_reset", "Reset wind.", new ConsoleCommandCallback((args) =>
			{
				EnvMan.instance.ResetDebugWind();
				Player localPlayer = GetLocalPlayer();
				localPlayer.Message(MessageHud.MessageType.Center, "Wind reseted!");
				Console.WriteLine("Wind reseted!");
			}));

			Console.RegisterCommand("wind_set", "Set wind.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count != 2)
				{
					Console.WriteLine($"Usage: wind_set [angle] [intensity]");
					return;
				}

				if (!int.TryParse(args[0], out int angle))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				if (!int.TryParse(args[1], out int intensity))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				EnvMan.instance.SetDebugWind(angle, intensity);

				Player localPlayer = GetLocalPlayer();
				string message = $"Wind has been changed to angle {angle} with intensity {intensity}";

				localPlayer.Message(MessageHud.MessageType.Center, message);
				Console.WriteLine(message);
			}));

			Console.RegisterCommand("teleport_coord", "Teleport to coord.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count != 3)
				{
					Console.WriteLine($"Usage: teleport_coord [x] [y] [z]");
					return;
				}

				int[] coords = { 0, 0, 0 };
				for(int i = 0; i < coords.Length; i++)
				{
					if (!int.TryParse(args[i], out coords[i]))
					{
						Console.WriteLine("Invalid input!");
						return;
					}
				}

				Player localPlayer = GetLocalPlayer();
				localPlayer.TeleportTo(new Vector3(coords[0], coords[1], coords[2]), localPlayer.transform.rotation, true);
				string message = $"Teleported to coord: {coords[0]} {coords[1]} {coords[2]}";

				localPlayer.Message(MessageHud.MessageType.Center, message);
				Console.WriteLine(message);
			}));

			Console.RegisterCommand("teleport_player", "Teleport to the given player.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine("Usage: teleport_player [name]");
					return;
				}

				Player targetPlayer = GetPlayerFromName(args[0]);
				if (targetPlayer == null)
				{
					Console.WriteLine($"No player found with name {args[0]}");
					return;
				}

				Player localPlayer = GetLocalPlayer();
				localPlayer.TeleportTo(targetPlayer.transform.position, targetPlayer.transform.rotation, true);
				string message = $"Teleported to player: {targetPlayer.GetPlayerName()}";

				Console.WriteLine(message);
				localPlayer.Message(MessageHud.MessageType.Center, message);
			}));

			Console.RegisterCommand("teleport_restrict", "Disable teleport restrictions for every item in your inventory.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: teleport_restrict [value]\nCurrent value: {Teleportable}");
					return;
				}

				if (!bool.TryParse(args[0], out Teleportable))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"No teleport restrict is now {Teleportable}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"No teleport restrict is now {Teleportable}");
			}));

			Console.RegisterCommand("no_cost", "Toggle no cost.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: no_cost [value]\nCurrent value: {NoCost}");
					return;
				}

				if (!bool.TryParse(args[0], out NoCost))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"No cost is now {NoCost}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"No cost is now {NoCost}");
			}));

			Console.RegisterCommand("player_list", "Prints the available players.", new ConsoleCommandCallback((args) =>
			{
				ListPlayers();
			}));

			Console.RegisterCommand("prefab_dump", "Dump prefabs to file 'prefabs_dump.txt'. (inside Valheim's folder)", new ConsoleCommandCallback((args) =>
			{
				DumpPrefabs();
			}));

			Console.RegisterCommand("prefab_spawn", "Spawn any game prefab. (Items, mobs, etc.)", new ConsoleCommandCallback((args) =>
			{
				if (args.Count != 3)
				{
					Console.WriteLine($"Usage: prefab_spawn [prefab_name] [amount] [level]");
					return;
				}

				GameObject prefab = ZNetScene.instance.GetPrefab(args[0]);
				if(prefab == null)
				{
					Console.WriteLine("Invalid prefab!\nFor prefab names, use 'prefab_dump'");
					return;
				}

				if (!int.TryParse(args[1], out int amount))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				if (amount < 1)
					return;

				if (!int.TryParse(args[2], out int level))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				try
				{
					Player localPlayer = GetLocalPlayer();
					DateTime now = DateTime.Now;

					for (int i = 0; i < amount; i++)
					{
						Character component = UnityEngine.Object.Instantiate<GameObject>(prefab, localPlayer.transform.position + localPlayer.transform.forward * 2.0f + Vector3.up + (UnityEngine.Random.insideUnitSphere * 0.5f), Quaternion.identity).GetComponent<Character>();
						
						if(component & level > 1)
							component.SetLevel(level);
					}

					string message = $"Spawned x{amount} of {prefab.name} (Level {level})";
					localPlayer.Message(MessageHud.MessageType.Center, message);
					Console.WriteLine(message + $" in {(DateTime.Now - now).TotalMilliseconds} ms");
				} catch (Exception ex)
				{
					Console.Debug(ex);
				}
			}));

			Console.RegisterCommand("font_size", "Set console font size.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: font_size [size]\nCurrent Value: {Console.GetFontSize()}");
					return;
				}

				int size = Console.GetFontSize();
				if (!int.TryParse(args[0], out size))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.SetFontSize(size);
				Console.WriteLine($"Changed console font size to {size}");
			}));

			Console.RegisterCommand("font_color", "Set console font color.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: font_color [color]\nAvailable Colors: blue white black yellow cyan grey gray green clear magenta red");
					return;
				}

				try
				{
					var colorField = typeof(Color).GetProperty(args[0]);
					if(colorField != null)
					{
						Console.SetFontColor((Color)colorField.GetValue(null));
						Console.WriteLine($"Changed console font color to {args[0]}");
					}
				} catch (Exception ex)
				{
					Console.Debug(ex);
				}
			}));

			Console.RegisterCommand("inventory_carryweight", "Set the maximum inventory weight limit.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: inventory_carryweight [weight]\nCurrent value: {CarryWeight}");
					return;
				}

				if (!int.TryParse(args[0], out CarryWeight))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Carry weight limit has been changed to {CarryWeight}");
			}));

			Console.RegisterCommand("inventory_height", "Set the inventory height. (aka more slots)", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: inventory_height [height]\nCurrent value: {InventoryHeight}");
					return;
				}

				if (!int.TryParse(args[0], out InventoryHeight))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Inventory height has been changed to {InventoryHeight}");
			}));

			Console.RegisterCommand("clear", "Clear the console output.", new ConsoleCommandCallback((args) =>
			{
				Console.Clear();
			}));
			
			Console.RegisterCommand("quit", "Quit from the game.", new ConsoleCommandCallback((args) =>
			{
				Application.Quit();
			}));

			Console.RegisterCommand("exec", "Execute configuration file. (Relative .cfg path from Valheim folder)", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: exec [path]");
					return;
				}

				Console.LoadConfig(args[0]);
			}));

			Console.RegisterCommand("equip_toggle", "Toggle instant equip.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: equip_toggle [value]\nCurrent value: {InstaEquip}");
					return;
				}

				if (!bool.TryParse(args[0], out InstaEquip))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Instant equip is now {InstaEquip}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Instant equip is now {InstaEquip}");
			}));

			Console.RegisterCommand("stamina_toggle", "Toggle infinite stamina.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: stamina_toggle [value]\nCurrent value: {UnlimitedStamina}");
					return;
				}

				if (!bool.TryParse(args[0], out UnlimitedStamina))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Infinite stamina is now {UnlimitedStamina}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Infinite stamina is now {UnlimitedStamina}");
			}));
			
			Console.RegisterCommand("comfort_level", "Set the comfort level.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: comfort_level [value]\nCurrent value: {ComfortLevel}");
					return;
				}

				if (!int.TryParse(args[0], out ComfortLevel))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Comfort level is now {ComfortLevel}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Comfort level is now {ComfortLevel}");
			}));

			Console.RegisterCommand("roof_state", "Set roof state.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: comfort_level [value]\nCurrent value: {Roof}");
					return;
				}

				if (!bool.TryParse(args[0], out Roof))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Roof state is now {Roof}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Roof state is now {Roof}");
			}));

			Console.RegisterCommand("guardian_cooldown", "Toggle guardian cooldown.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: guardian_cooldown [value]\nCurrent value: {NoGuardianCd}");
					return;
				}

				if (!bool.TryParse(args[0], out NoGuardianCd))
				{
					Console.WriteLine("Invalid input!");
					return;
				}

				Console.WriteLine($"Disable guardian cooldown is now {NoGuardianCd}");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Disable guardian cooldown is now {NoGuardianCd}");
			}));

			Console.RegisterCommand("guardian_power", "Set guardian power to any status effect.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: guardian_power [name]\nCurrent value: {GuardianPower}\nAvailable Status Effects: {GetStatusEffectNames()}");
					return;
				}

				foreach(var i in ObjectDB.instance.m_StatusEffects)
				{
					if(i.name == args[0])
					{
						GuardianPower = args[0];
						Console.WriteLine($"Guardian power has been changed to {GuardianPower}");
						GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Guardian power has been changed to {GuardianPower}");
						return;
					}
				}

				Console.WriteLine("Invalid status effect name!");
			}));

			Console.RegisterCommand("guardian_start", "Force start guardian power.", new ConsoleCommandCallback((args) =>
			{
				Player localPlayer = GetLocalPlayer();
				if(localPlayer == null)
				{
					Console.WriteLine("Not in-game");
					return;
				}

				if(localPlayer.StartGuardianPower())
				{
					Console.WriteLine("Force started guardian power.");
					GetLocalPlayer()?.Message(MessageHud.MessageType.Center, "Force started guardian power.");
				} else
				{
					Console.WriteLine("No guaridan power is equipped or coudln't force start it!");
				}
			}));

			Console.RegisterCommand("add_status_effect", "Add status effect.", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: add_status_effect [name]\nAvailable Status Effects: {GetStatusEffectNames()}");
					return;
				}

				Player localPlayer = GetLocalPlayer();
				if(localPlayer == null)
				{
					Console.WriteLine("Not in-game");
					return;
				}

				foreach (var i in ObjectDB.instance.m_StatusEffects)
				{
					if (i.name == args[0])
					{
						AddStatusEffect(args[0]);
						Console.WriteLine($"Added status effect {args[0]}");
						GetLocalPlayer()?.Message(MessageHud.MessageType.Center, $"Added status effect {args[0]}");
						return;
					}
				}

				Console.WriteLine("Invalid status effect name!");
				GetLocalPlayer()?.Message(MessageHud.MessageType.Center, "Invalid status effect name!");
			}));

			Console.RegisterCommand("set_keybind", "Set keybind to execute command", new ConsoleCommandCallback((args) =>
			{
				if (args.Count == 0)
				{
					Console.WriteLine($"Usage: set_keybind [keycode]\nAvailable keycodes: https://docs.unity3d.com/ScriptReference/KeyCode.html");
					return;
				}

				KeyCode keyCode = KeyCode.None;
				foreach (var i in Enum.GetValues(typeof(KeyCode)))
				{
					if(i.ToString().ToLower() == args[0])
					{
						keyCode = (KeyCode)i;
					}
				}

				if (keyCode != KeyCode.None)
				{
					string buffer = "";
					for(int i = 1; i < args.Count; i++)
						buffer += args[i] + " ";

					Keybinds[keyCode] = buffer;
				} else
				{
					Console.WriteLine("Invalid keycode!");
				}
			}));

			SendHudMessage("DURANDA LOADED");

			Console.LoadConfig();
		}

		void Update()
		{
			Console.Update();

			if (UnlockDLCs)
			{
				foreach (var i in DLCMan.instance.m_dlcs)
				{
					if (!i.m_installed)
						i.m_installed = true;
				}
			}

			foreach (KeyValuePair<KeyCode, string> bind in Keybinds)
			{
				if(Input.GetKeyDown(bind.Key))
				{
					List<string> @params = bind.Value.Split(' ').ToList();
					string command = @params[0].Trim();
					@params.RemoveAt(0);
					Console.ExecuteCommand(command, @params);
				}
			}

			Player.m_debugMode = DebugMode;
			Player localPlayer = GetLocalPlayer();
			if (localPlayer == null)
				return;

			SetFOV(FOV);
			localPlayer.SetGodMode(God);
			localPlayer.SetGhostMode(Ghost);

			localPlayer.m_maxCarryWeight = CarryWeight;
			localPlayer.m_baseCameraShake = CameraShake;
			localPlayer.m_jumpForce = JumpForce;
			localPlayer.m_autoPickupRange = PickupRange;
			localPlayer.m_flying = FlyMode;

			SetPrivateFieldValue<Player, int>(localPlayer, "m_comfortLevel", ComfortLevel);
			SetPrivateFieldValue<Player, bool>(localPlayer, "m_underRoof", Roof);
			SetPrivateFieldValue<Player, bool>(localPlayer, "m_noPlacementCost", NoCost);

			Inventory localPlayerInventory = localPlayer.GetInventory();
			
			if(InventoryHeight != 4)
			{
				SetPrivateFieldValue<Inventory, int>(localPlayerInventory, "m_height", InventoryHeight);
				CallPrivateVoidMethod<InventoryGui>(InventoryGui.instance, "UpdateInventory", localPlayer);
			}

			var inv = localPlayerInventory.GetAllItems();

			if(Durability)
			{
				foreach (var i in inv)
				{
					if(i.m_shared.m_useDurability)
						i.m_shared.m_useDurability = false;
				}
			}

			if(Teleportable)
			{
				foreach (var i in inv)
				{
					if (!i.m_shared.m_teleportable)
						i.m_shared.m_teleportable = true;
				}
			}

			if (InstaEquip)
			{
				foreach(var i in GetPrivateFieldValue<List<Player.EquipQueueData>, Player>(localPlayer, "m_equipQueue"))
				{
					i.m_duration = 0.0f;
					i.m_time = 0.0f;
				}
			}

			if (UnlimitedStamina)
			{
				localPlayer.AddStamina(localPlayer.GetMaxStamina());
			}

			if(NoGuardianCd)
			{
				SetPrivateFieldValue<Player, float>(localPlayer, "m_guardianPowerCooldown", 0.0f);
			}

			if(!string.IsNullOrEmpty(GuardianPower) && localPlayer.GetGuardianPowerName() != GuardianPower)
			{
				localPlayer.SetGuardianPower(GuardianPower);
			}
		}
		
		public void AddStatusEffect(string name)
		{
			Player localPlayer = GetLocalPlayer();
			if (localPlayer == null)
				return;

			List<Player> list = new List<Player>();
			Player.GetPlayersInRange(localPlayer.transform.position, 10f, list);
			foreach (Player player in list)
			{
				player.GetSEMan().AddStatusEffect(name, true);
			}
		}

		public int KillMobs(int amount = -1)
		{
			int killedAmount = 0;
			List<Character> characters = Character.GetAllCharacters();
			foreach (Character i in characters)
			{
				if (i.IsPlayer())
					continue;

				HitData hit = new HitData();
				hit.m_damage.m_damage = Constans.DAMAGE_MAX;
				i.Damage(hit);
				++killedAmount;

				if (amount != -1 && killedAmount == amount)
					break;
			}

			return killedAmount;
		}

		public bool KillPlayer(string name)
		{
			Player targetPlayer = GetPlayerFromName(name);

			if(targetPlayer != null)
			{
				HitData hit = new HitData();
				hit.m_damage.m_damage = Constans.DAMAGE_MAX;
				targetPlayer.ApplyDamage(hit, true, true);
				return true;
			}

			return false;
		}

		public void SetFOV(float value)
		{
			Camera mainCamera = Utils.GetMainCamera();
			Camera[] components = mainCamera.GetComponentsInChildren<Camera>();

			mainCamera.fieldOfView = value;
			foreach (var i in components)
			{
				i.fieldOfView = value;
			}
		}

		public PrefabList GetPrefabList()
		{
			return GetPrivateFieldValue<PrefabList, ZNetScene>(ZNetScene.instance, "m_namedPrefabs");
		}

		public void SetSkillLevel(Skills.SkillType skill, int value)
		{
			Player localPlayer = GetLocalPlayer();

			if (localPlayer == null)
				return;

			Skills playerSkills = localPlayer.GetSkills();
			List<Skills.Skill> playerSkillList = playerSkills.GetSkillList();

			if(skill == Skills.SkillType.All)
			{
				foreach (var i in playerSkillList)
				{
					i.m_level = value;
					i.Raise(value);
				}
			} else
			{
				foreach (var i in playerSkillList)
				{
					if (i.m_info.m_skill == skill)
					{
						i.m_level = value;
						i.Raise(value);
						return;
					}
				}
			}
		}

		public int GetSkillLevel(Skills.SkillType skill)
		{
			Player localPlayer = GetLocalPlayer();

			if (localPlayer == null)
				return 0;

			Skills playerSkills = localPlayer.GetSkills();
			List<Skills.Skill> playerSkillList = playerSkills.GetSkillList();

			foreach (var i in playerSkillList)
			{
				if (i.m_info.m_skill == skill)
				{
					return (int)i.m_level;
				}
			}

			return 0;
		}

		public Skills.SkillType GetSkillFromName(string name)
		{
			if (name == "all")
				return Skills.SkillType.All;

			foreach(var i in Enum.GetValues(typeof(Skills.SkillType)))
			{
				if(i.ToString().ToLower() == name)
				{
					return (Skills.SkillType)i;
				}
			}

			return Skills.SkillType.None;
		}

		public string GetSkillNames()
		{
			string buffer = "";
			foreach (var i in Enum.GetValues(typeof(Skills.SkillType)))
			{
				if ((Skills.SkillType)i == Skills.SkillType.None)
					continue;

				buffer += " " + i.ToString().ToLower();
			}

			return buffer;
		}

		public string GetStatusEffectNames()
		{
			string buffer = "";
			foreach(var i in ObjectDB.instance.m_StatusEffects)
			{
				buffer += i.name + " ";
			}

			return buffer;
		}

		public Player GetLocalPlayer()
		{
			return Player.m_localPlayer;
		}

		public Player GetPlayerFromName(string name, bool aliveOnly = true)
		{
			List<Player> players = Player.GetAllPlayers();
			foreach (Player i in players)
			{
				if (aliveOnly && i.IsDead())
					continue;

				if (!i.GetPlayerName().Contains(name))
					continue;

				return i;
			}

			return null;
		}

		public void ListPlayers()
		{
			List<Player> players = Player.GetAllPlayers();
			foreach (Player i in players)
			{
				Console.WriteLine(i.GetPlayerName());
			}
		}

		public void SendHudMessage(string message, bool stinger = false)
		{
			if (GetLocalPlayer() != null)
			{
				MessageHud.instance.ShowBiomeFoundMsg(message, stinger);
			}
		}

		public void DumpPrefabs()
		{
			NamedPrefabs = GetPrefabList();

			if (NamedPrefabs != null)
			{
				try
				{
					DateTime now = DateTime.Now;
					FileStream fs = File.Open("prefabs_dump.txt", FileMode.Create);
					StreamWriter sr = new StreamWriter(fs);

					sr.WriteLine($"Valheim version: {Constans.VERSION} {DateTime.Now}\nHASH\t\tNAME");

					int prefabs = 0;
					foreach (KeyValuePair<int, GameObject> i in NamedPrefabs)
					{
						sr.WriteLine(i.Key + "\t" + i.Value.name);
						++prefabs;
					}

					string log = $"Dumped {prefabs} prefab in {(DateTime.Now - now).TotalMilliseconds} ms";
					sr.WriteLine(log);
					sr.Close();
					fs.Close();

					Console.WriteLine(log);
				} catch (Exception ex)
				{
					Console.Debug(ex);
				}
			} else Console.WriteLine("Couldn't get 'ZNetScene.m_namedPrefabs'");
		}
	}
}