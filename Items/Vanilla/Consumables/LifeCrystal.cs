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
	public class LifeCrystal : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int DigestedHeal => 25;
		public static int DigestedRegenTime => V2Utils.SensibleTime(seconds: 35);
		public static float MaxEatenPermaWeightReduction => 0.15f;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.LifeCrystal;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 400;
			item.AsFood().Size = 0.45;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
				pred.AddStatus(BuffID.Regeneration, DigestedRegenTime, true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);

			if (pred is Player playerPred)
			{
				if (playerPred.ConsumedLifeCrystals < Player.LifeCrystalMax)
				{
					int lifeCrystalsLeftToMax = Math.Min(item.stack, Player.LifeCrystalMax - playerPred.ConsumedLifeCrystals);
					playerPred.statLifeMax += 20 * lifeCrystalsLeftToMax;
					playerPred.statLifeMax2 += 20 * lifeCrystalsLeftToMax;
					playerPred.ConsumedLifeCrystals += lifeCrystalsLeftToMax;
				}
				playerPred.Heal(DigestedHeal * item.stack);
				ModContent.GetInstance<EatFirstLifeCrystal>().TrySetCompletion(playerPred);
				if (playerPred.ConsumedLifeCrystals == Player.LifeCrystalMax && playerPred.ConsumedManaCrystals == Player.ManaCrystalMax)
					ModContent.GetInstance<EatMaxLifeAndManaCrystals>().TrySetCompletion(playerPred);
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
			Color lifeCrystalsUsedColor = Color.Lerp(Color.DarkRed, Color.HotPink, (float)player.ConsumedLifeCrystals / (float)Player.LifeCrystalMax);
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Consumables.LifeCrystal",
				new
				{
					LifeCrystalEatHeal = DigestedHeal,
					LifeCrystalEatRegenLength = ((double)DigestedRegenTime / 60.0).CastToDecimalPlaces(2),
					LifeCrystalsUsedColor = (lifeCrystalsUsedColor * ((int)Main.mouseTextColor / 255f)).Hex3(),
					LifeCrystalsUsed = player.ConsumedLifeCrystals,
					MaxLifeCrystalsPermaWeightReduction = MaxEatenPermaWeightReduction.CastToDecimalPlaces(2),
					LifeCrystalsMax = Player.LifeCrystalMax
				}
			);
		}
	}

	public class LifeCrystalPlayer : ModPlayer
	{
		public override void PreUpdateBuffs()
		{
			Player.AsPred().TUM.Base += Player.ConsumedLifeCrystals;
			if (Player.ConsumedLifeCrystals == Player.LifeCrystalMax)
				Player.AsPred().StomachWeightModifier *= 1f - LifeCrystal.MaxEatenPermaWeightReduction;
		}
	}
}
