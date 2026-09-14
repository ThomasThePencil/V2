using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Charms
{
	public class CharmLessStomachWeight : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static double MaxWeightReduction => 0.40;
		public static double FullnessEffectivenessLossThreshold => 0.70;
		public static double WeightReductionEffectiveness(Player player)
		{
			double stomachCapacityPercent = player.AsPred().StomachFullness / player.AsPred().StomachCapacity;
			return Math.Min(1.0 - stomachCapacityPercent, 1.0 - FullnessEffectivenessLossThreshold) / (1.0 - FullnessEffectivenessLossThreshold);
		}

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Charms.LessStomachWeight");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Charms.LessStomachWeight.Short");

		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(9, 10);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.AsCharm().IsCharm = true;
			Item.AsAnItem().AccessoryEffectCode += UpdateCharmLessStomachWeight;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(
				gold: 16,
				silver: 20
			);
		}

		public static void UpdateCharmLessStomachWeight(Item item, Player player, bool hideVisual)
		{
			player.AsPred().StomachWeightModifier *= 1f - (float)(MaxWeightReduction * WeightReductionEffectiveness(player));
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Charms.LessStomachWeight",
				new
				{
					WeightReduction = MaxWeightReduction.ToPercentage(2),
					FullnessEffectivenessLossThreshold = FullnessEffectivenessLossThreshold.ToPercentage(2),
					CurrentWeightReduction = (MaxWeightReduction * WeightReductionEffectiveness(Main.LocalPlayer)).ToPercentage(2),
				}
			);
		}
	}
}
