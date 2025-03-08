using HarmonyLib;

namespace CoinPurse
{
	[HarmonyPatch(typeof(Player))]
	public class PlayerPatches
	{
		[HarmonyPatch(nameof(Player.Load))]
		[HarmonyPrefix]
		public static void Load() => CoinPurse.LoadFromDisk();

		[HarmonyPatch(nameof(Player.Save))]
		[HarmonyPostfix]
		public static void Save() => CoinPurse.SaveToDisk();
	}
}
