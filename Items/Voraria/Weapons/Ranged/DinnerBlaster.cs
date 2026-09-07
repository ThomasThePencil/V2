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

namespace V2.Items.Voraria.Weapons.Ranged
{
	public class DinnerBlaster : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override void SetDefaults()
		{
			Item.DefaultToRangedWeapon(ModContent.ProjectileType<Burger>(), AmmoID.None, 20, 14f, true);
			Item.DamageType = DamageClass.Generic;
			Item.damage = -1;
			Item.width = 42;
			Item.height = 26;
			Item.rare = ItemRarityID.Lime;
			Item.value = Item.buyPrice(gold: 35);
			Item.UseSound = SoundID.Item61;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Lighting.AddLight(player.Center + new Vector2(16 * player.direction, 0), new Vector3(255, 255, 255) * 0.003f);
			return true;
		}
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
	public class Burger : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			AIType = ProjectileID.Bullet;

			Projectile.AsFood().DefinedSize = 0.3;
			Projectile.AsFood().MaxHealth = 25;
			Projectile.AsFood().Health = 25;

			Projectile.AsFood().OnSwallowedBy += OnSwallowedByPlayer_GiveInfFoodGoal;
		}
		public override bool? CanDamage()
		{
			return false;
		}
		public override void AI()
		{
			foreach (var item in Main.ActiveNPCs)
			{
				if (Projectile is not null && Projectile.active && Projectile.CurrentCaptor() is null && item.active && item.CurrentCaptor() is null && item.AsPred().CanBeForceFed.Invoke(item) && Projectile.Hitbox.Intersects(item.Hitbox))
				{
					PredNPC.Swallow(item, Projectile);
				}
			}
			foreach (var item in Main.ActiveProjectiles)
			{
				if (Projectile is not null && Projectile.active && Projectile.CurrentCaptor() is null && item.active && item.CurrentCaptor() is null && item.AsPred().CanBeForceFed.Invoke(item) && Projectile.Hitbox.Intersects(item.Hitbox))
				{
					PredProjectile.Swallow(item, Projectile);
				}
			}
			foreach (var item in Main.ActivePlayers)
			{
				if (Projectile is not null && Projectile.active && Projectile.CurrentCaptor() is null && item.active && Main.player[Projectile.owner] != item && item.CurrentCaptor() is null && Projectile.Hitbox.Intersects(item.Hitbox))
				{
					PredPlayer.Swallow(item, Projectile);
				}
			}
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(255, 160, 43));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(255, 160, 43));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(255, 160, 43));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(255, 160, 43));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(255, 160, 43));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(194, 0, 0));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(128, 49, 0));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(128, 49, 0));
			Dust.NewDustDirect(Projectile.position, 28, 28, DustID.TintablePaint, oldVelocity.X * -0.1f, oldVelocity.Y * -0.1f, 0, new Microsoft.Xna.Framework.Color(48, 99, 31));
			return true;
		}
		public static void OnSwallowedByPlayer_GiveInfFoodGoal(Projectile projectile, Entity pred)
		{
			Player owner = Main.player[projectile.owner];
			if (owner is not null && owner == pred)
			{
				ModContent.GetInstance<BlasterLoophole>().TrySetCompletion(owner);
			}

		}
	}
}
