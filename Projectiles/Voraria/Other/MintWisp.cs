using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using V2.Core;
using V2.Items;
using V2.Items.Voraria;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Projectiles.Voraria.Weapons.Ranged.Throwables;
using V2.Projectiles.Voraria.Weapons.Summon.ShroomFairy;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Projectiles.Voraria.Other
{
	public class MintWispPoof : ModDust
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Projectiles/Voraria/SporeTrail";
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 20, 20);
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied((int)Math.Clamp(255 - dust.alpha * 4f, 0, 255), 255, 255, 255 - dust.alpha), 0, new Vector2(10, 10), dust.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.velocity *= 0.9f;
			dust.position += dust.velocity;
			dust.scale *= 0.99f;
			dust.alpha += 1;
			dust.alpha = (int)Math.Clamp(dust.alpha * 1.1f, 0, 255);
			float light = dust.scale;

			Lighting.AddLight(dust.position, new Vector3(0.35f * light, 0.25f * light, light));

			if (dust.alpha >= 255)
			{
				dust.active = false;
			}
			return false;
		}
	}
	public class MintWispShotTrail : ModDust
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Projectiles/Voraria/SporeTrail";
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 20, 20);
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied((int)Math.Clamp(255 - dust.alpha * 4f, 0, 255), 255, 255, 255 - dust.alpha), 0, new Vector2(10, 10), dust.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.scale *= 0.95f;
			dust.alpha += 2;
			dust.alpha = (int)Math.Clamp(dust.alpha * 1.175f, 0, 255);
			float light = dust.scale;

			Lighting.AddLight(dust.position, new Vector3(0.35f * light, 0.25f * light, light));

			if (dust.alpha >= 255)
			{
				dust.active = false;
			}
			return false;
		}
	}

	public class MintWisp : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}
		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 60;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = -1;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
		}
		public override bool? CanCutTiles() => false;
		public override bool CanHitPlayer(Player target) => false;
		public override bool? CanHitNPC(NPC target) => false;

		public bool Shoot(Player player)
		{
			NPC target = null;
			float targetDistance = 400f;

			if (player.HasMinionAttackTargetNPC)
				target = Main.npc[player.MinionAttackTargetNPC];
			else
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.CurrentCaptor() is not null) continue;
					if (npc.friendly) continue;
					float distance = npc.Center.Distance(Projectile.Center);
					if (distance < targetDistance)
					{
						target = npc;
						targetDistance = distance;
					}
				}
			}
			if (target != null)
			{
				Vector2 direction = Projectile.Center.DirectionTo(target.Center);
				direction.Normalize();
				SoundEngine.PlaySound(SoundID.NPCHit36 with { Pitch = 0.6f, PitchVariance = 0.15f, Volume = 0.3f, MaxInstances = 5 }, Projectile.Center);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction * 4, ModContent.ProjectileType<MintWispShot>(), Projectile.damage, 0f, Projectile.owner, ai2: target.whoAmI + 1);
				return true;
			}
			return false;
		}
		public bool ShootSelf(Player player)
		{
			NPC target = null;
			float targetDistance = 400f;

			if (player.HasMinionAttackTargetNPC)
				target = Main.npc[player.MinionAttackTargetNPC];
			else
			{
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.CurrentCaptor() is not null) continue;
					if (npc.friendly) continue;
					float distance = npc.Center.Distance(Projectile.Center);
					if (distance < targetDistance)
					{
						target = npc;
						targetDistance = distance;
					}
				}
			}
			if (target != null)
			{
				SoundEngine.PlaySound(SoundID.Item42 with { Pitch = 0.3f, PitchVariance = 0.1f, Volume = 2.5f, MaxInstances = 0 }, Projectile.Center);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -12), ModContent.ProjectileType<MintWispCharge>(), Projectile.damage, 0f, Projectile.owner, ai2: target.whoAmI + 1);
				return true;
			}
			return false;
		}

		public override void AI()
		{
			Projectile.timeLeft = 60;
			Player owner = Main.player[Projectile.owner];
			if (!owner.active || owner.dead)
				Projectile.Kill();
			Projectile.ai[0] += 1;
			if (Projectile.ai[0] >= 66)
			{
				if (7 + Main.rand.Next(5) < Projectile.ai[1])
				{
					if (ShootSelf(owner))
					Projectile.Kill();
				}
				else
					if (Shoot(owner))
					{
						Projectile.ai[0] = -11;
						Projectile.ai[1]++;
					}
				else
					Projectile.ai[0] = Main.rand.Next(8);
			}
			float Distance = Projectile.Center.Distance(owner.Center - new Vector2(0, 32));
			if (Distance <= 120)
			{
				Projectile.velocity += new Vector2(Main.rand.Next(-50, 51) / 1000f, Main.rand.Next(-50, 51) / 1000f);
				return;
			}
			Vector2 direction = Projectile.Center.DirectionTo(owner.Center - new Vector2(0, 32));
			direction.Normalize();
			if (Distance > 1800)
				Projectile.velocity = direction * (Distance / 50);
			else
			{
				float randomVelocity = Main.rand.Next(980, 1000) / 1000f;
				Projectile.velocity *= randomVelocity;
				Projectile.velocity += direction / 3f * randomVelocity;
				float VelocityMagnitude = Projectile.velocity.Length();
				float SpeedLimit = Math.Clamp((Distance - 60f) / 12f, 1, 10);
				if (VelocityMagnitude > SpeedLimit)
				{
					Projectile.velocity.Normalize();
					Projectile.velocity *= SpeedLimit;
				}
			}

		}
		public override void PostAI()
		{
			Projectile.rotation = Projectile.velocity.X * 0.04f;
			int framerate = 6;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= framerate)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Projectile.type] - 1)
				{
					Projectile.frame = 0;
				}
			}
			if (Projectile.ai[0] < 0)
				Projectile.frame = 2;
			Lighting.AddLight(Projectile.Center, Color.MintCream.ToVector3() * 0.7f);
		}

		public override void OnKill(int timeLeft)
		{
			int Bow = Mod.Find<ModGore>("MintWispBowtie").Type;
			float XVelocity = Main.rand.Next(-60, 61) / 100f;
			Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center - new Vector2(0, 2), new Vector2(XVelocity, -1.5f), Bow);
			for (int i = 0; i < 8; i++)
			{
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MintWispPoof>(), new Vector2(Main.rand.Next(-125, 126) / 50f, Main.rand.Next(-125, 126) / 50f), Scale: Main.rand.Next(100, 200) / 100f);
			}

		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.White;

			Vector2 Offset = new Vector2(12, 40);

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			SpriteEffects val = Projectile.direction != -1 ? 0 : (SpriteEffects)1;
			SpriteEffects spriteEffects = val;

			Rectangle sourceRect = new Rectangle(0, 32 * Projectile.frame, 32, 32);
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + Offset;
				Color color = Projectile.GetAlpha(lightColor) * 0.8f * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
			}

			Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Offset, sourceRect, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
			return false;
		}
	}
	public class MintWispShot : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Projectiles/Voraria/SporeTrail";
		public sealed override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = 1;
			Projectile.extraUpdates = 2;
			Projectile.alpha = 255;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
		}
		public override bool? CanCutTiles() => false;
		public int TargetNPCIndex()
		{
			if (Projectile.ai[2] > 0)
				return Main.npc[(int)Projectile.ai[2] - 1].active ? (int)Projectile.ai[2] - 1 : -1;
			else
			{
				NPC target = null;
				float targetDistance = 99999f;
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.CurrentCaptor() is not null) continue;
					if (npc.friendly) continue;
					float distance = npc.Center.Distance(Projectile.Center);
					if (distance < targetDistance)
					{
						target = npc;
						targetDistance = distance;
					}
				}
				if (target != null)
					return target.whoAmI;
			}
			return -1;
		}
		public override void AI()
		{
			int targetIndex = TargetNPCIndex();
			if (targetIndex < 0)
			{
				Projectile.ai[2] = 0;
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MintWispShotTrail>(), Scale: 0.7f);
				return;
			}
			NPC target = Main.npc[targetIndex];
			Vector2 direction = Projectile.position.DirectionTo(target.Center);
			direction.Normalize();
			Projectile.velocity *= 0.991f;
			Projectile.velocity += direction / 4f;
			float VelocityMagnitude = Projectile.velocity.Length();
			if (VelocityMagnitude > 6)
			{
				Projectile.velocity.Normalize();
				Projectile.velocity *= 6;
			}
			Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MintWispShotTrail>(), Scale: 0.7f);
		}
		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 6; i++)
			{
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MintWispPoof>(), new Vector2(Main.rand.Next(-125, 126) / 50f, Main.rand.Next(-125, 126) / 50f), Scale: Main.rand.Next(50, 100) / 100f);
			}
		}
	}
	public class MintWispCharge : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}
		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.friendly = true;
			Projectile.extraUpdates = 2;
			Projectile.DamageType = DamageClass.Summon;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
		}
		public override bool? CanCutTiles() => false;
		public int TargetNPCIndex()
		{
			if (Projectile.ai[2] > 0)
				return Main.npc[(int)Projectile.ai[2] - 1].active ? (int)Projectile.ai[2] - 1 : -1;
			else
			{
				NPC target = null;
				float targetDistance = 99999f;
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.CurrentCaptor() is not null) continue;
					if (npc.friendly) continue;
					float distance = npc.Center.Distance(Projectile.Center);
					if (distance < targetDistance)
					{
						target = npc;
						targetDistance = distance;
					}
				}
				if (target != null)
					return target.whoAmI;
			}
			return -1;
		}
		public override void AI()
		{
			int targetIndex = TargetNPCIndex();
			if (targetIndex < 0)
			{
				Projectile.ai[2] = 0;
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MintWispShotTrail>());
				return;
			}
			NPC target = Main.npc[targetIndex];
			Vector2 direction = Projectile.position.DirectionTo(target.Center);
			direction.Normalize();
			Projectile.velocity *= 0.997f;
			Projectile.velocity += direction / 4f;
			float VelocityMagnitude = Projectile.velocity.Length();
			if (VelocityMagnitude > 8)
			{
				Projectile.velocity.Normalize();
				Projectile.velocity *= 8;
			}
			Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MintWispShotTrail>());
		}
		public override void PostAI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			int framerate = 6;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= framerate)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
				}
			}
			Lighting.AddLight(Projectile.Center, Color.MintCream.ToVector3() * 0.7f);
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath39 with { Pitch = 0.35f, PitchVariance = 0.1f, MaxInstances = 0 }, Projectile.Center);
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MintWispExplode>(), Projectile.damage * 3, 0f, Projectile.owner);
			for (int i = 0; i < 12; i++)
			{
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MintWispPoof>(), new Vector2(Main.rand.Next(-255, 256) / 30f, Main.rand.Next(-255, 256) / 30f), Scale: Main.rand.Next(150, 300) / 100f);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.White;

			Vector2 Offset = new Vector2(13, 13);

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			SpriteEffects val = Projectile.direction != -1 ? 0 : (SpriteEffects)1;
			SpriteEffects spriteEffects = val;

			Rectangle sourceRect = new Rectangle(0, 32 * Projectile.frame, 32, 32);
			Vector2 drawOrigin = new Vector2(13, 13);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + Offset;
				Color color = Projectile.GetAlpha(lightColor) * 0.8f * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
			}

			Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition + Offset, sourceRect, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
			return false;
		}
	}
	public class MintWispExplode : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Projectiles/Voraria/SporeTrail";
		public sealed override void SetDefaults()
		{
			Projectile.width = 160;
			Projectile.height = 160;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 6;
			Projectile.alpha = 255;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;
		}
	}

}
