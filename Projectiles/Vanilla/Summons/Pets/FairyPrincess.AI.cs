using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.Dryad;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.Projectiles.Vanilla.Summons.Pets
{
	public partial class FairyPrincess
	{
		public static bool MiniCandyFairyAI(Projectile projectile)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			bool ateOwner = ownerPlayer.IsFoodFor(projectile, out bool churnedOwner);
			if (PredProjectile.GetStomachTracker(projectile) is VoreTracker fairyPrincessTummy)
			{
				foreach (PreyData newFood in fairyPrincessTummy.PreyQueue)
				{
					if (newFood.NoHealth)
						continue;

					ateOwner |= ownerPlayer.IsFoodFor(newFood.Instance, out bool foodChurnedOwner);
					churnedOwner |= foodChurnedOwner;
				}
				foreach (PreyData food in fairyPrincessTummy.Prey)
				{
					if (food.NoHealth)
						continue;

					ateOwner |= ownerPlayer.IsFoodFor(food.Instance, out bool foodChurnedOwner);
					churnedOwner |= foodChurnedOwner;
				}
			}
			if (!projectile.AsFairyPrincess().WaitingForChurnedOwner)
				projectile.AsFairyPrincess().WaitingForChurnedOwner = ateOwner;
			float xOffset = V2Utils.TileCountAsPixelCount(2);
			float yOffset = -V2Utils.TileCountAsPixelCount(4.5);

			Vector2 offsetFromOwner = new Vector2((float)ownerPlayer.direction * xOffset, yOffset);
			Vector2 intendedLocation = ownerPlayer.MountedCenter + offsetFromOwner;
			float distanceToIntendedLocation = Vector2.Distance(projectile.Center, intendedLocation);

			if (projectile.AsFairyPrincess().WaitingForChurnedOwner)
			{
				projectile.timeLeft = 2;
				goto SkipOwnerDeadCheck;
			}

			if (!ateOwner)
			{
				if (ownerPlayer.dead)
				{
					projectile.Kill();
					return false;
				}

				if (ownerPlayer.petFlagFairyQueenPet)
					projectile.timeLeft = 2;

				if (distanceToIntendedLocation > 1000f)
					projectile.Center = ownerPlayer.Center + offsetFromOwner;
			}
			else
			{
				projectile.AsFairyPrincess().WaitingForChurnedOwner = true;
				projectile.timeLeft = 2;
			}

			SkipOwnerDeadCheck:
			VoreTracker tracker = PredProjectile.GetStomachTracker(projectile);
			if (tracker is null)
				goto ResetFrame;

			PreyData candyFairy = null;
			if (tracker.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss) is PreyData sprinkles && sprinkles.WeightLeftToDigest > 4.0)
				candyFairy = sprinkles;
			if (tracker.PreyQueue.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss) is PreyData sprinklesQueue && sprinklesQueue.WeightLeftToDigest > 4.0)
				candyFairy = sprinklesQueue;
			bool ateCandyFairy = tracker is not null;
			ateCandyFairy &= candyFairy is not null;
			if (ateCandyFairy)
			{
				if (projectile.width == 18 && projectile.height == 40)
				{
					projectile.width = 86;
					projectile.height = 148;
					projectile.position.X -= 86 - 18;
					projectile.position.Y -= 148 - 40;
				}
				projectile.velocity.X = 0;
				if (!candyFairy.NoHealth)
				{
					NPC realCandyFairy = candyFairy.Instance as NPC;
					if (projectile.AsV2Proj().CustomSprite is null)
						projectile.AsV2Proj().CustomSprite = new FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.Alive();
				}
				else
				{
					if (projectile.AsV2Proj().CustomSprite is null)
						projectile.AsV2Proj().CustomSprite = new FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.Alive();
					else if (projectile.AsV2Proj().CustomSprite is FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.Alive && projectile.AsV2Proj().CustomSprite.CanTransitionToNewAnim && GetEmpressDigestionStage(projectile) >= 2)
						projectile.AsV2Proj().CustomSprite = new FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.DigestStage1();
					else if (projectile.AsV2Proj().CustomSprite is FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.DigestStage1 && projectile.AsV2Proj().CustomSprite.CanTransitionToNewAnim && GetEmpressDigestionStage(projectile) >= 3)
						projectile.AsV2Proj().CustomSprite = new FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.DigestStage2();
					else if (projectile.AsV2Proj().CustomSprite is FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.DigestStage2 && projectile.AsV2Proj().CustomSprite.CanTransitionToNewAnim && GetEmpressDigestionStage(projectile) >= 4)
						projectile.AsV2Proj().CustomSprite = new FairyPrincessStuff.Animations.BaseWeight.OVHerOwnFuckingMother.DigestStage3();
				}
				goto SkipResetFrame;
			}

			ResetFrame:
			if (projectile.AsV2Proj().CustomSprite is not null)
				projectile.AsV2Proj().CustomSprite = null;

			SkipResetFrame:

			if (!MiniCandyFairyAI_TryFindNewFood(projectile))
			{
				if (projectile.AsFairyPrincess().WaitingForChurnedOwner)
				{
					projectile.velocity *= 0.925f;
					if (ownerPlayer.active && !ownerPlayer.dead && ownerPlayer.CurrentCaptor() is null && distanceToIntendedLocation <= V2Utils.TileCountAsPixelCount(20))
					{
						projectile.AsFairyPrincess().WaitingForChurnedOwner = false;
						MiniCandyFairyAI_HandleOwnerFollowing(projectile, ownerPlayer, intendedLocation);
					}
				}
				else if ((!(ateOwner && !churnedOwner)) && distanceToIntendedLocation <= V2Utils.TileCountAsPixelCount(20))
					MiniCandyFairyAI_HandleOwnerFollowing(projectile, ownerPlayer, intendedLocation);
				else
					projectile.velocity *= 0.925f;
			}

			if (projectile.velocity.Length() > 6f)
			{
				float rotationSpeed = projectile.velocity.X * 0.1f;
				if (Math.Abs(projectile.rotation - rotationSpeed) >= (float)Math.PI)
				{
					if (rotationSpeed < projectile.rotation)
						projectile.rotation -= (float)Math.PI * 2f;
					else
						projectile.rotation += (float)Math.PI * 2f;
				}

				float rotationInertia = 12f;
				projectile.rotation = (projectile.rotation * (rotationInertia - 1f) + rotationSpeed) / rotationInertia;
				if (++projectile.frameCounter >= 3)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
					if (projectile.frame >= Main.projFrames[projectile.type])
						projectile.frame = 0;
				}

				if (projectile.frameCounter == 0 || Main.rand.NextBool(15))
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
			else
			{
				if (projectile.rotation > (float)Math.PI)
					projectile.rotation -= (float)Math.PI * 2f;

				if (projectile.rotation > -0.005f && projectile.rotation < 0.005f)
					projectile.rotation = 0f;
				else
					projectile.rotation *= 0.96f;

				if (++projectile.frameCounter >= 5)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
					if (projectile.frame >= Main.projFrames[projectile.type])
						projectile.frame = 0;
				}
			}
			return false;
		}

		public static void MiniCandyFairyAI_HandleOwnerFollowing(Projectile projectile, Player ownerPlayer, Vector2 intendedLocation)
		{
			projectile.direction = projectile.spriteDirection = ownerPlayer.direction;

			float distanceToIntendedLocation = Vector2.Distance(projectile.Center, intendedLocation);

			Vector3 vector156 = new Vector3(1f, 0.6f, 1f) * 1.5f;
			DelegateMethods.v3_1 = vector156 * 0.75f;
			Utils.PlotTileLine(ownerPlayer.Center, ownerPlayer.Center + ownerPlayer.velocity * 6f, 40f, DelegateMethods.CastLightOpen);
			Utils.PlotTileLine(ownerPlayer.Left, ownerPlayer.Right, 40f, DelegateMethods.CastLightOpen);
			DelegateMethods.v3_1 = vector156 * 1.5f;
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 6f, 30f, DelegateMethods.CastLightOpen);
			Utils.PlotTileLine(projectile.Left, projectile.Right, 20f, DelegateMethods.CastLightOpen);

			Vector2 distanceFromIntendedLocation = intendedLocation - projectile.Center;
			float lockInDistance = 10f;
			if (distanceToIntendedLocation < lockInDistance)
				projectile.velocity *= 0.85f;

			if (distanceFromIntendedLocation != Vector2.Zero)
			{
				float maxSpeed = 15f;
				projectile.velocity = distanceFromIntendedLocation * 0.1f;
				if (projectile.velocity.Length() > maxSpeed)
				{
					projectile.velocity.Normalize();
					projectile.velocity *= maxSpeed;
				}
			}

			if (distanceToIntendedLocation > 50f)
			{
				projectile.direction = projectile.spriteDirection = 1;
				if (projectile.velocity.X < 0f)
					projectile.direction = projectile.spriteDirection = -1;
			}
		}
		public static bool MiniCandyFairyAI_TryFindNewFood(Projectile projectile)
		{
			Player ownerPlayer = Main.player[projectile.owner];
			List<(PreyType, int)> diet = [
				(PreyType.NPC, NPCID.FairyCritterBlue),
				(PreyType.NPC, NPCID.FairyCritterGreen),
				(PreyType.NPC, NPCID.FairyCritterPink),
				(PreyType.NPC, NPCID.GoldBird),
				(PreyType.NPC, NPCID.GoldBunny),
				(PreyType.NPC, NPCID.GoldButterfly),
				(PreyType.NPC, NPCID.GoldDragonfly),
				(PreyType.NPC, NPCID.GoldenSlime),
				(PreyType.NPC, NPCID.GoldFrog),
				(PreyType.NPC, NPCID.GoldGoldfish),
				(PreyType.NPC, NPCID.GoldGoldfishWalker),
				(PreyType.NPC, NPCID.GoldGrasshopper),
				(PreyType.NPC, NPCID.GoldLadyBug),
				(PreyType.NPC, NPCID.GoldMouse),
				(PreyType.NPC, NPCID.GoldSeahorse),
				(PreyType.NPC, NPCID.SquirrelGold),
				(PreyType.NPC, NPCID.GoldWaterStrider),
				(PreyType.NPC, NPCID.GoldWorm),
				(PreyType.NPC, NPCID.EnchantedNightcrawler),
				(PreyType.NPC, NPCID.GemBunnyAmber),
				(PreyType.NPC, NPCID.GemBunnyAmethyst),
				(PreyType.NPC, NPCID.GemBunnyDiamond),
				(PreyType.NPC, NPCID.GemBunnyEmerald),
				(PreyType.NPC, NPCID.GemBunnyRuby),
				(PreyType.NPC, NPCID.GemBunnySapphire),
				(PreyType.NPC, NPCID.GemBunnyTopaz),
				(PreyType.NPC, NPCID.GemSquirrelAmber),
				(PreyType.NPC, NPCID.GemSquirrelAmethyst),
				(PreyType.NPC, NPCID.GemSquirrelDiamond),
				(PreyType.NPC, NPCID.GemSquirrelEmerald),
				(PreyType.NPC, NPCID.GemSquirrelRuby),
				(PreyType.NPC, NPCID.GemSquirrelSapphire),
				(PreyType.NPC, NPCID.GemSquirrelTopaz),
				(PreyType.NPC, NPCID.KingSlime),
				(PreyType.NPC, NPCID.Pixie),
				(PreyType.NPC, NPCID.Unicorn),
				(PreyType.NPC, NPCID.RainbowSlime),
				(PreyType.NPC, NPCID.Gastropod),
				(PreyType.NPC, NPCID.QueenBee),
				(PreyType.NPC, NPCID.QueenSlimeBoss),
				(PreyType.NPC, NPCID.QueenSlimeMinionBlue),
				(PreyType.NPC, NPCID.QueenSlimeMinionPurple),
				(PreyType.NPC, NPCID.QueenSlimeMinionPink),
				(PreyType.NPC, NPCID.EnchantedSword),
				(PreyType.NPC, NPCID.BigMimicHallow),
				(PreyType.NPC, NPCID.SandsharkHallow),
				(PreyType.NPC, NPCID.EmpressButterfly),
				(PreyType.NPC, NPCID.Princess),
				(PreyType.NPC, NPCID.HallowBoss),
				(PreyType.NPC, NPCID.IceQueen),
				(PreyType.NPC, NPCID.VortexHornetQueen),
				(PreyType.NPC, NPCID.ShimmerSlime),
				(PreyType.NPC, NPCID.Shimmerfly),
				(PreyType.Projectile, ProjectileID.KingSlimePet),
				(PreyType.Projectile, ProjectileID.QueenSlimePet),
				(PreyType.Projectile, ProjectileID.IceQueenPet),
			];

			PreyType targetPreyType = PreyType.Undefined;
			int targetPreyIndex = -1;
			double maxPreyDistanceFromFairy = V2Utils.TileCountAsPixelCount(25);
			double maxPreyDistanceFromOwner = V2Utils.TileCountAsPixelCount(40);
			foreach (NPC potentialPrey in Main.ActiveNPCs)
			{
				if (potentialPrey.life <= 0)
					continue;

				if (potentialPrey.CurrentCaptor() is not null)
					continue;

				bool partOfDiet = false;
				foreach ((PreyType type, int ID) in diet)
				{
					if (type == PreyType.NPC && ID == potentialPrey.type)
						partOfDiet = true;
				}
				if (!partOfDiet)
					continue;

				float distanceToPotentialPrey = projectile.Distance(potentialPrey.TrueCenter());
				float distanceToPotentialPreyFromOwner = ownerPlayer.Distance(potentialPrey.TrueCenter());
				if (distanceToPotentialPrey <= maxPreyDistanceFromFairy && distanceToPotentialPreyFromOwner <= maxPreyDistanceFromOwner)
				{
					targetPreyType = PreyType.NPC;
					targetPreyIndex = potentialPrey.whoAmI;
					maxPreyDistanceFromFairy = distanceToPotentialPrey;
					maxPreyDistanceFromOwner = distanceToPotentialPreyFromOwner;
				}
			}
			foreach (Projectile potentialPrey in Main.ActiveProjectiles)
			{
				if (potentialPrey.AsFood().Health <= 0)
					continue;

				if (potentialPrey.CurrentCaptor() is not null)
					continue;

				bool partOfDiet = false;
				foreach ((PreyType type, int ID) in diet)
				{
					if (type == PreyType.Projectile && ID == potentialPrey.type)
						partOfDiet = true;
				}
				if (!partOfDiet)
					continue;

				float distanceToPotentialPrey = projectile.Distance(potentialPrey.TrueCenter());
				float distanceToPotentialPreyFromOwner = ownerPlayer.Distance(potentialPrey.TrueCenter());
				if (distanceToPotentialPrey <= maxPreyDistanceFromFairy && distanceToPotentialPreyFromOwner <= maxPreyDistanceFromOwner)
				{
					targetPreyType = PreyType.Projectile;
					targetPreyIndex = potentialPrey.whoAmI;
					maxPreyDistanceFromFairy = distanceToPotentialPrey;
					maxPreyDistanceFromOwner = distanceToPotentialPreyFromOwner;
				}
			}

			if (targetPreyType != PreyType.Undefined && targetPreyIndex != -1)
			{
				Entity targetPrey = targetPreyType switch
				{
					PreyType.Player => Main.player[targetPreyIndex],
					PreyType.NPC => Main.npc[targetPreyIndex],
					PreyType.Projectile => Main.projectile[targetPreyIndex],
					_ => null,
				};
				float speed = 8.75f;
				float inertia = 10f;
				projectile.ai[1] += 0.05f;
				if (projectile.ai[1] > 7f)
					projectile.ai[1] = 7f;
				Vector2 direction = targetPrey.TrueCenter() - projectile.TrueCenter();
				direction.Normalize();
				direction *= speed;
				projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
				projectile.netUpdate = true;
				projectile.direction = projectile.spriteDirection = 1;
				if (projectile.velocity.X < 0f)
					projectile.direction = projectile.spriteDirection = -1;
				projectile.DoContactGulpage(diet);
				return true;
			}
			return false;
		}
	}
}
