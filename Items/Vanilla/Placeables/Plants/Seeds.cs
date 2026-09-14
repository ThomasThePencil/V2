using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Plants
{
	public class GrassSeeds : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.GrassSeeds or ItemID.JungleGrassSeeds or ItemID.HallowedSeeds
			or ItemID.MushroomGrassSeeds or ItemID.AshGrassSeeds;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.03;
		}
	}
	public class HerbSeeds : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.DaybloomSeeds or ItemID.BlinkrootSeeds or ItemID.MoonglowSeeds
			or ItemID.WaterleafSeeds or ItemID.ShiverthornSeeds or ItemID.FireblossomSeeds;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.03;
			item.AsFood().WellFedPower = 0.33;
		}
	}
	public class EvilSeeds : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.CorruptSeeds or ItemID.CrimsonSeeds or ItemID.DeathweedSeeds;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.03;
			item.AsFood().WellFedPower = -0.33;
		}
	}
	public class FlowerPackets : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.FlowerPacketBlue or ItemID.FlowerPacketMagenta or ItemID.FlowerPacketPink
			or ItemID.FlowerPacketRed or ItemID.FlowerPacketTallGrass or ItemID.FlowerPacketViolet or ItemID.FlowerPacketWhite or ItemID.FlowerPacketWild or ItemID.FlowerPacketYellow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 28;
			item.AsFood().Size = 0.05;
		}
	}
	public class PumpkinSeed : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.PumpkinSeed;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 19;
			item.AsFood().Size = 0.04;
			item.AsFood().WellFedPower = 0.15;
		}
	}
}
