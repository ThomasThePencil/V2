using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.Projectiles.Vanilla.Summons.Pets;
using V2.Sounds.Vore;

namespace V2.Projectiles.Voraria.Armor.Familiars
{
	public static class GoldSetPrinceStuff
	{
		public static double SwallowCapacity => 1.5;
		public static double MaxStomachCapacity => 5.0;
		public static double DigestDamage => 35.0;
		public static double DigestRate => 1.2;
		public static double AbsorbRate => 1.0 / (double)V2Utils.SensibleTime(
			minutes: 0,
			seconds: 30
		);

		public static double DigestingRegen => 2.25;
		public static double DigestingDefense => 2.25;
		public static double Size => 2.25;
		public static double MaxHealth => 2.25;
	}

	public partial class GoldSetPrince : ModProjectile
	{
		public bool WaitingForChurnedOwner { get; set; }
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.Projectiles.DisplayName.Voraria.Armor.Familiars.GoldSetPrince");

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;

			Projectile.width = 18;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.hostile = false;

			Projectile.tileCollide = false;

			Projectile.AsV2Proj().Gender = EntityGender.Male;
			Projectile.AsV2Proj().NewAIMethod = GoldMiniPrinceAI;

			Projectile.AsPred().MaxStomachCapacity = GoldSetPrinceStuff.MaxStomachCapacity;
			Projectile.AsPred().BaseStomachacheMeterCapacity = -1;
			Projectile.AsPred().CanSwallowBosses = true;

			Projectile.AsFood().DefinedSize = GoldSetPrinceStuff.Size;
			Projectile.AsFood().MaxHealth = GoldSetPrinceStuff.MaxHealth;
			Projectile.AsFood().Health = GoldSetPrinceStuff.MaxHealth;

			Projectile.AsPred().MouthSoundRawOffset = new Vector2(2f, -14f);
			Projectile.AsPred().SmallGulps = Gulps.Short;
			Projectile.AsPred().SmallGulpThreshold = 0.1;
			Projectile.AsPred().BigGulps = Gulps.Standard;
			Projectile.AsPred().CanBeForceFed = CanGoldMiniPrinceBeForceFed;
			Projectile.AsPred().OnForceFed = OnGoldMiniPrinceForceFed;
			Projectile.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(12.5);

			Projectile.AsPred().DigestionType = EntityDigestionType.Acidic;
			Projectile.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			Projectile.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			Projectile.AsPred().OnDigestionKill = OnDigestionKill;
			Projectile.AsPred().SmallBurps = Burps.Humanoid.Small;
			Projectile.AsPred().StandardBurps = Burps.Humanoid.Standard;
			Projectile.AsPred().BurpPitchOffset = 0.285f;
			Projectile.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			Projectile.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			Projectile.AsPred().GetVisualBellySize = GetVisualBellySize;
			Projectile.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			Projectile.AsFood().OnKilledByDigestion += PreyProjectile.OnKilledByDigestion_GrantLivePreyGoal;
			Projectile.AsFood().OnKilledByDigestion += OnKilledByDigestion;
		}

		public static bool CanGoldMiniPrinceBeForceFed(Projectile projectile) => true;

		public static void OnGoldMiniPrinceForceFed(Projectile projectile, Player player)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(Projectile projectile, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Armor.Familiars.GoldSetPrince.1",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Armor.Familiars.GoldSetPrince.2",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Armor.Familiars.GoldSetPrince.3",
				"Mods.V2.Death.DigestedPlayer.SpecificProjectile.Armor.Familiars.GoldSetPrince.4",
			]);
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificProjectile.Armor.Familiars.GoldSetPrince.Hardcore");
			}
		}

		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey)
		{
			double digestDamage = GoldSetPrinceStuff.DigestDamage;
			if (Main.bloodMoon)
				digestDamage *= 1.5;

			return digestDamage;
		}
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey)
		{
			double digestRate = GoldSetPrinceStuff.DigestRate;
			if (Main.bloodMoon)
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
						DustID.Gold,
						DustID.GoldCoin,
						DustID.GoldCritter,
						DustID.GoldCritter_LessOutline,
						DustID.GoldFlame,
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

			string debugText = "Trying to spawn dusts for Aurifer's post-digestion-kill belch...\n";
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
			double absorbRate = GoldSetPrinceStuff.AbsorbRate;
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
				5
			);
		}

		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(1.4 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				2
			);
		}

		public override void OnKill(int timeLeft)
		{
			int particleCount = 10;
			float rotationPerParticle = MathHelper.ToRadians(360f / (float)particleCount);
			for (int i = 0; i < particleCount; i++)
			{
				int num974 = Dust.NewDust(
					Projectile.position,
					Projectile.width,
					Projectile.height,
					Main.rand.NextFromCollection(new List<int> {
						DustID.Gold,
						DustID.GoldCoin,
						DustID.GoldCritter,
						DustID.GoldCritter_LessOutline,
						DustID.GoldFlame,
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

		public static void OnKilledByDigestion(Projectile projectile, Entity pred)
		{
			
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Projectile.CurrentCaptor() is not null)
				return false;

			if (Projectile.AsV2Proj().CustomSprite is not null)
				return true;

			SpriteEffects spriteEffects = Projectile.direction switch
			{
				-1 => SpriteEffects.FlipHorizontally,
				_ => SpriteEffects.None,
			};
			string exactTextureToUse = "V2/Projectiles/Voraria/Armor/Familiars/GoldSetPrince";

			Texture2D texture = ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad).Value;
			Rectangle sourceRect = (GetVisualWeightStage(Projectile), GetVisualBellySize(Projectile)) switch
			{
				(0, 0) => new Rectangle(0,   0,   18, 40),
				(0, 1) => new Rectangle(20,  0,   18, 40),
				(0, 2) => new Rectangle(40,  0,   20, 40),
				(0, 3) => new Rectangle(62,  0,   24, 40),
				(0, 4) => new Rectangle(88,  0,   28, 42),
				(0, 5) => new Rectangle(118, 0,   34, 52),
				(1, 0) => new Rectangle(0,   54,  18, 40),
				(1, 1) => new Rectangle(20,  54,  18, 40),
				(1, 2) => new Rectangle(40,  54,  20, 40),
				(1, 3) => new Rectangle(62,  54,  24, 40),
				(1, 4) => new Rectangle(88,  54,  28, 42),
				(1, 5) => new Rectangle(118, 54,  34, 52),
				(2, 0) => new Rectangle(0,   108, 20, 40),
				(2, 1) => new Rectangle(22,  108, 20, 40),
				(2, 2) => new Rectangle(44,  108, 22, 40),
				(2, 3) => new Rectangle(68,  108, 26, 40),
				(2, 4) => new Rectangle(96,  108, 30, 42),
				(2, 5) => new Rectangle(128, 108, 34, 52),
				(_, _) => new Rectangle(0,   0,   18, 40),
			};

			Main.EntitySpriteDraw(
				texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRect,
				lightColor,
				Projectile.rotation,
				Projectile.direction == 1 ? new Vector2(10f, 26f) : new Vector2(sourceRect.Width - 10f, 26f),
				1,
				spriteEffects,
				0f
			);
			return false;
		}
	}
}
