using System;
using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using Handlers = Exiled.Events.Handlers;
using UserSettings.ServerSpecific;
using CustomFramework.CustomTeams;
using CustomFramework.CustomSubclasses;
using MEC;
using System.Collections.Generic;
using CustomFramework.Interfaces;
using System.Linq;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Enums;
using CustomFramework.Enums;
using RueI.Elements;
using RueI.Displays;
using Exiled.API.Features.Roles;
using Exiled.CustomRoles.API;

namespace CustomFramework
{
    public class CustomFrameworkPlugin : Plugin<Config>
    {
        internal static CustomFrameworkPlugin Instance;
        internal static List<ICoroutineObject> coroutineRoles {  get; set; }
        public static Random Random = new Random();

        public static DynamicElement DynamicElement = new DynamicElement(GetSubclassHint, 200);

        public static AutoElement AutoElement = new AutoElement(Roles.All, DynamicElement);

        public CustomFrameworkPlugin()
        {
            Instance = this;
        }

        public static string GetSubclassHint(DisplayCore hub)
        {
            if (!Round.InProgress) return null;

            Player player = Player.Get(hub.Hub);
            CustomSubclass subclass;

            if (player.IsAlive)
            {
                subclass = CustomSubclass.Get(player.UniqueRole);
            }
            else if (player.Role is SpectatorRole spectator)
            {
                subclass = CustomSubclass.Get(spectator.SpectatedPlayer.UniqueRole);
            }
            else
            {
                subclass = null;
            }

            return $"<align=left><size=20>{subclass?.Name}\nUse .roleinfo for information\nabout this role.</size></align><align=right>{subclass?.GetSpecificHint(player)}</align>";
        }

        public override void OnEnabled()
        {
            AutoElement.UpdateEvery = new AutoElement.PeriodicUpdate(new TimeSpan(0, 0, 1));

            RueI.RueIMain.EnsureInit();

            ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[]
            {
                new SSKeybindSetting(0, "Subclass Ability", UnityEngine.KeyCode.Y)
            };

            Handlers.Player.Spawned += Spawned;
            Handlers.Server.RoundStarted += RoundStarted;
            Handlers.Server.RoundEnded += RoundEnded;
            //Handlers.Server.RespawningTeam += RespawningTeam;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += SettingValueReceived;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Handlers.Player.Spawned -= Spawned;
            Handlers.Server.RoundStarted -= RoundStarted;
            Handlers.Server.RoundEnded -= RoundEnded;
            //Handlers.Server.RespawningTeam -= RespawningTeam;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= SettingValueReceived;

            base.OnDisabled();
        }

        private void SettingValueReceived(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (setting.Label == "Subclass Ability")
            {
                CustomSubclass.Get(Player.Get(sender).UniqueRole).OnAbility(Player.Get(sender));
            }
        }

        private void RespawningTeam(RespawningTeamEventArgs ev)
        {
            List<CustomTeam> teams = new List<CustomTeam>();

            Teams NextTeam = (Teams)Enum.Parse(typeof(Teams), ev.NextKnownTeam.GetTeam());

            foreach (var team in CustomTeam.Registered)
            {
                var attr = team.GetType().GetCustomAttribute<CustomTeamAttribute>();

                if (attr != null)
                {
                    var t = attr.TeamToReplace;

                    if ((t & NextTeam) == NextTeam)
                    {
                        teams.Add(team);
                    }
                }
            }

            ev.IsAllowed = false;
        }

        private void Spawned(SpawnedEventArgs ev)
        {
            if (ev.Player.UniqueRole != "" && ev.Player.UniqueRole != null)
            {
                var subclass = CustomSubclass.Get(ev.Player.UniqueRole);

                subclass.RemoveSubclass(ev.Player);
            }

            List<CustomSubclass> roleList = CustomSubclass.Registered
                .Where(t =>
                    t.GetType().GetCustomAttribute<CustomSubclassAttribute>()?.Team == ev.Player.GetTeam() &&
                    t.SpawnConditionsMet() &&
                    (
                        ev.Reason != SpawnReason.Escaped ||
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
            Log.Debug("Round started, starting Coroutines");

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
            Log.Debug("Registering all custom shit.");

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
                    Log.Debug($"Attempting to register subclass {subclass.Identifier}");
                    if (subclass.TryRegister())
                    {
                        Log.Debug($"Registered subclass {subclass.Identifier}");
                    }
                }
            }
        }

        public static void UnregisterAll()
        {
            foreach (CustomTeam team in CustomTeam.Registered)
            {
                team.TryUnregister();
            }

            foreach (CustomSubclass subclass in CustomSubclass.Registered)
            {
                subclass.UnsubscribeEvents();
            }
        }
    }

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
    }
}
