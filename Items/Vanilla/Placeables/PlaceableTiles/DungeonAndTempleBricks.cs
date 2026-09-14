using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;
using V2.PlayerHandling.PredPlayerGoals.Skilled;

namespace V2.Items.Vanilla.Placeables.PlaceableTiles
{
	public class LihzahrdBrick : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.LihzahrdBrick;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50000;
			item.AsFood().Size = 0.125;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().OnBreak += OnBreak;
		}
		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<DigestTempleBrick>().TrySetCompletion(predPlayer);
			}
			return true;
		}
	}
	public class DungeonBrick : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.PinkBrick or ItemID.BlueBrick or ItemID.GreenBrick;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 2000;
			item.AsFood().Size = 0.125;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().OnBreak += OnBreak;
		}
		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<DigestDungeonBrick>().TrySetCompletion(predPlayer);
			}
			return true;
		}
	}
}
