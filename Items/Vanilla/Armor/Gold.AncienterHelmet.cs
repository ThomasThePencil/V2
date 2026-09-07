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
	public class AncienterGoldHelmet : GlobalItem
	{
		public static float MagicDamageUpPercent => 0.06f;
		public static float MagicDamageUpFlat => 2f;
		public static float MagicCritChanceUp => 0.01f;
		public static float ManaEfficiencyUp => 0.10f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.AncientGoldHelmet;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = AncienterGoldHelmetEffect;

			item.AsFood().MaxHealth = 1250;
			item.AsFood().Size = 0.30;
			item.AsFood().AcidResistTier = 2;

			item.defense = 2;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void AncienterGoldHelmetEffect(Item item, Player player)
		{
			player.GetDamage(DamageClass.Magic) += MagicDamageUpPercent;
			player.GetDamage(DamageClass.Magic).Flat += MagicDamageUpFlat;
			player.GetCritChance(DamageClass.Magic) += MagicCritChanceUp * 100f;
			player.manaCost -= ManaEfficiencyUp;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.Gold.OldOldHead",
				new
				{
					GoldOldOldHeadMagDMGUpPercent = MagicDamageUpPercent.ToPercentage(2),
					GoldOldOldHeadMagDMGUpFlat = MagicDamageUpFlat,
					GoldOldOldHeadMagCritUp = MagicCritChanceUp.ToPercentage(2),
					GoldOldOldHeadManaEffUp = ManaEfficiencyUp.ToPercentage(2),
				}
			);
		}
	}
}
