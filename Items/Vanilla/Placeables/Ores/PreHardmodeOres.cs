using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Ores
{
	public class CopperOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CopperOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 100;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class TinOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TinOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 120;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class IronOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IronOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 175;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class LeadOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LeadOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 185;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class SilverOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SilverOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 215;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class TungstenOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TungstenOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 230;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class GoldOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GoldOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 265;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class PlatinumOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PlatinumOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 275;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class DemoniteOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.DemoniteOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 295;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class CrimtaneOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CrimtaneOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 315;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class MeteoriteOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.Meteorite;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 300;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class HellstoneOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.Hellstone;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 395;
			item.AsFood().Size = 0.075;
			item.AsFood().AcidResistTier = 2;
		}
	}
}
