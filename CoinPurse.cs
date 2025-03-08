using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FileHelpers;

namespace CoinPurse
{
	internal class CoinPurse
	{
		private static readonly string SaveFile = Utils.GetSaveDataPath(FileSource.Legacy) + "\\_coinpurse.fch";

		internal static GameObject Prefab;
		internal static GameObject Instance;
		internal static TextMeshProUGUI TextComponent;

		public static Dictionary<long, int> Currencies = [];

		// TODO: Set the recipe for Treasure Chest to be just coins
		// TODO: Register coins for all recipes and vendors

		public static int Currency
		{
			get {
				var id = Player.m_localPlayer.GetPlayerID();
				if (id == 0)
					return 0;

				if (!Currencies.ContainsKey(id))
					Currencies[id] = 0;

				return Currencies[id];
			}

			set {
				var id = Player.m_localPlayer.GetPlayerID();
				if (id == 0)
					return;

				Currencies[id] = value;
				Text = value.ToString();
			}
		}

		public static string Text
		{
			get => TextComponent.text;
			set => TextComponent.text = value;
		}

		public static Image Background { get; private set; }
		public static Image Icon { get; private set; }

		public static void CreatePrefab(string modPath)
		{
			Plugin.Logger.LogInfo("CoinPurse.CreatePrefab called");
			Prefab = new GameObject("CoinPurse", typeof(RectTransform), typeof(CanvasRenderer));
			if (Prefab == null)
				Plugin.Logger.LogFatal("Failed to create CoinPurse");

			Prefab.transform.localPosition = new Vector3(602f, -196.50f, 0f);
			Prefab.layer = 5;
			Prefab.GetComponent<CanvasRenderer>().cullTransparentMesh = false;

			var cp_rect = Prefab.GetComponent<RectTransform>();
			cp_rect.anchorMin = new Vector2(1f, 0.5f);
			cp_rect.anchorMax = new Vector2(1f, 0.5f);
			cp_rect.anchoredPosition = new Vector2(32f, -3f); 
			cp_rect.sizeDelta = new Vector2(80f, 64f);

			CreateBackground(modPath);
			CreateIcon(modPath);
			CreateText(modPath);

			PrefabManager.Instance.AddPrefab(Prefab);
		}

		private static void CreateBackground(string modPath)
		{
			var woodPanelFlik = GUIManager.Instance.GetSprite("woodpanel_flik");
			var bkg = new GameObject("bkg", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)) {
				layer = 5
			};

			bkg.transform.SetParent(Prefab.transform);

			var bkgImage = bkg.GetComponent<Image>();
			bkgImage.sprite = woodPanelFlik;
			bkgImage.color = Color.white;

			bkg.GetComponent<CanvasRenderer>().cullTransparentMesh = false;

			var bkgRect = bkg.GetComponent<RectTransform>();
			bkgRect.anchorMin = Vector2.zero;
			bkgRect.anchorMax = Vector2.one;
			bkgRect.anchoredPosition = Vector2.zero;
			bkgRect.sizeDelta = Vector2.zero;
			bkgRect.localScale = Vector3.one;
		}

		private static void CreateIcon(string modPath)
		{
			var icon = new GameObject("cp_icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)) {
				layer = 5
			};

			icon.transform.SetParent(Prefab.transform);

			var iconTex = AssetUtils.LoadTexture(Path.Combine(modPath, "Assets\\trader.png"));
			var iconSprite = Sprite.Create(iconTex, new Rect(0, 0, iconTex.width, iconTex.height), Vector2.zero);

			var iconImage = icon.GetComponent<Image>();
			iconImage.sprite = iconSprite;
			iconImage.color = Color.white;

			var iconRect = icon.GetComponent<RectTransform>();
			iconRect.anchorMin = new Vector2(0.5f, 0.5f);
			iconRect.anchorMax = new Vector2(0.5f, 0.5f);
			iconRect.anchoredPosition = new Vector2(2f, 30f);
			iconRect.sizeDelta = new Vector2(43f, 43f);
		}

