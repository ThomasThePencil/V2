using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Projectiles.Voraria.Weapons.Ranged.Throwables;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.Items.Voraria.Tools
{
	public class Spoon : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Melee;
			Item.width = 54;
			Item.height = 54;
			Item.knockBack = 4;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.crit = -2;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SpoonProjectile>();
			Item.shootSpeed = 8f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;

			Item.pick = 110;
			Item.axe = 22;
			Item.tileBoost = 3;
		}
		public override void HoldItem(Player player)
		{
			player.AddBuff(ModContent.BuffType<Trance>(), V2Utils.SensibleTime(seconds: 2));
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));
			position = position + (velocity * 3.5f) + new Vector2(Main.rand.Next(-150, 151) / 10, Main.rand.Next(-150, 151) / 10);
		}
		public override bool MeleePrefix()
		{
			return true;
		}
	}
	public class SpoonProjectile : ModProjectile
	{
		List<(PreyType, int)> IgnoreThese = [
				(PreyType.Projectile, ProjectileID.LastPrismLaser),
				(PreyType.Projectile, ProjectileID.RainCloudRaining),
				(PreyType.Projectile, ProjectileID.RainCloudMoving),
				(PreyType.Projectile, ProjectileID.BloodCloudRaining),
				(PreyType.Projectile, ProjectileID.BloodCloudMoving),
				(PreyType.Projectile, ProjectileID.RainbowFront),
				(PreyType.Projectile, ProjectileID.SpectreWrath),
				(PreyType.Projectile, ProjectileID.SpiritHeal),
				(PreyType.Projectile, ProjectileID.TerrarianBeam),
				(PreyType.Projectile, ProjectileID.MagnetSphereBall),
				(PreyType.Projectile, ProjectileID.TinyEater),
				(PreyType.Projectile, ModContent.ProjectileType<ThrowableFungalBottleProjectile>()),
				(PreyType.Projectile, ModContent.ProjectileType<ThrowableHoneyBottleProjectile>()),
				(PreyType.Projectile, ModContent.ProjectileType<ThrowableHotSauceBottleProjectile>()),
				];
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}
		public override void SetDefaults()
		{
			Projectile.ignoreWater = true;
			Projectile.damage = 10;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.knockBack = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 16;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.ownerHitCheck = true;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = 14;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Projectile.owner != Main.myPlayer) return;
			Player plr = Main.player[Projectile.owner];
			if (target.GetLifePercent() < 0.33f && target.life <= 150)
			{
				if (PredPlayer.CanSwallow(plr, target))
				{
					PredPlayer.Swallow(plr, target, ForceSwallow: true);
				}
			}
		}
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
			SetVisualOffsets();
			if (Projectile.owner != Main.myPlayer) return;
			Player plr = Main.player[Projectile.owner];
			Rectangle Hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
			foreach (var proj in Main.ActiveProjectiles)
			{
				if (!Hitbox.Intersects(proj.Hitbox)) continue;
				if (proj.CurrentCaptor() is not null) continue;
				if ((!proj.friendly || proj.hostile) && proj.damage > 0 && !proj.IsMinionOrSentryRelated)
				{
					bool shouldIgnore = false;
					foreach ((PreyType type, int ID) in IgnoreThese)
					{
						if (ID == proj.type)
							shouldIgnore = true;
					}
					if (shouldIgnore)
						continue;
					if (PredPlayer.CanSwallow(plr, proj))
					{
						PredPlayer.Swallow(plr, proj, ForceSwallow: true);
					}
				}
			}
			if (Projectile.timeLeft < 6)
			{
				Projectile.alpha = 255 - (255/6 * Projectile.timeLeft);
			}
			else if (Projectile.timeLeft >= 10)
			{
				Projectile.alpha = 255 / 4 * (14 - Projectile.timeLeft);
			}
		}
		private void SetVisualOffsets()
		{
			const int HalfSpriteWidth = 36 / 2;

			int HalfProjWidth = Projectile.width / 2;
			int HalfProjHeight = Projectile.height / 2;

			if (Projectile.spriteDirection == 1) {
				DrawOriginOffsetX = -(HalfProjWidth - HalfSpriteWidth);
				DrawOffsetX = (int)-DrawOriginOffsetX * 2;
				DrawOriginOffsetY = 0;
			}
			else {
				DrawOriginOffsetX = (HalfProjWidth - HalfSpriteWidth);
				DrawOffsetX = 0;
				DrawOriginOffsetY = 0;
			}
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}

