using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Plants
{
	public class RegularMoss : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.GreenMoss or ItemID.BrownMoss or ItemID.RedMoss
			or ItemID.BlueMoss or ItemID.PurpleMoss;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.03;
			item.AsFood().WellFedPower = 0.15;
		}
	}
	public class GlowingMoss : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.LavaMoss or ItemID.KryptonMoss or ItemID.XenonMoss
			or ItemID.ArgonMoss or ItemID.VioletMoss;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.03;
			item.AsFood().WellFedPower = 0.67;
			item.AsFood().CalorieMultiplier = 1.75;
		}
	}
	public class HeliumMoss : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.RainbowMoss;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.03;
			item.AsFood().WellFedPower = 0.05;
			item.AsFood().CalorieMultiplier = -0.75;
		}
	}
}
