using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Vanilla.Consumables;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Vanilla.Placeables.Traps
{
	public class Boulder : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Boulder;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 500;
			item.AsFood().Size = 1.4;
		}
	}
	public class BouncyBoulder : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BouncyBoulder;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 500;
			item.AsFood().Size = 1.4;
			item.AsFood().WellFedPower = 0.02;
		}
	}
	public class LifeCrystalBoulder : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LifeCrystalBoulder;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 500;
			item.AsFood().Size = 0.75;

			item.AsFood().UpdateInStomach += LifeCrystal.UpdateInStomach;
			item.AsFood().OnBreak += LifeCrystal.OnBreak;
		}
	}

	public class RollingCactus : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.RollingCactus;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 1.4;
			item.AsFood().OnSwallowDamage = 25;
			item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.Cactus";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 9);
		}
	}
}
