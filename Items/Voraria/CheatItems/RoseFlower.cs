using Microsoft.Xna.Framework.Input;
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

namespace V2.Items.Voraria.CheatItems
{
	[AutoloadEquip(EquipType.Face)]
	public class RoseFlower : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.CheatItems.RoseFlower");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.CheatItems.RoseFlower.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 0;
		}
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Master;
			Item.value = 0;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AsPred().Rose = true;
			player.AsPred().StomachWeightModifier *= 0.0f;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.CheatItems.RoseFlower",
				new
				{
					
				}
			);
		}
	}
}
