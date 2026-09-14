using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.NPCs.Vanilla.Forest;

namespace V2.Items.Voraria.Consumables.Catchables
{
	public class CaughtButterflyRedAdmiralWG2 : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.Catchables.Butterflies.RedAdmiral.WeightGain2");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.Catchables.Butterflies.RedAdmiral.WeightGain2.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
			Item.DefaultToCapturedCritter(NPCID.Butterfly);
			Item.placeStyle = (int)NormalButterflyStuff.VanillaButterflySpecies.RedAdmiral + (8 * 2) + 1;

			Item.bait = 45;

			Item.width = 12;
			Item.height = 12;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(
				silver: 60
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.Catchables.Butterflies.RedAdmiral.WeightGain2",
				new
				{

				}
			);
		}
	}
}
