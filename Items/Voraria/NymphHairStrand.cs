using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Voraria
{
	public class NymphHairStrand : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.NymphHairStrand");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.NymphHairStrand.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 25;
		}
		public override void SetDefaults()
		{
			Item.maxStack = Item.CommonMaxStack;

			Item.width = 26;
			Item.height = 26;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(
				platinum: 0,
				gold: 1,
				silver: 50,
				copper: 0
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.NymphHairStrand",
				new
				{
					
				}
			);
		}
	}
}
