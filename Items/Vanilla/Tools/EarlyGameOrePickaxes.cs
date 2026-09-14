using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class CopperPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CopperPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 300;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class TinPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 188;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class IronPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 458;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class LeadPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LeadPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 501;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class SilverPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SilverPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 419;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class TungstenPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 806;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class GoldPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GoldPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 289;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
	public class PlatinumPickaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PlatinumPickaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 723;
			item.AsFood().Size = 0.52;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Pickaxe = true;
		}
	}
}
