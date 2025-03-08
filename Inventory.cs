using HarmonyLib;
using UnityEngine;

namespace CoinPurse
{
	[HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItem), typeof(ItemDrop.ItemData))]
	public class Inventory_AddItem_Patch
	{
		// This gets called upon loading the inventory for each item in the inventory.
		public static bool Prefix(ItemDrop.ItemData item, ref bool __result)
		{
			if (item.m_shared.m_value <= 0)
				return true;

			CoinPurse.Currency += item.m_stack * item.m_shared.m_value;
			__result = true;
			return false; // skip original
		}
	}

	[HarmonyPatch(typeof(Inventory), nameof(Inventory.CanAddItem), typeof(ItemDrop.ItemData), typeof(int))]
	public class Inventory_CanAddItem_Patch
	{
		public static void Postfix(ItemDrop.ItemData item, ref bool __result)
			=> __result = __result || item.m_shared.m_value > 0;
	}

	[HarmonyPatch(typeof(Inventory), nameof(Inventory.RemoveItem), typeof(string), typeof(int), typeof(int), typeof(bool))]
	public class Inventory_RemoveItem_Patch
	{
		public static void Postfix(string name, int amount)
		{
			if (StoreGui.instance?.m_coinPrefab.m_itemData.m_shared.m_name == name)
				CoinPurse.Currency -= amount;
		}
	}
}
