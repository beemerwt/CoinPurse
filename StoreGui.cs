using HarmonyLib;

namespace CoinPurse
{
	[HarmonyPatch(typeof(StoreGui), nameof(StoreGui.GetPlayerCoins))]
	public class StoreGui_GetPlayerCoins_Patch
	{
		public static void Postfix(ref int __result)
			=> __result = CoinPurse.Currency;
	}
}
