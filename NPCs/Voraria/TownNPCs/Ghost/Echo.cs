using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Voraria.TownNPCs.Ghost
{
	public static class GhostStuff
	{
		public static GhostProfile GhostProfile = new GhostProfile();
	}

	public class GhostProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public GhostProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Voraria/TownNPCs/Ghost/Echo_Weight0";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => "Echo";

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;
			return ModContent.Request<Texture2D>("V2/NPCs/Voraria/TownNPCs/Ghost/Echo_Weight0");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("V2/NPCs/Voraria/TownNPCs/Ghost/Echo_Head");
	}

	[AutoloadHead]
	public class Echo : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/NPCs/Voraria/TownNPCs/Ghost/Echo_Weight0";
		public override string HeadTexture => "V2/NPCs/Voraria/TownNPCs/Ghost/Echo_Head";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.IsTownPet[NPC.type] = true;
			NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;
			NPCID.Sets.IsPetSmallForPetting[NPC.type] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f,
				Direction = -1
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.TownNPCs.Ghost"),
			});
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 22;
			NPC.height = 48;
			NPC.aiStyle = -1;
			NPC.lifeMax = 500;
			NPC.damage = 35;
			NPC.defense = 38;
			NPC.knockBackResist = 0.5f;
			NPC.housingCategory = 1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.dontTakeDamageFromHostiles = true;

			NPC.AsV2NPC().GetNewDialogue = GetGhostChat;

			NPC.AsFood().DefinedBaseSize = 1.3;
			NPC.AsPred().WeightGainRatio = 0.07;
			NPC.AsPred().MaxStomachCapacity = 2.8;
			NPC.AsPred().BaseStomachacheMeterCapacity = 400.0;

			NPC.AsPred().SmallGulps = Gulps.Short;
			NPC.AsPred().SmallGulpThreshold = 0.45;
			NPC.AsPred().BigGulps = Gulps.Standard;
			NPC.AsPred().CanBeForceFed = CanGhostBeForceFed;
			NPC.AsPred().OnForceFed = OnGhostForceFed;

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;
			NPC.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			NPC.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			NPC.AsPred().OnDigestionKill = OnDigestionKill;
			NPC.AsPred().MouthSoundRawOffset = NPC.TrueCenter() + new Vector2(NPC.direction * 8f, -14f);
			NPC.AsPred().SmallBurps = Burps.Humanoid.Small;
			NPC.AsPred().SmallBurpThreshold = 0.75;
			NPC.AsPred().StandardBurps = Burps.Humanoid.Standard;
			NPC.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			NPC.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			NPC.AsPred().GetVisualBellySize = GetVisualBellySize;

			NPC.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;
		}
		public override void OnSpawn(IEntitySource source)
		{
			NPC.velocity.Y = -2f;
		}
		public override void ModifyTypeName(ref string typeName) => typeName = "Ghost";

		public static List<(TargetType, int, TargetPriorityLevel)> Diet
		{
			get
			{
				List<(TargetType, int, TargetPriorityLevel)> diet = [
					// slimes
					(TargetType.NPC, NPCID.BigCrimslime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.LittleCrimslime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.JungleSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.YellowSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.RedSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PurpleSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BlackSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BabySlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Pinky, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.GreenSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Slimer2, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Slimeling, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BlueSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.MotherSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.LavaSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.DungeonSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.CorruptSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Slimer, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Gastropod, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.IlluminantSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.ToxicSludge, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.IceSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Crimslime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SpikedIceSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SpikedJungleSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.UmbrellaSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.RainbowSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SlimeMasked, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.BunnySlimed, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SlimeRibbonWhite, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SlimeRibbonYellow, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SlimeRibbonGreen, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SlimeRibbonRed, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SlimeSpiked, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.SandSlime, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.QueenSlimeMinionBlue, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.QueenSlimeMinionPink, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.QueenSlimeMinionPurple, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.ShimmerSlime, TargetPriorityLevel.Neutral),

					//there can only be one ghost
					(TargetType.NPC, NPCID.Ghost, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Wraith, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.Poltergeist, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.DungeonSpirit, TargetPriorityLevel.Neutral),
					(TargetType.NPC, NPCID.PirateGhost, TargetPriorityLevel.Neutral),
				];
				return diet;
			}
		}

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			if (digestedPrey.Type == PreyType.Item)
				if (digestedPrey.Instance != null)
				{
					Item itemPrey = digestedPrey.Instance as Item;
					if (itemPrey.dye > 0)
						npc.AsPred().LastSwallowedDye = itemPrey.dye;
				}
		}

		public override ITownNPCProfile TownNPCProfile() => GhostStuff.GhostProfile;

		public static List<string> GetGhostChat(NPC npc, Player player)
		{
			List<string> GhostChatPool = new List<string>();
			V2Utils.FigureOutWhatTimeItIs(
				out bool pastMorning,
				out int hour,
				out int minute,
				out int second,
				out MealTime mealTime
			);
			double totalBellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			bool playerIsFood = player.IsFoodFor(npc, out bool playerWasAlreadyDigested);
			bool isFoodForPlayer = npc.IsFoodFor(player);
			if (playerIsFood && !playerWasAlreadyDigested)
			{
				{
					GhostChatPool.AddRange(new List<string>
					{
						";>",
						";)",
						";D",
					});
				}
			}
			else
			{
				{
					{
						GhostChatPool.AddRange(new List<string>
						{
							":>",
							":?",
							":D",
							":)",
							"C:",
							":P",
						});
					}
				}
			}
			return GhostChatPool;
		}
		public static bool CanGhostBeForceFed(NPC npc) => true;

		public static void OnGhostForceFed(NPC npc, Player player)
		{
			// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
			PredNPC.SwallowWithTextIfApplicable(
				npc,
				player,
				"[c/7F7F7F:<Your body. Echo's mouth. Make it happen... And so you do.>]\n"
			  + Main.rand.NextFromCollection(new List<string>
				{
					"0_0",
				})
			);
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			/*deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.5",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Enigma.Hardcore");
			}*/
		}
		public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => 1.8;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 6;

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1
			);
			baseAbsorptionRate *= 1 + (GetVisualWeightStage(npc) / 10.0);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				6
			);
		}
		public static int GetVisualWeightStage(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(3 * Math.Sqrt(npc.AsPred().ExtraWeight)),
				7
			);
		}

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle(
				(int)NPC.Center.X - 22,
				(int)NPC.Center.Y - 50,
				44,
				70
			);
		}
		public override void AI()
		{
			ModdedTownNPCAI.AI_007_TownEntities(NPC);
		}
		public override void PostAI()
		{
			Lighting.AddLight(NPC.Center, Color.SkyBlue.ToVector3());
			int idleFrame = (int)(Main.GlobalTimeWrappedHourly * 5) % 4;
			if (!Main.gamePaused)
				NPC.frame.Y = idleFrame * NPC.frame.Height;
			switch (GetVisualBellySize(NPC))
			{
				case 0 or 1:
					NPCID.Sets.PlayerDistanceWhilePetting[NPC.type] = 22; break;
				case 2:
					NPCID.Sets.PlayerDistanceWhilePetting[NPC.type] = 24; break;
				case 3:
					NPCID.Sets.PlayerDistanceWhilePetting[NPC.type] = 28; break;
				case 4:
					NPCID.Sets.PlayerDistanceWhilePetting[NPC.type] = 34; break;
				case 5:
					NPCID.Sets.PlayerDistanceWhilePetting[NPC.type] = 42; break;
				case 6:
					NPCID.Sets.PlayerDistanceWhilePetting[NPC.type] = 54; break;

			}

			Rectangle SwallowHitbox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
			Entity target = null;

			foreach (var prey in Main.ActiveItems)
			{
				if (SwallowHitbox.Intersects(prey.Hitbox) && prey.dye > 0)
					target = prey;
			}

			if (target != null)
			{
				PredNPC.Swallow(NPC, target);
			}
			NPC.DoContactGulpage(Diet);
		}

		public void ExtraMainSpriteSize(int weight, out Vector2 SpriteSize, out Vector2 SpriteOffset)
		{
			SpriteSize = new Vector2(44, 72);
			SpriteOffset = Vector2.Zero;
			switch (weight)
			{
				case 0 or 1 or 2 or 3:
					SpriteSize = new Vector2(44, 72);
					SpriteOffset = Vector2.Zero;
					break;
				case 4:
					SpriteSize = new Vector2(50, 72);
					SpriteOffset = Vector2.Zero;
					break;
				case 5:
					SpriteSize = new Vector2(60, 72);
					SpriteOffset = new Vector2(4, 0);
					break;
				case 6:
					SpriteSize = new Vector2(78, 72);
					SpriteOffset = new Vector2(8, 0);
					break;
				case 7:
					SpriteSize = new Vector2(96, 72);
					SpriteOffset = new Vector2(12, 0);
					break;
			}
		}
		public void ExtraTumSpriteSize(int weight, out Vector2 SpriteSize, out Vector2 SpriteOffsetRight, out Vector2 SpriteOffsetLeft)
		{
			SpriteSize = new Vector2(54, 34);
			SpriteOffsetRight = new Vector2(14, 28);
			SpriteOffsetLeft = new Vector2(-28, 28);
			switch (weight)
			{
				case 0 or 1:
					SpriteSize = new Vector2(54, 34);
					SpriteOffsetRight = new Vector2(14, 28);
					SpriteOffsetLeft = new Vector2(-28, 28);
					NPC.width = 22;
					break;
				case 2 or 3:
					SpriteSize = new Vector2(60, 46);
					SpriteOffsetRight = new Vector2(10, 16);
					SpriteOffsetLeft = new Vector2(-30, 16);
					NPC.width = 26;
					break;
				case 4:
					SpriteSize = new Vector2(60, 46);
					SpriteOffsetRight = new Vector2(8, 16);
					SpriteOffsetLeft = new Vector2(-22, 16);
					NPC.width = 30;
					break;
				case 5:
					SpriteSize = new Vector2(62, 46);
					SpriteOffsetRight = new Vector2(12, 16);
					SpriteOffsetLeft = new Vector2(-18, 16);
					NPC.width = 34;
					break;
				case 6:
					SpriteSize = new Vector2(64, 46);
					SpriteOffsetRight = new Vector2(20, 16);
					SpriteOffsetLeft = new Vector2(-10, 16);
					NPC.width = 38;
					break;
				case 7:
					SpriteSize = new Vector2(64, 46);
					SpriteOffsetRight = new Vector2(28, 16);
					SpriteOffsetLeft = new Vector2(-2, 16);
					NPC.width = 42;
					break;
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			SpriteEffects val = NPC.direction != -1 ? 0 : (SpriteEffects)1;
			SpriteEffects spriteEffects = val;

			string Folder = "V2/NPCs/Voraria/TownNPCs/Ghost/";
			int weightStage = GetVisualWeightStage(NPC);
			int tumSize = GetVisualBellySize(NPC);
			ExtraMainSpriteSize(weightStage, out var SpriteSize, out var SpriteOffset);
			Rectangle sourceRect = NPC.frame;

			Texture2D spriteMain = ModContent.Request<Texture2D>(Folder + "Echo_Weight" + weightStage).Value;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			GameShaders.Armor.Apply(NPC.AsPred().LastSwallowedDye, NPC, new DrawData(spriteMain, NPC.position - Main.screenPosition + new Vector2(-12 - (int)SpriteOffset.X, -20 - (int)SpriteOffset.Y), sourceRect, new Color(255, 255, 255), NPC.rotation, new Vector2(0, 0), 1f, spriteEffects, 0f));

			spriteBatch.Draw(spriteMain, NPC.position - Main.screenPosition + new Vector2(-12 - (int)SpriteOffset.X, -20 - (int)SpriteOffset.Y), sourceRect, new Color(255, 255, 255), NPC.rotation, new Vector2(0, 0), 1f, spriteEffects, 0f);
			if (tumSize > 0)
			{
				ExtraTumSpriteSize(weightStage, out var SpriteSize2, out var SpriteOffset2R, out var SpriteOffset2L);
				Rectangle sourceRect2 = new Rectangle(0, (int)SpriteSize2.Y * (tumSize - 1), (int)SpriteSize2.X, (int)SpriteSize2.Y);
				Vector2 TumOffset = new Vector2((int)SpriteOffset2R.X, (int)SpriteOffset2R.Y);
				if (NPC.direction == -1) TumOffset = new Vector2((int)SpriteOffset2L.X, (int)SpriteOffset2L.Y);
				TumOffset -= SpriteOffset;
				Texture2D spriteTum = ModContent.Request<Texture2D>(Folder + "EchoTum_Weight" + weightStage).Value;
				GameShaders.Armor.Apply(NPC.AsPred().LastSwallowedDye, NPC, new DrawData(spriteTum, NPC.position - Main.screenPosition + TumOffset, sourceRect2, new Color(255, 255, 255), NPC.rotation, new Vector2(10, 10), 1f, spriteEffects, 0f));
				spriteBatch.Draw(spriteTum, NPC.position - Main.screenPosition + TumOffset, sourceRect2, new Color(255, 255, 255), NPC.rotation, new Vector2(10, 10), 1f, spriteEffects, 0f);
			}
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			return false;
		}
		public override void SaveData(TagCompound tag)
		{
			tag["LastDye"] = NPC.AsPred().LastSwallowedDye;
		}
		public override void LoadData(TagCompound tag)
		{
			NPC.AsPred().LastSwallowedDye = tag.GetInt("LastDye");
		}
	}
}
