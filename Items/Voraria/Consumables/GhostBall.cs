using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Ghost;
using V2.NPCs.Voraria.Underworld.HellHarpy;
using V2.Projectiles.Voraria.Weapons.Ranged.Throwables;
using V2.Projectiles.Voraria.Weapons.Summon;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.Items.Voraria.Consumables
{
	public class GhostBall : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.GhostBall");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.GhostBall");
		public override void SetDefaults()
		{
			Item.consumable = true;
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = Item.CommonMaxStack;

			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<GhostBallProjectile>();
			Item.shootSpeed = 11f;
			Item.consumable = true;

			Item.value = Item.buyPrice(gold: 12);
			Item.rare = ItemRarityID.Cyan;

			Item.AsFood().MaxHealth = 100;
			Item.AsFood().Size = 0.15;

			Item.AsFood().EdibleOnUse = true;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.GhostBall",
				new
				{
				}
			);
		}

	}
	public class GhostBallProjectile : ModProjectile
	{

		public override string Texture => "V2/Items/Voraria/Consumables/GhostBall";
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ProjectileName.Voraria.GhostBall");

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.width = 16;
			Projectile.height = 16;
		}

		public override void AI()
		{
			Projectile.ai[0] += 1;
			if (Projectile.ai[0] > 20f)
			{
				Projectile.velocity.X *= 0.998f;
				Projectile.velocity.Y += 0.1f;
			}
			Projectile.rotation += MathHelper.ToRadians(50f) * Math.Sign(Projectile.velocity.X);

			if (Projectile.ai[0] % 2 == 0)
			{
				float degrees = Main.rand.Next(-17, 17) / 100f;
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GhostBallDust>(), (Projectile.velocity * 0.8f).RotatedByRandom(degrees));
			}
		}
		public override void OnKill(int timeLeft)
		{
			int condimentDustCount = 32;
			float degreesPerCondimentDust = MathHelper.ToRadians(360f / (float)condimentDustCount);
			for (int i = 0; i < condimentDustCount; i++)
			{
				float scale = Main.rand.Next(50, 200);
				float speed = Main.rand.Next(50, 200) / 100f;
				
				Dust.NewDustPerfect(
					Projectile.TrueCenter(),
					ModContent.DustType<GhostBallDust>(),
					new Vector2(0, 4 * speed).RotatedBy(degreesPerCondimentDust * i).RotatedByRandom(degreesPerCondimentDust / 5f),
					0,
					default,
					scale / 100f
				);
			}
			SoundEngine.PlaySound(SoundID.Item103 with { Pitch = 0.25f }, Projectile.position);
			if (!NPC.AnyNPCs(ModContent.NPCType<Echo>()))
			{
				NPC npc = NPC.NewNPCDirect(
					Projectile.GetSource_FromAI(),
					(int)Projectile.Center.X,
					(int)Projectile.Center.Y - 16,
					ModContent.NPCType<Echo>()
				);
				npc.netUpdate = true;
			}

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
	public class GhostBallDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 8, 8);
			dust.customData = 1f;
			dust.alpha = 0;
			if (Main.rand.NextBool(2)) dust.customData = -1f;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(4, 3), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += (float)dust.customData / 5f;
			dust.alpha += 5;
			dust.scale *= 0.98f;
			dust.velocity.X *= 0.97f;
			dust.velocity.Y *= 0.97f;
			dust.velocity.Y -= 0.02f;
			float light = 0.01f * (255 - dust.alpha);

			Lighting.AddLight(dust.position, new Vector3(0.3f * light, 0.8f * light, light));

			if (dust.alpha >= 255)
			{
				dust.active = false;
			}

			return false;
		}
	}
}
