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
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Accessories
{
	public class NymphHairScarf : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public static float MoveSpeedBonus => 0.12f;
		public static float StomachWeightReduction => 0.12f;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.NymphHairScarf");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.NymphHairScarf.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(
				gold: 5
			);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.moveSpeed += MoveSpeedBonus;
			player.AsPred().StomachWeightModifier *= 1f - StomachWeightReduction;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Accessories.NymphHairScarf",
				new
				{
					NymphScarfMoveSpeedBuff = MoveSpeedBonus.ToPercentage(2),
					NymphScarfWeightReduction = StomachWeightReduction.ToPercentage(2),
				}
			);
		}
	}
}
