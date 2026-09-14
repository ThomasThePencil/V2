using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;

namespace V2.Projectiles.Vanilla.Traps
{
	public class Boulder : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.Boulder;

		public override void SetDefaults(Projectile Projectile)
		{
			Projectile.AsFood().MaxHealth = 500;
			Projectile.AsFood().Health = 500;
			Projectile.AsFood().DefinedSize = 1.4;
		}
	}
	public class BouncyBoulder : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.BouncyBoulder;

		public override void SetDefaults(Projectile Projectile)
		{
			Projectile.AsFood().MaxHealth = 500;
			Projectile.AsFood().Health = 500;
			Projectile.AsFood().DefinedSize = 1.4;
			Projectile.AsFood().WellFedPower = 0.02;
		}
	}
	public class LifeCrystalBoulder : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.LifeCrystalBoulder;

		public override void SetDefaults(Projectile Projectile)
		{
			Projectile.AsFood().MaxHealth = 500;
			Projectile.AsFood().Health = 500;
			Projectile.AsFood().DefinedSize = 0.75;
		}
	}

	public class RollingCactus : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.RollingCactus;

		public override void SetDefaults(Projectile Projectile)
		{
			Projectile.AsFood().MaxHealth = 200;
			Projectile.AsFood().Health = 200;
			Projectile.AsFood().DefinedSize = 1.4;
		}
	}
}
