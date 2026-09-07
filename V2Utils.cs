using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.NPCs.Voraria.Hallow;
using System.Collections;
using V2.PlayerHandling;
using V2.NPCs;
using V2.Projectiles;
using System.Security.Cryptography.X509Certificates;
using static Terraria.ID.ContentSamples.CreativeHelper;
using V2.Items;
using V2.Items.ItemGroupUtils;

namespace V2
{
	public enum MealTime
	{
		Breakfast,
		BetweenBreakfastAndLunch,
		Lunch,
		BetweenLunchAndDinner,
		Dinner,
		LateNightSnacking
	}

	public static class V2Colors
	{
		public static class Basic
		{
			public static Color LightGray => new Color(170, 170, 170);
			public static Color LightRed => new Color(255, 85, 85);
			public static Color Red => new Color(170, 0, 0);
			public static Color Gold => new Color(255, 170, 0);
			public static Color Aqua => new Color(0, 255, 255);
		}

		/// <summary>
		/// Special, royal shade of green used exclusively by Avarsician royalty.
		/// </summary>
		public static Color RoyalAcidicGreen => new Color(22, 114, 0);

		/// <summary>
		/// Special shade of carmine used exclusively by yours truly.<br/>
		/// </summary>
		public static Color CarmineThread => new Color(150, 0, 24);
	}

	public static class V2Utils
	{
		public static void DebugPointMarker(Vector2 position)
		{
			Texture2D p = ModContent.Request<Texture2D>("V2/DebugPoint").Value;

			Main.spriteBatch.Draw(p, position - p.Size()/2, Color.White);
		}
        public static class ItemIDSets
        {
            public static List<int> LargeGems =>
            [
                ItemID.LargeAmber,
                ItemID.LargeAmethyst,
                ItemID.LargeDiamond,
                ItemID.LargeEmerald,
                ItemID.LargeRuby,
                ItemID.LargeSapphire,
                ItemID.LargeTopaz,
            ];

            public static List<int> Dyes =>
            [
                ItemID.RedDye,
                ItemID.OrangeDye,
                ItemID.YellowDye,
                ItemID.LimeDye,
                ItemID.GreenDye,
                ItemID.TealDye,
                ItemID.CyanDye,
                ItemID.SkyBlueDye,
                ItemID.BlueDye,
                ItemID.PurpleDye,
                ItemID.VioletDye,
                ItemID.PinkDye,
                ItemID.BlackDye,
                ItemID.BrownDye,
                ItemID.SilverDye,
                ItemID.BrightRedDye,
                ItemID.BrightOrangeDye,
                ItemID.BrightYellowDye,
                ItemID.BrightLimeDye,
                ItemID.BrightGreenDye,
                ItemID.BrightTealDye,
                ItemID.BrightCyanDye,
                ItemID.BrightSkyBlueDye,
                ItemID.BrightBlueDye,
                ItemID.BrightPurpleDye,
                ItemID.BrightVioletDye,
                ItemID.BrightPinkDye,
                ItemID.BrightBrownDye,
                ItemID.BrightSilverDye,
                ItemID.RedandBlackDye,
                ItemID.OrangeandBlackDye,
                ItemID.YellowandBlackDye,
                ItemID.LimeandBlackDye,
                ItemID.GreenandBlackDye,
                ItemID.TealandBlackDye,
                ItemID.CyanandBlackDye,
                ItemID.SkyBlueandBlackDye,
                ItemID.BlueandBlackDye,
                ItemID.PurpleandBlackDye,
                ItemID.VioletandBlackDye,
                ItemID.PinkandBlackDye,
                ItemID.BrownAndBlackDye,
                ItemID.SilverAndBlackDye,
                ItemID.RedandSilverDye,
                ItemID.OrangeandSilverDye,
                ItemID.YellowandSilverDye,
                ItemID.LimeandSilverDye,
                ItemID.GreenandSilverDye,
                ItemID.TealandSilverDye,
                ItemID.CyanandSilverDye,
                ItemID.SkyBlueandSilverDye,
                ItemID.BlueandSilverDye,
                ItemID.PurpleandSilverDye,
                ItemID.VioletandSilverDye,
                ItemID.PinkandSilverDye,
                ItemID.BrownAndSilverDye,
                ItemID.BlackAndWhiteDye,
                ItemID.FlameDye,
                ItemID.GreenFlameDye,
                ItemID.BlueFlameDye,
                ItemID.YellowGradientDye,
                ItemID.CyanGradientDye,
                ItemID.VioletGradientDye,
                ItemID.RainbowDye,
                ItemID.IntenseFlameDye,
                ItemID.IntenseGreenFlameDye,
                ItemID.IntenseBlueFlameDye,
                ItemID.IntenseRainbowDye,
                ItemID.FlameAndBlackDye,
                ItemID.GreenFlameAndBlackDye,
                ItemID.BlueFlameAndBlackDye,
                ItemID.FlameAndSilverDye,
                ItemID.GreenFlameAndSilverDye,
                ItemID.BlueFlameAndSilverDye,
                ItemID.AcidDye,
                ItemID.BlueAcidDye,
                ItemID.RedAcidDye,
                ItemID.ChlorophyteDye,
                ItemID.GelDye,
                ItemID.MushroomDye,
                ItemID.GrimDye,
                ItemID.HadesDye,
                ItemID.BurningHadesDye,
                ItemID.ShadowflameHadesDye,
                ItemID.LivingOceanDye,
                ItemID.LivingFlameDye,
                ItemID.LivingRainbowDye,
                ItemID.MartianArmorDye,
                ItemID.MidnightRainbowDye,
                ItemID.MirageDye,
                ItemID.NegativeDye,
                ItemID.PixieDye,
                ItemID.PhaseDye,
                ItemID.PurpleOozeDye,
                ItemID.ReflectiveDye,
                ItemID.ReflectiveCopperDye,
                ItemID.ReflectiveGoldDye,
                ItemID.ReflectiveObsidianDye,
                ItemID.ReflectiveMetalDye,
                ItemID.ReflectiveSilverDye,
                ItemID.ShadowDye,
                ItemID.ShiftingSandsDye,
                ItemID.DevDye,
                ItemID.TwilightDye,
                ItemID.WispDye,
                ItemID.InfernalWispDye,
                ItemID.UnicornWispDye,
                ItemID.PinkGelDye,
                ItemID.ShiftingPearlSandsDye,
                ItemID.NebulaDye,
                ItemID.SolarDye,
                ItemID.StardustDye,
                ItemID.VortexDye,
                ItemID.VoidDye,
                ItemID.LokisDye,
                ItemID.TeamDye,
                ItemID.BloodbathDye,
                ItemID.FogboundDye,
                ItemID.HallowBossDye,
            ];

