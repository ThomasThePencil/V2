using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class CopperHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CopperHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 290;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hammer = true;
		}
	}
	public class TinHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 180;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hammer = true;
		}
	}
	public class IronHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 445;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class LeadHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LeadHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 486;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class SilverHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SilverHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 405;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hammer = true;
		}
	}
	public class TungstenHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 782;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hammer = true;
		}
	}
	public class GoldHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GoldHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 272;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hammer = true;
		}
	}
	public class PlatinumHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PlatinumHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 704;
			item.AsFood().Size = 0.5;
			item.AsFood().AcidResistTier = 2;

			item.AsTaggable().Hammer = true;
		}
	}
}
