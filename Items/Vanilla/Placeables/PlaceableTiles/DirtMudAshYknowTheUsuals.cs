using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.PlaceableTiles
{
	public class Dirt : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.DirtBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 90;
			item.AsFood().Size = 0.075;
		}
	}
	public class Clay : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.ClayBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 100;
			item.AsFood().Size = 0.075;
		}
	}
	public class Mud : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.MudBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 95;
			item.AsFood().Size = 0.09;
		}
	}
	public class Ash : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.AshBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 85;
			item.AsFood().Size = 0.06;
		}
	}
}
