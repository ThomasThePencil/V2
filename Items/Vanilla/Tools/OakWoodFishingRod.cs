using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class OakWoodFishingRod : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodFishingPole;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 58;
			item.AsFood().Size = 0.28;
		}
	}
}
