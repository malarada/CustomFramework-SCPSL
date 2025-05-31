using System;
using CustomFramework;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;

namespace CustomInformation
{
	public class CustomInfoPlugin : Plugin
	{
		public override string Name => "Custom Information Plugin";

		public override string Description => "Provides custom information to players.";

		public override string Author => "Pyro Cyclone Projects";

		public override Version Version => new Version(1, 0, 0);

		public override Version RequiredApiVersion => new Version(1, 0, 0);

		public override void Enable()
		{
			CustomHintService.RegisterHint((player) =>
			{
				if (player == null || !Round.IsRoundInProgress || !player.IsAlive)
					return string.Empty;

				var str = $"<align=right>Spectators:</align>";
	
				foreach (var ply in player.CurrentSpectators)
				{
					str += $"\n{ply.Nickname}";
				}

				return str;
			});
		}

		public override void Disable()
		{
		}
	}
}
