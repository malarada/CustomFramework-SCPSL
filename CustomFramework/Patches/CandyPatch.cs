using CustomFramework.EventArgs;
using HarmonyLib;
using InventorySystem.Items.Usables.Scp330;
using LabApi.Features.Console;

namespace CustomFramework.Patches
{
	[HarmonyPatch(typeof(CandyRainbow), "ServerApplyEffects")]
	[HarmonyPatch(typeof(CandyYellow), "ServerApplyEffects")]
	[HarmonyPatch(typeof(CandyPurple), "ServerApplyEffects")]
	[HarmonyPatch(typeof(CandyRed), "ServerApplyEffects")]
	[HarmonyPatch(typeof(CandyGreen), "ServerApplyEffects")]
	[HarmonyPatch(typeof(CandyBlue), "ServerApplyEffects")]
	[HarmonyPatch(typeof(CandyPink), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyBlack), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyBlue), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyBrown), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyEvil), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyGray), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyGreen), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyOrange), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyPink), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyPurple), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyRainbow), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyRed), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyWhite), "ServerApplyEffects")]
	[HarmonyPatch(typeof(HauntedCandyYellow), "ServerApplyEffects")]
	internal class CandyPatch
	{
		static CandyPatch()
		{
			Logger.Debug("CandyPatch initialized");
		}

		private static void Postfix(object __instance, ReferenceHub hub)
		{
			if (__instance is ICandy i)
			{
				Logger.Debug($"Candy eaten: {i.Kind}");
				EatenCandyEventArgs ev = new EatenCandyEventArgs(i.Kind, hub);
				CustomEventHandler.OnEatenCandy(ev);
			}
		}
	}
}
