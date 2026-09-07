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
using V2.Projectiles.Vanilla.Summons.Pets;

namespace V2.Items.Vanilla.Armor
{
	public class GoldChainDress : GlobalItem
	{
		public static float MagicDamageUp => 0.04f;
		public static float ManaEfficiencyUp => 0.08f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldChainmail;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = GoldChainDressEffect;

			item.AsFood().MaxHealth = 600;
			item.AsFood().Size = 0.485;
			item.AsFood().AcidResistTier = 2;

			item.defense = 3;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void GoldChainDressEffect(Item item, Player player)
		{
			if (player.position.Y < Main.worldSurface && player.behindBackWall && Main.dayTime)
				player.statDefense++;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.Gold.Chest",
				new
				{
					GoldDressMagDMGUp = MagicDamageUp.ToPercentage(2),
					GoldDressManaEffUp = ManaEfficiencyUp.ToPercentage(2)
				}
			);
		}
	}
}
