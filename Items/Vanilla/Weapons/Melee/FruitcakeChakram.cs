using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.Items.Vanilla.Weapons.Melee
{
	public class FruitcakeChakram : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int SwallowDamageToPred => 40;
		public static int PoisonTime => V2Utils.SensibleTime(seconds: 5);
		public static int WrathTime => V2Utils.SensibleTime(seconds: 5);
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FruitcakeChakram;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 444;
			item.AsFood().Size = 0.82;
			item.AsFood().MealSizeTextOverride = "Despite its size, it makes for a terrible meal";

			item.AsFood().OnSwallowDamage = 40;
			item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.Fruitcake";
			item.AsFood().OnSwallowSoreThroatTime = V2Utils.SensibleTime(seconds: 10, frames: 0);

			item.AsFood().UpdateInStomach += UpdateInStomach;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			pred.AddStatus(BuffID.Poisoned, PoisonTime, true);
			pred.AddStatus(BuffID.Wrath, WrathTime, true);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Weapons.Melee.FruitcakeChakram",
				new
				{
					FruitcakeChakramSwallowDamage = item.AsFood().OnSwallowDamage,
					FruitcakeChakramSoreThroatTime = ((double)item.AsFood().OnSwallowSoreThroatTime / 60.0).CastToDecimalPlaces(2),
					FruitcakeChakramPoisonTime = ((double)PoisonTime / 60.0).CastToDecimalPlaces(2),
					FruitcakeChakramWrathTime = ((double)WrathTime / 60.0).CastToDecimalPlaces(2)
				}
			);
		}
	}
}
