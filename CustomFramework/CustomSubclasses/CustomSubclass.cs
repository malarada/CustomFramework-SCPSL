using Exiled.API.Features;
using Exiled.API.Features.Roles;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VoiceChat;

namespace CustomFramework.CustomSubclasses
{
    public abstract class CustomSubclass
    {
        public static List<CustomSubclass> Registered = new List<CustomSubclass>();

        internal int Id { get; set; }
        public abstract string Identifier { get; set; }
        public abstract string Name { get; set; }
        public abstract float SpawnTickets { get; set; }
        public abstract string Description { get; set; }
        public abstract string Info { get; set; }
        public abstract string CustomInfo { get; set; }
        public virtual VoiceChatChannel VoiceChatChannel { get; set; } = VoiceChatChannel.None;
        public virtual Vector3 Scale { get; set; } = Vector3.one;
        public virtual bool IsEscapeRole { get; set; } = true;

        public List<Player> TrackedPlayers { get; set; } = new List<Player>();

        public virtual bool Check(Player player) => TrackedPlayers.Contains(player);

        public virtual bool SpawnConditionsMet() => true;

        public virtual void SubscribeEvents()
        {
        }

        public virtual void UnsubscribeEvents()
        {
        }

        public virtual void OnAbility(Player player)
        {
        }

        public virtual string GetSpecificHint(Player player)
        {
            return string.Empty;
        }

        protected Vector3 PriorScale = Vector3.one;

        public virtual void GiveSubclass(Player player)
        {
            GiveSubclass(player, RoleSpawnFlags.None);
        }

        public virtual void GiveSubclass(Player player, RoleSpawnFlags flags) //, bool setRole = false, bool useSpawnpoint = false, bool resetInventory = true)
        {
            Log.Debug($"Giving {player.Nickname} {Identifier} subclass.");

            TrackedPlayers.Add(player);
            player.CustomInfo = CustomInfo;
            player.UniqueRole = Identifier;
            PriorScale = player.Scale;
            player.Scale = Vector3.Scale(player.Scale, Scale);
            //player.Scale.Scale(Scale);
            player.Broadcast(5, $"You are the {Name} {GetType().GetCustomAttribute<CustomSubclassAttribute>().Team}.\n{Description}");
        }

        public virtual void RemoveSubclass(Player player)
        {
            Log.Debug($"Removing from {player.Nickname} {Identifier} subclass.");

            if (player == null) return;

            if (TrackedPlayers.Contains(player))
                TrackedPlayers.Remove(player);
            player.CustomInfo = "";
            player.UniqueRole = "";
            player.Scale = PriorScale;
        }

        public virtual void Init()
        {
            SubscribeEvents();
        }

        public virtual void Destroy()
        {
            UnsubscribeEvents();
        }

        internal bool TryRegister()
        {
            if (!CustomFrameworkPlugin.Instance.Config.IsEnabled)
            {
                Log.Warn($"Couldn't register CustomFramework, CustomFramework is not enabled.");
                return false;
            }

            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Identifier == Identifier))
                {
                    Log.Warn($"{Identifier} was already registered.");
                    return false;
                }

                Id = Registered.Count;
                Registered.Add(this);
                Init();
                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Identifier})");
            return false;
        }

        internal bool TryUnregister()
        {
            Destroy();
            return Registered.Remove(this);
        }

        public static CustomSubclass Get(string identifier) => Registered.FirstOrDefault(t => t.Identifier == identifier);

        internal static CustomSubclass Get(int id) => Registered.FirstOrDefault(t => t.Id == id);
    }
}
