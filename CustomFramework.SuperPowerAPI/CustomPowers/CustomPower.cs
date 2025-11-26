using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace CustomFramework.SuperPowerAPI.CustomPowers
{
    public abstract class CustomPower
    {
        public Dictionary<Player, int> TrackedPlayers { get; set; } = new Dictionary<Player, int>();
		public abstract int Id { get; }
		public abstract string Name { get; }
        public abstract string Info { get; }
        public virtual string Description => Info;

        public virtual void Enabled() { }

        public virtual void Disabled() { }

        public virtual bool CanUse() => true;

        public virtual bool Check(Player player) =>
			TrackedPlayers.ContainsKey(player) && TrackedPlayers[player] > 0;

		public static CustomPower Get(string power) =>
			Registered.FirstOrDefault(t => t.Name.Equals(power, System.StringComparison.OrdinalIgnoreCase));

		public static CustomPower Get(int id) =>
			Registered.FirstOrDefault(t => t.Id == id);

		public static bool TryGet(string power, out CustomPower customPower)
		{
			customPower = Get(power);
			return customPower != null;
		}

		internal void SetIntensity(Player player, byte intensity)
		{
			TrackedPlayers[player] = intensity;
			PowerEventHandler.OnRecievePower(new Events.EventArgs.RecievePowerEventArgs(player, this, intensity));
		}

		public static bool TryGet(int id, out CustomPower customPower)
		{
			customPower = Get(id);
			return customPower != null;
		}

		public static IEnumerable<CustomPower> RegisterPowers(Assembly assembly = null)
		{
			List<CustomPower> powers = new List<CustomPower>();
			if (assembly == null)
				assembly = Assembly.GetExecutingAssembly();

			foreach (Type type in assembly.GetTypes().Where(t => t.BaseType == typeof(CustomPower) && !t.IsAbstract).ToList())
			{
				CustomPower power = null;
				power = (CustomPower)Activator.CreateInstance(type);
				if (power.TryRegister())
					powers.Add(power);
			}

			return powers;
		}

		public static HashSet<CustomPower> Registered { get; } = new HashSet<CustomPower>();

		internal bool TryRegister()
		{
			if (!Registered.Contains(this))
			{
				Enabled();
				Registered.Add(this);
				Logger.Debug($"Power {Name} registered");
				return true;
			}
			return false;
		}
	}
}
