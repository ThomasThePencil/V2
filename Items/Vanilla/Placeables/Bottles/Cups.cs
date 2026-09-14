using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;

namespace V2.Items.Vanilla.Placeables.Bottles
{
	public class Mug : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Mug;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50;
			item.AsFood().Size = 0.04;
		}
	}
	public class DynastyCup : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.DynastyCup;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 10;
			item.AsFood().Size = 0.04;
		}
	}
	public class WineGlass : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WineGlass;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50;
			item.AsFood().Size = 0.03;
		}
	}
	public class HoneyCup : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.HoneyCup;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 10;
			item.AsFood().Size = 0.04;
		}
		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
			{
				pred.AddStatus(BuffID.Honey, V2Utils.SensibleTime(seconds: 5), true);
			}
		}
	}
	public class Chalice : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SteampunkCup;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 65;
			item.AsFood().Size = 0.03;
		}
	}
}
