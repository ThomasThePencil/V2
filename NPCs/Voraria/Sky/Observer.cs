using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria;
using V2.Items.Voraria.Charms;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;
using V2.PlayerHandling.PredPlayerGoals.Skilled;

namespace V2.NPCs.Voraria.Sky
{
	public class ObserverRed : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.NeedsExpertScaling[NPC.type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "V2/NPCs/Voraria/Sky/ObserverRed",
				Position = new Vector2(0, 8),
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 100;
			NPC.height = 100;
			NPC.aiStyle = -1;
			NPC.damage = 0;
			NPC.defense = 42;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath63;
			NPC.value = 2500f;
			NPC.noGravity = true;
			NPC.knockBackResist = 0.3f;
			NPC.AsFood().DefinedBaseSize = 10.0;
			NPC.AsFood().WellFedPower = -0.1;
			NPC.AsFood().OnDigestedBy += OnDigestedBy_GrantObserverGoal;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.Sky.Observer"),
			});
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (!spawnInfo.Sky)
				return 0f;

			return 0.03f;
		}
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}
		public void boing(NPC Observer, Entity Victim)
		{
			if (Observer != null && Victim != null)
			{
				float Velocity1 = Observer.velocity.Length();
				float Velocity2 = Victim.velocity.Length();
				float CombinedSpeed = Math.Min((Velocity1 + Velocity2) * 2, 3f);
				Vector2 Direction = Observer.Center.DirectionTo(Victim.Center);
				Observer.velocity = Direction * -CombinedSpeed;
				Victim.velocity = Direction * CombinedSpeed * 1.5f;
			}
		}
		public override void AI()
		{
			NPC.TargetClosest(false);
			if (NPC.collideX)
				NPC.velocity.X *= -1;
			if (NPC.collideY)
				NPC.velocity.Y *= -1;
			if (NPC.HasValidTarget)
			{
				Player? target = Main.player[NPC.target];
				if (target != null)
				{
					Vector2 direction = NPC.Center.DirectionTo(target.Center);
					direction.Normalize();
					NPC.velocity += direction / 50f;

				}
			}
			foreach (var npc in Main.ActiveNPCs)
			{
				if (NPC == npc) continue;
				if (npc.CurrentCaptor() is not null) continue;
				if (npc.AsV2NPC().IsTileEntity) continue;
				if (NPC.Hitbox.Intersects(npc.Hitbox))
				{
					boing(NPC, npc);
				}
			}
			foreach (var plr in Main.ActivePlayers)
			{
				if (plr.CurrentCaptor() is not null) continue;
				if (NPC.Hitbox.Intersects(plr.Hitbox))
				{
					boing(NPC, plr);
				}
			}
		}
		public override void PostAI()
		{
			NPC.rotation = Math.Clamp(NPC.velocity.X * 0.035f, -0.5f, 0.5f);
		}
		public override void FindFrame(int frameHeight)
		{
			int framerate = 5;
			NPC.frameCounter++;
			if (NPC.frameCounter >= framerate)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += 114;
				if (NPC.frame.Y >= 342)
				{
					NPC.frame.Y = 0;
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Rectangle sourceRect = new Rectangle(0, NPC.frame.Y, 100, 114);
			Texture2D sprite = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Sky/ObserverBody").Value;
			spriteBatch.Draw(sprite, NPC.position - Main.screenPosition, sourceRect, drawColor, NPC.rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			Rectangle sourceRect2 = new Rectangle(0, 0, 20, 20);
			Texture2D sprite2 = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Sky/ObserverEye").Value;
			Vector2 EyeDirection = Vector2.Zero;
			if (NPC.HasValidTarget)
			{
				EyeDirection = NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 20;
			}
			spriteBatch.Draw(sprite2, NPC.Center - Main.screenPosition + EyeDirection, sourceRect2, drawColor, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 0f);
			return false;
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
			{
				int Gore1 = Mod.Find<ModGore>("Gore_Observer").Type;
				int Gore2 = Mod.Find<ModGore>("Gore_ObserverRed").Type;
				for (int i = 0; i < 10; i++)
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-60, 61) / 10f, Main.rand.Next(-70, 61) / 10f), Gore1, 1f);
				}
				Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(0, 30), new Vector2(0, 1.5f), Gore2);
			}
		}
		public static void OnDigestedBy_GrantObserverGoal(NPC npc, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<EatObserver>().TrySetCompletion(predPlayer);
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(
				new V2CommonDropRules.DifficultyScalingDrop(
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					),
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					),
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					)
				)
			);
		}
	}

	public class ObserverGreen : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.NeedsExpertScaling[NPC.type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "V2/NPCs/Voraria/Sky/ObserverGreen",
				Position = new Vector2(0, 8),
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 100;
			NPC.height = 100;
			NPC.aiStyle = -1;
			NPC.damage = 0;
			NPC.defense = 42;
			NPC.lifeMax = 225;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath63;
			NPC.value = 2500f;
			NPC.noGravity = true;
			NPC.knockBackResist = 0.2f;
			NPC.AsFood().DefinedBaseSize = 10.0;
			NPC.AsFood().WellFedPower = -0.1;
			NPC.AsFood().OnDigestedBy += ObserverRed.OnDigestedBy_GrantObserverGoal;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.Sky.Observer"),
			});
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (!spawnInfo.Sky)
				return 0f;

			return 0.03f;
		}
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}
		public void boing(NPC Observer, Entity Victim)
		{
			if (Observer != null && Victim != null)
			{
				float Velocity1 = Observer.velocity.Length();
				float Velocity2 = Victim.velocity.Length();
				float CombinedSpeed = Math.Min((Velocity1 + Velocity2) * 2, 3f);
				Vector2 Direction = Observer.Center.DirectionTo(Victim.Center);
				Observer.velocity = Direction * -CombinedSpeed;
				Victim.velocity = Direction * CombinedSpeed * 1.5f;
			}
		}
		public override void AI()
		{
			NPC.TargetClosest(false);
			if (NPC.collideX)
				NPC.velocity.X *= -1;
			if (NPC.collideY)
				NPC.velocity.Y *= -1;
			if (NPC.HasValidTarget)
			{
				Player? target = Main.player[NPC.target];
				if (target != null)
				{
					Vector2 direction = NPC.Center.DirectionTo(target.Center);
					direction.Normalize();
					NPC.velocity += direction / 40f;

				}
			}
			foreach (var npc in Main.ActiveNPCs)
			{
				if (NPC == npc) continue;
				if (npc.CurrentCaptor() is not null) continue;
				if (npc.AsV2NPC().IsTileEntity) continue;
				if (NPC.Hitbox.Intersects(npc.Hitbox))
				{
					boing(NPC, npc);
				}
			}
			foreach (var plr in Main.ActivePlayers)
			{
				if (plr.CurrentCaptor() is not null) continue;
				if (NPC.Hitbox.Intersects(plr.Hitbox))
				{
					boing(NPC, plr);
				}
			}
		}
		public override void PostAI()
		{
			NPC.rotation = Math.Clamp(NPC.velocity.X * 0.035f, -0.5f, 0.5f);
		}
		public override void FindFrame(int frameHeight)
		{
			int framerate = 5;
			NPC.frameCounter++;
			if (NPC.frameCounter >= framerate)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += 114;
				if (NPC.frame.Y >= 342)
				{
					NPC.frame.Y = 0;
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Rectangle sourceRect = new Rectangle(100, NPC.frame.Y, 100, 114);
			Texture2D sprite = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Sky/ObserverBody").Value;
			spriteBatch.Draw(sprite, NPC.position - Main.screenPosition, sourceRect, drawColor, NPC.rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			Rectangle sourceRect2 = new Rectangle(20, 0, 20, 20);
			Texture2D sprite2 = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Sky/ObserverEye").Value;
			Vector2 EyeDirection = Vector2.Zero;
			if (NPC.HasValidTarget)
			{
				EyeDirection = NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 20;
			}
			spriteBatch.Draw(sprite2, NPC.Center - Main.screenPosition + EyeDirection, sourceRect2, drawColor, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 0f);
			return false;
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
			{
				int Gore1 = Mod.Find<ModGore>("Gore_Observer").Type;
				int Gore2 = Mod.Find<ModGore>("Gore_ObserverGreen").Type;
				for (int i = 0; i < 10; i++)
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-60, 61) / 10f, Main.rand.Next(-70, 61) / 10f), Gore1, 1f);
				}
				Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(0, 30), new Vector2(0, 1.5f), Gore2);
			}
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(
				new V2CommonDropRules.DifficultyScalingDrop(
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					),
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					),
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					)
				)
			);
		}
	}

	public class ObserverPurple : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.NeedsExpertScaling[NPC.type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "V2/NPCs/Voraria/Sky/ObserverPurple",
				Position = new Vector2(0, 8),
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 100;
			NPC.height = 100;
			NPC.aiStyle = -1;
			NPC.damage = 0;
			NPC.defense = 42;
			NPC.lifeMax = 275;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath63;
			NPC.value = 2500f;
			NPC.noGravity = true;
			NPC.knockBackResist = 0.4f;
			NPC.AsFood().DefinedBaseSize = 10.0;
			NPC.AsFood().WellFedPower = -0.1;
			NPC.AsFood().OnDigestedBy += ObserverRed.OnDigestedBy_GrantObserverGoal;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.Sky.Observer"),
			});
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (!spawnInfo.Sky)
				return 0f;

			return 0.03f;
		}
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}
		public void boing(NPC Observer, Entity Victim)
		{
			if (Observer != null && Victim != null)
			{
				float Velocity1 = Observer.velocity.Length();
				float Velocity2 = Victim.velocity.Length();
				float CombinedSpeed = Math.Min((Velocity1 + Velocity2) * 2, 3f);
				Vector2 Direction = Observer.Center.DirectionTo(Victim.Center);
				Observer.velocity = Direction * -CombinedSpeed;
				Victim.velocity = Direction * CombinedSpeed * 1.5f;
			}
		}
		public override void AI()
		{
			NPC.TargetClosest(false);
			if (NPC.collideX)
				NPC.velocity.X *= -1;
			if (NPC.collideY)
				NPC.velocity.Y *= -1;
			if (NPC.HasValidTarget)
			{
				Player? target = Main.player[NPC.target];
				if (target != null)
				{
					Vector2 direction = NPC.Center.DirectionTo(target.Center);
					direction.Normalize();
					NPC.velocity += direction / 60f;

				}
			}
			foreach (var npc in Main.ActiveNPCs)
			{
				if (NPC == npc) continue;
				if (npc.CurrentCaptor() is not null) continue;
				if (npc.AsV2NPC().IsTileEntity) continue;
				if (NPC.Hitbox.Intersects(npc.Hitbox))
				{
					boing(NPC, npc);
				}
			}
			foreach (var plr in Main.ActivePlayers)
			{
				if (plr.CurrentCaptor() is not null) continue;
				if (NPC.Hitbox.Intersects(plr.Hitbox))
				{
					boing(NPC, plr);
				}
			}
		}
		public override void PostAI()
		{
			NPC.rotation = Math.Clamp(NPC.velocity.X * 0.035f, -0.5f, 0.5f);
		}
		public override void FindFrame(int frameHeight)
		{
			int framerate = 5;
			NPC.frameCounter++;
			if (NPC.frameCounter >= framerate)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += 114;
				if (NPC.frame.Y >= 342)
				{
					NPC.frame.Y = 0;
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Rectangle sourceRect = new Rectangle(200, NPC.frame.Y, 100, 114);
			Texture2D sprite = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Sky/ObserverBody").Value;
			spriteBatch.Draw(sprite, NPC.position - Main.screenPosition, sourceRect, drawColor, NPC.rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			Rectangle sourceRect2 = new Rectangle(40, 0, 20, 20);
			Texture2D sprite2 = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Sky/ObserverEye").Value;
			Vector2 EyeDirection = Vector2.Zero;
			if (NPC.HasValidTarget)
			{
				EyeDirection = NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 20;
			}
			spriteBatch.Draw(sprite2, NPC.Center - Main.screenPosition + EyeDirection, sourceRect2, drawColor, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 0f);
			return false;
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
			{
				int Gore1 = Mod.Find<ModGore>("Gore_Observer").Type;
				int Gore2 = Mod.Find<ModGore>("Gore_ObserverPurple").Type;
				for (int i = 0; i < 10; i++)
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-60, 61) / 10f, Main.rand.Next(-70, 61) / 10f), Gore1, 1f);
				}
				Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(0, 30), new Vector2(0, 1.5f), Gore2);
			}
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(
				new V2CommonDropRules.DifficultyScalingDrop(
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					),
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					),
					new CommonDrop(
						itemId: ModContent.ItemType<ObserverPupil>(),
						chanceNumerator: 1,
						chanceDenominator: 1
					)
				)
			);
		}
	}
}
