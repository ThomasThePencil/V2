using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.Stylist;
using V2.Sounds.Vore;

namespace V2.Projectiles.Vanilla.Summons.Pets
{
	public static partial class FairyPrincessStuff
	{
		public static FairyPrincess AsFairyPrincess(this Projectile projectile)
		{
			if (!projectile.TryGetGlobalProjectile(out FairyPrincess predFairyPrincess))
				throw new Exception("this instance of the Heiress of Light can't be pred or prey");

			return predFairyPrincess;
		}
		public static int MaxHealth => 3500;
		public static double Size => 0.70;
		public static double MaxStomachCapacity => 30.0;
		public static double DigestDamage => 40.0;
		public static double DigestRate => 2.0;
		public static double AbsorbRate => 1.0 / (double)V2Utils.SensibleTime(
			minutes: 1,
			seconds: 30
		);
	}

	public partial class FairyPrincess : GlobalProjectile
	{
		public bool WaitingForChurnedOwner { get; set; }
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.FairyQueenPet;

		public override void SetDefaults(Projectile projectile)
		{
			projectile.Name = Language.GetTextValue("Mods.V2.Projectiles.DisplayName.Vanilla.Summons.Pets.FairyPrincess");

			projectile.AsV2Proj().Gender = EntityGender.Female;
			projectile.AsV2Proj().NewAIMethod = MiniCandyFairyAI;

			projectile.AsPred().MaxStomachCapacity = FairyPrincessStuff.MaxStomachCapacity;
			projectile.AsPred().BaseStomachacheMeterCapacity = -1;
			projectile.AsPred().CanSwallowBosses = true;

			projectile.AsFood().DefinedSize = FairyPrincessStuff.Size;
			projectile.AsFood().MaxHealth = FairyPrincessStuff.MaxHealth;
			projectile.AsFood().Health = FairyPrincessStuff.MaxHealth;

			projectile.AsPred().MouthSoundRawOffset = new Vector2(2f, -14f);
			projectile.AsPred().SmallGulps = Gulps.Short;
			projectile.AsPred().SmallGulpThreshold = 0.1;
			projectile.AsPred().BigGulps = Gulps.Standard;
			projectile.AsPred().CanBeForceFed = CanMiniCandyFairyBeForceFed;
			projectile.AsPred().OnForceFed = OnMiniCandyFairyForceFed;
			projectile.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(12.5);

			projectile.AsPred().DigestionType = EntityDigestionType.Acidic;
			projectile.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			projectile.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			projectile.AsPred().OnDigestionKill = OnDigestionKill;
			projectile.AsPred().SmallBurps = Burps.Humanoid.Small;
			projectile.AsPred().StandardBurps = Burps.Humanoid.Standard;
			projectile.AsPred().BurpPitchOffset = 0.285f;
			projectile.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			projectile.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			projectile.AsPred().GetVisualBellySize = GetVisualBellySize;
			projectile.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			projectile.AsFood().OnKilledByDigestion += PreyProjectile.OnKilledByDigestion_GrantLivePreyGoal;
			projectile.AsFood().OnKilledByDigestion += OnKilledByDigestion;
		}

		public static bool CanMiniCandyFairyBeForceFed(Projectile projectile) => true;

