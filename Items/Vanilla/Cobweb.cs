using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla
{
	public class Cobweb : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Cobweb;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 19;
			item.AsFood().Size = 0.005;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Cobweb",
				new
				{
					
				}
			);
		}
	}
}
