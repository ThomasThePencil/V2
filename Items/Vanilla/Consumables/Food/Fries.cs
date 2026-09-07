using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Consumables.Food
{
	public class Fries : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => false;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Fries;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 285;
			item.AsFood().Size = 0.08;

			item.buffType = 0;
			item.buffTime = 0;

			item.AsFood().EdibleOnUse = true;
			item.AsFood().AlwaysEatenByUse = true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Consumables.Food.Fries",
				new
				{
					
				}
			);
		}
	}
}
