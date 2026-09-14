using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Bottles
{
	public class Bottle : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Bottle;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50;
			item.AsFood().Size = 0.045;
		}
	}
}
