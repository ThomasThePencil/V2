using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Vases
{
	public class PinkVase : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PinkVase;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 55;
			item.AsFood().Size = 0.045;
		}
	}
	public class DungeonVase : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.BlueDungeonVase or ItemID.PinkDungeonVase or ItemID.GreenDungeonVase;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 800;
			item.AsFood().Size = 1.85;
			item.AsFood().AcidResistTier = 2;
		}
	}
	public class ObsidianVase : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.ObsidianVase;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 1.85;
			item.AsFood().AcidResistTier = 2;
		}
	}
}
