using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using V2.PlayerHandling;
using V2.UI.PredStatsMenu;
using V2.UI.SizeScanners;
using V2.UI.StomachacheMeter;
using V2.UI.StomachCapacityMeter;
using V2.UI.StruggleSystem;
using V2.UI.MintWispSummonMeter;

namespace V2.UI
{
	public class UISystem : ModSystem
	{
		public UserInterface MouseRestrictionDummyLayer;
		public MouseRestrictionDummyUI MouseRestrictionDummy;

		public UserInterface HeldItemInterfaceLayer;
		public HeldItemDrawingUI HeldItemInterface;

		public UserInterface StomachCapacityBarInterfaceLayer;
		public StomachCapacityMeterUI StomachCapacityBarInterface;

		public UserInterface StomachacheMeterInterfaceLayer;
		public StomachacheMeterUI StomachacheMeterInterface;

		public UserInterface PlayerPredStruggleInterfaceLayer;
		public StruggleSystemUI PlayerPredStruggleInterface;

		public UserInterface PredStatsMenuMouthInterfaceLayer;
		public PredStatsMenuMouthUI PredStatsMenuMouthInterface;
		public UserInterface PredStatsMenuInterfaceLayer;
		public PredStatsMenuUI PredStatsMenuInterface;

		public UserInterface MealSizeScannerInterfaceLayer;
		public MealSizeScannerUI MealSizeScannerInterface;
		public UserInterface PredCapacityScannerInterfaceLayer;
		public PredCapacityScannerUI PredCapacityScannerInterface;

		public UserInterface MintWispSummonMeterInterfaceLayer;
		public MintWispSummonMeterUI MintWispSummonMeterInterface;

