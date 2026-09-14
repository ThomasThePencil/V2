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

namespace V2.Items.Voraria.Accessories
{
	public class ShroomNecklace : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.ShroomNecklace");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.ShroomNecklace.Short");
		public override string Texture => "V2/Items/UnspritedItem";
		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		public override void HoldStyle(Player player, Rectangle heldItemFrame)
		{
			player.itemLocation.X -= 24 * 0.75f * player.direction;
			player.itemLocation.Y += 14;
		}
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.width = 30;
			Item.height = 30;
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Lime;
			Item.value = Item.sellPrice(
				gold: 10
			);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.PygmyNecklace)
				.AddIngredient(ItemID.GlowingMushroom, 100)
				.AddIngredient(ModContent.ItemType<MushroomToken>())
				.AddTile(TileID.Anvils)
				.Register();
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AsV2Player().ShroomNecklace = true;
			player.maxMinions += 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Accessories.ShroomNecklace",
				new
				{
					
				}
			);
		}
	}
}
