using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace CustomFramework.CustomTeams
{
    internal abstract class CustomTeam
    {
        public static List<CustomTeam> Registered = new List<CustomTeam>();

        internal int Id { get; set; }
        public abstract string Identifier { get; set; }
        public abstract string Name { get; set; }
        public abstract float SpawnTickets { get; set; }

        public virtual bool SpawnConditionsMet() => true;

        public virtual void SendCassieIntro()
        {
        }

        public virtual void SubscribeEvents()
        {
        }

        public virtual void UnsubscribeEvents()
        {
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
                return false;

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
    }
}
