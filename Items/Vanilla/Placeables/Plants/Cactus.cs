using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Plants
{
	public class Cactus : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Cactus;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 15;
			item.AsFood().Size = 0.08;

			item.AsFood().OnSwallowDamage = 3;
			item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.Cactus";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 1, frames: 30);

		}
	}
}
