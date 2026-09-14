using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.TilesPlaceableTiles
{
	public class OakWood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Wood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 14;
			item.AsFood().Size = 0.08;
		}
	}
	public class PalmWood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PalmWood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 14;
			item.AsFood().Size = 0.08;
		}
	}
	public class BorealWood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 14;
			item.AsFood().Size = 0.08;
		}
	}
	public class RichMahogany : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.RichMahogany;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 15;
			item.AsFood().Size = 0.08;
		}
	}
	public class AshWood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.AshWood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 16;
			item.AsFood().Size = 0.08;
		}
	}
	public class Ebonwood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Ebonwood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 19;
			item.AsFood().Size = 0.08;
			item.AsFood().WellFedPower = -0.04;
		}
	}
	public class Shadewood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Shadewood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 19;
			item.AsFood().Size = 0.08;
			item.AsFood().WellFedPower = -0.02;
		}
	}
	public class Pearlwood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Pearlwood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 21;
			item.AsFood().Size = 0.08;
		}
	}
	public class DynastyWood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.DynastyWood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.09;
		}
	}
	public class SpookyWood : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SpookyWood;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 28;
			item.AsFood().Size = 0.08;
			item.AsFood().WellFedPower = 0.25;
		}
	}
}
