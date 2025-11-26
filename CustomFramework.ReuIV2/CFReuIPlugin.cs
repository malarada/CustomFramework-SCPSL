using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using RueI;
using RueI.Displays;
using RueI.Elements;

namespace CustomFramework.ReuIV2
{
	internal class CFReuIPlugin : Plugin
	{
		public override string Name => "CustomFramework.RueI";

		public override string Description => "An extension module for CustomFramework that adds automatic support for ReuI (Version 2).";

		public override string Author => "Pyro Cyclone Projects";

		public override Version Version => new(2, 0, 0);

		public override Version RequiredApiVersion => new(1, 1, 0);

		public static AutoElement AE { get; } = new(Roles.All, DE);

		public static DynamicElement DE { get; } = new(GetHint, 100);

		public static string GetHint(DisplayCore hub)
		{
			var player = Player.Get(hub.Hub);
			return CustomFrameworkPlugin.Instance.GetPlayerHint(player);
		}

		public override void Enable()
		{
			RueIMain.EnsureInit();
		}

		public override void Disable()
		{
		}
	}
}
