using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Tools
{
	public class Haemorraxe : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BloodHamaxe;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 882;
			item.AsFood().Size = 0.37;

			item.AsTaggable().Hamaxe = true;
		}
	}
}