			#region Balloons
			public static List<int> BasicBalloons =>
			[
				ItemID.ShinyRedBalloon,
				ItemID.GelBalloon,
				ItemID.BalloonPufferfish,
				ItemID.SharkronBalloon
			];
			
			public static List<int> BalloonsWithClouds =>
			[
				ItemID.BlizzardinaBalloon,
				ItemID.FartInABalloon,
				ItemID.CloudinaBalloon,
				ItemID.SandstorminaBalloon
			];

			public static List<int> BalloonsWithHorseshoe =>
			[
				ItemID.BalloonHorseshoeFart,
				ItemID.BalloonHorseshoeHoney,
				ItemID.BalloonHorseshoeSharkron,
				ItemID.BlueHorseshoeBalloon,
				ItemID.WhiteHorseshoeBalloon,
				ItemID.YellowHorseshoeBalloon,
			];

			public static List<int> BalloonTiles =>
			[
				ItemID.SillyBalloonGreen,
				ItemID.SillyBalloonGreenWall,
				ItemID.SillyBalloonPink,
				ItemID.SillyBalloonPinkWall,
				ItemID.SillyBalloonPurple,
				ItemID.SillyBalloonPurpleWall,
			];
			
			public static List<int> EquipableBalloons => [..BasicBalloons, ..BalloonsWithClouds, ..BalloonsWithHorseshoe];
			#endregion Balloons

			public static List<int> Baits =>
			[
				ItemID.ApprenticeBait,
				ItemID.JourneymanBait,
				ItemID.MasterBait,
				ItemID.SharkBait
			];


			public static List<int> RegularBanners =>
			[
				ItemID.RedBanner,
				ItemID.GreenBanner,
				ItemID.BlueBanner,
				ItemID.YellowBanner,
				ItemID.MarchingBonesBanner,
				ItemID.NecromanticSign,
				ItemID.RustedCompanyStandard,
				ItemID.RaggedBrotherhoodSigil,
				ItemID.MoltenLegionFlag,
				ItemID.DiabolicSigil,
				ItemID.WorldBanner,
				ItemID.SunBanner,
				ItemID.GravityBanner,
				ItemID.HellboundBanner,
				ItemID.HellHammerBanner,
				ItemID.HelltowerBanner,
				ItemID.LostHopesofManBanner,
				ItemID.ObsidianWatcherBanner,
				ItemID.LavaEruptsBanner,
				ItemID.AnkhBanner,
				ItemID.SnakeBanner,
				ItemID.OmegaBanner,
			];

