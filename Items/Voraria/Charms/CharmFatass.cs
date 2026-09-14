using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Charms
{
	public class CharmFatass : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Charms.Fatass");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Charms.Fatass.Short");
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.AsCharm().IsCharm = true;
			Item.AsAnItem().AccessoryEffectCode += UpdateCharmFatass;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(
				gold: 20
			);
		}

		public static void UpdateCharmFatass(Item item, Player player, bool hideVisual)
		{
			player.AsPred().SwallowCapacityModifier *= 2f;
			player.AsPred().StomachCapacityModifier *= 2f;
			player.AsPred().DigestionTickDamageModifier *= 0.5f;
			player.AsPred().DigestionTickRateModifier *= 0.5f;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Charms.Fatass",
				new
				{
					
				}
			);
		}
	}
}
