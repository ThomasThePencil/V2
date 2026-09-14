using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Consumables
{
	public class FeatherDuster : ModItem
	{
		public static double StruggleDamage => 500;

		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.FeatherDuster");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.FeatherDuster.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 99;
		}
		public override void SetDefaults()
		{
			Item.consumable = true;
			Item.maxStack = Item.CommonMaxStack;

			Item.useAnimation = 12;
			Item.useTime = 12;
			Item.useStyle = ItemUseStyleID.Swing;

			Item.AsFood().Size = 0.15;
			Item.AsFood().MaxHealth = 115;
			Item.AsFood().AcidResistTier = 0;

			Item.AsFood().CanUseInStomach = CanUseInStomach;
			Item.AsFood().UseInStomach = UseInStomach;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(
				silver: 80
			);
		}
		public override bool CanUseItem(Player player) => player.CurrentCaptor() is not null;

		public override void UseAnimation(Player player)
		{
			
		}

		public static bool CanUseInStomach(Item item, Player player, Entity pred) => true;
		public static void UseInStomach(Item item, Player player, Entity pred)
		{
			player.CurrentCaptor().ModifyPredStomachacheMeter(StruggleDamage);
			if (player.whoAmI == Main.myPlayer && player.inventory[58] == item)
			{
				Main.mouseItem.stack--;
				if (Main.mouseItem.stack <= 0)
				{
					player.AsPred().ItemCooldownWhenSwallowingANonStackedItemFromTheMouseSlotBecauseThisGameIsCoolAndAwesome = 7;
					Main.mouseItem.TurnToAir();
				}
			}
			else
			{
				item.stack--;
				if (item.stack <= 0)
					item.TurnToAir();
			}
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.FeatherDuster",
				new
				{
					FeatherDusterStruggleDamage = StruggleDamage,
				}
			);
		}
	}
}
