using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Bars
{
	public class CopperBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CopperBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 100;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class TinBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 120;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class IronBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 175;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class LeadBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LeadBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 185;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class SilverBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SilverBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 215;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class TungstenBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 230;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class GoldBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GoldBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 265;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class PlatinumBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PlatinumBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 275;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class DemoniteBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.DemoniteBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 295;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class CrimtaneBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CrimtaneBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 315;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class MeteoriteBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MeteoriteBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 300;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class HellstoneBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.HellstoneBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 395;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
}
