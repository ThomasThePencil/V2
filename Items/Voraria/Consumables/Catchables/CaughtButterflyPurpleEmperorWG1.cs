using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.NPCs.Vanilla.Forest;

namespace V2.Items.Voraria.Consumables.Catchables
{
	public class CaughtButterflyPurpleEmperorWG1 : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.Catchables.Butterflies.PurpleEmperor.WeightGain1");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.Catchables.Butterflies.PurpleEmperor.WeightGain1.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
			Item.DefaultToCapturedCritter(NPCID.Butterfly);
			Item.placeStyle = (int)NormalButterflyStuff.VanillaButterflySpecies.PurpleEmperor + (8 * 1) + 1;

			Item.bait = 42;

			Item.width = 12;
			Item.height = 12;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(
				silver: 90
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.Catchables.Butterflies.PurpleEmperor.WeightGain1",
				new
				{

				}
			);
		}
	}
}
