using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Projectiles.Voraria.Weapons.Ranged.Throwables;

namespace V2.Items.Voraria.Weapons.Ranged.Throwables
{
	public static class ThrowableFungalDetails
	{
		public static string CondimentName => "Strange spores";
	}

	public class ThrowableFungalBottle : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Weapons.Ranged.Throwables.FragileBottles.Fungal");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Weapons.Ranged.Throwables.FragileBottles.Fungal.Short");

		public override void SetStaticDefaults()
		{
			
		}

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.width = 38;
			Item.height = 38;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 1.5f;
			Item.UseSound = SoundID.Item106 with { Pitch = 0.4f };
			Item.shoot = ModContent.ProjectileType<ThrowableFungalBottleProjectile>();
			Item.shootSpeed = 10f;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;

			Item.value = Item.buyPrice(
				silver: 75
			);
			Item.rare = ItemRarityID.Green;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Weapons.Ranged.Throwables.FragileBottles.Fungal",
				new
				{
					ThrowableBottleCondimentName = ThrowableFungalDetails.CondimentName,
					ThrowableBottleLowercaseCondimentName = ThrowableFungalDetails.CondimentName.ToLower(),
				}
			);
		}
	}
}