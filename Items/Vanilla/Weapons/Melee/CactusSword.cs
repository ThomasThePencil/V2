using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class CactusSword : GlobalItem
	{
		public static int ThornsBuffTime => V2Utils.SensibleTime(seconds: 10);
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.CactusSword;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 151;
			item.AsFood().Size = 0.52;

			item.AsFood().OnSwallowDamage = 6;
			item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.Cactus";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 3, frames: 30);

			item.AsFood().UpdateInStomach += UpdateInStomach;

			item.AsTaggable().Broadsword = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
				pred.AddStatus(BuffID.Thorns, ThornsBuffTime, true);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Melee.CactusSword",
				new
				{
					CactusSwordSwallowDamage = item.AsFood().OnSwallowDamage,
					CactusSwordSoreThroatTime = ((double)item.AsFood().OnSwallowSoreThroatTime / 60.0).CastToDecimalPlaces(2),
					CactusSwordThornsTime = ((double)ThornsBuffTime / 60.0).CastToDecimalPlaces(2)
				}
			);
		}
	}
}