            public static List<int> EnemyBanners =>
            [
                ItemID.SlimeBanner,
                ItemID.GreenSlimeBanner,
                ItemID.PurpleSlimeBanner,
                ItemID.UmbrellaSlimeBanner,
                ItemID.RedSlimeBanner,
                ItemID.YellowSlimeBanner,
                ItemID.BlackSlimeBanner,
                ItemID.MotherSlimeBanner,
                ItemID.DungeonSlimeBanner,
                ItemID.PinkyBanner,
                ItemID.JungleSlimeBanner,
                ItemID.SpikedJungleSlimeBanner,
                ItemID.IceSlimeBanner,
                ItemID.SpikedIceSlimeBanner,
                ItemID.SandSlimeBanner,
                ItemID.LavaSlimeBanner,
                ItemID.ShimmerSlimeBanner,
                ItemID.ToxicSludgeBanner,
                ItemID.SlimerBanner,
                ItemID.CorruptSlimeBanner,
                ItemID.CrimslimeBanner,
                ItemID.GastropodBanner,
                ItemID.IlluminantSlimeBanner,
                ItemID.RainbowSlimeBanner,
                ItemID.BirdBanner,
                ItemID.BunnyBanner,
                ItemID.GoldfishBanner,
                ItemID.ZombieBanner,
                ItemID.DemonEyeBanner,
                ItemID.GoblinScoutBanner,
                ItemID.GnomeBanner,
                ItemID.HarpyBanner,
                ItemID.CrabBanner,
                ItemID.PinkJellyfishBanner,
                ItemID.SquidBanner,
                ItemID.SeaSnailBanner,
                ItemID.SharkBanner,
                ItemID.PossessedArmorBanner,
                ItemID.WanderingEyeBanner,
                ItemID.WraithBanner,
                ItemID.WerewolfBanner,
                ItemID.WyvernBanner,
                ItemID.BatBanner,
                ItemID.CochinealBeetleBanner,
                ItemID.CrawdadBanner,
                ItemID.GiantShellyBanner,
                ItemID.SalamanderBanner,
                ItemID.WormBanner,
                ItemID.NypmhBanner, //lmao
                ItemID.SkeletonBanner,
                ItemID.TimBanner,
                ItemID.UndeadMinerBanner,
                ItemID.JellyfishBanner,
                ItemID.ArmoredSkeletonBanner,
                ItemID.SkeletonArcherBanner,
                ItemID.GiantBatBanner,
                ItemID.MimicBanner,
                ItemID.RockGolemBanner,
                ItemID.RuneWizardBanner,
                ItemID.AnglerFishBanner,
                ItemID.GreenJellyfishBanner,
                ItemID.GraniteFlyerBanner,
                ItemID.GraniteGolemBanner,
                ItemID.GreekSkeletonBanner,
                ItemID.MedusaBanner,
                ItemID.SpiderBanner,
                ItemID.BlackRecluseBanner,
                ItemID.AnomuraFungusBanner,
                ItemID.FungiBulbBanner,
                ItemID.MushiLadybugBanner,
                ItemID.SporeBatBanner,
                ItemID.SporeSkeletonBanner,
                ItemID.SporeZombieBanner,
                ItemID.FungoFishBanner,
                ItemID.CyanBeetleBanner,
                ItemID.IceBatBanner,
                ItemID.PenguinBanner,
                ItemID.SnowFlinxBanner,
                ItemID.UndeadVikingBanner,
                ItemID.ZombieEskimoBanner,
                ItemID.ArmoredVikingBanner,
                ItemID.IceElementalBanner,
                ItemID.IceTortoiseBanner,
                ItemID.IcyMermanBanner,
                ItemID.PigronBanner,
                ItemID.WolfBanner,
                ItemID.DoctorBonesBanner,
                ItemID.HornetBanner,
                ItemID.JungleBatBanner,
                ItemID.ManEaterBanner,
                ItemID.SnatcherBanner,
                ItemID.LacBeetleBanner,
                ItemID.PiranhaBanner,
                ItemID.AngryTrapperBanner,
                ItemID.DerplingBanner,
                ItemID.TortoiseBanner,
                ItemID.GiantFlyingFoxBanner,
                ItemID.JungleCreeperBanner,
                ItemID.ArapaimaBanner,
                ItemID.MossHornetBanner,
                ItemID.MothBanner,
                ItemID.LihzahrdBanner,
                ItemID.FlyingSnakeBanner,
                ItemID.AntlionBanner,
                ItemID.WalkingAntlionBanner,
                ItemID.FlyingAntlionBanner,
                ItemID.LarvaeAntlionBanner,
                ItemID.TombCrawlerBanner,
                ItemID.VultureBanner,
                ItemID.DesertBasiliskBanner,
                ItemID.DesertDjinnBanner,
                ItemID.DesertLamiaBanner,
                ItemID.DesertGhoulBanner,
                ItemID.DuneSplicerBanner,
                ItemID.RavagerScorpionBanner,
                ItemID.MummyBanner,
                ItemID.BoneSerpentBanner,
                ItemID.DemonBanner,
                ItemID.FireImpBanner,
                ItemID.HellbatBanner,
                ItemID.LavaBatBanner,
                ItemID.RedDevilBanner,
                ItemID.AngryBonesBanner,
                ItemID.CursedSkullBanner,
                ItemID.SkeletonMageBanner,
                ItemID.BlueArmoredBonesBanner,
                ItemID.RustyArmoredBonesBanner,
                ItemID.HellArmoredBonesBanner,
                ItemID.DiablolistBanner,
                ItemID.NecromancerBanner,
                ItemID.RaggedCasterBanner,
                ItemID.GiantCursedSkullBanner,
                ItemID.DungeonSpiritBanner,
                ItemID.BoneLeeBanner,
                ItemID.SkeletonCommandoBanner,
                ItemID.SkeletonSniperBanner,
                ItemID.TacticalSkeletonBanner,
                ItemID.PaladinBanner,
                ItemID.DevourerBanner,
                ItemID.EaterofSoulsBanner,
                ItemID.CorruptorBanner,
                ItemID.ClingerBanner,
                ItemID.BigMimicCorruptionBanner,
                ItemID.CursedHammerBanner,
                ItemID.DarkMummyBanner,
                ItemID.WorldFeederBanner,
                ItemID.BloodCrawlerBanner,
                ItemID.FaceMonsterBanner,
                ItemID.CrimeraBanner,
                ItemID.BloodFeederBanner,
                ItemID.BloodJellyBanner,
                ItemID.BloodMummyBanner,
                ItemID.IchorStickerBanner,
                ItemID.FloatyGrossBanner,
                ItemID.CrimsonAxeBanner,
                ItemID.BigMimicCrimsonBanner,
                ItemID.HerplingBanner,
                ItemID.ChaosElementalBanner,
                ItemID.IlluminantBatBanner,
                ItemID.BigMimicHallowBanner,
                ItemID.PixieBanner,
                ItemID.UnicornBanner,
                ItemID.EnchantedSwordBanner,
                ItemID.LightMummyBanner,
                ItemID.MeteorHeadBanner,
                ItemID.FlyingFishBanner,
                ItemID.AngryNimbusBanner,
                ItemID.RaincoatZombieBanner,
                ItemID.IceGolemBanner,
                ItemID.SandElementalBanner,
                ItemID.TumbleweedBanner,
                ItemID.SandsharkBanner,
                ItemID.SandsharkHallowedBanner,
                ItemID.SandsharkCorruptBanner,
                ItemID.SandsharkCrimsonBanner,
                ItemID.DandelionBanner,
                ItemID.BloodZombieBanner,
                ItemID.DripplerBanner,
                ItemID.ZombieMermanBanner,
                ItemID.BloodEelBanner,
                ItemID.GoblinSharkBanner,
                ItemID.EyeballFlyingFishBanner,
                ItemID.BloodNautilusBanner,
                ItemID.BloodSquidBanner,
                ItemID.ClownBanner,
                ItemID.CorruptBunnyBanner,
                ItemID.CorruptGoldfishBanner,
                ItemID.CorruptPenguinBanner,
                ItemID.CrimsonBunnyBanner,
                ItemID.CrimsonGoldfishBanner,
                ItemID.CrimsonPenguinBanner,
                ItemID.TheGroomBanner,
                ItemID.TheBrideBanner,
                ItemID.GoblinArcherBanner,
                ItemID.GoblinThiefBanner,
                ItemID.GoblinPeonBanner,
                ItemID.GoblinWarriorBanner,
                ItemID.GoblinSorcererBanner,
                ItemID.GoblinSummonerBanner,
                ItemID.RavenBanner,
                ItemID.GhostBanner,
                ItemID.HoppinJackBanner,
                ItemID.ParrotBanner,
                ItemID.PirateCaptainBanner,
                ItemID.PirateDeadeyeBanner,
                ItemID.PirateCrossbowerBanner,
                ItemID.PirateCorsairBanner,
                ItemID.PirateBanner,
                ItemID.MisterStabbyBanner,
                ItemID.SnowBallaBanner,
                ItemID.SnowmanGangstaBanner,
                ItemID.ButcherBanner,
                ItemID.VampireBanner,
                ItemID.CreatureFromTheDeepBanner,
                ItemID.DeadlySphereBanner,
                ItemID.DrManFlyBanner,
                ItemID.FritzBanner,
                ItemID.FrankensteinBanner,
                ItemID.MothronBanner,
                ItemID.ReaperBanner,
                ItemID.EyezorBanner,
                ItemID.NailheadBanner,
                ItemID.PsychoBanner,
                ItemID.SwampThingBanner,
                ItemID.ThePossessedBanner,
                ItemID.HeadlessHorsemanBanner,
                ItemID.ScarecrowBanner,
                ItemID.PoltergeistBanner,
                ItemID.HellhoundBanner,
                ItemID.SplinterlingBanner,
                ItemID.ZombieElfBanner,
                ItemID.ElfArcherBanner,
                ItemID.ElfCopterBanner,
                ItemID.GingerbreadManBanner,
                ItemID.FlockoBanner,
                ItemID.KrampusBanner,
                ItemID.PresentMimicBanner,
                ItemID.NutcrackerBanner,
                ItemID.YetiBanner,
                ItemID.MartianBrainscramblerBanner,
                ItemID.MartianDroneBanner,
                ItemID.MartianEngineerBanner,
                ItemID.MartianGigazapperBanner,
                ItemID.MartianGreyGruntBanner,
                ItemID.MartianOfficerBanner,
                ItemID.MartianRaygunnerBanner,
                ItemID.MartianScutlixGunnerBanner,
                ItemID.ScutlixBanner,
                ItemID.MartianWalkerBanner,
                ItemID.MartianTeslaTurretBanner,
                ItemID.BlueCultistArcherBanner,
                ItemID.BlueCultistCasterBanner,
                ItemID.BlueCultistFighterBanner,
                ItemID.WhiteCultistArcherBanner,
                ItemID.WhiteCultistCasterBanner,
                ItemID.WhiteCultistFighterBanner,
                ItemID.VortexLarvaBanner,
                ItemID.VortexHornetBanner,
                ItemID.VortexHornetQueenBanner,
                ItemID.VortexSoldierBanner,
                ItemID.VortexRiflemanBanner,
                ItemID.NebulaBrainBanner,
                ItemID.NebulaHeadcrabBanner,
                ItemID.NebulaSoldierBanner,
                ItemID.NebulaBeastBanner,
                ItemID.StardustLargeCellBanner,
                ItemID.StardustSmallCellBanner,
                ItemID.StardustSoldierBanner,
                ItemID.StardustWormBanner,
                ItemID.StardustSpiderBanner,
                ItemID.StardustJellyfishBanner,
                ItemID.SolarCoriteBanner,
                ItemID.SolarSolenianBanner,
                ItemID.SolarDrakomireBanner,
                ItemID.SolarDrakomireRiderBanner,
                ItemID.SolarSrollerBanner,
                ItemID.SolarCrawltipedeBanner,
                ItemID.DD2GoblinBanner,
                ItemID.DD2GoblinBomberBanner,
                ItemID.DD2JavelinThrowerBanner,
                ItemID.DD2SkeletonBanner,
                ItemID.DD2WyvernBanner,
                ItemID.DD2DrakinBanner,
                ItemID.DD2LightningBugBanner,
                ItemID.DD2KoboldBanner,
                ItemID.DD2KoboldFlyerBanner,
                ItemID.DD2WitherBeastBanner,
            ];

