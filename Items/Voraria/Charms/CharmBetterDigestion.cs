using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Charms
{
	public class CharmBetterDigestion : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int AcidStrengthBonus => 12;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Charms.BetterDigestion");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Charms.BetterDigestion.Short");
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.AsCharm().IsCharm = true;
			Item.AsAnItem().AccessoryEffectCode += UpdateCharmBetterDigestion;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(
				gold: 5
			);
		}

		public static void UpdateCharmBetterDigestion(Item item, Player player, bool hideVisual)
		{
			player.AsPred().ACI.Extra += AcidStrengthBonus;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Charms.BetterDigestion",
				new
				{
					AcidStrengthBonus
				}
			);
		}
	}
}
