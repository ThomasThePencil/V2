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
	public class BorealWoodBreastplate : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static double HealthRegenInCold => 0.4;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.BorealWoodBreastplate;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = BorealWoodBreastplateEffect;

			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 0.50;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void BorealWoodBreastplateEffect(Item item, Player player)
		{
			if (player.GetModPlayer<V2Player>().InTheCold)
				player.AddHealthRegenEffect(
					HealthRegenInCold
				);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.BorealWood.Chest",
				new
				{
					BorealWoodBreastplateColdHealthRegenBonus = HealthRegenInCold
				}
			);
		}
	}
}
