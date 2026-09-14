using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Weapons.Ranged
{
	public class FlareGun : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.FlareGun;

		public override void SetDefaults(Item item)
		{
			item.damage = 20;
			item.DamageType = DamageClass.Ranged;
			item.shootSpeed = 10f;

			item.useTime = 27;
			item.useAnimation = 27;
			item.autoReuse = false;

			item.AsFood().MaxHealth = 900;
			item.AsFood().Size = 0.242;
			item.AsFood().AcidResistTier = 1;

			item.AsTaggable().Gun = true;
		}
	}
}
