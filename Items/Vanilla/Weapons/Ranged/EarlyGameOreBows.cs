using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Ranged
{
	public class CopperBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CopperBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 270;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
	public class TinBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.TinBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 167;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
	public class IronBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.IronBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 423;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
	public class LeadBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LeadBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 469;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
	public class SilverBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SilverBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 377;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
	public class TungstenBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.TungstenBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 771;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
	public class GoldBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 266;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
	public class PlatinumBow : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PlatinumBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 701;
			item.AsFood().Size = 0.28;

			item.AsTaggable().Bow = true;
		}
	}
}
