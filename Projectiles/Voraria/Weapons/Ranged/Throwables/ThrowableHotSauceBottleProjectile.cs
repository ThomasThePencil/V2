using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using V2.StatusEffects.Voraria.Debuffs;
using V2.Core;

namespace V2.Projectiles.Voraria.Weapons.Ranged.Throwables
{
	public class ThrowableHotSauceBottleProjectile : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ProjectileName.Voraria.Weapons.Ranged.Throwables.FragileBottles.HotSauce");

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.width = 38;
			Projectile.height = 38;
			Projectile.scale = 0.5f;
		}

		public override void AI()
		{
			Projectile.ai[0] += 1;
			if (Projectile.ai[0] > 20f)
			{
				Projectile.velocity.X *= 0.995f;
				Projectile.velocity.Y += 0.265f;
			}
			Projectile.rotation += MathHelper.ToRadians(50f) * Math.Sign(Projectile.velocity.X);

			if (Main.rand.NextBool(8) || Projectile.ai[0] % 8 == 0)
			{
				Dust.NewDustPerfect(
					new Vector2(
						Projectile.TrueCenter().X + Main.rand.NextFloat(-Projectile.width * Projectile.scale, Projectile.width * Projectile.scale),
						Projectile.TrueCenter().Y + Main.rand.NextFloat(-Projectile.height * Projectile.scale, Projectile.height * Projectile.scale)
					),
					Main.rand.NextBool() ? DustID.Lava : DustID.Torch,
					Projectile.velocity * 0.1f,
					0,
					default,
					1.25f
				);
			}
		}

		public override bool CanHitPlayer(Player target) => target.whoAmI != Projectile.owner;

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(
				ModContent.BuffType<TastySpicy>(),
				V2Utils.SensibleTime(seconds: 20)
			);	
		}

		public override void OnKill(int timeLeft)
		{
			int condimentDustCount = 32;
			float degreesPerCondimentDust = MathHelper.ToRadians(360f / (float)condimentDustCount);
			for (int i = 0; i < condimentDustCount; i++)
			{
				Dust.NewDustPerfect(
					Projectile.TrueCenter(),
					Main.rand.NextBool() ? DustID.Lava : DustID.Torch,
					new Vector2(0, 4).RotatedBy(degreesPerCondimentDust * i).RotatedByRandom(degreesPerCondimentDust / 5f),
					0,
					default,
					1.25f
				);
			}
			int glassDustCount = 4 + Main.rand.Next(5);
			for (int i = 0; i < glassDustCount; i++)
			{
				Dust.NewDustPerfect(
					Projectile.TrueCenter(),
					DustID.Glass,
					new Vector2(0, 3).RotatedByRandom(MathHelper.ToRadians(360f)),
					0,
					default,
					1.5f
				);
			}
			SoundEngine.PlaySound(SoundID.Item107 with { Pitch = 0.2f }, Projectile.position);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
			Vector2 origin = sourceRectangle.Size() / 2f;

			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0f);

			return false;
		}
	}
}
