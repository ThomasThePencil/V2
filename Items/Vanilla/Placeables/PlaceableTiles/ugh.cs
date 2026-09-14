using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.TilesPlaceableTiles
{
	public class Poo : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PoopBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 1;
			item.AsFood().Size = 0.06;
			item.AsFood().MealSizeTextOverride = "Please don't.";

			item.AsFood().OnSwallowDamage = 999999;
			item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.Poop";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(hours: 24);
		}
	}
	public class PooWall : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PoopWall;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 1;
			item.AsFood().Size = 0.6;
			item.AsFood().MealSizeTextOverride = "Please don't.";

			item.AsFood().OnSwallowDamage = 999999;
			item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.Poop";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(hours: 24);
		}
	}
}
