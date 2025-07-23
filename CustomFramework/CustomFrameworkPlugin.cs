using System;
using System.Reflection;
using UserSettings.ServerSpecific;
using CustomFramework.CustomSubclasses;
using MEC;
using System.Collections.Generic;
using CustomFramework.Interfaces;
using System.Linq;
using LabApi.Loader.Features.Plugins;
using LabApi.Features.Wrappers;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using Handlers = LabApi.Events.Handlers;
using HarmonyLib;
using CustomFramework.CustomItems;

namespace CustomFramework
{
    public class CustomFrameworkPlugin : Plugin<Config>
    {
        public static CustomFrameworkPlugin Instance;
        internal static List<ICoroutineObject> coroutineRoles { get; set; }
        public static Dictionary<Player, string> PlayerSubclasses { get; set; } = new Dictionary<Player, string>();

        public static Random Random = new Random();

        public static Harmony Patcher = new Harmony("PyroCycloneProjects.CustomFramework");

        public CustomFrameworkPlugin()
        {
            Instance = this;
        }

        public override string Name => "Custom Framework";

        public override string Description => "A minimalist framework used to give more power to developers.";

        public override string Author => "Pyro Cyclone Projects";

        public override Version Version => new Version(2, 0, 0);

        public override Version RequiredApiVersion => new Version(1, 0, 0);

