using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Items.Voraria.Consumables.Potions
{
	public class StomachacheMeterCapacityPotion : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public static double StomachacheMeterCapacityBonus => 0.10;
		public static int StomachacheDefenseBonus => 5;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.Potions.StomachacheMeterCapacityPotion");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.Potions.StomachacheMeterCapacityPotion.Short");

		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				new Color(121, 255, 76),
				new Color(121, 255, 76),
				new Color(50, 191, 38),
			];
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

			Item.value = Item.buyPrice(0, 1, 25, 0);
			Item.rare = ItemRarityID.Green;

			Item.AsFood().EdibleOnUse = true;
			Item.AsFood().AlwaysEatenByUse = true;

			Item.AsFood().MaxHealth = 80;
			Item.AsFood().Size = 0.15;

			Item.AsFood().OnSwallow += OnSwallow;

			Item.AsFood().UpdateInStomach += UpdateInStomach;

			Item.AsFood().OnBreak += OnBreak;
		}

		public static void OnSwallow(Item item, Entity pred)
		{

		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
			{
				pred.AddStatus(
					ModContent.BuffType<StomachacheMeterCapacityPotionBuff>(),
					V2Utils.SensibleTime(minutes: 3),
					true
				);
			}
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.Potions.StomachacheMeterCapacityPotion",
				new
				{
					StomachacheMeterCapacityPotionMeterCapacityBonus = StomachacheMeterCapacityBonus.ToPercentage(3),
					StomachacheMeterCapacityPotionUneaseDefenseBonus = StomachacheDefenseBonus,
				}
			);
		}
	}
}