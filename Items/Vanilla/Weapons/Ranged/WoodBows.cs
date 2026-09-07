using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Ranged
{
	public class OakWoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodenBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 65;
			item.AsFood().Size = 0.25;

			item.AsTaggable().Bow = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Ranged.OakWoodBow",
				new
				{

				}
			);
		}
	}
	public class BorealWoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 65;
			item.AsFood().Size = 0.25;

			item.AsTaggable().Bow = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Ranged.BorealWoodBow",
				new
				{

				}
			);
		}
	}
	public class PalmWoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PalmWoodBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 65;
			item.AsFood().Size = 0.25;

			item.AsTaggable().Bow = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.PalmWoodBow",
				new
				{

				}
			);
		}*/
	}
	public class RichMahoganyBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.RichMahoganyBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 80;
			item.AsFood().Size = 0.25;

			item.AsTaggable().Bow = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Melee.RichMahoganyBow",
				new
				{

				}
			);
		}
	}
	public class AshWoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.AshWoodBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 90;
			item.AsFood().Size = 0.25;

			item.AsTaggable().Bow = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.AshWoodBow",
				new
				{

				}
			);
		}*/
	}
	public class EbonwoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.EbonwoodBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 110;
			item.AsFood().Size = 0.25;
			item.AsFood().WellFedPower = -0.04;

			item.AsTaggable().Bow = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.EbonwoodBow",
				new
				{

				}
			);
		}*/
	}
	public class ShadewoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ShadewoodBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 110;
			item.AsFood().Size = 0.25;
			item.AsFood().WellFedPower = -0.02;

			item.AsTaggable().Bow = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.ShadewoodBow",
				new
				{

				}
			);
		}*/
	}
	public class PearlwoodBow : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PearlwoodBow;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 140;
			item.AsFood().Size = 0.25;

			item.AsTaggable().Bow = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.ShadewoodBow",
				new
				{

				}
			);
		}*/
	}
}
