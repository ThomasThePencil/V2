using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Projectiles.Voraria.Weapons.Ranged.Throwables;

namespace V2.Items.Voraria.Weapons.Throwing.Knives
{
	public class PiranhaTooth : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Weapons.Throwing.Knives.PiranhaTooth");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Weapons.Throwing.Knives.PiranhaTooth.Short");
		public override string Texture => "V2/Items/UnspritedItem";

		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.width = 38;
			Item.height = 38;
			Item.useTime = 16;
			Item.useAnimation = 16;
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
				"Voraria.Weapons.Throwing.Knives.PiranhaTooth",
				new
				{
					
				}
			);
		}
	}
}