using LabApi.Features.Wrappers;
using PlayerStatsSystem;

namespace CustomFramework.Extensions
{
	public static class PlayerExtensions
	{
		public static void SetMaxStamina(this Player player, float value)
		{
			if (player == null) return;

			player.GetStatModule<StaminaStat>().MaxValue = value;
		}
	}
}
