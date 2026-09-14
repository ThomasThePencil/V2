using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class CopperBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CopperBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 320;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
	public class TinBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 210;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
	public class IronBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 495;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
	public class LeadBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LeadBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 516;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
	public class SilverBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SilverBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 435;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
	public class TungstenBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 822;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
	public class GoldBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GoldBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 312;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
	public class PlatinumBroadsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PlatinumBroadsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 744;
			item.AsFood().Size = 0.54;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Broadsword = true;
		}
	}
}
