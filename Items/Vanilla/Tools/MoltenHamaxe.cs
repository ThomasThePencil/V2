using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class MoltenHamaxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.MoltenHamaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 999;
			item.AsFood().Size = 0.74;

			item.AsTaggable().Hamaxe = true;
		}
	}
}
