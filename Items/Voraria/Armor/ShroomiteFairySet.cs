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
	public class ShroomiteHairpin : ModItem
	{
		public static LocalizedText SetBonusText => Language.GetText("Mods.V2.ItemTooltip.Voraria.Armor.FungalFairySetBonus");
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Armor.ShroomiteHairpin");
		public static int GLPBonus => 19;
		public static int ABSBonus => 19;
		public static int ACIBonus => 6;

		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(
				gold: 7
			);
			Item.rare = ItemRarityID.Yellow;
			Item.defense = 10;
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
			player.AsPred().ACI.Extra += ACIBonus;
			player.maxMinions += 3;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Armor.ShroomiteHairpin",
				new
				{

				}
			);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<MushroomHairpin>()
				.AddIngredient(ItemID.ShroomiteBar, 2)
				.AddIngredient(ItemID.Ectoplasm, 5)
				.AddTile(TileID.Autohammer)
				.Register();
		}
	}
	[AutoloadEquip(EquipType.Body)]
	public class ShroomiteDress : ModItem
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Armor.ShroomiteDress");
		public static int TUMBonus => 22;
		public static int ACIBonus => 6;
		public static float StomachWeightReduction => 0.4f;

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
				gold: 7, silver: 50
			);
			Item.rare = ItemRarityID.Yellow;
			Item.defense = 12;
		}
		public override void UpdateEquip(Player player)
		{
			player.AsPred().TUM.Extra += TUMBonus;
			player.AsPred().ACI.Extra += ACIBonus;
			player.AsPred().StomachWeightModifier *= 1f - StomachWeightReduction;
			player.maxMinions += 2;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Armor.ShroomiteDress",
				new
				{

				}
			);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<FungalDress>()
				.AddIngredient(ItemID.ShroomiteBar, 5)
				.AddIngredient(ItemID.Ectoplasm, 2)
				.AddTile(TileID.Autohammer)
				.Register();
		}
	}
}
