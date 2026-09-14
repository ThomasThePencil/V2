using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class CopperShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CopperShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 1;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class TinShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 150;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 1;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class IronShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 165;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class LeadShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LeadShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 172;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 3;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class SilverShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SilverShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 145;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class TungstenShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 274;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 2;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class GoldShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GoldShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 104;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 3;

			item.AsTaggable().Shortsword = true;
		}
	}
	public class PlatinumShortsword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PlatinumShortsword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 248;
			item.AsFood().Size = 0.18;
			item.AsFood().AcidResistTier = 2;

			item.AsAnItem().StruggleDamageBaseMod = 5;

			item.AsTaggable().Shortsword = true;
		}
	}
}
