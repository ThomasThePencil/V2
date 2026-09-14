using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Voraria.Consumables.PermanentUpgrades
{
	public class PureSwallowBoost1 : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int GLPBonus => 8;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.PermanentUpgrades.PureSwallowBoost1");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.PermanentUpgrades.PureSwallowBoost1.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;

			ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
				new Color(121, 255, 76),
				new Color(121, 255, 76),
				new Color(50, 191, 38),
			};
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;
			Item.UseSound = SoundID.Item3;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.consumable = true;

			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;

			Item.AsFood().Size = 0.05;
			Item.AsFood().MaxHealth = 80;

			Item.AsFood().EdibleOnUse = true;

			Item.AsFood().OnSwallowDamage = 15;
			Item.AsFood().OnSwallowDeathReason = "Mods.V2.Death.SwallowDamage.NurseNeedle";

			Item.AsFood().OnBreak += OnBreak;
		}

		public override bool? UseItem(Player player)
		{
			if (!player.AsPred().PermanentUpgradesGained.ContainsKey("PureSwallow1"))
				player.AsPred().PermanentUpgradesGained.Add("PureSwallow1", false);

			if (!player.AsPred().PermanentUpgradesGained["PureSwallow1"])
				player.AsPred().PermanentUpgradesGained["PureSwallow1"] = true;

			return true;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.TrueCenter());
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			if (pred is Player playerPred)
			{
				if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("PureSwallow1"))
					playerPred.AsPred().PermanentUpgradesGained.Add("PureSwallow1", false);

				if (!playerPred.AsPred().PermanentUpgradesGained["PureSwallow1"])
					playerPred.AsPred().PermanentUpgradesGained["PureSwallow1"] = true;
			}
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.PermanentUpgrades.PureSwallowBoost1",
				new
				{
					PureSwallow1GLPBonus = GLPBonus
				}
			);
		}
	}
}