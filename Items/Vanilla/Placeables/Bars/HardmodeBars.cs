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
	public class CobaltBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CobaltBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 420;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class PalladiumBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PalladiumBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 435;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class MythrilBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MythrilBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 510;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class OrichalcumBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.OrichalcumBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 540;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class AdamantiteBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.AdamantiteBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 600;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class TitaniumBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TitaniumBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 665;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class SynthesiteBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.HallowedBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 695;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class ChlorophyteBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ChlorophyteBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 775;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class ShroomiteBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ShroomiteBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 800;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class SpectreBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SpectreBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 790;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class LuminiteBar : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.LunarBar;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 2500;
			item.AsFood().Size = 0.05;
			item.AsFood().AcidResistTier = 2;
		}
	}
}
