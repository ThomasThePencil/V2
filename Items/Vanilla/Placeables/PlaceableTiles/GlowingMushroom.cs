using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.TilesPlaceableTiles
{
	public class GlowingMushroom : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GlowingMushroom;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 16;
			item.AsFood().Size = 0.08;
			item.AsFood().WellFedPower = 0.15;
			item.AsFood().CalorieMultiplier = 1.5;
		}
	}
}
