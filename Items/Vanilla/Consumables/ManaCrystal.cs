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

namespace V2.Items.Vanilla.Consumables
{
	public class ManaCrystal : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int DigestedManaHeal => 100;
		public static int DigestedManaRegenTime => V2Utils.SensibleTime(seconds: 2, frames: 30);
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.ManaCrystal;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 0.42;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (!dead)
				return;

			pred.AddStatus(BuffID.ManaRegeneration, DigestedManaRegenTime, true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);

			if (pred is Player playerPred)
			{
				if (playerPred.ConsumedManaCrystals < Player.ManaCrystalMax)
				{
					int manaCrystalsLeftToMax = Math.Min(item.stack, Player.ManaCrystalMax - playerPred.ConsumedManaCrystals);
					playerPred.statManaMax += 20 * manaCrystalsLeftToMax;
					playerPred.statManaMax2 += 20 * manaCrystalsLeftToMax;
					playerPred.ConsumedManaCrystals += manaCrystalsLeftToMax;
				}
				playerPred.statMana += DigestedManaHeal * item.stack;
				if (playerPred.statMana > playerPred.statManaMax2)
					playerPred.statMana = playerPred.statManaMax2;

				ModContent.GetInstance<EatFirstManaCrystal>().TrySetCompletion(playerPred);
				if (playerPred.ConsumedLifeCrystals == Player.LifeCrystalMax && playerPred.ConsumedManaCrystals == Player.ManaCrystalMax)
					ModContent.GetInstance<EatMaxLifeAndManaCrystals>().TrySetCompletion(playerPred);
			}
			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			Color manaCrystalsUsedColor = Color.Lerp(Color.DarkBlue, Color.CornflowerBlue, (float)player.ConsumedManaCrystals / (float)Player.ManaCrystalMax);
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Consumables.ManaCrystal",
				new
				{
					ManaCrystalEatManaHeal = DigestedManaHeal,
					ManaCrystalEatManaRegenLength = ((double)DigestedManaRegenTime / 60.0).CastToDecimalPlaces(2),
					ManaCrystalsUsedColor = (manaCrystalsUsedColor * ((int)Main.mouseTextColor / 255f)).Hex3(),
					ManaCrystalsUsed = player.ConsumedManaCrystals,
					ManaCrystalsMax = Player.ManaCrystalMax
				}
			);
		}
	}

	public class ManaCrystalPlayer : ModPlayer
	{
		public override void PreUpdateBuffs()
		{
			Player.AsPred().ABS.Base += Player.ConsumedManaCrystals;
		}
	}
}
