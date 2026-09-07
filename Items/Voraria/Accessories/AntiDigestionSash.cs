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

namespace V2.Items.Voraria.Accessories
{
	public class AntiDigestionSash : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public static float DigestionDefenseBonus => 4;
		public static float SoftenedBuildupReduction => 0.25f;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.AntiDigestionSash");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.AntiDigestionSash.Short");
		public override string Texture => "V2/Items/UnspritedItem";

		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.AsFood().MaxHealth = 350;
			Item.AsFood().Size = 0.37;
			Item.AsFood().AcidResistTier = 1;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(
				gold: 1
			);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AsFood().TakenDigestionDamageModifier.Flat -= DigestionDefenseBonus;
			player.AsFood().SoftenedDigestionDamageModifier *= 1f - SoftenedBuildupReduction;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Accessories.AntiDigestionSash",
				new
				{
					AntiDigestionSashDigestionDefenseBonus = DigestionDefenseBonus,
					AntiDigestionSashSoftenedBuildupReduction = SoftenedBuildupReduction.ToPercentage(2),
				}
			);
		}
	}
}
