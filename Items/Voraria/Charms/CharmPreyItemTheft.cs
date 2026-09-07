using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Charms
{
	public class CharmPreyItemTheft : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public static int AcidStrengthBonus => 12;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Charms.PreyItemTheft");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Charms.PreyItemTheft.Short");
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.AsCharm().IsCharm = true;
			Item.AsAnItem().AccessoryEffectCode += UpdateCharmPreyItemTheft;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(
				gold: 5
			);
		}

		public static void UpdateCharmPreyItemTheft(Item item, Player player, bool hideVisual)
		{
			player.AsPred().charmStealPreyLoot = true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Charms.PreyItemTheft",
				new
				{
					
				}
			);
		}
	}
}