            /*public static Dictionary<int, (double HPMult, double CalorieMult, double WellFedPower)> FoodItems => new Dictionary<int, (double HPMult, double CalorieMult, double WellFedPower)>
            {
                { ItemID.Marshmallow, (0.8, 1.1, 1) },
                { ItemID., (0.8, 1.1, 1.1) },
            };*/
        }

		public static class NPCIDSets
		{
			public static List<int> Slimes =>
			[
				NPCID.GreenSlime,
				NPCID.BlueSlime,
				NPCID.PurpleSlime,
				NPCID.YellowSlime,
				NPCID.RedSlime,
				NPCID.BlackSlime,
				NPCID.SlimeRibbonGreen,
				NPCID.SlimeRibbonRed,
				NPCID.SlimeRibbonWhite,
				NPCID.SlimeRibbonYellow,
				NPCID.MotherSlime,
				NPCID.BabySlime,
				NPCID.JungleSlime,
				NPCID.SpikedJungleSlime,
				NPCID.KingSlime,
				NPCID.SlimeSpiked,
				NPCID.QueenSlimeBoss,
				NPCID.QueenSlimeMinionBlue,
				NPCID.QueenSlimeMinionPink,
				NPCID.QueenSlimeMinionPurple,
				NPCID.RainbowSlime,
				NPCID.CorruptSlime,
				NPCID.Crimslime,
				NPCID.ToxicSludge,
				NPCID.Slimer,
				NPCID.Slimer2,
				NPCID.BigCrimslime,
				NPCID.LittleCrimslime,
				NPCID.IlluminantSlime,
				NPCID.Gastropod,
				NPCID.ShimmerSlime,
				NPCID.SlimeMasked,
				NPCID.UmbrellaSlime,
				NPCID.Pinky,
				NPCID.GoldenSlime,
				NPCID.TownSlimeBlue,
				NPCID.TownSlimeCopper,
				NPCID.TownSlimeGreen,
				NPCID.TownSlimeOld,
				NPCID.TownSlimePurple,
				NPCID.TownSlimeRainbow,
				NPCID.TownSlimeRed,
				NPCID.TownSlimeYellow,
				NPCID.BoundTownSlimeOld,
				NPCID.BoundTownSlimePurple,
				NPCID.BoundTownSlimeYellow,
				NPCID.LavaSlime,
				NPCID.DungeonSlime,
				NPCID.IceSlime,
				NPCID.SpikedIceSlime,
				NPCID.SandSlime,
				ModContent.NPCType<GumdropSlime>()
			];

			public static List<int> GemCritters =>
			[
				NPCID.GemBunnyAmber,
				NPCID.GemBunnyAmethyst,
				NPCID.GemBunnyDiamond,
				NPCID.GemBunnyEmerald,
				NPCID.GemBunnyRuby,
				NPCID.GemBunnySapphire,
				NPCID.GemBunnyTopaz,
				NPCID.GemSquirrelAmber,
				NPCID.GemSquirrelAmethyst,
				NPCID.GemSquirrelDiamond,
				NPCID.GemSquirrelEmerald,
				NPCID.GemSquirrelRuby,
				NPCID.GemSquirrelSapphire,
				NPCID.GemSquirrelTopaz,
			];

			public static List<int> GoldCritters =>
			[
				NPCID.GoldBird,
				NPCID.GoldBunny,
				NPCID.GoldButterfly,
				NPCID.GoldDragonfly,
				NPCID.GoldenSlime,
				NPCID.GoldFrog,
				NPCID.GoldGoldfish,
				NPCID.GoldGrasshopper,
				NPCID.GoldLadyBug,
				NPCID.GoldMouse,
				NPCID.GoldSeahorse,
				NPCID.GoldWaterStrider,
				NPCID.GoldWorm,
				NPCID.SquirrelGold,
			];

			public static List<int> LivingWeapons =>
			[
				NPCID.EnchantedSword,
				NPCID.CursedHammer,
				NPCID.CrimsonAxe,
			];

			public static List<int> Mimics =>
			[
				NPCID.Mimic,
				NPCID.IceMimic,
				NPCID.PresentMimic,
				NPCID.BigMimicCorruption,
				NPCID.BigMimicCrimson,
				NPCID.BigMimicHallow,
				NPCID.BigMimicJungle,
			];

			public static List<int> MiniFairies =>
			[
				NPCID.FairyCritterBlue,
				NPCID.FairyCritterGreen,
				NPCID.FairyCritterPink,
			];

			public static List<int> Butterflies =>
			[
				NPCID.Butterfly,
				NPCID.GoldButterfly,
				NPCID.HellButterfly,
				NPCID.EmpressButterfly,
			];
		}
		/// <summary>
		/// Takes the given amount of readable time and converts it to a concrete frame count.<br/>
		/// Measurements are all based on real-life time, and assume a constant FPS (frames per second rate) of 60.<br/>
		/// Used for the purpose of setting time-related fields, such as enemy attack delays or status effect durations.<br/>
		/// </summary>
		/// <param name="hours">
		/// The number of hours to convert.<br/>
		/// Defaults to 0, because no sane person ever makes anything last for an hour or more...right?<br/>
		/// </param>
		/// <param name="minutes">
		/// The number of minutes to convert.<br/>
		/// Defaults to 0.<br/>
		/// </param>
		/// <param name="seconds">
		/// The number of seconds to convert.<br/>
		/// Defaults to 0.<br/>
		/// </param>
		/// <param name="frames">
		/// The number of individual frames to add to the converted time.<br/>
		/// Used only for very specific adjustments.<br/>
		/// Defaults to 0. To cover many common use cases for this:<br/>
		/// - 15 is a quarter of a second.<br/>
		/// - 20 is a third of a second.<br/>
		/// - 30 is a half of a second.<br/>
		/// </param>
		/// <returns>The total number of frames needed to cover the input amount of time.</returns>
		public static int SensibleTime(int hours = 0, int minutes = 0, int seconds = 0, int frames = 0)
		{
			int totalFrameCount = hours * 60 * 60 * 60;
			totalFrameCount += minutes * 60 * 60;
			totalFrameCount += seconds * 60;
			totalFrameCount += frames;
			return totalFrameCount;
		}

