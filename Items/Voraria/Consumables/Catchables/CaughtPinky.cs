using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Vanilla.Forest;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Consumables.Catchables
{
	public class CaughtPinky : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.Catchables.Pinky");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.Catchables.Pinky.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToCapturedCritter(NPCID.BlueSlime);
			Item.AsAnItem().ReleasedNPCNetID = NPCID.Pinky;

			Item.width = 30;
			Item.height = 30;
			Item.alpha = 100;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(
				gold: 1,
				silver: 25
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.Catchables.Pinky",
				new
				{
					PinkyEatHeal = Pinky.DigestedHeal,
					PinkyEatHappyLength = (Pinky.EatenHappyLength / 60.0).CastToDecimalPlaces(2),
					PinkyEatRegenLength = (Pinky.DigestedRegenTime / 60.0).CastToDecimalPlaces(2),
				}
			);
		}
	}
}
