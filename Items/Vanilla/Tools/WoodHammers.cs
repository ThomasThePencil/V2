using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class OakWoodHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodenHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 165;
			item.AsFood().Size = 0.45;

			item.AsTaggable().Hammer = true;
		}
	}
	public class BorealWoodHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 165;
			item.AsFood().Size = 0.45;

			item.AsTaggable().Hammer = true;
		}
	}
	public class PalmWoodHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PalmWoodHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 165;
			item.AsFood().Size = 0.45;

			item.AsTaggable().Hammer = true;
		}
	}
	public class RichMahoganyHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.RichMahoganyHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 0.45;

			item.AsTaggable().Hammer = true;
		}
	}
	public class AshWoodHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.AshWoodHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 230;
			item.AsFood().Size = 0.45;

			item.AsTaggable().Hammer = true;
		}
	}
	public class EbonwoodHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.EbonwoodHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 280;
			item.AsFood().Size = 0.45;
			item.AsFood().WellFedPower = -0.04;

			item.AsTaggable().Hammer = true;
		}
	}
	public class ShadewoodHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ShadewoodHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 280;
			item.AsFood().Size = 0.45;
			item.AsFood().WellFedPower = -0.02;

			item.AsTaggable().Hammer = true;
		}
	}
	public class PearlwoodHammer : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PearlwoodHammer;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 325;
			item.AsFood().Size = 0.45;

			item.AsTaggable().Hammer = true;
		}
	}
}
