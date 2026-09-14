using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class SolarFlareHamaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LunarHamaxeSolar;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 6300;
			item.AsFood().Size = 1.66;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hamaxe = true;
		}
	}
	public class VortexHamaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LunarHamaxeVortex;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 6200;
			item.AsFood().Size = 1.66;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hamaxe = true;
		}
	}
	public class NebulaHamaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LunarHamaxeNebula;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 6100;
			item.AsFood().Size = 1.66;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hamaxe = true;
		}
	}
	public class StardustHamaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LunarHamaxeStardust;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 6000;
			item.AsFood().Size = 1.66;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hamaxe = true;
		}
	}
}
