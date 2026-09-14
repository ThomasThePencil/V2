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
	public class AncientGoldHelmet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static float MagicDamageUpPercent => 0.05f;
		public static float MagicDamageUpFlat => 1f;
		public static float ManaEfficiencyUp => 0.08f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldHelmet;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = AncientGoldHelmetEffect;

			item.AsFood().MaxHealth = 930;
			item.AsFood().Size = 0.434;
			item.AsFood().AcidResistTier = 2;

			item.defense = 3;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void AncientGoldHelmetEffect(Item item, Player player)
		{
			player.GetDamage(DamageClass.Magic) += MagicDamageUpPercent;
			player.GetDamage(DamageClass.Magic).Flat += MagicDamageUpFlat;
			player.manaCost -= ManaEfficiencyUp;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.Gold.OldHead",
				new
				{
					GoldOldHeadMagDMGUpPercent = MagicDamageUpPercent.ToPercentage(2),
					GoldOldHeadMagDMGUpFlat = MagicDamageUpFlat,
					GoldOldHeadManaEffUp = ManaEfficiencyUp.ToPercentage(2),
				}
			);
		}
	}
}
