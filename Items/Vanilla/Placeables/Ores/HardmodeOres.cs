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
	public class CobaltOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CobaltOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 420;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class PalladiumOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PalladiumOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 435;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class MythrilOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MythrilOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 510;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class OrichalcumOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.OrichalcumOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 540;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class AdamantiteOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.AdamantiteOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 600;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class TitaniumOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TitaniumOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 665;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class ChlorophyteOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ChlorophyteOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 775;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class LuminiteOre : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LunarOre;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 2500;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
}
