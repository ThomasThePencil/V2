using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Gems
{
	public class Amethyst : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Amethyst;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 332;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Placeables.Gems.Amethyst",
				new
				{

				}
			);
		}
	}
	public class Topaz : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Topaz;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 348;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Placeables.Gems.Topaz",
				new
				{

				}
			);
		}
	}
	public class Sapphire : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Sapphire;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 662;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Placeables.Gems.Sapphire",
				new
				{

				}
			);
		}
	}
	public class Emerald : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Emerald;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 676;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Placeables.Gems.Emerald",
				new
				{

				}
			);
		}
	}
	public class Amber : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Amber;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 777;
			item.AsFood().Size = 0.1;
			item.AsFood().WellFedPower = 0.33;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Placeables.Gems.Amber",
				new
				{

				}
			);
		}
	}
	public class Ruby : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Ruby;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 850;
			item.AsFood().Size = 0.030;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.67;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Placeables.Gems.Ruby",
				new
				{

				}
			);
		}
	}
	public class Diamond : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Diamond;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 1200;
			item.AsFood().Size = 0.0325;
			item.AsFood().AcidResistTier = 2;
			item.AsFood().WellFedPower = 1;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Placeables.Gems.Diamond",
				new
				{

				}
			);
		}
	}
}
