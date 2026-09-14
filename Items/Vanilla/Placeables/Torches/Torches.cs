using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Torches
{
	public class Torch : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Torch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 11;
			item.AsFood().Size = 0.05;
		}
	}
	public class GemTorches : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.PurpleTorch or ItemID.YellowTorch or ItemID.GreenTorch
			or ItemID.BlueTorch or ItemID.OrangeTorch or ItemID.RedTorch or ItemID.WhiteTorch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 44;
			item.AsFood().Size = 0.05;
		}
	}
	public class UniqueTorches : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.IceTorch or ItemID.DesertTorch or ItemID.RainbowTorch
			or ItemID.UltrabrightTorch or ItemID.CoralTorch or ItemID.JungleTorch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 14;
			item.AsFood().Size = 0.05;
		}
	}
	public class TastyTorches : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.PinkTorch or ItemID.MushroomTorch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 14;
			item.AsFood().Size = 0.05;
			item.AsFood().WellFedPower = 0.2;
			item.AsFood().CalorieMultiplier = 1.5;
		}
	}
	public class NotTastyTorches : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.CorruptTorch or ItemID.CrimsonTorch or ItemID.DemonTorch
			or ItemID.BoneTorch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 16;
			item.AsFood().Size = 0.05;
			item.AsFood().WellFedPower = -0.2;
		}
	}
	public class VeryNotTastyTorches : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.CursedTorch or ItemID.IchorTorch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 22;
			item.AsFood().Size = 0.05;
			item.AsFood().WellFedPower = -1;
		}
	}
	public class AetherTorch : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.ShimmerTorch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 25;
			item.AsFood().Size = 0.05;
			item.AsFood().WellFedPower = -1.5;
			item.AsFood().CalorieMultiplier = -1.75;
		}
	}
}