		private static void CreateText(string modPath)
		{
			Color32 faceColor = new(255, 216, 0, 255);
			Color32 outlineColor = new(0, 0, 0, 255);
			var normalFont = PrefabManager.Cache.GetPrefab<TMP_FontAsset>("Valheim-AveriaSansLibre");
			var fontMaterial = PrefabManager.Cache.GetPrefab<Material>("Valheim-AveriaSansLibre - Outline");

			var text = new GameObject("cp_text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)) {
				layer = 5
			};

			text.transform.SetParent(Prefab.transform);

			var text_rect = text.GetComponent<RectTransform>();
			text_rect.anchorMin = new Vector2(0.5f, 0.5f);
			text_rect.anchorMax = new Vector2(0.5f, 0.5f);
			text_rect.anchoredPosition = new Vector2(4f, 0f);
			text_rect.sizeDelta = new Vector2(67f, 22f);

			var tmpro = text.GetComponent<TextMeshProUGUI>();
			tmpro.font = normalFont;
			tmpro.fontSize = 16;
			tmpro.fontSharedMaterial = fontMaterial;
			tmpro.fontMaterial = fontMaterial;
			tmpro.color = faceColor;
			tmpro.outlineColor = outlineColor;
			tmpro.fontSizeMin = 1;
			tmpro.fontSizeMax = 16;
			tmpro.horizontalAlignment = HorizontalAlignmentOptions.Center;
			tmpro.verticalAlignment = VerticalAlignmentOptions.Middle;
			tmpro.outlineWidth = 0.175f;
			tmpro.fontFeatures.Add(UnityEngine.TextCore.OTL_FeatureTag.kern);
			tmpro.lineSpacing = 1f;
		}
		public static void Instantiate()
		{
			Plugin.Logger.LogInfo("CoinPurse.Instantiate called");

			// Not in game yet
			if (!InventoryGui.m_instance?.m_player)
				return;

			// Attach to inventory
			Instance = UnityEngine.Object.Instantiate(Prefab, InventoryGui.m_instance.m_player.transform);
			TextComponent = Instance.GetComponentInChildren<TextMeshProUGUI>();
			Background = Instance.transform.Find("bkg").GetComponent<Image>();
			Icon = Instance.transform.Find("cp_icon").GetComponent<Image>();

			Instance.transform.SetSiblingIndex(3);

			// Copy the material and color from another inventory element
			var weight = InventoryGui.m_instance.m_player.Find("Weight");
			if (weight == null)
			{
				Plugin.Logger.LogError("Failed to find weight inventory element");
				return;
			}

			var bkg = weight.Find("bkg");
			if (bkg == null)
			{
				Plugin.Logger.LogError("Failed to find bkg inventory element");
				return;
			}

			var bkgImage = bkg.GetComponent<Image>();
			Background.material = bkgImage.material;
			Background.color = bkgImage.color;

			Icon.material = bkgImage.material;
		}
		public static void SaveToDisk()
		{
			ZPackage pkg = new();
			foreach (var currency in Currencies)
			{
				pkg.Write(currency.Key);
				pkg.Write(currency.Value);
			}

			byte[] array = pkg.GetArray();

			try
			{
				FileWriter fileWriter = new(SaveFile, fileSource: FileSource.Local);
				fileWriter.m_binary.Write(array.Length);
				fileWriter.m_binary.Write(array);
				fileWriter.Finish();
			} catch (Exception e)
			{
				Plugin.Logger.LogWarning($"Error saving CoinPurse data: Path: {SaveFile}, Error: {e.Message}");
			}
		}
		public static void LoadFromDisk()
		{
			if (!File.Exists(SaveFile))
			{
				File.Create(SaveFile).Close();
				return;
			}

			FileReader fileReader;

			try
			{
				fileReader = new(SaveFile, fileSource: FileSource.Local);
			} catch (Exception e)
			{
				Plugin.Logger.LogWarning("Failed to create FileReader for CoinPurse data (" + e.Message + ")");
				return;
			}

			byte[] data;
			try
			{
				BinaryReader binary = fileReader.m_binary;
				data = binary.ReadBytes(binary.ReadInt32());
			} catch (Exception e)
			{
				Plugin.Logger.LogWarning($"Error loading CoinPurse data: Path: {SaveFile}, Error: {e.Message}");
				return;
			} finally
			{
				fileReader.Dispose();
			}

			ZPackage pkg = new(data);
			while (pkg.GetPos() < pkg.Size())
			{
				var key = pkg.ReadLong();
				var value = pkg.ReadInt();
				Currencies[key] = value;
			}
		}
	}
}
