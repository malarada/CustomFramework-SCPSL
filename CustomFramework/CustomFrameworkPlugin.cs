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

namespace CustomFramework
{
	public class CustomFrameworkPlugin : Plugin
    {
        internal static CustomFrameworkPlugin Instance;
        internal static List<ICoroutineObject> coroutineRoles {  get; set; }
        public static Dictionary<Player, string> PlayerSubclasses { get; set; } = new Dictionary<Player, string>();

		public static Random Random = new Random();

        public static Harmony Patcher = new Harmony("PyroCycloneProjects.CustomFramework");

        public CustomFrameworkPlugin()
        {
            Instance = this;
		}

		public CoroutineHandle coroutine { get; set; }

        public override string Name => "Custom Framework";

        public override string Description => "A minimalist framework used to give more power to developers.";

        public override string Author => "Pyro Cyclone Projects";

        public override Version Version => new Version(1, 0, 0);

		public override Version RequiredApiVersion => new Version(1, 0, 0);

		public IEnumerator<float> Coroutine()
		{
            while (true)
            {
                foreach (var player in Player.List)
                {
                    var hint = GetSubclassHint(player);
                    if (hint != string.Empty)
                        player.SendHint(hint);
				}

                yield return Timing.WaitForSeconds(1f);
			}
		}

		public static string GetSubclassHint(Player player)
        {
            if (!Round.IsRoundInProgress || player == null) return string.Empty;

            CustomSubclass subclass;

            if (player.IsAlive)
            {
                subclass = CustomSubclass.Get(PlayerSubclasses[player]);
            }
            else
            {
                subclass = CustomSubclass.Get(PlayerSubclasses[player.CurrentlySpectating]);
            }

            string str;
            if (subclass != null)
            {
                str = $"<align=left><size=20>{subclass?.Name}\nUse .roleinfo for information\nabout this role.</size></align><align=right>{subclass?.GetSpecificHint(player)}</align>";
            }
            else
            {
                str = string.Empty;
            }
            foreach (var hint in CustomHintService.hints)
			{
                str += hint.Invoke(player);
			}
			return str;
        }

        public override void Enable()
        {
            Patcher.PatchAll();

            coroutine = Timing.RunCoroutine(Coroutine());

			ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[]
            {
                new SSKeybindSetting(0, "Subclass Ability", UnityEngine.KeyCode.Z)
            };

            Handlers.PlayerEvents.Spawned += Spawned;
            Handlers.ServerEvents.RoundStarted += RoundStarted;
            Handlers.ServerEvents.RoundEnded += RoundEnded;
            //Handlers.Server.RespawningTeam += RespawningTeam;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += SettingValueReceived;
        }

        public override void Disable()
        {
            if (coroutine.IsRunning)
				Timing.KillCoroutines(coroutine);

			Handlers.PlayerEvents.Spawned -= Spawned;
            Handlers.ServerEvents.RoundStarted -= RoundStarted;
            Handlers.ServerEvents.RoundEnded -= RoundEnded;
            //Handlers.Server.RespawningTeam -= RespawningTeam;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= SettingValueReceived;
        }

        private void SettingValueReceived(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (setting.Label == "Subclass Ability")
            {
                var player = Player.Get(sender);
                var cs = CustomSubclass.Get(PlayerSubclasses[player]);
                cs?.OnAbility(player);
            }
        }

        private void Spawned(PlayerSpawnedEventArgs ev)
        {
            if (!PlayerSubclasses.TryGetValue(ev.Player, out _))
            {
                PlayerSubclasses.Add(ev.Player, null);
            }

            if (PlayerSubclasses[ev.Player] != "" && PlayerSubclasses[ev.Player] != null)
            {
                var subclass = CustomSubclass.Get(PlayerSubclasses[ev.Player]);

                subclass.RemoveSubclass(ev.Player);
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

                CustomSubclass chosenRole = weightedRoles[Random.Next(weightedRoles.Count)];

                chosenRole.GiveSubclass(ev.Player);
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
                if (coroutineRole.coroutine != null)
                    if (coroutineRole.coroutine.IsRunning)
                        Timing.KillCoroutines(coroutineRole.coroutine);
        }

        public static void RegisterAll()
        {
            Logger.Debug("Registering all custom shit.");

            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                //if (typeof(CustomTeam).IsAssignableFrom(type))
                //{
                //    CustomTeam team = (CustomTeam)Activator.CreateInstance(type);
                //    team?.TryRegister();
                //}
                if (typeof(CustomSubclass).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    CustomSubclass subclass = (CustomSubclass)Activator.CreateInstance(type);
                    Logger.Debug($"Attempting to register subclass {subclass.Identifier}");
                    if (subclass.TryRegister())
                    {
                        Logger.Debug($"Registered subclass {subclass.Identifier}");
                    }
                }
            }
        }

        public static void UnregisterAll()
        {
            //foreach (CustomTeam team in CustomTeam.Registered)
            //{
            //    team.TryUnregister();
            //}

            foreach (CustomSubclass subclass in CustomSubclass.Registered)
            {
                subclass.UnsubscribeEvents();
            }
        }
	}
}
