using CustomFramework;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace Chaos_Agent
{
    public class ChaosAgentPlugin : Plugin<Config>
    {
		public override void OnEnabled()
		{
			CustomFrameworkPlugin.RegisterAll();

			base.OnEnabled();
		}
	}

	public class Config : IConfig
	{
		public bool IsEnabled { get; set; }
		public bool Debug { get; set; }
	}
}