		public override void OnWorldLoad()
		{
			MouseRestrictionDummyLayer = new UserInterface();
			MouseRestrictionDummy = new MouseRestrictionDummyUI();
			MouseRestrictionDummy.Activate();
			MouseRestrictionDummyLayer.SetState(MouseRestrictionDummy);

			HeldItemInterfaceLayer = new UserInterface();
			HeldItemInterface = new HeldItemDrawingUI();
			HeldItemInterface.Activate();
			HeldItemInterfaceLayer.SetState(HeldItemInterface);

			StomachCapacityBarInterfaceLayer = new UserInterface();
			StomachCapacityBarInterface = new StomachCapacityMeterUI();
			StomachCapacityBarInterface.Activate();
			StomachCapacityBarInterfaceLayer.SetState(StomachCapacityBarInterface);

			StomachacheMeterInterfaceLayer = new UserInterface();
			StomachacheMeterInterface = new StomachacheMeterUI();
			StomachacheMeterInterface.Activate();
			StomachacheMeterInterfaceLayer.SetState(StomachacheMeterInterface);

			PlayerPredStruggleInterfaceLayer = new UserInterface();
			PlayerPredStruggleInterface = new StruggleSystemUI();
			PlayerPredStruggleInterface.Activate();
			PlayerPredStruggleInterfaceLayer.SetState(PlayerPredStruggleInterface);

			PredStatsMenuMouthInterfaceLayer = new UserInterface();
			PredStatsMenuMouthInterface = new PredStatsMenuMouthUI();
			PredStatsMenuMouthInterface.Activate();
			PredStatsMenuMouthInterfaceLayer.SetState(PredStatsMenuMouthInterface);
			PredStatsMenuInterfaceLayer = new UserInterface();
			PredStatsMenuInterface = new PredStatsMenuUI();
			PredStatsMenuInterface.Activate();
			PredStatsMenuInterfaceLayer.SetState(PredStatsMenuInterface);

			MealSizeScannerInterfaceLayer = new UserInterface();
			MealSizeScannerInterface = new MealSizeScannerUI();
			MealSizeScannerInterface.Activate();
			MealSizeScannerInterfaceLayer.SetState(MealSizeScannerInterface);
			PredCapacityScannerInterfaceLayer = new UserInterface();
			PredCapacityScannerInterface = new PredCapacityScannerUI();
			PredCapacityScannerInterface.Activate();
			PredCapacityScannerInterfaceLayer.SetState(PredCapacityScannerInterface);

			MintWispSummonMeterInterfaceLayer = new UserInterface();
			MintWispSummonMeterInterface = new MintWispSummonMeterUI();
			MintWispSummonMeterInterface.Activate();
			MintWispSummonMeterInterfaceLayer.SetState(MintWispSummonMeterInterface);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (layers.FirstOrDefault(x => x.Name == "Vanilla: Hair Window") is LegacyGameInterfaceLayer hairStyleWindowLegacyLayer)
			{
				int hairStyleWindowLegacyLayerIndex = layers.IndexOf(hairStyleWindowLegacyLayer);
				layers.Remove(hairStyleWindowLegacyLayer);
				layers.Insert(
					hairStyleWindowLegacyLayerIndex, new LegacyGameInterfaceLayer(
						"Vanilla: Hair Window (VSC Override)",
						delegate
						{
							if (Main.LocalPlayer.talkNPC != -1)
							{
								NPC stylist = Main.npc[Main.LocalPlayer.talkNPC];
								if (stylist.active && stylist.type == NPCID.Stylist)
									UIOverrides.DrawInterface_21_HairWindow(stylist);
							}
							return true;
						},
						InterfaceScaleType.UI
					)
				);
			}
			if (layers.FirstOrDefault(x => x.Name == "Vanilla: Death Text") is LegacyGameInterfaceLayer deathTextLegacyLayer)
			{
				int deathTextLegacyLayerIndex = layers.IndexOf(deathTextLegacyLayer);
				layers.Remove(deathTextLegacyLayer);
				layers.Insert(
					deathTextLegacyLayerIndex, new LegacyGameInterfaceLayer(
						"Vanilla: Death Text (VSC Override)",
						delegate
						{
							UIOverrides.DrawInterface_35_YouDied();
							return true;
						},
						InterfaceScaleType.UI
					)
				);
			}
			if (layers.FirstOrDefault(x => x.Name == "Vanilla: Cursor") is LegacyGameInterfaceLayer cursorLegacyLayer)
			{
				int cursorLegacyLayerIndex = layers.IndexOf(cursorLegacyLayer);
				if (!V2.BasicMode)
				{
					layers.Remove(cursorLegacyLayer);
					layers.Insert(
						cursorLegacyLayerIndex, new LegacyGameInterfaceLayer(
							"Vanilla: Cursor (VSC Override)",
							delegate
							{
								UIOverrides.DrawInterface_36_Cursor();
								return true;
							},
							InterfaceScaleType.UI
						)
					);
				}
				AddInterfaceLayer(layers, MouseRestrictionDummyLayer, MouseRestrictionDummy, cursorLegacyLayerIndex, "Mouse Restriction Dummy State");
			}

			int OverriddenHairWindowIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hair Window (VSC Override)"));
			if (OverriddenHairWindowIndex != -1)
			{
				AddInterfaceLayer(layers, StomachCapacityBarInterfaceLayer, StomachCapacityBarInterface, OverriddenHairWindowIndex, "Stomach Capacity Meter");
				AddInterfaceLayer(layers, StomachacheMeterInterfaceLayer, StomachacheMeterInterface, OverriddenHairWindowIndex + 1, "Stomachache Meter");
				if (!V2.BasicMode)
				{
					AddInterfaceLayer(layers, MealSizeScannerInterfaceLayer, MealSizeScannerInterface, OverriddenHairWindowIndex + 2, "Sizemic Scanner");
					AddInterfaceLayer(layers, PredCapacityScannerInterfaceLayer, PredCapacityScannerInterface, OverriddenHairWindowIndex + 3, "Servant's Scanner");
					AddInterfaceLayer(layers, PlayerPredStruggleInterfaceLayer, PlayerPredStruggleInterface, OverriddenHairWindowIndex + 4, "Player Pred Struggles");
					AddInterfaceLayer(layers, PredStatsMenuInterfaceLayer, PredStatsMenuInterface, OverriddenHairWindowIndex + 5, "Pred Stats Menu");
					AddInterfaceLayer(layers, PredStatsMenuMouthInterfaceLayer, PredStatsMenuMouthInterface, OverriddenHairWindowIndex + 6, "Rose");
					AddInterfaceLayer(layers, MintWispSummonMeterInterfaceLayer, MintWispSummonMeterInterface, OverriddenHairWindowIndex + 7, "Mint Wisp Summon Meter");
				}
			}
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (!V2.BasicMode && MouseTextIndex != -1)
				AddInterfaceLayer(layers, HeldItemInterfaceLayer, HeldItemInterface, MouseTextIndex, "Held Item");
		}

		public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, string customName = null)
		{
			string name;
			if (customName == null)
				name = state.ToString();
			else
				name = customName;

			layers.Insert(index, new LegacyGameInterfaceLayer("VSC: " + name,
				delegate
				{
					userInterface.Update(Main._drawInterfaceGameTime);
					state.Draw(Main.spriteBatch);
					return true;
				}, InterfaceScaleType.UI
			));
		}
	}
}
