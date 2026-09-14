using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.TilesPlaceableTiles
{
	public class Stone : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.StoneBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 110;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().Size = 0.1;
		}
	}
	public class EvilStone : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.EbonstoneBlock || entity.type == ItemID.CrimstoneBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 350;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().Size = 0.1;
		}
	}
	public class Pearlstone : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PearlstoneBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 350;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().Size = 0.1;
		}
	}
}
