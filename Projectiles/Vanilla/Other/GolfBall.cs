using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Golf;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;

namespace V2.Projectiles.Vanilla.Other
{
	public class GolfBall : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;

		//das a lotta balls
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) =>
			entity.type == ProjectileID.DirtGolfBall || entity.type == ProjectileID.GolfBallDyedBlack || entity.type == ProjectileID.GolfBallDyedBlue
			|| entity.type == ProjectileID.GolfBallDyedBrown || entity.type == ProjectileID.GolfBallDyedCyan || entity.type == ProjectileID.GolfBallDyedGreen
			|| entity.type == ProjectileID.GolfBallDyedLimeGreen || entity.type == ProjectileID.GolfBallDyedOrange || entity.type == ProjectileID.GolfBallDyedPink
			|| entity.type == ProjectileID.GolfBallDyedPurple || entity.type == ProjectileID.GolfBallDyedRed || entity.type == ProjectileID.GolfBallDyedSkyBlue
			|| entity.type == ProjectileID.GolfBallDyedTeal || entity.type == ProjectileID.GolfBallDyedViolet || entity.type == ProjectileID.GolfBallDyedYellow;

		public override void SetDefaults(Projectile projectile)
		{
			projectile.AsFood().DefinedSize = 0.05;
			projectile.AsFood().MaxHealth = 70;
			projectile.AsFood().Health = 70;

			projectile.AsFood().OnSwallowedBy += OnSwallowedByNPC_GiveGolfGoal;
		}

		public override void AI(Projectile projectile)
		{
			if (projectile.velocity.Length() > 7f)
			{
				Rectangle hitboxIGuess = projectile.Hitbox;
				hitboxIGuess.Y += 16;
				foreach (var item in Main.ActiveNPCs)
				{
					if (projectile is not null && projectile.active && projectile.CurrentCaptor() is null && item.active && item.CurrentCaptor() is null && item.AsPred().CanBeForceFed.Invoke(item) && hitboxIGuess.Intersects(item.Hitbox))
					{
						PredNPC.Swallow(item, projectile);
					}
				}
				foreach (var item in Main.ActiveProjectiles)
				{
					if (projectile is not null && projectile.active && projectile.CurrentCaptor() is null && item.active && item.CurrentCaptor() is null && item.AsPred().CanBeForceFed.Invoke(item) && hitboxIGuess.Intersects(item.Hitbox))
					{
						PredProjectile.Swallow(item, projectile);
					}
				}
				foreach (var item in Main.ActivePlayers)
				{
					if (projectile is not null && projectile.active && projectile.CurrentCaptor() is null && item.active && Main.player[projectile.owner] != item && item.CurrentCaptor() is null && hitboxIGuess.Intersects(item.Hitbox))
					{
						PredPlayer.Swallow(item, projectile);
					}
				}
			}
		}

		public static void OnSwallowedByNPC_GiveGolfGoal(Projectile projectile, Entity pred)
		{
			Player owner = Main.player[projectile.owner];
			if (owner is not null && !projectile.npcProj && owner.Center.Distance(pred.Center) >= 2400f)
			{
				ModContent.GetInstance<LongGolf>().TrySetCompletion(owner);
			}

		}
	}
}
