using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class SolarFlarePickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SolarFlarePickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 4850;
			item.AsFood().Size = 0.91;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class VortexPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.VortexPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 4800;
			item.AsFood().Size = 0.91;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class NebulaPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.NebulaPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 4750;
			item.AsFood().Size = 0.91;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class StardustPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.StardustPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 4700;
			item.AsFood().Size = 0.91;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
}
