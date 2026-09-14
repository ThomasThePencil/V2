using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.TilesPlaceableTiles
{
	public class LunarBricks : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.LunarBrick or ItemID.LunarRustBrick or ItemID.DarkCelestialBrick
			or ItemID.AstraBrick or ItemID.CosmicEmberBrick or ItemID.CryocoreBrick or ItemID.MercuryBrick or ItemID.StarRoyaleBrick or ItemID.HeavenforgeBrick;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 7500;
			item.AsFood().Size = 0.11;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().CalorieMultiplier = 1.25;
		}
	}
	public class LunarBrickWalls : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.LunarBrickWall or ItemID.LunarRustBrickWall or ItemID.DarkCelestialBrickWall
			or ItemID.AstraBrickWall or ItemID.CosmicEmberBrickWall or ItemID.CryocoreBrickWall or ItemID.MercuryBrickWall or ItemID.StarRoyaleBrickWall or ItemID.HeavenforgeBrickWall;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 11000;
			item.AsFood().Size = 2;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().CalorieMultiplier = 1.25;
		}
	}
}