		/// <summary>
		/// Determines what time it is given the current state of the save, and outputs that time in measures comprehensible to the human mind.<br/>
		/// Assumes 12-hour time.<br/>
		/// </summary>
		/// <param name="pastMorning">
		/// Whether noon has passed for the day or not.<br/>
		/// </param>
		/// <param name="hour">
		/// The current hour.<br/>
		/// Ranges from 1-12.<br/>
		/// </param>
		/// <param name="minute">
		/// The current minute of the current hour.<br/>
		/// Ranges from 0-59.<br/>
		/// </param>
		/// <param name="second">
		/// The current second of the current minute of the current hour.<br/>
		/// Ranges from 0-59.<br/>
		/// </param>
		/// <param name="mealTime">
		/// The time of day that it's currently considered for the purpose of when you got ate, you dirty gut slut.<br/>
		/// If between 5:00 AM and 8:00 AM: <see cref="MealTime.Breakfast"/>.<br/>
		/// If between 8:00 AM and 11:00 AM: <see cref="MealTime.BetweenBreakfastAndLunch"/>.<br/>
		/// If between 11:00 AM and 2:00 PM: <see cref="MealTime.Lunch"/>.<br/>
		/// If between 2:00 PM and 5:00 PM: <see cref="MealTime.BetweenLunchAndDinner"/>.<br/>
		/// If between 5:00 PM and 9:00 PM: <see cref="MealTime.Dinner"/>.<br/>
		/// If between 9:00 PM and 5:00 AM: <see cref="MealTime.LateNightSnacking"/>.<br/>
		/// </param>
		public static void FigureOutWhatTimeItIs(out bool pastMorning, out int hour, out int minute, out int second, out MealTime mealTime)
		{
			pastMorning = false;
			double hours = Main.time;
			if (!Main.dayTime)
				hours += 54000.0;

			hours = hours / 86400.0 * 24.0;
			double mainTimeOffset = 7.5;
			hours = hours - mainTimeOffset - 12.0;
			if (hours < 0.0)
				hours += 24.0;

			if (hours >= 12.0)
				pastMorning = true;

			hour = (int)hours;
			double minutes = hours - (double)hour;
			minute = (int)(minutes * 60.0);
			double seconds = minutes - (double)minute;
			second = (int)(seconds * 60.0);

			if (hour > 12)
				hour -= 12;

			if (hour == 0)
				hour = 12;

			if (!pastMorning)
			{
				switch (hour)
				{
					case 12:
					case 1:
					case 2:
					case 3:
					case 4:
						mealTime = MealTime.LateNightSnacking;
						break;
					case 5:
					case 6:
					case 7:
						mealTime = MealTime.Breakfast;
						break;
					case 8:
					case 9:
					case 10:
						mealTime = MealTime.BetweenBreakfastAndLunch;
						break;
					case 11:
					default:
						mealTime = MealTime.Lunch;
						break;
				}
			}
			else
			{
				switch (hour)
				{
					case 12:
					case 1:
					default:
						mealTime = MealTime.Lunch;
						break;
					case 2:
					case 3:
					case 4:
						mealTime = MealTime.BetweenLunchAndDinner;
						break;
					case 5:
					case 6:
					case 7:
					case 8:
						mealTime = MealTime.Dinner;
						break;
					case 9:
					case 10:
					case 11:
						mealTime = MealTime.LateNightSnacking;
						break;
				}
			}
		}

