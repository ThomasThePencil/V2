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

namespace V2.Items.Voraria.Charms
{
	public class CharmRegenFromAbsorption : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static double HealthRegenerationRatio => 1.5;
		public static double ManaRegenerationRatio => 4.25;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Charms.RegenFromAbsorption");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Charms.RegenFromAbsorption.Short");

		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.AsCharm().IsCharm = true;
			Item.AsAnItem().AccessoryEffectCode += UpdateCharmRegenFromAbsorption;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(
				gold: 5
			);
		}

		public static void UpdateCharmRegenFromAbsorption(Item item, Player player, bool hideVisual)
		{
			if (player.AsPred().StomachTracker?.Prey.Count > 0)
			{
				double effectiveness = (double)player.AsPred().StomachTracker?.Prey.FindAll(x => x.NoHealth).Count / (double)player.AsPred().StomachTracker?.Prey.Count;
				player.AddHealthRegenEffect(
					healthPerSecond: HealthRegenerationRatio * player.AsPred().PreyAbsorptionRatePerSecond * effectiveness,
					natural: true
				);
				player.AsPred().specialManaRegenCount += ManaRegenerationRatio * player.AsPred().PreyAbsorptionRatePerSecond * effectiveness;
			}
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			double regenEffectiveness = 0.0;
			if (player.AsPred().StomachTracker?.Prey.Count > 0)
				regenEffectiveness = (double)player.AsPred().StomachTracker?.Prey.FindAll(x => x.NoHealth).Count / (double)player.AsPred().StomachTracker?.Prey.Count;
			tooltips.AddVorariaItemTooltip(
				"Voraria.Charms.RegenFromAbsorption",
				new
				{
					HealthRegenerationRatio = HealthRegenerationRatio.ToPercentage(0),
					ManaRegenerationRatio = ManaRegenerationRatio.ToPercentage(0),
					RegenEffectiveness = regenEffectiveness.ToPercentage(2),
					LivePreyRemaining = player.AsPred().StomachTracker?.Prey.FindAll(x => !x.NoHealth).Count ?? 0,
					PreyRemaining = player.AsPred().StomachTracker?.Prey.Count ?? 0,
					CurrentHealthRegen = (regenEffectiveness > 0.0 ? (HealthRegenerationRatio * player.AsPred().PreyAbsorptionRate * regenEffectiveness) : 0.0).CastToDecimalPlaces(2),
					CurrentManaRegen = (regenEffectiveness > 0.0 ? (ManaRegenerationRatio * player.AsPred().PreyAbsorptionRate * regenEffectiveness) : 0.0).CastToDecimalPlaces(2),
				}
			);
		}
	}
}
