using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.NPCs;
using V2.Projectiles;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Tools
{
	public class BottomlessBuckets : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
			entity.type == ItemID.BottomlessBucket || entity.type == ItemID.BottomlessLavaBucket || entity.type == ItemID.BottomlessHoneyBucket || entity.type == ItemID.BottomlessShimmerBucket;

		public override void SetDefaults(Item item)
		{
			item.AsFood().PreSwallow = PreSwallow;
		}
		public static bool PreSwallow(Item item, Entity pred)
		{
			int liquid = item.type switch
			{
				ItemID.BottomlessBucket => LiquidID.Water,
				ItemID.BottomlessLavaBucket => LiquidID.Lava,
				ItemID.BottomlessHoneyBucket => LiquidID.Honey,
				ItemID.BottomlessShimmerBucket => LiquidID.Shimmer,
				_ => LiquidID.Water,
			};
			if (pred is Player predPlayer)
			{
				//if (predPlayer.AsPred().Rose || predPlayer.AsPred().StomachCapacity - predPlayer.AsPred().StomachFullness >= predPlayer.AsPred().EffectiveLiquidSwallowSize(liquid))
				//{
					PredPlayer.Drink(predPlayer, liquid, predPlayer.AsPred().LiquidSwallowSize);
				//}
			}
			return true;
		}
	}
}
