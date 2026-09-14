using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Sets;
using V2.NPCs.Voraria.Mushroom;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.NPCs.Voraria.Underworld.HellHarpy;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Projectiles;
using V2.Projectiles.Voraria.Weapons.Summon.ShroomFairy;
using V2.Tiles.Vanilla.Paintings;
using V2.Tiles.Voraria.Paintings;

namespace V2.NPCs.Voraria.Hallow
{

	public static class GumdropSlimeStuff
	{
		public static GlobalGumdropSlime AsGumdropSlime(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out GlobalGumdropSlime GumdropSlime))
				throw new Exception("this instance of a Gumdrop Slime, somehow, against all odds, doesn't exist");

			return GumdropSlime;
		}
	}

	public partial class GlobalGumdropSlime : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<GumdropSlime>();
		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().NewAIMethod = V2GumdropSlimeAI;

			npc.AsSlime().JumpSpeed = new Vector2(3.5f, 2.5f);
			npc.AsSlime().JumpDelayBase = V2Utils.SensibleTime(
				seconds: 1,
				frames: 30
			);
			npc.AsSlime().JumpDelayExtra = (
				V2Utils.SensibleTime(
					frames: 0
				),
				V2Utils.SensibleTime(
					seconds: 1,
					frames: 30
				)
			);

			npc.AsSlime().OccasionalHighJumps = true;
			npc.AsSlime().HighJumpFrequency = 2;
			npc.AsSlime().HighJumpXModifier -= 0.2f;
			npc.AsSlime().HighJumpYModifier += 1f;
		}
	}
	public class GumdropSlime : ModNPC
	{
		public Color GumColor = new Color(200, 0, 0);
		public List<Color> GumColors =>
			[
			new Color(200, 0, 0),
			new Color(200, 200, 0),
			new Color(0, 200, 0),
			new Color(0, 200, 200),
			new Color(0, 0, 200),
			new Color(200, 0, 200),
			new Color(200, 133, 0),
			];

		public static List<(TargetType, int, TargetPriorityLevel)> FeedTo
		{
			get
			{
				List<(TargetType, int, TargetPriorityLevel)> feedto = [
					// Town NPCs
					(TargetType.NPC, NPCID.Guide, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Nurse, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Dryad, TargetPriorityLevel.Neutral),
					(TargetType.NPC, ModContent.NPCType<Lucinda>(), TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Painter, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.ArmsDealer, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Stylist, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Mechanic, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PartyGirl, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Steampunker, TargetPriorityLevel.Neutral),

					// Misc. NPCs
					(TargetType.NPC, NPCID.LostGirl, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Nymph, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Harpy, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.EmpressButterfly, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.HallowBoss, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.TheBride, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.TheGroom, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.ThePossessed, TargetPriorityLevel.Neutral),
					(TargetType.NPC, ModContent.NPCType<OversizedFairy>(), TargetPriorityLevel.Neutral),

					// Certain projectiles
					(TargetType.Projectile, ProjectileID.FairyQueenPet, TargetPriorityLevel.Neutral),
					(TargetType.Projectile, ModContent.ProjectileType<ShroomFairy>(), TargetPriorityLevel.Neutral),
					(TargetType.Projectile, ModContent.ProjectileType<Dryadisque_ProjectileEntity>(), TargetPriorityLevel.Neutral),
					(TargetType.Projectile, ModContent.ProjectileType<DoNotEatTheVileMushroom_ProjectileEntity>(), TargetPriorityLevel.Neutral),
					(TargetType.Projectile, ModContent.ProjectileType<MyFairy_ProjectileEntity>(), TargetPriorityLevel.Neutral),

					// Players, of course
					(TargetType.Player, -1, TargetPriorityLevel.Neutral),
				];
				return feedto;
			}
		}
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.SlimeRibbonWhite];
		}
		public override void SetDefaults()
		{
			NPC.width = 13;
			NPC.height = 13;
			NPC.aiStyle = -1;
			AnimationType = NPCID.SlimeRibbonWhite;
			AIType = NPCID.SlimeRibbonWhite;
			NPC.damage = 8;
			NPC.defense = 11;
			NPC.lifeMax = 333;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 88f;

			NPC.AsV2NPC().Gender = EntityGender.Other;

			NPC.AsFood().CannotBeRegurgitated = true;
			NPC.AsFood().DefinedBaseSize = 0.3;
			NPC.AsPred().MaxStomachCapacity = 0.3;
			NPC.AsFood().WellFedPower = 0.3;
			NPC.AsFood().CalorieMultiplier = 3;

			NPC.AsPred().SmallGulpThreshold = 0.00;
			NPC.AsPred().BigGulps = null;
			NPC.AsPred().CanBeForceFed = CanBlueSlimeBeForceFed;

			NPC.AsPred().DigestionType = EntityDigestionType.Other;
			NPC.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			NPC.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			NPC.AsPred().StandardBurps = null;
			NPC.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			NPC.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			NPC.AsFood().OnDigestedBy += SlimeNPC.OnKilledByDigestion_GrantSlimeMultiPreyGoal;
		}

		public override void OnSpawn(IEntitySource source)
		{
			NPC.AsPred().ExtraWeight = Main.rand.Next(0, 61) / 10d;
			if (NPC.ai[2] == 0)
			{
				NPC slime1 = NPC.NewNPCDirect(
					NPC.GetSource_FromAI(),
					(int)NPC.position.X + Main.rand.Next(-16,17),
					(int)NPC.position.Y + Main.rand.Next(-30, 0),
					ModContent.NPCType<GumdropSlime>(),
					ai2: 1
				);
				NPC slime2 = NPC.NewNPCDirect(
					NPC.GetSource_FromAI(),
					(int)NPC.position.X + Main.rand.Next(-16, 17),
					(int)NPC.position.Y + Main.rand.Next(-30, 0),
					ModContent.NPCType<GumdropSlime>(),
					ai2: 1
				);
			}
			NPC.ai[2] = 0;
			GumColor = Main.rand.Next(GumColors);
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.SpawnTileType is TileID.HallowedGrass or TileID.HallowedIce or TileID.Pearlstone)
				return 0.11f;

			return 0f;
		}

		public override void PostAI()
		{
			NPC.DoContactFeed(FeedTo);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.WriteRGB(GumColor);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			GumColor = reader.ReadRGB();
		}
		public static bool CanBlueSlimeBeForceFed(NPC npc) => true;

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SlimePred.1",
				"Mods.V2.Death.DigestedPlayer.SlimePred.2",
				"Mods.V2.Death.DigestedPlayer.SlimePred.3",
			]);
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 0.65;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey)
		{
			double baseDigestionTickDamage = 4.0;
			baseDigestionTickDamage *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseDigestionTickDamage;
		}
		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 15
			);
			baseAbsorptionRate *= npc.AsFood().DefinedEffectiveSize / npc.AsFood().DefinedBaseSize;
			return baseAbsorptionRate;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			int ColorVal(int A, int B)
			{
				return A * B / 255;
			}
			Color actualColor = new Color(ColorVal(drawColor.R, GumColor.R), ColorVal(drawColor.G, GumColor.G), ColorVal(drawColor.B, GumColor.B), 175);
			Rectangle sourceRect = new Rectangle(0, NPC.frame.Y, 26, 26);
			Texture2D sprite = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Hallow/GumdropSlime").Value;
			spriteBatch.Draw(sprite, NPC.position - Main.screenPosition, sourceRect, actualColor, 0f, new Vector2(0, 0), NPC.scale / 2f, SpriteEffects.None, 0f);
			Texture2D sprite2 = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Hallow/GumdropSlimeLayer").Value;
			spriteBatch.Draw(sprite2, NPC.position - Main.screenPosition, sourceRect, new Color(drawColor.R, drawColor.G, drawColor.B, 60), 0f, new Vector2(0, 0), NPC.scale / 2f, SpriteEffects.None, 0f);
			return false;
		}
	}
}
