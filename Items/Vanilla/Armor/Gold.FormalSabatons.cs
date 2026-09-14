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
using V2.Projectiles.Vanilla.Summons.Pets;

namespace V2.Items.Vanilla.Armor
{
	public class GoldFormalSabatons : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static float ManaEfficiencyUp => 0.06f;
		public static float MovingManaRateUp => 0.08f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldGreaves;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = GoldFormalSabatonsEffect;

			item.AsFood().MaxHealth = 860;
			item.AsFood().Size = 0.42;
			item.AsFood().AcidResistTier = 2;

			item.defense = 4;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void GoldFormalSabatonsEffect(Item item, Player player)
		{
			player.manaCost -= ManaEfficiencyUp;
			player.AddManaRegenEffect(
				manaPerSecond: 0.0,
				modifyTotalManaRegenMethod: GoldFormalSabatonsModifyManaRegen
			);
		}

		public static void GoldFormalSabatonsModifyManaRegen(Player player, ref double naturalRegenAdditive, ref double naturalRegenMultiplicative, ref double artificialRegenAdditive, ref double artificialRegenMultiplicative)
		{
			if (player.velocity.Length() > 0)
			{
				naturalRegenAdditive += MovingManaRateUp;
				artificialRegenAdditive += MovingManaRateUp;
			}
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.Gold.Legs",
				new
				{
					GoldSabsManaEffUp = ManaEfficiencyUp.ToPercentage(2),
					GoldSabsMovingManaRateUp = MovingManaRateUp.ToPercentage(2)
				}
			);
		}
	}
}
