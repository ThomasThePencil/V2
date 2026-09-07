using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;
using V2.StatusEffects.Vanilla.Buffs;
using V2.StatusEffects.Vanilla.Debuffs;

namespace V2.Items.Vanilla.Consumables.Potions
{
	public class Ale : GlobalItem
	{
		public static int DigestedTipsyTime => V2Utils.SensibleTime(minutes: 1, seconds: 30);
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Ale;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 50;
			item.AsFood().Size = 0.04;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
			item.AsFood().AlwaysEatenByUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			pred.AddStatus(BuffID.Tipsy, DigestedTipsyTime, true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Consumables.Potions.Ale",
				new
				{
					
				}
			);
			tooltips.FirstOrDefault(x => x.Name == "BuffTime").Hide();
		}
	}
}
