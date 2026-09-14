using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;

namespace V2.Projectiles.Vanilla.Summons.Pets
{
	public class MiniIceQueen : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.IceQueenPet;

		public override void SetDefaults(Projectile projectile)
		{
			projectile.Name = Language.GetTextValue("Mods.V2.Projectiles.DisplayName.Vanilla.Summons.Pets.MiniIceQueen");
			projectile.AsFood().DefinedSize = 0.85;
			projectile.AsFood().MaxHealth = 2500;
			projectile.AsFood().Health = 2500;

			projectile.AsFood().OnKilledByDigestion += PreyProjectile.OnKilledByDigestion_GrantLivePreyGoal;
		}
	}
}
