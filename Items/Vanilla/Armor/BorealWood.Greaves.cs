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
	public class BorealWoodGreaves : GlobalItem
	{
		public static double MoveSpeedInCold => 0.08;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodGreaves;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = BorealWoodGreavesEffect;

			item.AsFood().MaxHealth = 160;
			item.AsFood().Size = 0.40;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void BorealWoodGreavesEffect(Item item, Player player)
		{
			if (player.GetModPlayer<V2Player>().InTheCold)
				player.moveSpeed += (float)MoveSpeedInCold;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.BorealWood.Legs",
				new
				{
					BorealWoodBootsColdMoveSpeedBonus = MoveSpeedInCold.ToPercentage()
				}
			);
		}
	}
}
