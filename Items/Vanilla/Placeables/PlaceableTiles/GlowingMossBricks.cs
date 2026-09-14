using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.TilesPlaceableTiles
{
	public class GlowingMossBricks : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.LavaMossBlock or ItemID.KryptonMossBlock or ItemID.XenonMossBlock
			or ItemID.ArgonMossBlock or ItemID.VioletMossBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 180;
			item.AsFood().Size = 0.11;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.33;
			item.AsFood().CalorieMultiplier = 1.45;
		}
	}
	public class HeliumMossBrick : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.RainbowMossBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 180;
			item.AsFood().Size = 0.11;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.01;
			item.AsFood().CalorieMultiplier = -0.45;
		}
	}
	public class GlowingMossBrickWalls : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.LavaMossBlockWall or ItemID.KryptonMossBlockWall
			or ItemID.XenonMossBlockWall or ItemID.ArgonMossBlockWall or ItemID.VioletMossBlockWall;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 695;
			item.AsFood().Size = 2;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.33;
			item.AsFood().CalorieMultiplier = 1.45;
		}
	}
	public class HeliumMossBrickWall : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.RainbowMossBlockWall;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 695;
			item.AsFood().Size = 2;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.01;
			item.AsFood().CalorieMultiplier = -0.45;
		}
	}
}
