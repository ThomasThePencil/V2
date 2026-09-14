using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Accessories.Informational
{
	public class MealSizeScanner : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Informational.MealSizeScanner");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.Informational.MealSizeScanner.Short");

		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;

			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PredCapacityScanner>();
		}
		public override void HoldStyle(Player player, Rectangle heldItemFrame)
		{
			player.itemLocation.X -= 24 * 0.75f * player.direction;
			player.itemLocation.Y += 14;
		}
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.width = 16;
			Item.height = 16;
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(
				gold: 5
			);

			Item.holdStyle = ItemUseStyleID.Swing;


			Item.AsFood().Size = 0.26;
			Item.AsFood().MaxHealth = 120;
		}

		public override void UpdateInventory(Player player)
		{
			player.AsPred().SizeScanner = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AsPred().SizeScanner = true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Accessories.Informational.MealSizeScanner",
				new
				{
					
				}
			);
		}
	}
}
