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
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Vanilla.Consumables
{
	public class LifeFruit : GlobalItem
	{
		public static int DigestedHeal => 50;
		public static int DigestedRegenTime => V2Utils.SensibleTime(minutes: 1, seconds: 30);
		public static int StomachStrengthBonus => 2;
		public static int AcidStrengthBonus => 2;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LifeFruit;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 500;
			item.AsFood().Size = 0.34;
			item.AsFood().WellFedPower = 1;
			item.AsFood().CalorieMultiplier = 1.5;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
			item.AsFood().AlwaysEatenByUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
				pred.AddStatus(BuffID.Regeneration, DigestedRegenTime, true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);

			if (pred is Player playerPred)
			{
				if (playerPred.ConsumedLifeFruit < Player.LifeFruitMax && playerPred.ConsumedLifeCrystals == Player.LifeCrystalMax)
				{
					int lifeFruitLeftToMax = Math.Min(item.stack, Player.LifeFruitMax - playerPred.ConsumedLifeFruit);
					playerPred.statLifeMax += 5 * lifeFruitLeftToMax;
					playerPred.statLifeMax2 += 5 * lifeFruitLeftToMax;
					playerPred.ConsumedLifeFruit += lifeFruitLeftToMax;
				}
				playerPred.Heal(DigestedHeal * item.stack);
			}
			else if (pred is NPC NPCPred)
			{
				NPCPred.life += DigestedHeal * item.stack;
				if (NPCPred.life > NPCPred.lifeMax)
					NPCPred.life = NPCPred.lifeMax;
			}
			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			Color lifeFruitUsedColor = Color.Lerp(Color.DarkGoldenrod, Color.Goldenrod, (float)player.ConsumedLifeFruit / (float)Player.LifeFruitMax);
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Consumables.LifeFruit",
				new
				{
					LifeFruitEatHeal = DigestedHeal,
					LifeFruitEatRegenLength = ((double)DigestedRegenTime / 60.0).CastToDecimalPlaces(2),
					LifeFruitHealthBonusColor = Color.Goldenrod.Hex3(),
					LifeFruitStomachStrengthBonus = StomachStrengthBonus,
					LifeFruitAcidStrengthBonus = AcidStrengthBonus,
					LifeFruitUsedColor = (lifeFruitUsedColor * ((int)Main.mouseTextColor / 255f)).Hex3(),
					LifeFruitUsed = player.ConsumedLifeFruit,
					LifeFruitMax = Player.LifeFruitMax,
				}
			);
		}
	}

	public class LifeFruitPlayer : ModPlayer
	{
		public override void PreUpdateBuffs()
		{
			Player.AsPred().TUM.Base += LifeFruit.StomachStrengthBonus * Player.ConsumedLifeFruit;
			Player.AsPred().ACI.Base += LifeFruit.AcidStrengthBonus * Player.ConsumedLifeFruit;
		}
	}
}
