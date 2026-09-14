using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Lamps
{
	public class GraniteLamp : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.GraniteLamp;
		public override void SetDefaults(Item item)
		{
			item.DefaultToPlaceableTile(ModContent.TileType<global::V2.Tiles.Vanilla.Furniture.GraniteSet.GraniteLamp>());

			item.AsAnItem().PlaceableCanBeHungry = true;
			item.AsAnItem().PlaceableHungryByDefault = false;
		}

	}
}
