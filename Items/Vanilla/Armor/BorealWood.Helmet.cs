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
	public class BorealWoodHelmet : GlobalItem
	{
		public static double ManaRegenInCold => 2.0;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodHelmet;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = BorealWoodHelmetEffect;

			item.AsFood().MaxHealth = 120;
			item.AsFood().Size = 0.30;

			item.defense = 1;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void BorealWoodHelmetEffect(Item item, Player player)
		{
			if (player.GetModPlayer<V2Player>().InTheCold)
				player.AddManaRegenEffect(
					ManaRegenInCold
				);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.BorealWood.Head",
				new
				{
					BorealWoodHelmColdManaRegenBonus = ManaRegenInCold
				}
			);
		}
	}
}
