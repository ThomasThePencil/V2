using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Weapons.Summon
{
	public static class PaperMaidDetails
	{
		public static string Name => "Macheline";
		public static double NeededMinionSlots => 2;
		public static double PaperPlateDamage => 1.0;
		public static double SilverwarePerDamage => 0.8;
		public static double ForkArmorPen => 15;
		public static double SpoonInorganicDamageBonus => 0.20;
		public static int KnifeBleedProcDuration => V2Utils.SensibleTime(seconds: 3);
		public static double StomachCapacity => 2.0;
		public static double StomachacheMeterCapacity => 250.0;
		public static double DigestionDamage => 0.7;
		public static double DigestionRate => 1.6;
		public static double DigestionBleedRatio => 0.5;
		public static double Health => 500;
	}
	public class PaperMaidSummon : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Weapons.Summon.PaperMaidSummon");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Weapons.Summon.PaperMaidSummon.Short");

		public override void SetStaticDefaults()
		{
			
		}

		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.mana = 200;
			Item.width = 22;
			Item.height = 38;
			Item.useTime = 19;
			Item.useAnimation = 19;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 5f;
			Item.UseSound = SoundID.Item15;
			//Item.shoot = ModContent.ProjectileType<Macheline>();
			Item.shootSpeed = 10f;
			Item.DamageType = DamageClass.Summon;

			Item.value = Item.buyPrice(
				gold: 5
			);
			Item.rare = ItemRarityID.Blue;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Weapons.Summon.PaperMaidSummon",
				new
				{
					PaperMaidDetails.Name,
					PaperMaidDetails.NeededMinionSlots,
					PaperPlateDamage = PaperMaidDetails.PaperPlateDamage.ToPercentage(2),
					SilverwarePerDamage = PaperMaidDetails.SilverwarePerDamage.ToPercentage(2),
					PaperMaidDetails.ForkArmorPen,
					SpoonInorganicDamageBonusPercent = PaperMaidDetails.SpoonInorganicDamageBonus.ToPercentage(2),
					KnifeBleedTime = (double)PaperMaidDetails.KnifeBleedProcDuration / 60.0,
					PaperMaidDetails.StomachCapacity,
					PaperMaidDetails.StomachacheMeterCapacity,
					DigestionDamage = PaperMaidDetails.DigestionDamage.ToPercentage(2),
					DigestionRate = PaperMaidDetails.DigestionRate.ToPercentage(2),
					DigestionBleedRatio = PaperMaidDetails.DigestionBleedRatio.ToPercentage(2)
				}
			);
		}
	}
}