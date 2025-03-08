using HarmonyLib;

namespace CoinPurse
{
	[HarmonyPatch(typeof(Piece), nameof(Piece.OnPlaced))]
	public class Piece_OnPlaced_Patch
	{
		public static void Postfix(Piece __instance)
		{
			if (__instance.m_name != "$piece_chesttreasure")
				return;
		}
	}
}