		public static void OnMiniCandyFairyForceFed(Projectile projectile, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(Projectile projectile, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.1",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.2",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.3",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.4",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificProjectile.Summons.Pets.MiniCandyFairy.Hardcore");
			}
		}

		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey)
		{
			double digestDamage = FairyPrincessStuff.DigestDamage;
			if (Main.dayTime)
				digestDamage *= 2.0;
			else if (Main.bloodMoon)
				digestDamage *= 1.5;

			return digestDamage;
		}
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey)
		{
			double digestRate = FairyPrincessStuff.DigestRate;
			if (Main.dayTime)
				digestRate *= 2.0;
			else if (Main.bloodMoon)
				digestRate *= 1.5;

			Player ownerPlayer = Main.player[projectile.owner];
			if (!ownerPlayer.dead && ownerPlayer.sleeping.FullyFallenAsleep)
			{
				digestRate *= 1.25f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
					digestRate *= (float)Main.dayRate;
			}

			return digestRate;
		}

		public static void OnDigestionKill(Projectile projectile, PreyData digestedPrey)
		{
			int dustCount = 4 + (int)Math.Floor(10.5 * Math.Sqrt(digestedPrey.WeightLeftToDigest));
			int spawnedDustCount = 0;
			for (int i = 0; i < dustCount; i++)
			{
				Dust belchedUpDust = Dust.NewDustPerfect(
					projectile.TrueCenter() + PredProjectile.MouthSoundOffset(projectile),
					Main.rand.NextFromCollection(new List<int> {
						DustID.GreenTorch,
						DustID.GreenTorch,
						DustID.PinkTorch,
						DustID.BlueTorch,
						DustID.YellowTorch,
					}),
					new Vector2(projectile.direction * 2.5f, -0.5f),
					50,
					default,
					Main.rand.NextFloat(2.25f, 2.75f)
				);
				belchedUpDust.position += new Vector2(Main.rand.NextFloat(2f), 0).RotatedByRandom(MathHelper.ToRadians(360));
				belchedUpDust.velocity *= Main.rand.NextFloat(0.85f, 1.15f);
				belchedUpDust.velocity = belchedUpDust.velocity.RotatedByRandom(MathHelper.ToRadians(18));
				belchedUpDust.noGravity = true;
				spawnedDustCount++;
			}

			if (!ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
				return;

			string debugText = "Trying to spawn dusts for the Heiress' post-digestion-kill belch...\n";
			if (spawnedDustCount == dustCount)
				debugText += "All " + dustCount + " dusts were successfully spawned!";
			else
				debugText += "ERROR: Only " + spawnedDustCount + " out of " + dustCount + " dusts were spawned.";
			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(debugText, Color.PaleVioletRed);
			else if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugText), Color.PaleVioletRed);
		}

		public static double GetPreyAbsorptionRate(Projectile projectile)
		{
			double absorbRate = FairyPrincessStuff.AbsorbRate;
			if (Main.dayTime)
				absorbRate *= 3.0;

			Player ownerPlayer = Main.player[projectile.owner];
			if (!ownerPlayer.dead && ownerPlayer.sleeping.FullyFallenAsleep)
			{
				absorbRate *= 1.25f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
					absorbRate *= (float)Main.dayRate;
			}
			return absorbRate;
		}

		public static int GetVisualBellySize(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(4.0 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				8
			);
		}

		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(1.4 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				0
			);
		}

		public static int GetEmpressDigestionStage(Projectile projectile)
		{
			if (PredProjectile.GetStomachTracker(projectile) is null)
				return 0;

			PreyData candyFairy = PredProjectile.GetStomachTracker(projectile).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss);
			if (candyFairy is null || candyFairy.WeightLeftToDigest < 6.0)
				return 0;
			else
			{
				if (!candyFairy.NoHealth)
					return 1;
				else
				{
					if (candyFairy.WeightLeftToDigest > 34.0)
						return 1;
					else if (candyFairy.WeightLeftToDigest > 28.0)
						return 2;
					else if (candyFairy.WeightLeftToDigest > 16.0)
						return 3;
					else if (candyFairy.WeightLeftToDigest > 6.0)
						return 4;
					else
						return 0;
				}
			}
		}

		public static void OnKilledByDigestion(Projectile projectile, Entity pred)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			ownerPlayer.ClearBuff(BuffID.FairyQueenPet);
		}

		public override void OnKill(Projectile projectile, int timeLeft)
		{
			int particleCount = 10;
			float rotationPerParticle = MathHelper.ToRadians(360f / (float)particleCount);
			for (int i = 0; i < particleCount; i++)
			{
				int num974 = Dust.NewDust(
					projectile.position,
					projectile.width,
					projectile.height,
					Main.rand.NextFromCollection(new List<int> {
								DustID.PinkTorch,
								DustID.PinkTorch,
								DustID.BlueTorch,
								DustID.YellowTorch,
					}),
					0f,
					0f,
					50,
					default,
					2f
				);
				Main.dust[num974].noGravity = true;
			}
		}

		public override bool PreDraw(Projectile projectile, ref Color lightColor)
		{
			if (projectile.CurrentCaptor() is not null)
				return false;

			if (projectile.AsV2Proj().CustomSprite is not null)
				return true;

			SpriteEffects spriteEffects = projectile.direction switch
			{
				-1 => SpriteEffects.FlipHorizontally,
				_ => SpriteEffects.None,
			};
			string exactTextureToUse = "V2/Projectiles/Vanilla/Summons/Pets/FairyPrincess";
			int weightStage = projectile.AsPred().GetVisualWeightStage.Invoke(projectile);
			string weightString = "_Weight" + (weightStage == 0 ? "Base" : weightStage);
			exactTextureToUse += weightString;
			int bellySize = projectile.AsPred().GetVisualBellySize.Invoke(projectile);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			Texture2D texture = ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad).Value;
			Rectangle sourceRect = new Rectangle(0, projectile.frame * 74, 64, 74);

			Main.EntitySpriteDraw(
				texture,
				projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
				sourceRect,
				lightColor,
				projectile.rotation,
				new Vector2(28f, 28f),
				1,
				spriteEffects,
				0f
			);
			return false;
		}
	}
}
