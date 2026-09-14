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
	public class GoldBattleTiara : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static float MagicDamageUp => 0.04f;
		public static float ManaEfficiencyUp => 0.08f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldCrown;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = GoldBattleTiaraEffect;

			item.AsFood().MaxHealth = 690;
			item.AsFood().Size = 0.35;
			item.AsFood().AcidResistTier = 2;

			item.defense = 4;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void GoldBattleTiaraEffect(Item item, Player player)
		{
			player.GetDamage(DamageClass.Magic) += MagicDamageUp;
			player.manaCost -= ManaEfficiencyUp;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.Gold.Head",
				new
				{
					GoldCrownMagDMGUp = MagicDamageUp.ToPercentage(),
					GoldCrownManaEffUp = ManaEfficiencyUp.ToPercentage(),
				}
			);
		}
	}
}
