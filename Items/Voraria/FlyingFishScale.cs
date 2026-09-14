using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Voraria
{
	public class FlyingFishScale : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.FlyingFishScale");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.FlyingFishScale.Short");
		public override string Texture => "V2/Items/UnspritedItem";
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 25;
		}
		public override void SetDefaults()
		{
			Item.maxStack = Item.CommonMaxStack;

			Item.width = 26;
			Item.height = 26;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(
				platinum: 0,
				gold: 0,
				silver: 30,
				copper: 0
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.FlyingFishScale",
				new
				{
					
				}
			);
		}
	}
}
