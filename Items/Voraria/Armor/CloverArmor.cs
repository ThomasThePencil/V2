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
using V2.PlayerHandling;

namespace V2.Items.Voraria.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class CloverHeadAccessories : ModItem
	{
		public static LocalizedText SetBonusText => Language.GetText("Mods.V2.ItemTooltip.Voraria.Armor.CloverArmorSetBonus");
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Armor.CloverHeadAccessories");
		public static float StruggleBonus => 0.75f;
		public static int GLPBonus => 5;
		public static int CritBonus => 7;

		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 30;
			Item.value = Item.buyPrice(gold: 7, silver: 7, copper: 7); //funny
			Item.rare = ItemRarityID.Lime;
			Item.defense = 7;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<CloverSweater>() && legs.type == ModContent.ItemType<CloverStockings>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = SetBonusText.Value;
			player.statDefense += 7;
			player.AsFood().StruggleDamageModifier += 1.5f;
		}
		public override void UpdateEquip(Player player)
		{
			player.AsFood().StruggleDamageModifier += StruggleBonus;
			player.AsPred().GLP.Extra += GLPBonus;
			player.GetCritChance(DamageClass.Generic) += CritBonus;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Armor.CloverHeadAccessories",
				new
				{

				}
			);
		}
	}
	[AutoloadEquip(EquipType.Body)]
	public class CloverSweater : ModItem
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Armor.CloverSweater");
		public static float StruggleBonus => 1f;
		public static int TUMBonus => 8;
		public static int ABSBonus => 6;
		public static int CritBonus => 7;
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 34;
			Item.value = Item.buyPrice(gold: 7, silver: 7, copper: 7); //funny
			Item.rare = ItemRarityID.Lime;
			Item.defense = 7;
		}
		public override void UpdateEquip(Player player)
		{
			player.AsFood().StruggleDamageModifier += StruggleBonus;
			player.AsPred().TUM.Extra += TUMBonus;
			player.AsPred().ABS.Extra += ABSBonus;
			player.GetCritChance(DamageClass.Generic) += CritBonus;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Armor.CloverSweater",
				new
				{

				}
			);
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class CloverStockings : ModItem
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Armor.CloverStockings");
		public static float StruggleBonus => 0.75f;
		public static int CritBonus => 7;
		public static float MoveSpeedBonus => 0.20f;
		public static float StomachWeightReduction => 0.15f;
		public override void SetStaticDefaults()
		{
			ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.value = Item.buyPrice(gold: 7, silver: 7, copper: 7); //funny
			Item.rare = ItemRarityID.Lime;
			Item.defense = 7;
		}
		public override void UpdateEquip(Player player)
		{
			player.AsFood().StruggleDamageModifier += StruggleBonus;
			player.moveSpeed += MoveSpeedBonus;
			player.AsPred().StomachWeightModifier *= 1f - StomachWeightReduction;
			player.GetCritChance(DamageClass.Generic) += CritBonus;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Armor.CloverStockings",
				new
				{

				}
			);
		}
	}
}
