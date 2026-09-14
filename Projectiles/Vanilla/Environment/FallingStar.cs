using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;

namespace V2.Projectiles.Vanilla.Environment
{
	public class FallingStar : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.FallingStar;

		public override void SetDefaults(Projectile projectile)
		{
			projectile.AsFood().DefinedSize = 0.63;
			projectile.AsFood().MaxHealth = 100;
			projectile.AsFood().Health = 100;

			projectile.AsFood().OnSwallowedBy += OnSwallowedByPlayer_GiveFallingStarGoal;
		}

		public static void OnSwallowedByPlayer_GiveFallingStarGoal(Projectile projectile, Entity pred)
		{
			if (pred is Player predPlayer)
				ModContent.GetInstance<CatchFallingStar>().TrySetCompletion(predPlayer);
		}
	}
}
