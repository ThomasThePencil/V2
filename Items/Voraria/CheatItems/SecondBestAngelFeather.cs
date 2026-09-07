using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.CheatItems
{
	public class SecondBestAngelFeather : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.CheatItems.SecondBestAngelFeather");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.CheatItems.SecondBestAngelFeather.Short");
		public override string Texture => "V2/Items/UnspritedItem";
		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
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
			player.AsFood().PerfectMeal = true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.CheatItems.SecondBestAngelFeather",
				new
				{
					
				}
			);
		}
	}
}
