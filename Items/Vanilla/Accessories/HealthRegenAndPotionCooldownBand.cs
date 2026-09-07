using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Items.Vanilla.Accessories
{
	public class HealthRegenAndPotionCooldownBand : GlobalItem
	{
		public static double WornHealthRegenFlat => 1.0;
		public static double DigestingHealthRegenFlat => 2.0;
		public static int DigestingEffectTime => V2Utils.SensibleTime(minutes: 10);
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CharmofMyths;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 1500;
			item.AsFood().Size = 0.08;
			item.AsFood().AcidResistTier = 1;

			item.AsAnItem().AccessoryEffectCode += UpdateHealthRegenAndPotionCooldownBand;

			item.lifeRegen = 0;

			item.AsFood().UpdateInStomach += UpdateInStomach;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void UpdateHealthRegenAndPotionCooldownBand(Item item, Player player, bool hideVisual)
		{
			player.AddHealthRegenEffect(
				healthPerSecond: WornHealthRegenFlat
			);
			player.pStone = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead && pred is Player player)
				player.AddStatus(ModContent.BuffType<CharmofMythsChurnBuff>(), DigestingEffectTime, true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.player[Main.myPlayer];
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Accessories.HealthRegenAndPotionCooldownBand",
				new
				{
					HealthRegenBandIIWornHealthRegen = WornHealthRegenFlat,
					HealthRegenBandIIEatenHealthRegen = DigestingHealthRegenFlat,
				}
			);
		}
	}
}
