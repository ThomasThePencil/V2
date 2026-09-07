using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.NPCs.Vanilla.Forest;

namespace V2.Items.Voraria.Consumables.Catchables
{
	public class CaughtButterflyZebraSwallowtailWG2 : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.Catchables.Butterflies.ZebraSwallowtail.WeightGain2");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
			Item.DefaultToCapturedCritter(NPCID.Butterfly);
			Item.placeStyle = (int)NormalButterflyStuff.VanillaButterflySpecies.ZebraSwallowtail + (8 * 2) + 1;

			Item.bait = 22;

			Item.width = 12;
			Item.height = 12;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(
				silver: 22,
				copper: 50
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.Catchables.Butterflies.ZebraSwallowtail.WeightGain2",
				new
				{

				}
			);
		}
	}
}
