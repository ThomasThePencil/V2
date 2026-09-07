using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Items.Voraria.Weapons.Summon;

namespace V2.Items.Voraria
{
	public class ObserverPupil : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.ObserverPupil");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.ObserverPupil.Short");
		public override void SetDefaults()
		{
			Item.maxStack = Item.CommonMaxStack;

			Item.width = 16;
			Item.height = 16;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(
				platinum: 0,
				gold: 0,
				silver: 50,
				copper: 0
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.ObserverPupil",
				new
				{
					
				}
			);
		}
	}
	public class Binoculars : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Binoculars;
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ItemID.Binoculars);
			recipe
				.AddRecipeGroup(RecipeGroupID.IronBar, 6)
				.AddIngredient(ItemID.Lens, 4)
				.AddIngredient<ObserverPupil>(4)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
