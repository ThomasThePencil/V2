using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Weapons.Summon;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Items.Voraria
{
	public class MushroomToken : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.MushroomToken");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.MushroomToken.Short");
		public override string Texture => "V2/Items/UnspritedItem";

		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.maxStack = Item.CommonMaxStack;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(
				gold: 5
			);

			Item.AsFood().MaxHealth = 500;
			Item.AsFood().Size = 0.21;
			Item.AsFood().OnBreak += OnBreak;
		}
		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			if (pred is Player predPlayer)
			{
				ModContent.GetInstance<EatFungalGift>().TrySetCompletion(predPlayer);
			}
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.MushroomToken",
				new
				{
					
				}
			);
		}
	}
}