        public CoroutineHandle coroutine { get; set; }
        public IEnumerator<float> Coroutine()
        {
            if (Config.Debug)
				Logger.Debug("CustomHintService coroutine started.");

			while (true)
            {
                try
                {
                    foreach (var player in Player.List.ToList())
                    {
                        var hint = GetPlayerHint(player);
						if (!string.IsNullOrEmpty(hint))
                            player.SendHint(hint);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"[CustomFramework] Error in CustomHintService coroutine: {ex}");
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public string GetPlayerHint(Player player)
        {
			var hint = GetSubclassHint(player);
			foreach (var h in CustomHintService.hints)
			{
				var n = h.Invoke(player);
				if (!string.IsNullOrEmpty(n))
					hint += n;
			}
			foreach (var h in CustomHintService.timedHints.ToList())
			{
				if (player != h.player) continue;
				if ((DateTime.UtcNow - h.startTime).TotalSeconds >= h.seconds) CustomHintService.timedHints.Remove(h);
				else hint += h.hint;
			}
            return hint;
		}

        public static string GetSubclassHint(Player player)
        {
            if (!Round.IsRoundInProgress || player == null) return string.Empty;

            CustomSubclass subclass = null;

            if (player.IsAlive)
            {
                if (PlayerSubclasses.ContainsKey(player))
                    subclass = CustomSubclass.Get(PlayerSubclasses[player]);
            }
            else if (player.CurrentlySpectating != null && PlayerSubclasses.ContainsKey(player.CurrentlySpectating))
            {
                subclass = CustomSubclass.Get(PlayerSubclasses[player.CurrentlySpectating]);
            }

            if (subclass != null)
                return $"<align=left><size=20>{subclass?.Name}\nUse .roleinfo for information\nabout this role.</size></align><align=right>{subclass?.GetSpecificHint(player)}</align>";
            return string.Empty;
        }

        public override void Enable()
        {
            Patcher.PatchAll();

            coroutine = Timing.RunCoroutine(Coroutine());

			Handlers.PlayerEvents.Joined += PlayerEvents_Joined;
            Handlers.PlayerEvents.ChangedRole += Spawned;
            Handlers.ServerEvents.RoundStarted += RoundStarted;
            Handlers.ServerEvents.RoundEnded += RoundEnded;
			Handlers.ServerEvents.MapGenerated += ServerEvents_MapGenerated;

            ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[]
            {
                new SSKeybindSetting(0, "Subclass Ability", UnityEngine.KeyCode.Z)
            };
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += SettingValueReceived;
        }

		private void PlayerEvents_Joined(PlayerJoinedEventArgs ev)
		{
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
		}

		public override void Disable()
        {
            if (coroutine.IsRunning)
                Timing.KillCoroutines(coroutine);

            Handlers.PlayerEvents.Joined -= PlayerEvents_Joined;
            Handlers.PlayerEvents.ChangedRole -= Spawned;
            Handlers.ServerEvents.RoundStarted -= RoundStarted;
            Handlers.ServerEvents.RoundEnded -= RoundEnded;
            Handlers.ServerEvents.MapGenerated -= ServerEvents_MapGenerated;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= SettingValueReceived;
        }

		private void ServerEvents_MapGenerated(MapGeneratedEventArgs ev)
		{
            
		}

        private void SettingValueReceived(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (setting.Label == "Subclass Ability")
            {
                var player = Player.Get(sender);
                if (PlayerSubclasses.ContainsKey(player))
                {
                    var cs = CustomSubclass.Get(PlayerSubclasses[player]);
                    cs?.OnAbility(player);
                }
            }
        }

        private void Spawned(PlayerChangedRoleEventArgs ev)
        {
            if (!PlayerSubclasses.TryGetValue(ev.Player, out _))
            {
                PlayerSubclasses.Add(ev.Player, null);
            }

            if (!string.IsNullOrEmpty(PlayerSubclasses[ev.Player]))
            {
                var subclass = CustomSubclass.Get(PlayerSubclasses[ev.Player]);
                subclass?.RemoveSubclass(ev.Player);
                PlayerSubclasses[ev.Player] = null;
            }

            List<CustomSubclass> roleList = CustomSubclass.Registered
                .Where(t =>
                    t.GetType().GetCustomAttribute<CustomSubclassAttribute>()?.Team == ev.Player.GetTeam() &&
                    t.SpawnConditionsMet() &&
                    (
                        ev.Player.RoleBase.ServerSpawnReason != PlayerRoles.RoleChangeReason.Escaped ||
                        t.IsEscapeRole
                    )
                )
                .ToList();

            if (roleList.Count > 0)
            {
                List<CustomSubclass> weightedRoles = new List<CustomSubclass>();

                foreach (var role in roleList)
                {
                    for (int i = 0; i < (int)role.SpawnTickets; i++)
                    {
                        weightedRoles.Add(role);
                    }
                }

                if (weightedRoles.Count > 0)
                {
                    CustomSubclass chosenRole = weightedRoles[Random.Next(weightedRoles.Count)];
                    chosenRole.GiveSubclass(ev.Player);
                }

                Logger.Debug("Finished player spawned on Framework");
            }
            else
            {
                Logger.Debug($"No subclasses found for team: {ev.Player.GetTeam()}");
            }
        }

        private void RoundStarted()
        {
            Logger.Debug("Round started, starting Coroutines");

            coroutineRoles = CustomSubclass.Registered
                .OfType<ICoroutineObject>()
                .ToList();

            foreach (var coroutineRole in coroutineRoles)
                coroutineRole.coroutine = Timing.RunCoroutine(coroutineRole.Coroutine());
        }

        private void RoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var coroutineRole in coroutineRoles)
                if (coroutineRole.coroutine != null && coroutineRole.coroutine.IsRunning)
                    Timing.KillCoroutines(coroutineRole.coroutine);
        }

        public static void RegisterAll()
        {
            Logger.Debug("Registering all custom subclasses.");

            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract) continue;
                else if (typeof(CustomSubclass).IsAssignableFrom(type))
                {
                    try
                    {
                        CustomSubclass subclass = (CustomSubclass)Activator.CreateInstance(type);
                        Logger.Debug($"Attempting to register subclass {subclass.Identifier}");
                        if (!subclass.TryRegister())
                            Logger.Debug($"Could not register subclass {subclass.Identifier}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to instantiate subclass {type.FullName}: {ex}");
                    }
                }
                else if (typeof(CustomItem).IsAssignableFrom(type))
                {
                    try
                    {
                        CustomItem item = (CustomItem)Activator.CreateInstance(type);
                        Logger.Debug($"Attempting to register custom item {item.Identifier}");
                        if (!item.TryRegister())
                            Logger.Debug($"Could not register custom item {item.Identifier}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to instantiate custom item {type.FullName}: {ex}");
                    }
                }
            }

            CustomSubclass.Registered = CustomSubclass.Registered.OrderBy(t => t.Id).ToHashSet();
            CustomItem.Registered = CustomItem.Registered.OrderBy(t => t.Id).ToHashSet();
        }

        public static void UnregisterAll()
        {
            foreach (CustomSubclass subclass in CustomSubclass.Registered) subclass.TryUnregister();
            foreach (CustomItem item in CustomItem.Registered) item.TryUnregister();
        }
    }

    public class Config
    {
        public bool Debug { get; set; } = false;
    }
}
