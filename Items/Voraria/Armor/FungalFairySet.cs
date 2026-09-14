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
using V2.PlayerHandling;

namespace V2.Items.Voraria.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class MushroomHairpin : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static LocalizedText SetBonusText => Language.GetText("Mods.V2.ItemTooltip.Voraria.Armor.FungalFairySetBonus");
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Armor.MushroomHairpin");
		public static int GLPBonus => 7;
		public static int ABSBonus => 7;

		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.sellPrice(
				gold: 5, silver: 75
			);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 6;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FungalDress>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			player.AsPred().FungalFairySetBonus = true;
		}
		public override void UpdateEquip(Player player)
		{
			player.AsPred().GLP.Extra += GLPBonus;
			player.AsPred().ABS.Extra += ABSBonus;
			player.maxMinions += 1;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Armor.MushroomHairpin",
				new
				{

				}
			);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<MushroomToken>()
				.AddIngredient(ItemID.GlowingMushroom)
				.AddIngredient(ItemID.MushroomGrassSeeds, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
	[AutoloadEquip(EquipType.Body)]
	public class FungalDress : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Armor.FungalDress");
		public static int TUMBonus => 9;
		public static float StomachWeightReduction => 0.2f;

		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(
				gold: 5, silver: 95
			);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 7;
		}
		public override void UpdateEquip(Player player)
		{
			player.AsPred().TUM.Extra += TUMBonus;
			player.AsPred().StomachWeightModifier *= 1f - StomachWeightReduction;
			player.maxMinions += 1;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Armor.FungalDress",
				new
				{

				}
			);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<MushroomToken>()
				.AddIngredient(ItemID.GlowingMushroom, 15)
				.AddIngredient(ItemID.MushroomGrassSeeds, 4)
				.AddIngredient(ItemID.Silk, 8)
				.AddTile(TileID.Loom)
				.Register();
		}
	}
}
