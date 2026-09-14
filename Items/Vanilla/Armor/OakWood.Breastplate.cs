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
	public class OakWoodBreastplate : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int SunlightDefenseBonus => 1;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WoodBreastplate;

		public override void SetDefaults(Item item)
		{
			item.AsAnItem().ArmorEffectCode = OakWoodBreastplateEffect;

			item.AsFood().MaxHealth = 200;
			item.AsFood().Size = 0.50;

			item.defense = 1;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void OakWoodBreastplateEffect(Item item, Player player)
		{
			if (player.position.Y < Main.worldSurface && player.behindBackWall && Main.dayTime)
				player.statDefense += SunlightDefenseBonus;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => direct;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Armor.OakWood.Chest",
				new
				{
					
				}
			);
		}
	}
}
