using HarmonyLib;

namespace CoinPurse
{
	[HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Show))]
	public class InventoryGui_Show_Patch
	{
		public static void Prefix()
		{
			CoinPurse.Text = CoinPurse.Currency.ToString();
		}
	}
}
