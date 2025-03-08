using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CoinPurse
{
	[BepInDependency(Jotunn.Main.ModGuid)]
	[BepInPlugin(CoinPurseGuid, CoinPurseName, CoinPurseVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string CoinPurseGuid = "com.github.beemerwt.coinpurse";
		public const string CoinPurseName = "CoinPurse";
		public const string CoinPurseVersion = "1.0.0";

		public static new ManualLogSource Logger;

		private static readonly Harmony Harmony = new("mod.coinpurse");

		private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {CoinPurseGuid} is loaded!");

			Harmony.PatchAll();

			PrefabManager.OnVanillaPrefabsAvailable += AddPrefabItems;
			// ItemManager.OnItemsRegistered += ReplaceRecipes;
			GUIManager.OnCustomGUIAvailable += CoinPurse.Instantiate;
		}

		private void AddPrefabItems()
		{
			CoinPurse.CreatePrefab(Path.GetDirectoryName(Info.Location));
			PrefabManager.OnVanillaPrefabsAvailable -= AddPrefabItems;

			ReplaceRecipes();
		}

		private void ReplaceRecipes()
		{
			// Total 259 coins
			// 8 fine wood
			// 2 silver

			var treasureChest = PrefabManager.Instance.GetPrefab("piece_chest_treasure");
			var piece = treasureChest.GetComponent<Piece>();
			List<Piece.Requirement> requirements = new(piece.m_resources);
			requirements.First(r => r.m_resItem.m_itemData.m_shared.m_name == "$item_coins").m_amount = 259;

			var rubyItem = requirements.First(r => r.m_resItem.m_itemData.m_shared.m_name == "$item_ruby");
			requirements.Remove(rubyItem);

			var silverNeckItem = requirements.First(r => r.m_resItem.m_itemData.m_shared.m_name == "$item_silvernecklace");
			requirements.Remove(silverNeckItem);

			piece.m_resources = [.. requirements];
		}
	}
}
