using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables;
using V2.Sounds.Vore;

namespace V2.NPCs.Voraria.Underworld.HellHarpy
{
	public class HellHarpy : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[NPC.type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "V2/NPCs/Voraria/Underworld/HellHarpy/HellHarpy",
				Position = new Vector2(0, 22),
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 100;
			NPC.aiStyle = -1;
			NPC.damage = 30;
			NPC.defense = 15;
			NPC.lifeMax = 6000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 37500f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.behindTiles = true;

			NPC.AsFood().DefinedBaseSize = 19.5;
			NPC.AsPred().WeightGainRatio = 0.111;
			NPC.AsPred().MaxStomachCapacity = 11.0;
			NPC.AsPred().BaseStomachacheMeterCapacity = 775.0;

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;
			NPC.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			NPC.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			NPC.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			NPC.AsPred().GetVisualBellySize = GetVisualBellySize;
			NPC.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			NPC.AsPred().CanBeForceFed = CanHellHarpyBeForceFed;

			NPC.AsPred().SmallBurps = Burps.Humanoid.Small;
			NPC.AsPred().SmallBurpThreshold = 0.35;
			NPC.AsPred().StandardBurps = Burps.Humanoid.Standard;
			NPC.AsPred().SmallGulps = Gulps.Short;
			NPC.AsPred().SmallGulpThreshold = 0.35;
			NPC.AsPred().BigGulps = Gulps.Standard;

		}
		public override void OnSpawn(IEntitySource source)
		{
			NPC.velocity.Y = -7f;
			NPC.ai[1] = -100;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.Underworld.HellHarpy"),
			});
		}
		public static bool CanHellHarpyBeForceFed(NPC npc) => true;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 33;
		public static double GetDigestionTickRate(NPC npc, PreyData prey)
		{
			return 1.5;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 0,
				seconds: 15
			);
			return baseAbsorptionRate;
		}
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}
		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(2.75 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				7
			);
		}

		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(2 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				3
			);
		}
		
		public Entity FindClosestTarget()
		{
			Vector2 center = NPC.Center;
			float num = float.MaxValue;
			Entity target = null;
			bool FoundCandy = false;
			foreach (var item in Main.ActiveItems)
			{
				if (item.active && item.type == ModContent.ItemType<DemonCandy>() && item.CurrentCaptor() is null)
				{
					float num3 = Vector2.DistanceSquared(center, item.Center);
					if (num3 < num)
					{
						num = num3;
						target = item;
						FoundCandy = true;
					}
				}
			}
			if (!FoundCandy)
			{
				foreach (var plr in Main.ActivePlayers)
				{
					if (plr.active && !plr.dead && !plr.ghost && plr.CurrentCaptor() is null)
					{
						float num3 = Vector2.DistanceSquared(center, plr.Center);
						if (num3 < num)
						{
							num = num3;
							target = plr;
						}
					}
				}
				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.active && npc.type != NPCID.BurningSphere && npc.CurrentCaptor() is null && npc != NPC && npc.AsFood().DefinedEffectiveSize < 7 && PreyData.GetPreySize(npc) < NPC.AsPred().MaxStomachCapacity - PredNPC.GetCurrentBellyWeight(NPC) && !npc.AsFood().CannotBeEatenDueToShenanigans)
					{
						float num3 = Vector2.DistanceSquared(center, npc.Center);
						if (num3 < num)
						{
							num = num3;
							target = npc;
						}
					}
				}
			}
			return target;
		}
		public override void AI()
		{
			Entity Target = FindClosestTarget();
			NPC.ai[0]++;
			if (NPC.ai[0] >= 100) NPC.ai[0] = 40;
			if (NPC.ai[2] >= 5) NPC.ai[2]++;
			if (NPC.ai[2] >= 30) NPC.ai[2] = 0;
			if (Target is not null && Target.Distance(NPC.Center) < 1000f)
			{
				if (NPC.ai[1] > 0) NPC.ai[1] = -100;
				if (Target is Player)
				{
					NPC.ai[1]--;
					if (NPC.ai[1] <= (Main.expertMode ? -240 : -290))
					{
						NPC.frame.X = 46;
					}
					else if (NPC.ai[1] > -90)
					{
						NPC.frame.X = 92;
					}
					else if (NPC.ai[1] > -100)
					{
						NPC.frame.X = 46;
					}
					else NPC.frame.X = 0;
					if (NPC.ai[1] <= (Main.expertMode ? -250 : -300) && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.ai[1] = -70;
						Vector2 Velocity = Target.DirectionFrom(NPC.Center - new Vector2(19, 55));
						Velocity *= 9f;
						var projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center - new Vector2(19, 55), Velocity, ProjectileID.Fireball, NPC.damage, 0);
					}
				}
				else
				{
					NPC.frame.X = 0;
				}
				int TargetDirection = 0;
				if (Target.Center.X >= NPC.Center.X) TargetDirection = 1;
				else TargetDirection = -1;
				NPC.velocity.X = Math.Clamp(NPC.velocity.X + (float)(0.07f * TargetDirection), -4.5f, 4.5f);
				NPC.velocity.Y = Math.Clamp(NPC.velocity.Y + (0.05f * (1 + GetVisualBellySize(NPC) / 10f)), -7f, 4f);
				if (NPC.ai[0] >= 40)
				if (NPC.Center.Y > Target.Center.Y + 45 && NPC.ai[0] >= 40 - GetVisualBellySize(NPC) * 3)
				{
					NPC.ai[0] = 0;
					NPC.ai[2] = 5;
					NPC.velocity.Y = Math.Clamp(NPC.velocity.Y - 4f, -5f, 4f);

				}
				Rectangle MouthHitbox = new Rectangle((int)NPC.Center.X - 30, (int)NPC.Center.Y - 85, 60, 60);
				if (MouthHitbox.Intersects(Target.Hitbox))
				{
					PredNPC.Swallow(NPC, Target);
				}
			}
			else
			{
				if (NPC.ai[1] <= 0) NPC.ai[1] = NPC.Center.Y;
				NPC.frame.X = 0;
				NPC.velocity.X = Math.Clamp(NPC.velocity.X * 0.9f, -4.5f, 4.5f);
				NPC.velocity.Y = Math.Clamp(NPC.velocity.Y + (0.05f * (1 + GetVisualBellySize(NPC) / 10f)), -7f, 4f);
				if (NPC.Center.Y > NPC.ai[1] && NPC.ai[0] >= 40 - GetVisualBellySize(NPC) * 3)
				{
					NPC.ai[0] = 0;
					NPC.ai[2] = 5;
					NPC.velocity.Y = Math.Clamp(NPC.velocity.Y - 4f, -5f, 4f);
				}
			}
			
		}
		public override void FindFrame(int frameHeight)
		{
			int framerate = 5;
			NPC.frameCounter++;
			if (NPC.frameCounter >= framerate)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += 98;
				if (NPC.frame.Y >= 196)
				{
					NPC.frame.Y = 0;
				}
				if (NPC.ai[2] >= 1)
				{
					int Frame = (int)Math.Floor(NPC.ai[2] / 5);
					NPC.frame.Y = 98 + (98 * Frame);
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Rectangle sourceRectWings = new Rectangle(0, NPC.frame.Y, 218, 98);
			Texture2D spriteWings = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Underworld/HellHarpy/HellHarpyWings").Value;
			spriteBatch.Draw(spriteWings, NPC.Center - Main.screenPosition - new Vector2(109, 86), sourceRectWings, drawColor, NPC.rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			Rectangle sourceRectBody = new Rectangle(82 * GetVisualBellySize(NPC), 100 * GetVisualWeightStage(NPC), 82, 100);
			Texture2D spriteBody = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Underworld/HellHarpy/HellHarpyBody").Value;
			spriteBatch.Draw(spriteBody, NPC.Center - Main.screenPosition - new Vector2(41, 50), sourceRectBody, drawColor, NPC.rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			Rectangle sourceRectHead = new Rectangle(0, NPC.frame.X, 38, 46);
			Texture2D spriteHead = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Underworld/HellHarpy/HellHarpyHead").Value;
			spriteBatch.Draw(spriteHead, NPC.Center - Main.screenPosition - new Vector2(19, 88), sourceRectHead, drawColor, NPC.rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			return false;
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
			{

			}
		}
	}
}
