using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;
using V2.UI.VoreBestiary;

namespace V2.Items.Voraria
{
	public class VoreBestiary : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.VoreBestiary");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.VoreBestiary");
		public override string Texture => "V2/Items/UnspritedItem";
		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;

			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.maxStack = 1;

			Item.useAnimation = 5;
			Item.useTime = 5;
			Item.noUseGraphic = true;

			Item.width = 26;
			Item.height = 26;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.buyPrice(
				platinum: 0,
				gold: 15,
				silver: 0,
				copper: 0
			);
		}

		public override bool? UseItem(Player player)
		{
			player.AsV2Player().LookingAtAEM = true;
			VoreBestiaryUI.SelectedTab = AEMTab.DivineIntervention;
			return null;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.VoreBestiary",
				new
				{
					
				}
			);
		}
	}
}
