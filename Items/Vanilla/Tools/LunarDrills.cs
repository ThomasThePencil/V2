using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class SolarFlareDrill : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SolarFlareDrill;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 5900;
			item.AsFood().Size = 1.32;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Drill = true;
		}
	}
	public class VortexDrill : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.VortexDrill;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 5800;
			item.AsFood().Size = 1.32;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Drill = true;
		}
	}
	public class NebulaDrill : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.NebulaDrill;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 5700;
			item.AsFood().Size = 1.32;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Drill = true;
		}
	}
	public class StardustDrill : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.StardustDrill;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 5600;
			item.AsFood().Size = 1.32;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Drill = true;
		}
	}
}
