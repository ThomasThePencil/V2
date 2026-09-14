using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class CopperAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CopperAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 280;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Axe = true;
		}
	}
	public class TinAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 171;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Axe = true;
		}
	}
	public class IronAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 434;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class LeadAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LeadAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 482;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class SilverAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SilverAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 398;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Axe = true;
		}
	}
	public class TungstenAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 785;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Axe = true;
		}
	}
	public class GoldAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GoldAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 266;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Axe = true;
		}
	}
	public class PlatinumAxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PlatinumAxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 701;
			item.AsFood().Size = 0.41;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Axe = true;
		}
	}
}