		public static void AddVorariaItemTooltip(this List<TooltipLine> tooltips, string itemTooltipKey, object tooltipVariables)
		{
			TooltipLine dynamicTooltip = new TooltipLine(
				V2.Instance,
				"V2DynamicTooltip",
				Language.GetTextValueWith(
					"Mods.V2.ItemTooltip." + itemTooltipKey,
					tooltipVariables
				)
			);

			if (tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip")) is TooltipLine tooltipLine)
			{
				foreach (TooltipLine potentialTooltipLine in tooltips)
				{
					if (potentialTooltipLine.Mod == "Terraria" && potentialTooltipLine.Name.Contains("Tooltip"))
						potentialTooltipLine.Hide();
				}
				tooltips.Insert(
					tooltips.IndexOf(tooltipLine) + 1,
					dynamicTooltip
				);
			}
			else if (FindLastTooltipLineBeforeFlavorText(tooltips, out TooltipLine lastPreFlavorLine))
			{
				tooltips.Insert(
					tooltips.IndexOf(lastPreFlavorLine) + 1,
					dynamicTooltip
				);
			}
		}

		public static int TileCountAsPixelCount(double tileCount) => (int)Math.Round(tileCount * 16.0);


		// TO-DO: this shit is dumb. refactor tooltips once tooltip rework happens...assumin' it'll ever happen, that is. why do people love talkin' a big game and playin' none of it?
		// for the moment, what this does is search for each potential tooltip line before Tooltip0 in reverse order and return the first one that isn't null
		public static bool FindLastTooltipLineBeforeFlavorText(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "V2EdibleByNormalUse")
				?? tooltips.FirstOrDefault(x => x.Name == "V2AcidResist")
				?? tooltips.FirstOrDefault(x => x.Name == "V2SizeAsFood")
				?? tooltips.FirstOrDefault(x => x.Name == "V2Durability")
				?? tooltips.FirstOrDefault(x => x.Name == "Material")
				?? tooltips.FirstOrDefault(x => x.Name == "Consumable")
				?? tooltips.FirstOrDefault(x => x.Name == "Ammo")
				?? tooltips.FirstOrDefault(x => x.Name == "Placeable")
				?? tooltips.FirstOrDefault(x => x.Name == "UseManaPerSecond")
				?? tooltips.FirstOrDefault(x => x.Name == "UseMana")
				?? tooltips.FirstOrDefault(x => x.Name == "HealMana")
				?? tooltips.FirstOrDefault(x => x.Name == "HealLife")
				?? tooltips.FirstOrDefault(x => x.Name == "TileBoost")
				?? tooltips.FirstOrDefault(x => x.Name == "HammerPower")
				?? tooltips.FirstOrDefault(x => x.Name == "AxePower")
				?? tooltips.FirstOrDefault(x => x.Name == "PickPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Defense")
				?? tooltips.FirstOrDefault(x => x.Name == "VanityLegal")
				?? tooltips.FirstOrDefault(x => x.Name == "Vanity")
				?? tooltips.FirstOrDefault(x => x.Name == "Quest")
				?? tooltips.FirstOrDefault(x => x.Name == "WandConsumes")
				?? tooltips.FirstOrDefault(x => x.Name == "Equipable")
				?? tooltips.FirstOrDefault(x => x.Name == "BaitPower")
				?? tooltips.FirstOrDefault(x => x.Name == "NeedsBait")
				?? tooltips.FirstOrDefault(x => x.Name == "FishingPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Knockback")
				?? tooltips.FirstOrDefault(x => x.Name == "Speed")
				?? tooltips.FirstOrDefault(x => x.Name == "CritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "Damage")
				?? tooltips.FirstOrDefault(x => x.Name == "SocialDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Social")
				?? tooltips.FirstOrDefault(x => x.Name == "FavoriteNoNoms")
				?? tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Favorite")
				?? tooltips.FirstOrDefault(x => x.Name == "ItemName");
			return line != null;
		}

		public static bool FindFirstTooltipLineThatIsOrComesAfterFlavorText(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "V2DynamicTooltip")
				?? tooltips.FirstOrDefault(x => x.Name == "V2SetBonus")
				?? tooltips.FirstOrDefault(x => x.Name == "V2LongAndFlavorTooltipNotice")
				?? tooltips.FirstOrDefault(x => x.Name == "EtherianManaWarning")
				?? tooltips.FirstOrDefault(x => x.Name == "WellFedExpert")
				?? tooltips.FirstOrDefault(x => x.Name == "BuffTime")
				?? tooltips.FirstOrDefault(x => x.Name == "OneDropLogo")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixDamage")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixCritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixUseMana")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixSize")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixShootSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixKnockback")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccDefense")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccMaxMana")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccCritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccDamage")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccMoveSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "PrefixAccMeleeSpeed")
				?? tooltips.FirstOrDefault(x => x.Name == "SetBonus")
				?? tooltips.FirstOrDefault(x => x.Name == "Expert")
				?? tooltips.FirstOrDefault(x => x.Name == "Master")
				?? tooltips.FirstOrDefault(x => x.Name == "JourneyResearch")
				?? tooltips.FirstOrDefault(x => x.Name == "BestiaryNotes")
				?? tooltips.FirstOrDefault(x => x.Name == "SpecialPrice")
				?? tooltips.FirstOrDefault(x => x.Name == "Price");
			return line != null;
		}

		public static bool FindLastDamageRelatedTooltipLine(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "Knockback")
				?? tooltips.FirstOrDefault(x => x.Name == "Speed")
				?? tooltips.FirstOrDefault(x => x.Name == "CritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "Damage");
			return line != null;
		}

		public static bool FindLastTooltipLineBeforeManaCost(List<TooltipLine> tooltips, out TooltipLine line)
		{
			line = tooltips.FirstOrDefault(x => x.Name == "HealMana")
				?? tooltips.FirstOrDefault(x => x.Name == "HealLife")
				?? tooltips.FirstOrDefault(x => x.Name == "TileBoost")
				?? tooltips.FirstOrDefault(x => x.Name == "HammerPower")
				?? tooltips.FirstOrDefault(x => x.Name == "AxePower")
				?? tooltips.FirstOrDefault(x => x.Name == "PickPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Defense")
				?? tooltips.FirstOrDefault(x => x.Name == "VanityLegal")
				?? tooltips.FirstOrDefault(x => x.Name == "Vanity")
				?? tooltips.FirstOrDefault(x => x.Name == "Quest")
				?? tooltips.FirstOrDefault(x => x.Name == "WandConsumes")
				?? tooltips.FirstOrDefault(x => x.Name == "Equipable")
				?? tooltips.FirstOrDefault(x => x.Name == "BaitPower")
				?? tooltips.FirstOrDefault(x => x.Name == "NeedsBait")
				?? tooltips.FirstOrDefault(x => x.Name == "FishingPower")
				?? tooltips.FirstOrDefault(x => x.Name == "Knockback")
				?? tooltips.FirstOrDefault(x => x.Name == "Speed")
				?? tooltips.FirstOrDefault(x => x.Name == "CritChance")
				?? tooltips.FirstOrDefault(x => x.Name == "Damage")
				?? tooltips.FirstOrDefault(x => x.Name == "SocialDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Social")
				?? tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc")
				?? tooltips.FirstOrDefault(x => x.Name == "Favorite")
				?? tooltips.FirstOrDefault(x => x.Name == "ItemName");
			return line != null;
		}

		public static void InsertNewTooltipLine(ref List<TooltipLine> tooltips, TooltipLine baseLine, int lineOffset, string lineName, string lineContents)
		{
			TooltipLine newLine = new TooltipLine(V2.Instance, lineName, lineContents);
			InsertNewTooltipLine(ref tooltips, baseLine, lineOffset, newLine);
		}
		public static void InsertNewTooltipLine(ref List<TooltipLine> tooltips, TooltipLine baseLine, int lineOffset, TooltipLine newLine)
		{
			if (tooltips.IndexOf(baseLine) + lineOffset > tooltips.Count - 1)
				tooltips.Add(newLine);
			else
				tooltips.Insert(tooltips.IndexOf(baseLine) + lineOffset, newLine);
		}

        // damn terraria code and making shit private methods so i have to copy paste them >:C
        public static void ExtractItemInTum(int ItemID, Entity pred)
        {
			int extractinatorBlockType = 219;
            int num = 5000;
            int num2 = 25;
            int num3 = 50;
            int num4 = -1;
            int num5 = -1;
            int num6 = -1;
            int num7 = 1;
            if (ItemID != 2337)
            {
                if (ItemID != 3347)
                {
                    if (ItemID == 4354)
                    {
                        num = -1;
                        num2 = -1;
                        num3 = -1;
                        num4 = -1;
                        num5 = -1;
                        num7 = -1;
                        num6 = 1;
                    }
                }
                else
                {
                    num /= 3;
                    num2 *= 2;
                    num3 = 20;
                    num4 = 10;
                }
            }
            else
            {
                num = -1;
                num2 = -1;
                num3 = -1;
                num4 = -1;
                num5 = 1;
                num7 = -1;
            }
            int num8 = -1;
            int num9 = 1;
            if (num4 != -1 && Main.rand.NextBool(num4))
            {
                num8 = 3380;
                if (Main.rand.NextBool(5))
                {
                    num9 += Main.rand.Next(2);
                }
                if (Main.rand.NextBool(10))
                {
                    num9 += Main.rand.Next(3);
                }
                if (Main.rand.NextBool(15))
                {
                    num9 += Main.rand.Next(4);
                }
            }
            else if (num7 != -1 && Main.rand.NextBool(2))
            {
                if (Main.rand.NextBool(12000))
                {
                    num8 = 74;
                    if (Main.rand.NextBool(14))
                    {
                        num9 += Main.rand.Next(0, 2);
                    }
                    if (Main.rand.NextBool(14))
                    {
                        num9 += Main.rand.Next(0, 2);
                    }
                    if (Main.rand.NextBool(14))
                    {
                        num9 += Main.rand.Next(0, 2);
                    }
                }
                else if (Main.rand.NextBool(800))
                {
                    num8 = 73;
                    if (Main.rand.NextBool(6))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(6))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(6))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(6))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(6))
                    {
                        num9 += Main.rand.Next(1, 20);
                    }
                }
                else if (Main.rand.NextBool(60))
                {
                    num8 = 72;
                    if (Main.rand.NextBool(4))
                    {
                        num9 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        num9 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        num9 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        num9 += Main.rand.Next(5, 25);
                    }
                }
                else
                {
                    num8 = 71;
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(10, 25);
                    }
                }
            }
            else if (num != -1 && Main.rand.NextBool(num))
            {
                num8 = 1242;
            }
            else if (num5 != -1)
            {
                num8 = ((!Main.rand.NextBool(4)) ? 2674 : ((!Main.rand.NextBool(3)) ? 2006 : ((Main.rand.NextBool(3)) ? 2675 : 2002)));
            }
            else if (num6 != -1 && extractinatorBlockType == 642)
            {
                if (Main.rand.NextBool(10))
                {
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            num8 = 4354;
                            break;
                        case 1:
                            num8 = 4389;
                            break;
                        case 2:
                            num8 = 4377;
                            break;
                        case 3:
                            num8 = 5127;
                            break;
                        default:
                            num8 = 4378;
                            break;
                    }
                }
                else
                {
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            num8 = 4349;
                            break;
                        case 1:
                            num8 = 4350;
                            break;
                        case 2:
                            num8 = 4351;
                            break;
                        case 3:
                            num8 = 4352;
                            break;
                        default:
                            num8 = 4353;
                            break;
                    }
                }
            }
            else if (num6 != -1)
            {
                switch (Main.rand.Next(5))
                {
                    case 0:
                        num8 = 4349;
                        break;
                    case 1:
                        num8 = 4350;
                        break;
                    case 2:
                        num8 = 4351;
                        break;
                    case 3:
                        num8 = 4352;
                        break;
                    default:
                        num8 = 4353;
                        break;
                }
            }
            else if (num2 != -1 && Main.rand.NextBool(num2))
            {
                switch (Main.rand.Next(6))
                {
                    case 0:
                        num8 = 181;
                        break;
                    case 1:
                        num8 = 180;
                        break;
                    case 2:
                        num8 = 177;
                        break;
                    case 3:
                        num8 = 179;
                        break;
                    case 4:
                        num8 = 178;
                        break;
                    default:
                        num8 = 182;
                        break;
                }
                if (Main.rand.NextBool(20))
                {
                    num9 += Main.rand.Next(0, 2);
                }
                if (Main.rand.NextBool(30))
                {
                    num9 += Main.rand.Next(0, 3);
                }
                if (Main.rand.NextBool(40))
                {
                    num9 += Main.rand.Next(0, 4);
                }
                if (Main.rand.NextBool(50))
                {
                    num9 += Main.rand.Next(0, 5);
                }
                if (Main.rand.NextBool(60))
                {
                    num9 += Main.rand.Next(0, 6);
                }
            }
            else if (num3 != -1 && Main.rand.NextBool(num3))
            {
                num8 = 999;
                if (Main.rand.NextBool(20))
                {
                    num9 += Main.rand.Next(0, 2);
                }
                if (Main.rand.NextBool(30))
                {
                    num9 += Main.rand.Next(0, 3);
                }
                if (Main.rand.NextBool(40))
                {
                    num9 += Main.rand.Next(0, 4);
                }
                if (Main.rand.NextBool(50))
                {
                    num9 += Main.rand.Next(0, 5);
                }
                if (Main.rand.NextBool(60))
                {
                    num9 += Main.rand.Next(0, 6);
                }
            }
            else if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool(5000))
                {
                    num8 = 74;
                    if (Main.rand.NextBool(10))
                    {
                        num9 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.NextBool(10))
                    {
                        num9 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.NextBool(10))
                    {
                        num9 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.NextBool(10))
                    {
                        num9 += Main.rand.Next(0, 3);
                    }
                    if (Main.rand.NextBool(10))
                    {
                        num9 += Main.rand.Next(0, 3);
                    }
                }
                else if (Main.rand.NextBool(400))
                {
                    num8 = 73;
                    if (Main.rand.NextBool(5))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(5))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(5))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(5))
                    {
                        num9 += Main.rand.Next(1, 21);
                    }
                    if (Main.rand.NextBool(5))
                    {
                        num9 += Main.rand.Next(1, 20);
                    }
                }
                else if (Main.rand.NextBool(30))
                {
                    num8 = 72;
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(5, 26);
                    }
                    if (Main.rand.NextBool(3))
                    {
                        num9 += Main.rand.Next(5, 25);
                    }
                }
                else
                {
                    num8 = 71;
                    if (Main.rand.NextBool(2))
                    {
                        num9 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.NextBool(2))
                    {
                        num9 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.NextBool(2))
                    {
                        num9 += Main.rand.Next(10, 26);
                    }
                    if (Main.rand.NextBool(2))
                    {
                        num9 += Main.rand.Next(10, 25);
                    }
                }
            }
            else if (extractinatorBlockType == 642)
            {
                switch (Main.rand.Next(14))
                {
                    case 0:
                        num8 = 12;
                        break;
                    case 1:
                        num8 = 11;
                        break;
                    case 2:
                        num8 = 14;
                        break;
                    case 3:
                        num8 = 13;
                        break;
                    case 4:
                        num8 = 699;
                        break;
                    case 5:
                        num8 = 700;
                        break;
                    case 6:
                        num8 = 701;
                        break;
                    case 7:
                        num8 = 702;
                        break;
                    case 8:
                        num8 = 364;
                        break;
                    case 9:
                        num8 = 1104;
                        break;
                    case 10:
                        num8 = 365;
                        break;
                    case 11:
                        num8 = 1105;
                        break;
                    case 12:
                        num8 = 366;
                        break;
                    default:
                        num8 = 1106;
                        break;
                }
                if (Main.rand.NextBool(20))
                {
                    num9 += Main.rand.Next(0, 2);
                }
                if (Main.rand.NextBool(30))
                {
                    num9 += Main.rand.Next(0, 3);
                }
                if (Main.rand.NextBool(40))
                {
                    num9 += Main.rand.Next(0, 4);
                }
                if (Main.rand.NextBool(50))
                {
                    num9 += Main.rand.Next(0, 5);
                }
                if (Main.rand.NextBool(60))
                {
                    num9 += Main.rand.Next(0, 6);
                }
            }
            else
            {
                switch (Main.rand.Next(8))
                {
                    case 0:
                        num8 = 12;
                        break;
                    case 1:
                        num8 = 11;
                        break;
                    case 2:
                        num8 = 14;
                        break;
                    case 3:
                        num8 = 13;
                        break;
                    case 4:
                        num8 = 699;
                        break;
                    case 5:
                        num8 = 700;
                        break;
                    case 6:
                        num8 = 701;
                        break;
                    default:
                        num8 = 702;
                        break;
                }
                if (Main.rand.NextBool(20))
                {
                    num9 += Main.rand.Next(0, 2);
                }
                if (Main.rand.NextBool(30))
                {
                    num9 += Main.rand.Next(0, 3);
                }
                if (Main.rand.NextBool(40))
                {
                    num9 += Main.rand.Next(0, 4);
                }
                if (Main.rand.NextBool(50))
                {
                    num9 += Main.rand.Next(0, 5);
                }
                if (Main.rand.NextBool(60))
                {
                    num9 += Main.rand.Next(0, 6);
                }
            }
            //ItemLoader.ExtractinatorUse(ref num8, ref num9, extractType, extractinatorBlockType);
            if (num8 > 0)
            {
                //int ResultingItem = Item.NewItem(Item.GetSource_None(), (int)pred.Center.X, (int)pred.Center.Y, 1, 1, num8, num9, false, -1, false, false);
                //if (Main.netMode == NetmodeID.MultiplayerClient)
                //{
                //    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, ResultingItem, 1f, 0f, 0f, 0, 0, 0);
                //}
				
				if (pred is Player)
                {
					Player predPlayer = pred as Player;
                    Item eatenItem = new Item();
                    eatenItem.SetDefaults(num8);
                    eatenItem.stack = num9;
					SummonItemHere(pred, pred.Center, ref eatenItem, out Item itemDrop);
					if (itemDrop.AsFood().MaxHealth > 0)
						PredPlayer.Swallow(predPlayer, itemDrop, ForceSwallow: true, Silent: true);
                }
                else if (pred is NPC)
                {
                    NPC predNPC = pred as NPC;
                    Item eatenItem = new Item();
                    eatenItem.SetDefaults(num8);
                    eatenItem.stack = num9;
                    SummonItemHere(pred, pred.Center, ref eatenItem, out Item itemDrop);
                    if (itemDrop.AsFood().MaxHealth > 0)
                        PredNPC.Swallow(predNPC, itemDrop);
                }
                else if (pred is Projectile)
                {
                    Projectile predProjectile = pred as Projectile;
                    Item eatenItem = new Item();
                    eatenItem.SetDefaults(num8);
                    eatenItem.stack = num9;
                    SummonItemHere(pred, pred.Center, ref eatenItem, out Item itemDrop);
                    if (itemDrop.AsFood().MaxHealth > 0)
                        PredProjectile.Swallow(predProjectile, itemDrop);
                }
                //this.DropItemFromExtractinator(num8, num9);
            }
        }

        public static void SummonItemHere(Entity entity, Vector2 position, ref Item item, out Item itemDrop)
        {
            itemDrop = null;
            if (item.IsAir)
                return;
            if (item.favorited)
                return;

            int itemDropId = Item.NewItem(entity.GetSource_Misc("ThrowItem"), (int)position.X, (int)position.Y, entity.width, entity.height, item);
            itemDrop = Main.item[itemDropId];

            itemDrop.velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
            itemDrop.velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
            itemDrop.noGrabDelay = 100;
            itemDrop.newAndShiny = false;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemDropId);

            item.TurnToAir();
        }

        public static string GetStatChangeString(float Amount, bool Flat = false, bool NegativeIsGood = false, bool IsVoreStat = false, bool IsMultiplier = false)
        {
            string PositiveColor = "[c/00FF00:+";
            string NeutralColor = "[c/BDBDBD:";
            string NegativeColor = "[c/FFBF5F:";

            string text = string.Empty;

            if (IsMultiplier)
            {
                PositiveColor = "[c/00FF00:x";
                NeutralColor = "[c/BDBDBD:x";
                NegativeColor = "[c/FFBF5F:x";


                if (NegativeIsGood && IsVoreStat)
                {
                    PositiveColor = "[c/BF5F00:x";
                    NegativeColor = "[c/007F00:x";
                }
                else if (IsVoreStat)
                {
                    PositiveColor = "[c/007F00:x";
                    NegativeColor = "[c/BF5F00:x";
                }
                else if (NegativeIsGood)
                {
                    PositiveColor = "[c/FFBF5F:x";
                    NegativeColor = "[c/00FF00:x";
                }

                text = Amount.ToString();
                if (Amount > 1)
                {
                    text = PositiveColor + text + "]";
                }
                else if (Amount < 1)
                {
                    text = NegativeColor + text + "]";
                }
                else
                    text = NeutralColor + text + "]";
                return text;
            }


            if (NegativeIsGood && IsVoreStat)
            {
                PositiveColor = "[c/BF5F00:+";
                NegativeColor = "[c/007F00:";
            }
            else if (IsVoreStat)
            {
                PositiveColor = "[c/007F00:+";
                NegativeColor = "[c/BF5F00:";
            }
            else if (NegativeIsGood)
			{
                PositiveColor = "[c/FFBF5F:+";
                NegativeColor = "[c/00FF00:";
            }

            text = Flat ? Amount.ToString() : Amount.ToString() + "%";

            if (Amount > 0)
            {
                text = PositiveColor + text + "]";
            }
            else if (Amount < 0)
            {
                text = NegativeColor + text + "]";
            }
            else
                text = NeutralColor + text + "]";
            return text;
        }

        // public static bool IsDigestibleLoot(Item item)
        // {
	    //     return ItemID.Sets.OpenableBag[item.type] || ItemID.Sets.ExtractinatorMode[item.type] >= 0; // || DigestibleLoot.DIGESTIBLE_LOOT_IDs.Contains(item.type);
        // }

        public static bool MarkLootDigested(Entity pred)
        {
	        switch (pred)
	        {
		        case Player player:
			        player.AsPred().MarkLootDigested();
			        return true;
		        case NPC npc:
			        npc.AsPred().MarkLootDigested();
			        return true;
		        default:
			        return false;
	        }
        }
	}
}
