using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Projectiles.Voraria.Weapons.Ranged
{
	public class ThrowingDice : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 1200;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 8;
		}
		public override void OnSpawn(IEntitySource source)
		{
			Projectile.ai[0] = Main.rand.Next(1, 7);
			Projectile.damage *= (int)Projectile.ai[0];
		}
		public override void AI()
		{
			if (Projectile.velocity.X > 0.02f)
				Projectile.velocity.X -= 0.02f;
			else if (Projectile.velocity.X < -0.02f)
				Projectile.velocity.X += 0.02f;
			else
				Projectile.velocity.X = 0f;
			Projectile.velocity.Y += 0.3f;
			Projectile.rotation += Projectile.velocity.Length() / 40f;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Vector2 Direction = target.Center.DirectionTo(Projectile.Center);
			float Velocity = Projectile.velocity.Length();
			Projectile.velocity = Direction * Velocity;
			Projectile.timeLeft -= 60;
			Projectile.netUpdate = true;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (oldVelocity.X != Projectile.velocity.X)
			{
				Projectile.velocity.X = (0f - oldVelocity.X) * 0.9f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y)
			{
				Projectile.velocity.Y = (0f - oldVelocity.Y) * 0.8f;
			}
			Projectile.timeLeft -= 60;
			Projectile.netUpdate = true;
			return false;
		}
		public override void OnKill(int timeLeft)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Dust.NewDustPerfect(Projectile.BottomRight + new Vector2(22 + Main.rand.Next(-6, 7), 11 + Main.rand.Next(-6, 7)), ModContent.DustType<ThrowingDice_Dust>(), new Vector2(Main.rand.Next(-50, 51) / 15f, Main.rand.Next(-50, 26) / 15f));
				Dust.NewDustPerfect(Projectile.BottomRight + new Vector2(22 + Main.rand.Next(-6, 7), 11 + Main.rand.Next(-6, 7)), ModContent.DustType<ThrowingDice_Dust>(), new Vector2(Main.rand.Next(-50, 51) / 15f, Main.rand.Next(-50, 26) / 15f));
				Dust.NewDustPerfect(Projectile.BottomRight + new Vector2(22 + Main.rand.Next(-6, 7), 11 + Main.rand.Next(-6, 7)), ModContent.DustType<ThrowingDice_Dust>(), new Vector2(Main.rand.Next(-50, 51) / 15f, Main.rand.Next(-50, 26) / 15f));
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			string text = "V2/Projectiles/Voraria/Weapons/Ranged/ThrowingDice";
			Texture2D sprite = ModContent.Request<Texture2D>(text).Value;
			Rectangle sourceRect = new Rectangle(22 * ((int)Projectile.ai[0] - 1), 0, 22, 22);
			Main.EntitySpriteDraw(sprite, Projectile.position - Main.screenPosition + new Vector2(11, 11), sourceRect, lightColor, Projectile.rotation, new Vector2(11, 11), Projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
	public class ThrowingDice_Dust : ModDust
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Projectiles/Voraria/Weapons/Ranged/ThrowingDice_Dust";
		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(14 * Main.rand.Next(0, 3), 0, 14, 14);
			dust.noGravity = true;
			dust.customData = Main.rand.Next(-10, 11) / 50f;
			dust.rotation = (float)dust.customData;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(40, 25.5f), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity.Y += 0.15f;
			dust.velocity.X *= 0.98f;
			dust.rotation += (float)dust.customData / 5f;
			dust.alpha += 4;
			if (dust.alpha >= 255)
			{
				dust.active = false;
			}

			return false;
		}
	}
}
