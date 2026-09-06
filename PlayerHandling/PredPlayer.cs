using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.Items;
using V2.Items.Voraria.Accessories.Thingymajigs;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using V2.NPCs;
using V2.PlayerHandling.PredPlayerGoals;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;
using V2.PlayerHandling.PredPlayerGoals.Skilled;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.Projectiles;
using V2.Projectiles.Voraria.Other;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Buffs;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.PlayerHandling
{
	public class PredStat
	{
		public int Spent { get; set; }
		public int Base { get; set; }
		public int Extra { get; set; }
		public int Total => Spent + Base + Extra;

		public PredStat()
		{
			Spent = 0;
			Base = 0;
			Extra = 0;
		}

		public void Reset()
		{
			Spent = 0;
			Base = 0;
			Extra = 0;
		}
	}
	public partial class PredPlayer : ModPlayer
	{
		public bool LootRecentlyDigested { get; set; } = false;
		public bool SyncRequired_PredPoints { get; set; }
		public VoreTracker StomachTracker
		{
			get
			{
				if (Main.gameMenu)
					return null;

				return ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.Predator is Player predPlayer && predPlayer.whoAmI == Player.whoAmI);
			}
		}

		private double _stomachache;
		public double Stomachache
		{
			get => _stomachache;
			set => _stomachache = Math.Max(0, value);
		}

		public double Overstuff { get; set; }
		public bool InPredStatsMenu { get; set; }
		public Dictionary<string, bool> GoalsCompleted { get; set; }
		public bool CheatedStatPointsWork => Rose;
		public int CheatedStatPoints { get; set; }
		public int LegitStatPoints
		{
			get
			{
				int points = 0;
				foreach (PredPlayerGoal goal in PredPlayerGoalLoader.PredPlayerGoals)
				{
					GoalsCompleted.TryAdd(goal.InternalName, false);
					if (GoalsCompleted[goal.InternalName])
						points += goal.StatPointsFromCompletion;
				}
				return points;
			}
		}
		public int TotalStatPoints => CheatedStatPointsWork ? (LegitStatPoints + CheatedStatPoints) : LegitStatPoints;
		public int AllocatedStatPoints => GLP.Spent + TUM.Spent + ACI.Spent + ABS.Spent;
		public int AvailableStatPoints => TotalStatPoints - AllocatedStatPoints;
		public PredStat GLP { get; set; }
		public StatModifier SwallowCapacityModifier;
		public static double BaseSwallowSize => 0.75;
		public static double SwallowSizePerLevel => 0.05;
		public double SwallowCapacity
		{
			get
			{
				if (V2.GetFooled)
					return -1;

				if (Rose)
					return -1;

				double baseSwallowSize = BaseSwallowSize;
				baseSwallowSize += SwallowSizePerLevel * GLP.Total;
				return SwallowCapacityModifier.ApplyTo((float)baseSwallowSize);
			}
		}
		public static int BaseLiquidSwallowSize => 5;
		public static int LiquidSwallowSizePer5Levels => 1;
		public StatModifier LiquidSwallowSizeModifier;
		public int LiquidSwallowSize
		{
			get
			{
				int baseLiquidSwallowSize = BaseLiquidSwallowSize;
				baseLiquidSwallowSize += LiquidSwallowSizePer5Levels * (int)Math.Floor(GLP.Total / 5.0);
				return (int)Math.Round(LiquidSwallowSizeModifier.ApplyTo((float)baseLiquidSwallowSize));
			}
		}
		public double EffectiveLiquidSwallowSize(int liquidType)
		{
			double effectiveBaseLiquidSwallowSize = (double)LiquidSwallowSize / 255.0;
			return liquidType switch
			{
				LiquidID.Lava => effectiveBaseLiquidSwallowSize * 4.0,
				LiquidID.Honey => effectiveBaseLiquidSwallowSize * 1.5,
				LiquidID.Shimmer => effectiveBaseLiquidSwallowSize * 0.75,
				_ => effectiveBaseLiquidSwallowSize,
			};
		}
		public static int LiquidSwallowDelay => 3;
		public static double LiquidSwallowRatePerMinute => 60.0 / (double)LiquidSwallowDelay;
		public StatModifier StruggleGraceTimeModifier;
		public static double BaseStruggleGraceTime => 0.8;
		public static double StruggleGraceTimePer5Levels => 0.1;
		public double StruggleGraceTime
		{
			get
			{
				if (V2.GetFooled)
					return 0.0;

				double baseGracePeriod = BaseStruggleGraceTime;
				baseGracePeriod += StruggleGraceTimePer5Levels * Math.Floor(GLP.Total / 5.0);
				return StruggleGraceTimeModifier.ApplyTo((float)baseGracePeriod);
			}
		}
		public string StruggleGraceTimeReadable
		{
			get
			{
				double seconds = StruggleGraceTime.CastToDecimalPlaces(2);
				int hours = 0;
				int minutes = 0;
				while (seconds > 3600.0)
				{
					hours += 1;
					seconds -= 60.0;
				}
				while (seconds > 60.0)
				{
					minutes += 1;
					seconds -= 60.0;
				}

				string readableTime = seconds + "sec";
				if (minutes > 0)
					readableTime = minutes + "min" + readableTime;
				if (hours > 0)
					readableTime = hours + "hr" + readableTime;

				return readableTime;
			}
		}
		public PredStat TUM { get; set; }
		public StatModifier StomachCapacityModifier;
		public static double BaseStomachCapacity => 1.20;
		public static double StomachCapacityPerLevel => 0.08;
		public double StomachCapacity
		{
			get
			{
				if (V2.GetFooled)
					return -1;

				if (Rose)
					return -1;

				double baseStomachCapacity = BaseStomachCapacity;
				baseStomachCapacity += StomachCapacityPerLevel * TUM.Total;
				return StomachCapacityModifier.ApplyTo((float)baseStomachCapacity);
			}
		}
		public StatModifier StomachacheMeterCapacityModifier;
		public static double BaseStomachacheMeterCapacity => 250.0;
		public static double StomachacheMeterCapacityPer5Levels => 25.0;
		public double StomachacheMeterCapacity
		{
			get
			{
				if (V2.GetFooled)
					return -1;

				if (Rose)
					return -1;

				double baseStomachacheMeterCapacity = BaseStomachacheMeterCapacity;
				baseStomachacheMeterCapacity += StomachacheMeterCapacityPer5Levels * Math.Floor(TUM.Total / 5.0);
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		public StatModifier StomachacheDefense;
		public PredStat ACI { get; set; }
		/// <summary>
		/// Denotes the tier of stomach acids this player currently has.<br/>
		/// Defaults to 0.<br/>
		/// <br/>
		/// 0 - Normal<br/>
		/// 1 - Enchanted<br/>
		/// 2 - Royal<br/>
		/// 99 - Divine<br/>
		/// 100 - Chronological<br/>
		/// 888 - Rose (and friends!)<br/>
		/// </summary>
		public int AcidTier
		{
			get
			{
				if (Rose)
					return 888;

				if (V2.GetFooled)
					return 100;

				if (PermanentUpgradesGained.TryGetValue("AcidTier2", out bool acidTier2Acquired) && acidTier2Acquired)
					return 2;

				if (Player.HasBuff(ModContent.BuffType<FastDigestionPotionBuff>()))
					return 2;

				if (PermanentUpgradesGained.TryGetValue("AcidTier1", out bool acidTier1Acquired) && acidTier1Acquired)
					return 1;

				return 0;
			}
		}
		public StatModifier DigestionTickDamageModifier;
		public static double BaseDigestionTickDamage => 10.0;
		public static double DigestionTickDamagePerLevel => 1.0;
		public double DigestionTickDamage
		{
			get
			{
				if (V2.GetFooled)
					return 10;

				double baseDigestionDamage = BaseDigestionTickDamage;
				baseDigestionDamage += DigestionTickDamagePerLevel * ACI.Total;
				return DigestionTickDamageModifier.ApplyTo((float)baseDigestionDamage);
			}
		}
		public StatModifier DigestionTickRateModifier;
		public static double BaseDigestionTickRate => 1.0;
		public static double DigestionTickRatePer5Levels => 0.005;
		public double DigestionTickRate
		{
			get
			{
				if (V2.GetFooled)
					return 30;

				double baseDigestionRate = BaseDigestionTickRate;
				baseDigestionRate += DigestionTickRatePer5Levels * Math.Floor(ACI.Total / 5.0);
				if (Rose)
					baseDigestionRate *= 4.0;
				return DigestionTickRateModifier.ApplyTo((float)baseDigestionRate);
			}
		}
		public PredStat ABS { get; set; }
		public StatModifier PreyAbsorptionRateModifier;
		public static double BasePreyAbsorptionRate => 0.4;
		public static double PreyAbsorptionRatePerLevel => 0.03;
		public double PreyAbsorptionRate
		{
			get
			{
				if (V2.GetFooled)
					return 5.0;

				double basePreyAbsorptionRate = BasePreyAbsorptionRate;
				basePreyAbsorptionRate += PreyAbsorptionRatePerLevel * ABS.Total;
				if (Rose)
					basePreyAbsorptionRate *= 8.0;
				return PreyAbsorptionRateModifier.ApplyTo((float)basePreyAbsorptionRate);
			}
		}
		public double PreyAbsorptionRatePerTick => PreyAbsorptionRate / (double)V2Utils.SensibleTime(minutes: 1);
		public double PreyAbsorptionRatePerSecond => PreyAbsorptionRate / (double)V2Utils.SensibleTime(seconds: 1);
		public StatModifier BuffExtensionTimeModifier;
		public static double BuffExtensionTimePer5Levels => 0.06;
		public double BuffExtensionFactor
		{
			get
			{
				if (V2.GetFooled)
					return 1.0;

				double baseBuffExtensionTime = BuffExtensionTimePer5Levels * Math.Floor(ABS.Total / 5.0);
				return 1.0 + BuffExtensionTimeModifier.ApplyTo((float)baseBuffExtensionTime);
			}
		}
		public StatModifier DebuffDisextensionTimeModifier;
		public static double DebuffDisextensionTimePer5Levels => 0.06;
		public double DebuffDisextensionFactor
		{
			get
			{
				if (V2.GetFooled)
					return 1.0;

				double baseDebuffDisextensionTime = DebuffDisextensionTimePer5Levels * Math.Floor(ABS.Total / 5.0);
				return 1.0 + DebuffDisextensionTimeModifier.ApplyTo((float)baseDebuffDisextensionTime);
			}
		}

		public SoundStyle SmallBurps { get; set; }
		public SoundStyle StandardBurps { get; set; }
		public SoundStyle BigBurps { get; set; }

		public SoundStyle SmallGulps { get; set; }
		public SoundStyle BigGulps { get; set; }

		public SlotId BellySlosh { get; set; }

		public int PreyStealLootLevel { get; set; }
		public float BurpPitchOffset { get; set; }

		public bool charmBracelet;
		public int CharmBraceletSlots
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Denotes whether or not this player has the Indigestion Charm equipped.<br/>
		/// Defaults to <see langword="false"/> at the start of each tick. Set to <see langword="true"/> if the player has the Indigestion Charm equipped.<br/>
		/// </summary>
		public bool charmNoDigest;
		public bool charmNoAirDrain;
		public bool charmStealPreyLoot;

		public bool EndoToggleUnlocked { get; set; }
		private bool endoToggle;
		public bool SafeStomach
		{
			get => (charmNoDigest && charmNoAirDrain) || (EndoToggleUnlocked && endoToggle);
			set
			{
				if (!EndoToggleUnlocked)
					return;

				endoToggle = value;
			}
		}

		public Dictionary<string, bool> PermanentUpgradesGained { get; set; }

		public string lastEntitySwallowed;
		public string lastEntitySwallowedMod;
		public Dictionary<string, int> mealCount;
		public bool lastSwallowWasDrinking;
		public string lastLiquidDrank;
		public string lastLiquidDrankMod;
		public Dictionary<string, int> drinkCount;
		public int TotalMeals
		{
			get
			{
				if (mealCount.Count <= 0)
					return 0;
				
				int meals = 0;
				foreach (KeyValuePair<string, int> keyValuePair in mealCount)
				{
					meals += keyValuePair.Value;
				}
				return meals;
			}
		}

		public bool CanDrinkLavaSafe
		{
			get
			{
				if (Player.lavaImmune)
					return true;

				if (Player.lavaTime > 0)
					return true;

				return false;
			}
		}
		public bool MoltenTummy => Player.HasBuff(ModContent.BuffType<MoltenStomach>());

		public bool CanDrinkShimmerSafe
		{
			get
			{
				for (int i = 3; i < 10; i++)
				{
					if (!Player.armor[i].IsAir && Player.armor[i].type == ItemID.ShimmerCloak)
						return true;
				}

				return false;
			}
		}
		public bool PrimedForShimmerStomachDeath { get; set; }
		public bool ShimmeringTummy
		{
			get => Player.HasBuff(ModContent.BuffType<ShimmeringStomach>());
			set
			{
				if (value)
				{
					if (!Player.HasBuff(ModContent.BuffType<ShimmeringStomach>()))
						Player.AddBuff(ModContent.BuffType<ShimmeringStomach>(), V2Utils.SensibleTime(seconds: 5));
				}
				else
				{
					if (!Player.HasBuff(ModContent.BuffType<ShimmeringStomach>()))
						Player.ClearBuff(ModContent.BuffType<ShimmeringStomach>());
				}
			}
		}

		public double StomachWeightAtSleepStart;
		public int OverfullTime;

		public double specialManaRegenCount;

		public bool BlockSwallowAttempts
		{
			get
			{
				if (Player.CurrentCaptor() is not null)
					return true;

				if (Player.HasBuff(ModContent.BuffType<SoreThroat>()))
					return true;

				return false;
			}
		}
		public SlotId ActiveStomachNoises { get; set; }
		public double StomachFullness
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						totalBellyWeight += prey.WeightLeftToDigest;
						if (prey.NoHealth)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachFullness;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += preyPredNPC.AsPred().ExtraWeight;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
							case PreyType.Projectile:
								Projectile preyPredProjectile = prey.Instance as Projectile;
								totalBellyWeight += preyPredProjectile.AsPred().ExtraWeight;
								totalBellyWeight += PredProjectile.GetCurrentBellyWeight(preyPredProjectile);
								break;
						}
					}
				}
				return totalBellyWeight;
			}
		}

		public double KickyStomachFullness
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						if (prey.NoHealth)
							continue;

						totalBellyWeight += prey.WeightLeftToDigest;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachFullness;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += preyPredNPC.AsPred().ExtraWeight;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
							case PreyType.Projectile:
								Projectile preyPredProjectile = prey.Instance as Projectile;
								totalBellyWeight += preyPredProjectile.AsPred().ExtraWeight;
								totalBellyWeight += PredProjectile.GetCurrentBellyWeight(preyPredProjectile);
								break;
						}
					}
				}
				return totalBellyWeight;
			}
		}

		public StatModifier StomachWeightModifier;
		public double FlatStomachWeightModifier { get; set; }

		public double StomachWeight
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						totalBellyWeight += prey.WeightLeftToDigest;
						if (prey.NoHealth)
							continue;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += preyPredNPC.AsPred().ExtraWeight;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
							case PreyType.Projectile:
								Projectile preyPredProjectile = prey.Instance as Projectile;
								totalBellyWeight += preyPredProjectile.AsPred().ExtraWeight;
								totalBellyWeight += PredProjectile.GetCurrentBellyWeight(preyPredProjectile);
								break;
						}
					}
				}
				totalBellyWeight = (double)StomachWeightModifier.ApplyTo((float)totalBellyWeight);
				return Math.Max(totalBellyWeight + FlatStomachWeightModifier, 0);
			}
		}

		public double KickyStomachWeight
		{
			get
			{
				double totalBellyWeight = 0.0;
				if (StomachTracker is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						if (prey.NoHealth)
							continue;

						totalBellyWeight += prey.WeightLeftToDigest;

						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPredPlayer = prey.Instance as Player;
								totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
								break;
							case PreyType.NPC:
								NPC preyPredNPC = prey.Instance as NPC;
								totalBellyWeight += preyPredNPC.AsPred().ExtraWeight;
								totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
								break;
							case PreyType.Projectile:
								Projectile preyPredProjectile = prey.Instance as Projectile;
								totalBellyWeight += preyPredProjectile.AsPred().ExtraWeight;
								totalBellyWeight += PredProjectile.GetCurrentBellyWeight(preyPredProjectile);
								break;
						}
					}
				}
				totalBellyWeight = (double)StomachWeightModifier.ApplyTo((float)totalBellyWeight);
				return Math.Max(totalBellyWeight + FlatStomachWeightModifier, 0);
			}
		}

		public double PercentBellySizeModifier { get; set; }
		public int FlatBellySizeModifier { get; set; }
		public int StomachSize
		{
			get
			{
				int tummySize = (int)Math.Floor(5.0 * Math.Sqrt(StomachFullness));
				tummySize = (int)Math.Round((double)tummySize * PercentBellySizeModifier);
				tummySize += FlatBellySizeModifier;

				return Math.Min(tummySize, BellyDrawLayer.RegularBelly.StandardBellies.Count);
			}
		}

		public static double StomachFullnessFromSize(int tummySize, double percentBellySizeModifier = 1)
		{
			// tummySize = 5.0 * Math.Sqrt(StomachFullness);
			// tummySize = tummySize * PercentBellySizeModifier;
			// (tummySize / 5 / PercentBellySizeModifier)^2 = StomachFullness
			// int clampedTummySize = Math.Clamp(tummySize, 0, BellyDrawLayer.RegularBelly.StandardBellies.Count);
			return Math.Pow(tummySize / 5d / percentBellySizeModifier, 2d);
		}

		public bool SizeScanner { get; set; }

		public int ItemCooldownWhenSwallowingANonStackedItemFromTheMouseSlotBecauseThisGameIsCoolAndAwesome { get; set; }

		public bool Rose { get; set; }
		public bool Venomizeous { get; set; }
		public bool FungalFairySetBonus { get; set; }
		public double WellFed_Multiplier { get; set; }

		//basically everything related to wg or other things
		public double BaeTransformation_ExtraWeight { get; set; }
		public double KroniiTransformation_ExtraWeight { get; set; }
		public double OllieTransformation_ExtraWeight { get; set; }
		public double SoraTransformation_ExtraWeight { get; set; }
		public double MintTransformation_ExtraWeight { get; set; }
		public bool WeightDisplay { get; set; }
		public double ActuallyReasonableAmountOfFood { get; set; }
		public double BaseWeightGainRatio { get; set; }
		public double WeightGainMultiplier { get; set; }
		public double WeightLossMultiplier { get; set; }

		public StatModifier BodyWeightModifier;
		public double FlatBodyWeightModifier { get; set; }
		public bool HasJumped { get; set; }

		//study shows that being really heavy and then falling on other people hurts said people
		public int isAtCrushingSpeed { get; set; }
		public int CrushingDamage { get; set; }
		public int LandState { get; set; }

		public bool StrangeThingymajig {  get; set; }
		public bool AquaticThingymajig {  get; set; }
		public bool PermanentAquaticThingymajig { get; set; }

		public override void Initialize()
		{
			SmallGulps = Gulps.Short;
			BigGulps = Gulps.Standard;
			SmallBurps = Burps.Humanoid.Small;
			StandardBurps = Burps.Humanoid.Standard;
			// BellySloshes = Sloshes.Humanoid.Standard;


			if (V2.GetFooled)
			{
				SmallGulps = Gulps.AprilFools;
				BigGulps = Gulps.AprilFools;
				SmallBurps = Burps.AprilFools;
				StandardBurps = Burps.AprilFools;
			}

			GLP = new PredStat();
			ACI = new PredStat();
			TUM = new PredStat();
			ABS = new PredStat();

			Stomachache = 0;
			Overstuff = 0;

			charmNoDigest = false;
			charmNoAirDrain = false;
			charmStealPreyLoot = false;

			EndoToggleUnlocked = false;
			endoToggle = false;

			lastEntitySwallowed = null;
			lastEntitySwallowedMod = null;
			mealCount = [];
			lastSwallowWasDrinking = false;
			lastLiquidDrank = null;
			lastLiquidDrankMod = null;
			drinkCount = [];

			PrimedForShimmerStomachDeath = false;

			PercentBellySizeModifier = 1.0;
			FlatBellySizeModifier = 0;

			PermanentUpgradesGained = new Dictionary<string, bool>();

			GoalsCompleted = [];
			foreach (PredPlayerGoal goal in PredPlayerGoalLoader.PredPlayerGoals)
			{
				GoalsCompleted.Add(goal.InternalName, false);
			}

			InPredStatsMenu = false;

			StomachWeightAtSleepStart = 0.0;
			OverfullTime = 0;
			BaeTransformation_ExtraWeight = 0;
			KroniiTransformation_ExtraWeight = 0;
			OllieTransformation_ExtraWeight = 0;
			SoraTransformation_ExtraWeight = 0;
			MintTransformation_ExtraWeight = 0;
		}

		public override void ResetEffects()
		{
			BurpPitchOffset = 0;

			SyncRequired_PredPoints = false;

			charmNoDigest = false;
			charmNoAirDrain = false;
			charmStealPreyLoot = false;
			FungalFairySetBonus = false;

			GLP.Base = 0;
			GLP.Extra = 0;
			SwallowCapacityModifier = StatModifier.Default;
			LiquidSwallowSizeModifier = StatModifier.Default;
			StruggleGraceTimeModifier = StatModifier.Default;
			TUM.Base = 0;
			TUM.Extra = 0;

			if (StomachacheMeterCapacity != -1)
			{
				double stomachacheQuellPerTick = StomachacheMeterCapacity * (0.05 / (double)V2Utils.SensibleTime(seconds: 1));
				if (StomachTracker is not null && KickyStomachFullness > 0.0)
					stomachacheQuellPerTick *= 0.1;
				Stomachache -= stomachacheQuellPerTick;
			}

			StomachCapacityModifier = StatModifier.Default;
			StomachacheMeterCapacityModifier = StatModifier.Default;
			StomachacheDefense = StatModifier.Default;
			ACI.Base = 0;
			ACI.Extra = 0;
			DigestionTickDamageModifier = StatModifier.Default;
			DigestionTickRateModifier = StatModifier.Default;
			ABS.Base = 0;
			ABS.Extra = 0;
			PreyAbsorptionRateModifier = StatModifier.Default;
			BuffExtensionTimeModifier = StatModifier.Default;
			DebuffDisextensionTimeModifier = StatModifier.Default;

			StomachWeightModifier = StatModifier.Default;
			BodyWeightModifier = StatModifier.Default;
			if (V2.GetFooled)
				StomachWeightModifier *= 0.0f;

			PercentBellySizeModifier = 1.0;
			FlatBellySizeModifier = 0;

			SizeScanner = false;
			WeightDisplay = false;

			UpdatePredStatPointsFromPermUpgrades();

			BaseWeightGainRatio = 1;
			WeightGainMultiplier = 1;
			WeightLossMultiplier = 1;

			Rose = false;
			Venomizeous = false;

			StrangeThingymajig = false;
			AquaticThingymajig = false;
			PermanentAquaticThingymajig = false;

			if (Player.sleeping.FullyFallenAsleep)
			{
				Player.AsPred().DigestionTickRateModifier += 0.25f;
				Player.AsPred().PreyAbsorptionRateModifier += 0.25f;
				bool isEveryoneAsleep = Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0;
				if (isEveryoneAsleep)
				{
					Player.AsPred().DigestionTickRateModifier *= (float)Main.dayRate;
					Player.AsPred().PreyAbsorptionRateModifier *= (float)Main.dayRate;
				}
			}

			if (Player.AsPred().WellFed_Multiplier != 0 && !Player.HasBuff<Overstuffed>())
				Player.AddBuff(ModContent.BuffType<WellFed>(), 3);
			if (Player.jump <= 0)
				Player.AsPred().HasJumped = false;
			if (Player.AsPred().ItemCooldownWhenSwallowingANonStackedItemFromTheMouseSlotBecauseThisGameIsCoolAndAwesome > 0)
				Player.AsPred().ItemCooldownWhenSwallowingANonStackedItemFromTheMouseSlotBecauseThisGameIsCoolAndAwesome--;
		}

		public void UpdatePredStatPointsFromPermUpgrades()
		{
			if (PermanentUpgradesGained.TryGetValue("PureSwallow1", out bool swallowStimsEaten) && swallowStimsEaten)
				GLP.Base += PureSwallowBoost1.GLPBonus;
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeForest", out bool eatenForestThingy) && eatenForestThingy)
			{
				SwallowCapacityModifier += BiomeForestThingy.PermBuff;
				PreyAbsorptionRateModifier += BiomeForestThingy.PermBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeShimmer", out bool eatenShimmerThingy) && eatenShimmerThingy)
			{
				GLP.Base += BiomeShimmerThingy.PermBuff;
				TUM.Base += BiomeShimmerThingy.PermBuff;
				ACI.Base += BiomeShimmerThingy.PermBuff;
				ABS.Base += BiomeShimmerThingy.PermBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeSky", out bool eatenSkyThingy) && eatenSkyThingy)
			{
				StomachWeightModifier += BiomeSkyThingy.PermBuff;
				BodyWeightModifier += BiomeSkyThingy.PermBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeSnow", out bool eatenSnowThingy) && eatenSnowThingy)
			{
				StomachacheMeterCapacityModifier += BiomeSnowThingy.PermAcheBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeDesert", out bool eatenDesertThingy) && eatenDesertThingy)
			{
				Player.AsFood().StruggleDamageModifier += BiomeDesertThingy.PermStrBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeJungle", out bool eatenJungleThingy) && eatenJungleThingy)
			{
				StomachCapacityModifier += BiomeJungleThingy.PermCapBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeCorruption", out bool eatenCorruptionThingy) && eatenCorruptionThingy)
			{
				StomachacheMeterCapacityModifier += BiomeCorruptionThingy.PermAcheBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeCrimson", out bool eatenCrimsonThingy) && eatenCrimsonThingy)
			{
				DigestionTickDamageModifier += BiomeCrimsonThingy.PermDigestBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeHallow", out bool eatenHallowThingy) && eatenHallowThingy)
			{
				Player.moveSpeed += BiomeHallowThingy.PermSpdBuff;
				PreyAbsorptionRateModifier += BiomeHallowThingy.PermAbsBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeMushroom", out bool eatenMushroomThingy) && eatenMushroomThingy)
			{
				BodyWeightModifier += BiomeMushroomThingy.PermBWeightBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeDungeon", out bool eatenDungeonThingy) && eatenDungeonThingy)
			{
				Player.statDefense += BiomeDungeonThingy.PermDefBuff;
				StomachacheMeterCapacityModifier += BiomeDungeonThingy.PermAcheBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeUnderworld", out bool eatenUnderworldThingy) && eatenUnderworldThingy)
			{
				DigestionTickDamageModifier += BiomeUnderworldThingy.PermDigestBuff;
			}
			if (PermanentUpgradesGained.TryGetValue("Thingy_BiomeOcean", out bool eatenOceanThingy) && eatenOceanThingy)
			{
				PermanentAquaticThingymajig = true;
			}
		}

		public override bool HoverSlot(Item[] inventory, int context, int slot)
		{
			if (inventory.Length == 59)
			{
				if (Player.whoAmI == Main.myPlayer && (V2.ItemGulpHotkey.JustPressed || (Main.keyState.IsKeyDown(Keys.LeftShift) && V2.ItemGulpHotkey.Current)))
				{
					if (inventory[slot].IsAir)
						return true;

					int origStack = inventory[slot].stack;
                    if (!(Main.keyState.IsKeyDown(Keys.LeftShift) && V2.ItemGulpHotkey.Current))
                        PlayerInput.Triggers.JustPressed.KeyStatus[$"{Mod.Name}/EatItems"] = false;
                    if (inventory[slot].AsFood().PreSwallow is not null && !inventory[slot].AsFood().PreSwallow.Invoke(inventory[slot], Player))
					{
						return false;
					}
					inventory[slot].stack = 1;
					if (CanSwallow(Player, inventory[slot]))
					{
						if (origStack > 1)
						{
							Item eatenItem = new Item();
							eatenItem.SetDefaults(inventory[slot].type);
							eatenItem.stack = 1;
							Player.ForceDropItem(Player.Center, ref eatenItem, out Item itemDrop);
							Swallow(Player, itemDrop);
							inventory[slot].stack = origStack - 1;
						}
						else
						{
							Player.ForceDropItem(Player.Center, ref inventory[slot], out Item itemDrop);
							Swallow(Player, itemDrop);
						}
						ModContent.GetInstance<FirstItemEaten>().TrySetCompletion(Player);
					}
					else
						inventory[slot].stack = origStack;
				}
			}
			return false;
		}
		public override void UpdateBadLifeRegen()
		{
			if (Player.AsPred().MoltenTummy)
			{
				if (Player.lifeRegen > 0)
					Player.lifeRegen = 0;
				Player.lifeRegen -= 75;
				Player.lifeRegenTime = 0;
			}
		}
		public override void PostUpdateMiscEffects()
		{
			while (specialManaRegenCount >= 60.0)
			{
				specialManaRegenCount -= 60.0;
				Player.statMana += 1;
				if (Player.statMana > Player.statManaMax2)
					Player.statMana = Player.statManaMax2;
			}
		}
		public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
		{
			if (Player.AsPred().FungalFairySetBonus)
			{
				if (CanSwallow(Player, proj))
				{
					Swallow(Player, proj);
					modifiers.Cancel();
				}
			}
		}
		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			if (Player.AsV2Player().MintTransformation && npc.AsFood().IsAGhostlySnackForACertainMaid)
			{
				Swallow(Player, npc, ForceSwallow: true, Silent: true);
				modifiers.Cancel();
			}
		}

		public override void PostUpdateBuffs()
		{
			double multiplier = Math.Clamp(WellFed_Multiplier, -3.5, 3.5);
			if (Player.HasBuff<Overstuffed>())
			{
				multiplier = Math.Max(-Overstuff * 3, -10);
				StomachacheMeterCapacityModifier -= (float)Math.Min(Overstuff / 2f, 0.8);
				if (Overstuff >= 3)
					ModContent.GetInstance<JustABitMore>().TrySetCompletion(Player);
			}
			else
			{
				if (multiplier >= 3.5)
					ModContent.GetInstance<PerfectMeal>().TrySetCompletion(Player);
				else if (multiplier <= -3.5)
					ModContent.GetInstance<Recycler>().TrySetCompletion(Player);
			}
			int def = (int)Math.Round(WellFed.Def * multiplier);
			int crit = (int)Math.Round(WellFed.Crit * multiplier);
			float atkspd = (float)Math.Round((int)(WellFed.AtkSpd * 100) * multiplier) / 100;
			float dmg = (float)Math.Round((int)(WellFed.Dmg * 100) * multiplier) / 100;
			float kb = (float)Math.Round((int)(WellFed.KB * 100) * multiplier) / 100;
			float runspd = (float)Math.Round((int)(WellFed.RunSpd * 100) * multiplier) / 100;
			float minespd = (float)Math.Round((int)(WellFed.MineSpd * 100) * multiplier) / 100;

			Player.statDefense += def;
			Player.GetCritChance(DamageClass.Generic) += crit;
			Player.GetAttackSpeed(DamageClass.Generic) += atkspd;
			Player.GetDamage(DamageClass.Generic) += dmg;
			Player.GetKnockback(DamageClass.Generic) += kb;
			Player.moveSpeed = (float)Math.Max(Player.moveSpeed + runspd, 0.01);
			Player.pickSpeed -= minespd;
		}

		public bool IsRunningAgainstConveyor(Player player)
		{
			List<Point> tiles = Collision.GetTilesIn(player.Hitbox.BottomLeft() - new Vector2(-2, -2), player.Hitbox.BottomRight() + new Vector2(2, 10));
			int RightConveyors = 0;
			int LeftConveyors = 0;
			foreach (var point in tiles)
			{
				Tile tile = Framing.GetTileSafely(point);
				if (tile.HasTile)
				{
					if (!tile.IsActuated && tile.TileType == TileID.ConveyorBeltRight)
						RightConveyors++;
					else if (!tile.IsActuated && tile.TileType == TileID.ConveyorBeltLeft)
						LeftConveyors++;
				}
			}
			if (RightConveyors == LeftConveyors) return false;
			if (RightConveyors > LeftConveyors && player.controlRight)
				return true;
			if (RightConveyors < LeftConveyors && player.controlLeft)
				return true;
			return false;
		}
		public static float WeightMovementMultiplier(Player player)
		{
			float Weight = (float)PlayerGaining.GetPlayerWeight(player, true);

			return 1.0f / (float)Math.Max(1.0, Weight);
		}

		public override void PostUpdateEquips()
		{
			PlayerGaining.ReduceWeight(Player, 0.000005);
			if (!Player.AsPred().HasJumped && Player.jump > 0)
			{
				Player.AsPred().HasJumped = true;
				PlayerGaining.ReduceWeight(Player, 0.00008);
			}
			if (!(Player.IsAirborne() || Player.sleeping.isSleeping || Player.sitting.isSitting || Player.grappling[0] >= 0 || Player.pulley))
			{
				if ((Player.controlRight && Player.velocity.X > 0) || (Player.controlLeft && Player.velocity.X < 0))
				{
					PlayerGaining.ReduceWeight(Player, 0.000005);
					
				}
				if (IsRunningAgainstConveyor(Player))
				{
					//PlayerGaining.ReduceWeight(Player, 0.00009);
					PlayerGaining.ReduceWeight(Player, 0.1);
				}
			}

			//moar weight bull
			PlayerGaining.GetPlayerWeightGainStats(Player, out float DamageMult, out float AttackSpeedMult, out int MaxLifeIncrease);
			Player.GetDamage(DamageClass.Generic) *= DamageMult;
			Player.GetAttackSpeed(DamageClass.Generic) *= AttackSpeedMult;
			Player.statLifeMax2 += MaxLifeIncrease;
		}

		public override void PostUpdateRunSpeeds()
		{
			if (!Player.mount.Active && Player.AsV2Player().OllieDashDuration <= 0)
			{
				float weightMovementMult = WeightMovementMultiplier(Player);

				Player.runAcceleration *= weightMovementMult;
				Player.jumpSpeed *= Math.Min(1.0f, weightMovementMult * 2);
				Player.jumpHeight = (int)Math.Round((float)Player.jumpHeight * Math.Min(1.0f, weightMovementMult * 2));
				Player.gravity /= (2f + weightMovementMult) / 3f;
				Player.maxFallSpeed /= weightMovementMult;
				float weightSpeedMult = 1.0f / (float)Math.Max(1.0, ((Player.AsPred().StomachWeight - 0.5) / 2.0) + 1.0);
				Player.maxRunSpeed *= weightSpeedMult;
				Player.accRunSpeed *= weightSpeedMult;
				Player.rocketTimeMax = (int)Math.Ceiling(Player.rocketTimeMax * Math.Min(1.0f, weightMovementMult * 3f));
				if (Player.rocketTime > Player.rocketTimeMax)
					Player.rocketTime = Player.rocketTimeMax;
				Player.wingTimeMax = (int)Math.Ceiling(Player.wingTimeMax * Math.Min(1.0f, weightMovementMult * 1.25f));
				if (Player.wingTime > Player.wingTimeMax)
					Player.wingTime = Player.wingTimeMax;
			}
		}
		public int FallingForce(Player player)
		{
			return (int)Math.Ceiling(player.velocity.Length() * PlayerGaining.GetPlayerWeight(player, true, false) / 3.5) - 20;
		}
		public bool CheckForSolidGround(Player player)
		{
			List<Point> tiles = Collision.GetTilesIn(player.Hitbox.BottomLeft() - new Vector2(-2, -2), player.Hitbox.BottomRight() + new Vector2(2, 6));
			bool HasSolidTile = false;
			foreach (var point in tiles)
			{
				Tile tile = Framing.GetTileSafely(point);
				if (tile.HasTile)
				{
					if (Main.tileSolid[tile.TileType])
						HasSolidTile = true;
					if (Main.tileSolidTop[tile.TileType])
						HasSolidTile = true;
				}
			}
			if (HasSolidTile)
			{
				if (LandState == 0)
					LandState = 1;
				else
					LandState = 2;
			}
			else
				LandState = 0;
			return HasSolidTile;
		}

		public override void PostUpdate()
		{
			if (Player.pulley)
			{

				double PlayerWeight = Player.AsPred().StomachWeight + 1.0;
				if (Player.AsV2Player().BaeTransformation)
				{
					PlayerWeight += Player.AsPred().BaeTransformation_ExtraWeight;
				}

				float additionalWeight = ((float)Math.Max(1.0, PlayerWeight) - 1) / 5f;

				float DownWeigh = 0.5f * additionalWeight;
				if (!Player.controlDown)
				{
					DownWeigh = Math.Max(0, DownWeigh - 0.1f);
				}
				if (Player.controlUp)
				{
					DownWeigh = Math.Max(0, DownWeigh / 1.25f);
				}
				Player.velocity.Y += DownWeigh;
			}

			double overstuff = Player.AsPred().StomachFullness / Player.AsPred().StomachCapacity;
			Overstuff = overstuff;
			if (overstuff > 1)
			{
				Player.ClearBuff(ModContent.BuffType<WellFed>());
				Player.AddBuff(ModContent.BuffType<Overstuffed>(), 3);
			}

			if (Main.myPlayer == Player.whoAmI)
			{
				Player.AsPred().isAtCrushingSpeed = Math.Max(Player.AsPred().isAtCrushingSpeed - 1, 0);
				if (FallingForce(Player) > 15)
				{
					Player.AsPred().isAtCrushingSpeed = 3;
					Player.AsPred().CrushingDamage = FallingForce(Player);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Player.velocity, Vector2.Zero, ModContent.ProjectileType<FallingHitbox>(), Player.AsPred().CrushingDamage, (int)Math.Ceiling(Math.Sqrt(Player.AsPred().CrushingDamage)), Main.myPlayer, Player.width, Player.height);
				}
				if (CheckForSolidGround(Player) && Player.AsPred().isAtCrushingSpeed > 0)
				{
					Player.AsPred().isAtCrushingSpeed = 0;
					if (LandState == 1)
						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), Player.AsPred().CrushingDamage, (int)Math.Ceiling(Math.Sqrt(Player.AsPred().CrushingDamage)), Main.myPlayer, 0, 5f + (Player.AsPred().CrushingDamage / 10f));
				}
				if (Player.AsPred().isAtCrushingSpeed == 0)
				{
					Player.AsPred().CrushingDamage = 0;
				}
			}
			UpdateWellFed(Player);

			if (AquaticThingymajig && (Player.wet || Player.honeyWet || Player.HasBuff(BuffID.Wet) || Player.HasBuff(BuffID.Honey)))
			{
				DigestionTickDamageModifier += BiomeOceanThingy.StatBuff;
				PreyAbsorptionRateModifier += BiomeOceanThingy.StatBuff;
				Player.endurance += BiomeOceanThingy.EnduBuff;
			}
			if (PermanentAquaticThingymajig && (Player.wet || Player.honeyWet || Player.HasBuff(BuffID.Wet) || Player.HasBuff(BuffID.Honey)))
			{
				DigestionTickDamageModifier += BiomeOceanThingy.PermBuff;
				PreyAbsorptionRateModifier += BiomeOceanThingy.PermBuff;
			}
			if (StrangeThingymajig)
			{
				double fat = PlayerGaining.GetPlayerWeight(Player, false, false, true) / 4;
				StomachCapacityModifier += (float)fat;
			}
		}
		public override void PostItemCheck()
		{
			if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer && !Player.AsPred().BlockSwallowAttempts)
			{
				#region Swallowing nearby prey
				if (V2.SwallowHotkey.JustPressed || (V2.SwallowHotkey.Current && Main.keyState.IsKeyDown(Keys.LeftShift) && Main.GameUpdateCount % 2 == 0))
				{
					string mealType = "none";
					int mealIndex = -1;
					Vector2 playerLocation = Player.MountedCenter;
					Vector2 cursorLocation = Main.MouseWorld;
					double maxDistanceFromPlayer = V2Utils.TileCountAsPixelCount(4.25);
					double maxDistanceFromCursor = 2000;
					for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
					{
						NPC potentialMeal = Main.npc[npcIndex];
						if (!potentialMeal.active)
							continue;

						if (potentialMeal.realLife != -1 && potentialMeal.realLife != potentialMeal.whoAmI)
							continue;

						if (potentialMeal.CurrentCaptor() is not null)
							continue;

						if (!Collision.CanHit(Player.TrueCenter(), 1, 1, potentialMeal.TrueCenter(), 1, 1))
							continue;

						if (potentialMeal.Distance(playerLocation) >= maxDistanceFromPlayer)
							continue;

						if (potentialMeal.Distance(cursorLocation) < maxDistanceFromCursor)
						{
							mealIndex = npcIndex;
							mealType = "NPC";
							maxDistanceFromCursor = potentialMeal.Distance(cursorLocation);
						}
					}
					for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
					{
						Projectile potentialMeal = Main.projectile[projIndex];
						if (!potentialMeal.active)
							continue;

						if (potentialMeal.CurrentCaptor() is not null)
							continue;

						if (!Collision.CanHit(Player.TrueCenter(), 1, 1, potentialMeal.TrueCenter(), 1, 1))
							continue;

						if (potentialMeal.Distance(playerLocation) >= maxDistanceFromPlayer)
							continue;

						if (potentialMeal.Distance(cursorLocation) < maxDistanceFromCursor)
						{
							mealIndex = projIndex;
							mealType = "projectile";
							maxDistanceFromCursor = potentialMeal.Distance(cursorLocation);
						}
					}
					for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
					{
						Player potentialMeal = Main.player[playerIndex];
						if (!potentialMeal.active || potentialMeal.dead || potentialMeal.whoAmI == Player.whoAmI)
							continue;

						if (potentialMeal.CurrentCaptor() is not null)
							continue;

						if (!Collision.CanHit(Player.TrueCenter(), 1, 1, potentialMeal.TrueCenter(), 1, 1))
							continue;

						if (potentialMeal.Distance(playerLocation) >= maxDistanceFromPlayer)
							continue;

						if (potentialMeal.Distance(cursorLocation) < maxDistanceFromCursor)
						{
							mealIndex = playerIndex;
							mealType = "player";
							maxDistanceFromCursor = potentialMeal.Distance(cursorLocation);
						}
					}

					if (mealType != "none" && mealIndex != -1)
					{
						switch (mealType)
						{
							case "NPC":
								Swallow(Player, Main.npc[mealIndex]);
								Player.lastCreatureHit = Item.NPCtoBanner(Main.npc[mealIndex].BannerID());
								break;
							case "projectile":
								Swallow(Player, Main.projectile[mealIndex]);
								break;
							case "player":
								Swallow(Player, Main.player[mealIndex]);
								break;
						}
					}
				}
				#endregion
				#region Drinking liquids
				bool inAnyLiquid = Player.wet || Player.lavaWet || Player.honeyWet || Player.shimmerWet;
				if (V2.SwallowHotkey.Current && inAnyLiquid && Main.GameUpdateCount % LiquidSwallowDelay == 0 && !V2.GetFooled)
				{
					Point playerTileLocation = (Player.Center + new Vector2(0, -10)).ToTileCoordinates();
					Tile tile = Main.tile[playerTileLocation];
					if (tile.LiquidAmount > 0 && (Player.AsPred().Rose || Player.AsPred().StomachCapacity - Player.AsPred().StomachFullness >= Player.AsPred().EffectiveLiquidSwallowSize(tile.LiquidType)))
					{
						int liquidToDrink = (tile.LiquidAmount > Player.AsPred().LiquidSwallowSize) ? Player.AsPred().LiquidSwallowSize : tile.LiquidAmount;

						Drink(Player, tile.LiquidType, liquidToDrink);

						if (tile.LiquidAmount <= (byte)Player.AsPred().LiquidSwallowSize)
						{
							tile.LiquidAmount = 0;
							tile.LiquidType = 0;
						}
						else
							tile.LiquidAmount -= (byte)Player.AsPred().LiquidSwallowSize;
						WorldGen.SquareTileFrame(playerTileLocation.X, playerTileLocation.Y);
						if (Main.netMode == NetmodeID.MultiplayerClient)
							NetMessage.SendTileSquare(-1, playerTileLocation.X, playerTileLocation.Y);

						if (Main.GameUpdateCount % 60 == 0)
						{
							SoundEngine.PlaySound(
								Player.AsPred().SmallGulps with { Volume = 0.45f, Pitch = 0.25f },
								Player.position + new Vector2(0f, -10f)
							);
						}
					}
				}
				#endregion
				#region Regurgitating swallowed prey
				if (V2.RegurgitateHotkey.JustPressed && Player.AsPred().StomachTracker?.Prey.Count > 0)
				{
					PreyData prey = Player.AsPred().StomachTracker.Prey.FindLast(x => !x.NoHealth && x.Type != PreyType.Liquid);
					if (prey is not null)
						Regurgitate(Player, Player.AsPred().StomachTracker.Prey.IndexOf(prey));
				}
				#endregion
			}

			UpdateGeneralPredGoalsLogic(Player);
		}

		public static bool CanSwallow(Player pred, Entity prey, bool forced = false)
		{
			if (pred.AsPred().BlockSwallowAttempts)
				return false;

			switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
			{
				default:
					// do absolutely fucking nothing lmao
					break;
				case "No Male":
					if (pred.Male)
						return false;
					break;
				case "No Female":
					if (!pred.Male)
						return false;
					break;
				case "No M or F...but why?":
					return false;
			}

			if (prey.CurrentCaptor() is not null)
				return false;

			if (prey is Player preyPlayer)
			{
				if (preyPlayer.AsFood().PerfectMeal)
					return true;

				if (forced) return true;
			}
			else if (prey is NPC preyNPC)
			{
				if (preyNPC.AsFood().CannotBeEatenDueToShenanigans)
					return false;
				if (V2.VoreNPCBlacklist is not null && V2.VoreNPCBlacklist.Count > 0 && V2.VoreNPCBlacklist.Contains(preyNPC.type))
					return false;

				if (forced) return true;

				bool tastesLikeSkittles = preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress;
				if (tastesLikeSkittles)
					return true;

				bool isThisAFuckingBoss = preyNPC.boss || (preyNPC.type >= NPCID.EaterofWorldsHead && preyNPC.type <= NPCID.EaterofWorldsTail); // I hate EoW
				if (isThisAFuckingBoss && !pred.AsPred().Rose)
					return false;
			}
			else if (prey is Projectile preyProjectile)
			{
				if (preyProjectile.AsFood().CannotBeEatenDueToShenanigans)
					return false;
				if (V2.VoreNPCBlacklist is not null && V2.VoreProjectileBlacklist.Count > 0 && V2.VoreProjectileBlacklist.Contains(preyProjectile.type))
					return false;

				if (forced) return true;

				if (preyProjectile.AsFood().MaxHealth == -1 && !pred.AsPred().FungalFairySetBonus && !pred.HasBuff<Trance>())
					return false;
			}
			else if (prey is Item preyItem)
			{
				if (preyItem.AsFood().MaxHealth == -1)
					return false;

				if (forced) return true;

				if (preyItem.favorited)
					return false;
			}

			if (pred.AsPred().SwallowCapacity != -1 && PreyData.GetPreySize(prey) > pred.AsPred().SwallowCapacity && !pred.AsPred().Rose)
				return false;
			if (pred.AsPred().Stomachache >= pred.AsPred().StomachacheMeterCapacity && !pred.AsPred().Rose)
				return false;

			return true;
		}

		/// <summary>
		/// Causes the given predator player to swallow the given prey entity, if the given prey entity can be swallowed.
		/// </summary>
		/// <param name="pred">The predator which will attempt to swallow the given prey.</param>
		/// <param name="prey">The prey which will be attempt to be swallowed by the given predator.</param>
		/// <param name="MPstate">
		/// </param>
		/// <param name="MPwhoAmI">
		/// The <see cref="Player.whoAmI"/> of the client that sent a request for this swallow.<br/>
		/// Unused in singleplayer, but used in multiplayer to correctly send and subsequently receive netcode messages.<br/>
		/// </param>
		public static void Swallow(Player pred, Entity prey, int MPstate = 0, int MPwhoAmI = -1, bool skipRealLifeCheck = false, bool ForceSwallow = false, bool Silent = false)
		{
			if (!CanSwallow(pred, prey, ForceSwallow))
				return;

			if (MPstate == 0 && Main.netMode == NetmodeID.MultiplayerClient)
			{
				MPstate = 1;
				MPwhoAmI = Main.myPlayer;
			}

			PreyData food = PreyData.NewData(prey);
			if ((prey is not NPC preyNPC || preyNPC.realLife == -1) && !Silent)
			{
				SoundEngine.PlaySound(
					food.WeightLeftToDigest <= 0.3
					? pred.AsPred().SmallGulps
					: pred.AsPred().BigGulps,
					pred.Center
				);
			}
			pred.AsPred().lastSwallowWasDrinking = false;
			switch (food.Type)
			{
				case PreyType.Player:
					Player player = prey as Player;
					player.AsFood().TotalTimesSwallowed += 1;
					pred.AsPred().lastEntitySwallowed = "Player";
					pred.AsPred().lastEntitySwallowedMod = "Terraria";
					break;
				case PreyType.NPC:
					NPC npc = prey as NPC;
					npc.AsFood().OnSwallowedBy?.Invoke(npc, pred);

					if (npc.AsFood().OnSwallowDamage > 0 && npc.AsFood().OnSwallowDeathReason is not null)
					{
						pred.Hurt(
							damageSource: PlayerDeathReason.ByCustomReason(NetworkText.FromKey(
								npc.AsFood().OnSwallowDeathReason,
								pred.name)),
							Damage: npc.AsFood().OnSwallowDamage,
							hitDirection: 0,
							dodgeable: false,
							scalingArmorPenetration: 1f
						);
					}

					// this is a really fuckin' stupid way to have to do this check
					// basically, if this is the original call, look through the entire NPC list for NPCs attached to this NPC via realLife
					// if there are any, swallow all of those connected NPCs as well
					// this exists purely to allow swallowin' worm enemies all at once instead of havin' to spam your Swallow bind to eat 'em
					// ideally there'd be a sensible way to allow, like. slurpin' up the tasty noodles gradually instead of havin' to eat them all at once
					// but this is Terraria and a lot of what you have to do isn't ideal here, so whatever
					if (!skipRealLifeCheck)
					{
						for (int i = 0; i < Main.maxNPCs; i++)
						{
							if (i != npc.whoAmI && Main.npc[i].realLife != -1 && Main.npc[i].realLife == npc.whoAmI)
							{
								Swallow(pred, Main.npc[i], MPstate, MPwhoAmI, true);
							}
						}
					}

					pred.AsPred().lastEntitySwallowed = npc.TypeName;
					pred.AsPred().lastEntitySwallowedMod = npc.ModNPC != null ? npc.ModNPC.Mod.DisplayName : "Terraria";
					break;
				case PreyType.Projectile:
					Projectile projectile = prey as Projectile;
					if (projectile.AsFood().MaxHealth == -1)
					{
						food = PreyData.NewData(PreyType.Projectile, projectile.type, projectile.Name, PreyData.GetPreySize(projectile));
						projectile.active = false;
					}
					else
					{
						projectile.AsFood().OnSwallowedBy?.Invoke(projectile, pred);

						pred.AsPred().lastEntitySwallowed = projectile.Name;
						pred.AsPred().lastEntitySwallowedMod = projectile.ModProjectile != null ? projectile.ModProjectile.Mod.DisplayName : "Terraria";
					}
					break;
				case PreyType.Item:
					Item item = prey as Item;
					pred.AsPred().lastEntitySwallowed = item.Name;
					pred.AsPred().lastEntitySwallowedMod = item.ModItem != null ? item.ModItem.Mod.DisplayName : "Terraria";

					if (item.AsFood().PreSwallow is not null && !item.AsFood().PreSwallow.Invoke(item, pred))
					{
						food = null;
						return;
					}

					for (int i = 0; i < item.stack; i++)
						item.AsFood().OnSwallow?.Invoke(item, pred);

					if (item.AsFood().OnSwallowDamage > 0 && item.AsFood().OnSwallowDeathReason is not null)
					{
						pred.Hurt(
							damageSource: PlayerDeathReason.ByCustomReason(NetworkText.FromKey(
								item.AsFood().OnSwallowDeathReason,
								pred.name)),
							Damage: item.AsFood().OnSwallowDamage * item.stack,
							hitDirection: 0,
							dodgeable: false,
							scalingArmorPenetration: 1f
						);
					}
					if (item.AsFood().OnSwallowSoreThroatTime > 0)
						pred.AddBuff(ModContent.BuffType<SoreThroat>(), item.AsFood().OnSwallowSoreThroatTime);
					break;
			}
			if (food is null)
				return;
			AddNewPrey(pred, food);

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestSwallowPrey);
				packet.Write((byte)0);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send();
			}
			else if (MPstate == 2)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.SyncSwallowPrey);
				packet.Write((byte)0);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send(ignoreClient: MPwhoAmI);
			}
		}

		public static void Drink(Player pred, int liquidType = -1, int liquidAmount = -1, PreyData newDrink = null, int MPstate = 0, int MPwhoAmI = -1)
		{
			pred.AsPred().lastLiquidDrank = liquidType switch
			{
				0 => "Water",
				1 => "Lava",
				2 => "Honey",
				3 => "Shimmer",
				_ => "Some other liquid",
			};

			if (liquidType == 0 && liquidAmount == 0 && newDrink is null)
				throw new ArgumentException("you're supposed to make sure either the PreyData instance provided or the liquid type and amount provided are valid for PredPlayer.Drink. try again");

			newDrink ??= new PreyData(liquidType, liquidAmount);
			if (liquidType == -1 && liquidAmount == -1)
			{
				liquidType = newDrink.ExactType;
				liquidAmount = (int)Math.Round(newDrink.WeightLeftToDigest / (liquidType switch
				{
					LiquidID.Lava => 4.0,
					LiquidID.Honey => 1.5,
					LiquidID.Shimmer => 0.75,
					_ => 1.0,
				}) * 256.0);
			}
			if (pred.AsPred().StomachTracker is not null && pred.AsPred().StomachTracker.Prey.FirstOrDefault(x => x.Type == PreyType.Liquid && x.ExactType == liquidType) is PreyData existingDrink)
				existingDrink.WeightLeftToDigest += newDrink.WeightLeftToDigest;
			else
				AddNewPrey(pred, newDrink);

			void AddVanillaDrinkCount()
			{
				pred.AsPred().lastLiquidDrankMod = "Terraria";
				if (!pred.AsPred().drinkCount.ContainsKey(pred.AsPred().lastLiquidDrankMod + ": " + pred.AsPred().lastLiquidDrank))
					pred.AsPred().drinkCount.Add(pred.AsPred().lastLiquidDrankMod + ": " + pred.AsPred().lastLiquidDrank, 0);
				pred.AsPred().drinkCount[pred.AsPred().lastLiquidDrankMod + ": " + pred.AsPred().lastLiquidDrank] += liquidAmount;
				pred.AsPred().lastSwallowWasDrinking = true;
			}
			bool VanillaDrinkCountHas(int req) => pred.AsPred().drinkCount[pred.AsPred().lastLiquidDrankMod + ": " + pred.AsPred().lastLiquidDrank] >= req;
			switch (liquidType)
			{
				case LiquidID.Water:
					AddVanillaDrinkCount();
					if (VanillaDrinkCountHas(255))
						ModContent.GetInstance<FirstDrink>().TrySetCompletion(pred);
					break;
				case LiquidID.Lava:
					if (pred.AsPred().CanDrinkLavaSafe)
					{
						AddVanillaDrinkCount();
						if (VanillaDrinkCountHas(255))
							ModContent.GetInstance<DrinkLava>().TrySetCompletion(pred);
					}
					break;
				case LiquidID.Honey:
					AddVanillaDrinkCount();
					if (VanillaDrinkCountHas(255))
						ModContent.GetInstance<DrinkHoney>().TrySetCompletion(pred);
					break;
				case LiquidID.Shimmer:
					if (!pred.AsPred().CanDrinkShimmerSafe && !pred.AsPred().PrimedForShimmerStomachDeath)
					{
						pred.AddBuff(ModContent.BuffType<ShimmeringStomach>(), 300);
						pred.AsPred().PrimedForShimmerStomachDeath = true;
					}
					else if (pred.AsPred().CanDrinkShimmerSafe)
					{
						AddVanillaDrinkCount();
						//	if (VanillaDrinkCountHas(255))
						//		ModContent.GetInstance<FirstDrink>().TrySetCompletion(pred);
					}
					break;
			}

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestSwallowPrey);
				packet.Write((byte)0);
				packet.Write(pred.whoAmI);
				packet.Write((byte)PreyType.Liquid);
				packet.Write(liquidType);
				packet.Write(liquidAmount);
				packet.Write(MPwhoAmI);
				packet.Send();
			}
			else if (MPstate == 2)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.SyncSwallowPrey);
				packet.Write((byte)0);
				packet.Write(pred.whoAmI);
				packet.Write((byte)PreyType.Liquid);
				packet.Write(liquidType);
				packet.Write(liquidAmount);
				packet.Write(MPwhoAmI);
				packet.Send(ignoreClient: MPwhoAmI);
			}
		}

		public static void Regurgitate(Player pred, int index = -1, int MPstate = 0, int MPwhoAmI = -1)
		{
			if (MPstate == 0 && Main.netMode == NetmodeID.MultiplayerClient)
			{
				MPstate = 1;
				MPwhoAmI = Main.myPlayer;
			}

			double totalRegurgiweight = 0.0;

			List<PreyData> clearedPrey = new List<PreyData>();

			void Regurgitate_Inner(Player pred, PreyData prey)
			{
				if (prey.Instance is null || prey.NoHealth)
					return;

				if (prey.CannotBeRegurgitated)
					return;

				Entity realPrey = prey.Type switch
				{
					PreyType.Player => prey.Instance as Player,
					PreyType.NPC => prey.Instance as NPC,
					PreyType.Projectile => prey.Instance as Projectile,
					PreyType.Item => prey.Instance as Item,
					_ => throw new NotImplementedException(),
				};
				realPrey.position = pred.TrueCenter() + new Vector2(pred.direction * 8f + realPrey.width * pred.direction, -10f - realPrey.height);
				realPrey.velocity = new Vector2(pred.direction * 10f, Main.rand.Next(-100,101) / 100f);
				if (realPrey is NPC realPreyNPC)
				{
					realPreyNPC.AsFood().EatenSafetyFrames = 20;
				}
				else if (realPrey is Projectile realPreyProjectile)
				{

				}
				else if (realPrey is Player realPreyPlayer)
				{

				}
				else if (realPrey is Item realPreyItem)
				{
					realPreyItem.noGrabDelay = 60;
					for (int i = 0; i < realPreyItem.stack; i++)
						if (realPreyItem.AsFood().OnRegurgitate is not null && realPreyItem.AsFood().OnRegurgitate.Invoke(realPreyItem, pred))
						{
							realPreyItem.stack--;
						}
					if (realPreyItem.stack <= 0)
						realPreyItem.TurnToAir();
				}
				totalRegurgiweight += prey.WeightLeftToDigest;
				clearedPrey.Add(prey);
			}

			if (index == -1)
			{
				foreach (PreyData prey in pred.AsPred().StomachTracker.Prey)
					Regurgitate_Inner(pred, prey);

				foreach (PreyData prey in clearedPrey)
				{
					pred.AsPred().StomachTracker.Prey.Remove(prey);
				}
				pred.AsPred().StomachTracker.RefreshStruggleChartList();
			}
			else
			{
				PreyData prey = pred.AsPred().StomachTracker.Prey[index];
				Regurgitate_Inner(pred, prey);

				if (clearedPrey.Count > 0)
					pred.AsPred().StomachTracker.Prey.Remove(prey);
			}

			if (totalRegurgiweight > 0)
				SoundEngine.PlaySound(
					totalRegurgiweight <= 0.3 ? pred.AsPred().SmallBurps : pred.AsPred().StandardBurps,
					pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
				);

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestRegurgitatePrey);
				packet.Write((byte)0);
				packet.Write(Main.myPlayer);
				packet.Write(index);
				packet.Write(Main.myPlayer);
				packet.Send();
			}
			else if (MPstate == 2)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.SyncRegurgitatePrey);
				packet.Write((byte)0);
				packet.Write(Main.myPlayer);
				packet.Write(index);
				packet.Write(Main.myPlayer);
				packet.Send(ignoreClient: MPwhoAmI);
			}
		}

		public static void AddNewPrey(Player pred, PreyData prey)
		{
			if (pred.AsPred().StomachTracker is null)
				VoreTracker.NewTracker(pred, [prey]);
			else
				pred.AsPred().StomachTracker.QueueNewPrey(prey);
		}

		/// <summary>
		/// Runs update ticks on all food in this predatory player's stomach.
		/// </summary>
		public static void UpdatePrey(Player pred)
		{
			if (pred.AsPred().StomachacheMeterCapacity > 0 && pred.AsPred().Stomachache >= pred.AsPred().StomachacheMeterCapacity && pred.AsPred().StomachTracker is not null && pred.AsPred().StomachTracker.Prey.Count > 0)
			{
				Regurgitate(pred);
				ModContent.GetInstance<BLUH>().TrySetCompletion(pred);
				return;
			}
			bool hasDoneDigestionTick = false;
			foreach (PreyData prey in pred.AsPred().StomachTracker.Prey)
			{
				if (!prey.NoHealth)
				{
					prey.UpdateInStomach?.Invoke(prey.Instance, pred, false);

					switch (prey.Type)
					{
						case PreyType.Player:
							Player preyPlayer = prey.Instance as Player;
							preyPlayer.velocity = Vector2.Zero;
							preyPlayer.position = pred.position;
							break;
						case PreyType.NPC:
							if (prey.Instance is null)
							{
								prey.NoHealth = true;
								break;
							}
							NPC preyNPC = prey.Instance as NPC;
							preyNPC.velocity = Vector2.Zero;
							preyNPC.position = pred.position;
							pred.AsPred().WellFed_Multiplier += prey.WellFedPower * prey.WeightLeftToDigest;
							break;
						case PreyType.Projectile:
							if (prey.Instance is null)
							{
								prey.NoHealth = true;
								break;
							}
							Projectile preyProjectile = prey.Instance as Projectile;
							if (preyProjectile is null || !preyProjectile.active)
							{
								prey.NoHealth = true;
								break;
							}
							preyProjectile.timeLeft += 1 + preyProjectile.extraUpdates;
							preyProjectile.velocity = Vector2.Zero;
							preyProjectile.position = pred.position;
							pred.AsPred().WellFed_Multiplier += prey.WellFedPower * prey.WeightLeftToDigest;
							break;
						case PreyType.Item:
							if (prey.Instance is null)
							{
								prey.NoHealth = true;
								break;
							}
							Item preyItem = prey.Instance as Item;
							if (preyItem is null || !preyItem.active)
							{
								prey.NoHealth = true;
								break;
							}
							preyItem.AsFood().UpdateInStomach?.Invoke(preyItem, pred, prey.NoHealth);
							preyItem.velocity = Vector2.Zero;
							preyItem.position = pred.position;
							bool canProperlyDigestItem = pred.AsPred().AcidTier >= preyItem.AsFood().AcidResistTier;
							if (canProperlyDigestItem)
								pred.AsPred().WellFed_Multiplier += prey.WellFedPower * prey.WeightLeftToDigest;
							else
								pred.AsPred().WellFed_Multiplier += prey.WellFedPower * prey.WeightLeftToDigest / 4;
							break;
					}

					if (hasDoneDigestionTick)
						continue;
					double digestionDamage = pred.AsPred().DigestionTickDamage;
					double digestionRate = pred.AsPred().DigestionTickRate;
					if (digestionRate <= 0.0)
						digestionRate = 1.0;

					int digestionFrameRate = (int)Math.Round(60.0 / digestionRate);
					if (prey.timeSpentInStomach % digestionFrameRate == 0)
					{
						switch (prey.Type)
						{
							case PreyType.Player:
								if (prey.Instance is null)
									break;
								Player preyPlayer = prey.Instance as Player;
								bool shouldDigestPlayer = !pred.AsPred().charmNoDigest;
								if (shouldDigestPlayer)
								{
									hasDoneDigestionTick = true;
									prey.NoHealth = preyPlayer.AsFood().TakeDigestionDamage(pred, digestionDamage);
									if (prey.NoHealth)
									{
										pred.AsPred().mealCount.TryAdd("Terraria: Player", 0);
										pred.AsPred().mealCount["Terraria: Player"] += 1;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? pred.AsPred().SmallBurps with { Pitch = pred.AsPred().BurpPitchOffset }
											: pred.AsPred().StandardBurps with { Pitch = pred.AsPred().BurpPitchOffset },
											pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
										);
									}
								}
								break;
							case PreyType.NPC:
								if (prey.Instance is null)
									break;
								NPC preyNPC = prey.Instance as NPC;
								bool shouldDigestNPC = !pred.AsPred().charmNoDigest;
								if (shouldDigestNPC)
								{
									hasDoneDigestionTick = true;
									if (preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
										digestionDamage *= 20.0;
									prey.NoHealth = PreyNPC.TakeDigestionDamage(preyNPC, pred, digestionDamage);
									if (prey.NoHealth)
									{
										prey.Instance = null;
										string preyNPCMod = preyNPC.ModNPC != null ? preyNPC.ModNPC.Mod.DisplayName : "Terraria";
										pred.AsPred().mealCount.TryAdd(preyNPCMod + ": " + preyNPC.TypeName, 0);
										pred.AsPred().mealCount[preyNPCMod + ": " + preyNPC.TypeName] += 1;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? pred.AsPred().SmallBurps with { Pitch = pred.AsPred().BurpPitchOffset }
											: pred.AsPred().StandardBurps with { Pitch = pred.AsPred().BurpPitchOffset },
											pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
										);
									}
								}
								break;
							case PreyType.Projectile:
								if (prey.Instance is null)
									break;
								Projectile preyProjectile = prey.Instance as Projectile;
								bool shouldDigestProjectile = !pred.AsPred().charmNoDigest;
								if (shouldDigestProjectile)
								{
									hasDoneDigestionTick = true;
									prey.NoHealth = preyProjectile.TakeDigestionDamage(pred, digestionDamage);
									preyProjectile.netUpdate = true;
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyProjectile.Name);
									else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyProjectile.Name);
									if (prey.NoHealth)
									{
										prey.Instance = null;
										string preyProjectileMod = preyProjectile.ModProjectile != null ? preyProjectile.ModProjectile.Mod.DisplayName : "Terraria";
										pred.AsPred().mealCount.TryAdd(preyProjectileMod + ": " + preyProjectile.Name, 0);
										pred.AsPred().mealCount[preyProjectileMod + ": " + preyProjectile.Name] += 1;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? pred.AsPred().SmallBurps with { Pitch = pred.AsPred().BurpPitchOffset }
											: pred.AsPred().StandardBurps with { Pitch = pred.AsPred().BurpPitchOffset },
											pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
										);
									}
								}
								break;
							case PreyType.Item:
								if (prey.Instance is null)
									break;
								Item preyItem = prey.Instance as Item;
								if (preyItem.IsAir)
									break;
								bool shouldDigestItem = !pred.AsPred().SafeStomach;
								shouldDigestItem &= pred.AsPred().AcidTier >= preyItem.AsFood().AcidResistTier;
								if (shouldDigestItem)
								{
									hasDoneDigestionTick = true;
									prey.NoHealth = preyItem.TakeDigestionDamage(pred, digestionDamage);
									if (prey.NoHealth)
									{
										string preyItemMod = preyItem.ModItem != null ? preyItem.ModItem.Mod.DisplayName : "Terraria";
										if (!pred.AsPred().mealCount.ContainsKey(preyItemMod + ": " + preyItem.Name))
											pred.AsPred().mealCount.Add(preyItemMod + ": " + preyItem.Name, 0);
										pred.AsPred().mealCount[preyItemMod + ": " + preyItem.Name] += preyItem.stack;
										SoundEngine.PlaySound(
											prey.WeightLeftToDigest < 0.3 ? pred.AsPred().SmallBurps with { Pitch = pred.AsPred().BurpPitchOffset }
											: pred.AsPred().StandardBurps with { Pitch = pred.AsPred().BurpPitchOffset },
											pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
										);
									}
								}
								break;
						}
					}
				}
				else
				{
					prey.UpdateInStomach?.Invoke(null, pred, true);

					double absorptionRate = pred.AsPred().PreyAbsorptionRatePerTick / (double)pred.AsPred().StomachTracker?.Prey.Count;
					if (prey.WeightLeftToDigest <= absorptionRate)
					{
						PlayerGaining.AddWeight(pred, prey.WeightLeftToDigest, prey);
						if (pred.AsV2Player().MintTransformation)
							pred.AsV2Player().MintWispSummonMeter += prey.WeightLeftToDigest * prey.CalorieMultiplier * (0.4 + (pred.maxMinions - 1) / 10.0);
						prey.WeightLeftToDigest = 0;
					}
					else
					{
						PlayerGaining.AddWeight(pred, absorptionRate, prey);
						if (pred.AsV2Player().MintTransformation)
							pred.AsV2Player().MintWispSummonMeter += absorptionRate * prey.CalorieMultiplier * (0.4 + (pred.maxMinions - 1) / 10.0);
						prey.WeightLeftToDigest -= absorptionRate;
						pred.AsPred().WellFed_Multiplier += prey.WellFedPower * prey.WeightLeftToDigest;
					}

					switch (prey.Type)
					{
						case PreyType.Liquid:
							switch (prey.ExactType)
							{
								case LiquidID.Water:
									break;
								case LiquidID.Lava:
									if (!pred.AsPred().CanDrinkLavaSafe)
									{
										pred.AddBuff(ModContent.BuffType<MoltenStomach>(), 3);
									}
									break;
								case LiquidID.Honey:
									break;
								case LiquidID.Shimmer:
									if (!pred.AsPred().CanDrinkShimmerSafe)
									{
										if (!pred.AsPred().PrimedForShimmerStomachDeath)
										{
											pred.AsPred().PrimedForShimmerStomachDeath = true;
											pred.AddBuff(ModContent.BuffType<ShimmeringStomach>(), 300);
										}
										else if (!pred.AsPred().ShimmeringTummy)
										{
											pred.AsPred().PrimedForShimmerStomachDeath = false;
											pred.KillMe(
												PlayerDeathReason.ByCustomReason(
													Language.GetTextValueWith(
														Main.rand.NextFromCollection(new List<string>
														{
															"Mods.V2.Death.OverlyHungryPlayer.UnsafeShimmerDrink.1",
															"Mods.V2.Death.OverlyHungryPlayer.UnsafeShimmerDrink.2",
															"Mods.V2.Death.OverlyHungryPlayer.UnsafeShimmerDrink.3",
														}),
														new
														{
															Player = pred.name
														}
													)
												),
												9999,
												0
											);
										}
									}
									break;
							}
							break;
					}
				}
			}
			if (pred.CurrentCaptor() is null)
			{
				if (pred.velocity.LengthSquared() > 0)
				{
					if(!SoundEngine.TryGetActiveSound(pred.AsPred().BellySlosh, out ActiveSound slosh))
					{
						pred.AsPred().BellySlosh = SoundEngine.PlaySound(Sloshes.Humanoid.Standard with { Volume = pred.AsPred().StomachSize * 0.75f }, pred.TrueCenter());
						SoundEngine.TryGetActiveSound(pred.AsPred().BellySlosh, out slosh);	
					}

					slosh.Position = pred.TrueCenter();
					slosh.Volume = (float)pred.AsPred().StomachFullness * 0.75f;
					// SoundEngine.PlaySound(pred.AsPred().BellySloshes with { Volume = pred.AsPred().StomachSize }, pred.TrueCenter());
				}

				bool stomachNoisesPlaying = SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out ActiveSound stomachNoises);
				if (!stomachNoisesPlaying)
				{
					pred.AsPred().ActiveStomachNoises = SoundEngine.PlaySound(
						(V2.GetFooled
							? StomachNoises.AprilFools
							: StomachNoises.Muffled) with
						{ Volume = 0.25f + (0.2f * pred.AsPred().StomachSize) },
						pred.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;
				
				stomachNoises.Position = pred.TrueCenter();
				stomachNoises.Volume = 0.25f;
				stomachNoises.Volume += 0.2f * pred.AsPred().StomachSize;
			}
		}

		public static void UpdateGeneralPredGoalsLogic(Player pred)
		{
			if (pred.sleeping.FullyFallenAsleep)
			{
				if (pred.AsPred().StomachWeightAtSleepStart == -1.0)
					pred.AsPred().StomachWeightAtSleepStart = pred.AsPred().StomachWeight;

				if (pred.AsPred().StomachWeight == 0.0 && pred.AsPred().StomachWeightAtSleepStart > 0.0 && pred.AsPred().StomachWeightAtSleepStart >= SleepSpeedsDigestion.FlatFullnessThreshold)
					ModContent.GetInstance<SleepSpeedsDigestion>().TrySetCompletion(pred);
			}
			else
				pred.AsPred().StomachWeightAtSleepStart = -1.0;

			if (pred.AsPred().StomachFullness / pred.AsPred().StomachCapacity > TooFull.FullnessThreshold)
			{
				pred.AsPred().OverfullTime += 1;
				if (pred.AsPred().OverfullTime >= TooFull.TimeThreshold)
					ModContent.GetInstance<TooFull>().TrySetCompletion(pred);
			}
			else
				pred.AsPred().OverfullTime = 0;
		}

		public static void UpdateWellFed(Player player)
		{
			player.AsPred().WellFed_Multiplier = 0;
			if (player.HasBuff(BuffID.WellFed3))
				player.AsPred().WellFed_Multiplier += 1.5;
			else if (player.HasBuff(BuffID.WellFed2))
				player.AsPred().WellFed_Multiplier += 1;
			else if (player.HasBuff(BuffID.WellFed))
				player.AsPred().WellFed_Multiplier += 0.5;
		}

		public static NetworkText GetDigestedPlayerDeathReason(Player player, Player prey)
		{
			if (player.whoAmI == prey.whoAmI)
			{
				return NetworkText.FromKey(
					"Mods.V2.Death.DigestedPlayer.Paradox",
					prey.name
				);
			}
			List<string> deathMessageKeyList = [
				"Mods.V2.Death.DigestedPlayer.Universal.1",
				"Mods.V2.Death.DigestedPlayer.Universal.2",
				"Mods.V2.Death.DigestedPlayer.Universal.3",
				"Mods.V2.Death.DigestedPlayer.Universal.4",
				"Mods.V2.Death.DigestedPlayer.Universal.5",
				"Mods.V2.Death.DigestedPlayer.Universal.6",
				"Mods.V2.Death.DigestedPlayer.Universal.7",
				"Mods.V2.Death.DigestedPlayer.Universal.8",
				"Mods.V2.Death.DigestedPlayer.Universal.9",
				"Mods.V2.Death.DigestedPlayer.Universal.10",
				"Mods.V2.Death.DigestedPlayer.Universal.11",
				"Mods.V2.Death.DigestedPlayer.Universal.12",
				"Mods.V2.Death.DigestedPlayer.Universal.13",
				"Mods.V2.Death.DigestedPlayer.Universal.14",
				"Mods.V2.Death.DigestedPlayer.Universal.15",
				"Mods.V2.Death.DigestedPlayer.Universal.16",
				"Mods.V2.Death.DigestedPlayer.Universal.17",
				"Mods.V2.Death.DigestedPlayer.Universal.18",
				"Mods.V2.Death.DigestedPlayer.Universal.19",
				"Mods.V2.Death.DigestedPlayer.Universal.20",
				"Mods.V2.Death.DigestedPlayer.Universal.21",
				"Mods.V2.Death.DigestedPlayer.Universal.22",
			];
			if (prey.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathMessageKeyList.AddRange([
					"Mods.V2.Death.DigestedPlayer.Hardcore.1",
					"Mods.V2.Death.DigestedPlayer.Hardcore.2",
					"Mods.V2.Death.DigestedPlayer.Hardcore.3",
					"Mods.V2.Death.DigestedPlayer.Hardcore.4",
				]);
			}
			string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);

			return NetworkText.FromKey(
				finalDeathReasonKey,
				prey.name,
				player.name
			);
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Player.CurrentCaptor() is not null)
			{

			}
			else
			{
				if (Player.AsPred().MoltenTummy)
				{
					damageSource = PlayerDeathReason.ByCustomReason(
						Language.GetTextValueWith(
							Main.rand.NextFromCollection(new List<string>
							{
								"Mods.V2.Death.OverlyHungryPlayer.UnsafeLavaDrink.1",
								"Mods.V2.Death.OverlyHungryPlayer.UnsafeLavaDrink.2",
								"Mods.V2.Death.OverlyHungryPlayer.UnsafeLavaDrink.3",
							}),
							new
							{
								Player = Player.name
							}
						)
					);
				}
			}
			return true;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			if (StomachTracker is not null)
			{
				if (Player.CurrentCaptor() is not null)
				{
					foreach (PreyData prey in StomachTracker.Prey)
					{
						Player.CurrentCaptor().QueueNewPrey(prey);
					}
				}
				StomachTracker.Prey.Clear();
			}

			InPredStatsMenu = false;
		}

		public override void UpdateDead()
		{
			Player.AsPred().PrimedForShimmerStomachDeath = false;
			Player.AsPred().Stomachache = 0;
		}

		public override void OnRespawn()
		{
			if (Player.SpawnX != -1 && Main.rand.NextBool(7, 1000000))
			{
				Swallow(Player, Player);
			}
		}
		public static void CountDigestionKillForBannersAndDropThem(Player player, NPC npc)
		{
			int num = Item.NPCtoBanner(npc.BannerID());
			if (num <= 0 || npc.ExcludedFromDeathTally())
				return;

			NPC.killCount[num]++;
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendData(MessageID.NPCKillCountDeathTally, -1, -1, null, num);

			int num2 = ItemID.Sets.KillsToBanner[Item.BannerToItem(num)];
			if (NPC.killCount[num] % num2 == 0 && num > 0)
			{
				int npcID = Item.BannerToNPC(num);
				int num4 = npc.lastInteraction;
				if (!Main.player[num4].active || Main.player[num4].dead)
					num4 = npc.FindClosestPlayer();

				NetworkText networkText = NetworkText.FromLiteral(Language.GetTextValueWith("Mods.V2.Death.DigestedEnemiesAnnouncement", new
				{
					Pred = player.name,
					Number = NPC.killCount[num],
					Prey = NetworkText.FromKey(Lang.GetNPCName(npcID).Key)
				}));

				if (Main.netMode == NetmodeID.SinglePlayer)
					Main.NewText(networkText.ToString(), 250, 250, 0);
				else if (Main.netMode == NetmodeID.Server)
					ChatHelper.BroadcastChatMessage(networkText, new Color(250, 250, 0));

				int num5 = Item.BannerToItem(num);
				Vector2 vector = npc.position;
				if (num4 >= 0 && num4 < 255)
					vector = Main.player[num4].position;

				Item.NewItem(npc.GetSource_Loot(), (int)vector.X, (int)vector.Y, npc.width, npc.height, num5);
			}
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("GLPSpent", GLP.Spent);
			tag.Add("TUMSpent", TUM.Spent);
			tag.Add("ACISpent", ACI.Spent);
			tag.Add("ABSSpent", ABS.Spent);
			foreach (KeyValuePair<string, bool> keyValuePair in Player.AsPred().PermanentUpgradesGained)
			{
				tag.Add("[PERM UPGRADES] " + keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<string, int> keyValuePair in Player.AsPred().mealCount)
			{
				tag.Add("[DIGESTED] " + keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<string, int> keyValuePair in Player.AsPred().drinkCount)
			{
				tag.Add("[DRANK] " + keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<string, bool> keyValuePair in Player.AsPred().GoalsCompleted)
			{
				tag.Add("[GOAL] " + keyValuePair.Key, keyValuePair.Value);
			}

			tag.Add("Bae_ExtraWeight", BaeTransformation_ExtraWeight);
			tag.Add("Kronii_ExtraWeight", KroniiTransformation_ExtraWeight);
			tag.Add("Ollie_ExtraWeight", OllieTransformation_ExtraWeight);
			tag.Add("Sora_ExtraWeight", SoraTransformation_ExtraWeight);
			tag.Add("Mint_ExtraWeight", MintTransformation_ExtraWeight);
			tag.Add("Saturation", ActuallyReasonableAmountOfFood);
		}

		public override void LoadData(TagCompound tag)
		{
			GLP.Spent = tag.GetInt("GLPSpent");
			TUM.Spent = tag.GetInt("TUMSpent");
			ACI.Spent = tag.GetInt("ACISpent");
			ABS.Spent = tag.GetInt("ABSSpent");
			PermanentUpgradesGained = [];
			mealCount = [];
			drinkCount = [];
			GoalsCompleted = [];
			foreach (KeyValuePair<string, object> keyValuePair in tag)
			{
				if (keyValuePair.Key.StartsWith("[PERM UPGRADES] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 16);
					bool permUpgradeUsed = tag.GetBool(keyValuePair.Key);
					PermanentUpgradesGained.Add(realKey, permUpgradeUsed);
					continue;
				}
				if (keyValuePair.Key.StartsWith("[DIGESTED] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 11);
					int specificMealCount = tag.GetInt(keyValuePair.Key);
					mealCount.Add(realKey, specificMealCount);
					continue;
				}
				if (keyValuePair.Key.StartsWith("[DRANK] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 8);
					int specificDrinkCount = tag.GetInt(keyValuePair.Key);
					drinkCount.Add(realKey, specificDrinkCount);
					continue;
				}
				if (keyValuePair.Key.StartsWith("[GOAL] "))
				{
					string realKey = keyValuePair.Key.Remove(0, 7);
					bool completeState = tag.GetBool(keyValuePair.Key);
					GoalsCompleted.Add(realKey, completeState);
					continue;
				}
			}
			BaeTransformation_ExtraWeight = tag.GetDouble("Bae_ExtraWeight");
			KroniiTransformation_ExtraWeight = tag.GetDouble("Kronii_ExtraWeight");
			OllieTransformation_ExtraWeight = tag.GetDouble("Ollie_ExtraWeight");
			SoraTransformation_ExtraWeight = tag.GetDouble("Sora_ExtraWeight");
			MintTransformation_ExtraWeight = tag.GetDouble("Mint_ExtraWeight");
			ActuallyReasonableAmountOfFood = tag.GetDouble("Saturation");
		}

		public bool LootDigested()
		{
			if (!LootRecentlyDigested) return LootRecentlyDigested;
			LootRecentlyDigested = false;
			return true;
		}

		public void MarkLootDigested()
		{
			LootRecentlyDigested = true;
		}
	}
}
