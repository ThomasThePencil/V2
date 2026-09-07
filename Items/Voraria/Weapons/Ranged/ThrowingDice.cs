using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using V2.Projectiles.Voraria.Pets;
using V2.Projectiles;
using System.Drawing;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles.Voraria.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using V2.PlayerHandling.PredPlayerGoals.Skilled;
using V2.Projectiles.Voraria.Weapons.Ranged;

namespace V2.Items.Voraria.Weapons.Ranged
{
	public class ThrowingDice : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override string Texture => "V2/Items/UnspritedItem";
		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		public override void SetDefaults()
		{
			Item.DefaultToRangedWeapon(ModContent.ProjectileType<Projectiles.Voraria.Weapons.Ranged.ThrowingDice>(), AmmoID.None, 20, 14f, true);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noUseGraphic = true;
			Item.damage = 9;
			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.buyPrice(gold: 35);
			Item.UseSound = SoundID.Item1;
		}
		/*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			return true;
		}*/
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.DinnerBlaster",
				new
				{

				}
			);
		}
	}
}
