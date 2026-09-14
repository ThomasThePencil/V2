using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class OakWoodSword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.WoodenSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 110;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Melee.OakWoodSword",
				new
				{
					
				}
			);
		}
	}
	public class BorealWoodSword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.BorealWoodSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 110;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Melee.BorealWoodSword",
				new
				{

				}
			);
		}
	}
	public class PalmWoodSword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PalmWoodSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 110;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.PalmWoodSword",
				new
				{

				}
			);
		}*/
	}
	public class RichMahoganySword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.RichMahoganySword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 180;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Melee.RichMahoganySword",
				new
				{

				}
			);
		}
	}
	public class AshWoodSword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.AshWoodSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.AshWoodSword",
				new
				{

				}
			);
		}*/
	}
	public class EbonwoodSword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.EbonwoodSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 220;
			item.AsFood().Size = 0.40;
			item.AsFood().WellFedPower = -0.04;

			item.AsTaggable().Broadsword = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.EbonwoodSword",
				new
				{

				}
			);
		}*/
	}
	public class ShadewoodSword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ShadewoodSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 220;
			item.AsFood().Size = 0.40;
			item.AsFood().WellFedPower = -0.02;

			item.AsTaggable().Broadsword = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.ShadewoodSword",
				new
				{

				}
			);
		}*/
	}
	public class PearlwoodSword : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PearlwoodSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 285;
			item.AsFood().Size = 0.40;

			item.AsTaggable().Broadsword = true;
		}

		/*public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Weapons.Melee.ShadewoodSword",
				new
				{

				}
			);
		}*/
	}
}
