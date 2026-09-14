using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Plants
{
	public class Acorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.Acorn;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 22;
			item.AsFood().Size = 0.03;
		}
	}
	public class AmethystGemcorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GemTreeAmethystSeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 342;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}
	}
	public class TopazGemcorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GemTreeTopazSeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 358;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}
	}
	public class SapphireGemcorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GemTreeSapphireSeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 672;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}
	}
	public class EmeraldGemcorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GemTreeEmeraldSeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 686;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}
	}
	public class AmberGemcorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GemTreeAmberSeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 777;
			item.AsFood().Size = 0.1;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.33;
		}
	}
	public class RubyGemcorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GemTreeRubySeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 860;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.67;
		}
	}
	public class DiamondGemcorn : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GemTreeDiamondSeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 1210;
			item.AsFood().Size = 0.0325;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().WellFedPower = 1;
		}
	}
}
