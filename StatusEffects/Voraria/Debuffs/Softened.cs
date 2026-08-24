using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Debuffs
{
	public class Softened : ModBuff
	{
		public static double MaxHealthDigestedForOneStack(Player player)
		{
			double threshold = 0.05;
			threshold = player.AsFood().SoftenedDigestionDamageThresholdModifier.ApplyTo((float)threshold);
			return threshold;
		}
		public static double MaxHealthDigestedForOneStack(NPC npc)
		{
			double threshold = 0.05;
			return threshold;
		}
		public static double DefenseReductionPerStack => 0.075;
		public static float DigestionDamageIncreasePerStack => 0.15f;
		public static int MaxStacks => 10;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.Softened.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.Softened.Description.Base");

		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			if (Main.LocalPlayer.AsFood().SoftenedStacks > 0)
				buffName += " " + Main.LocalPlayer.AsFood().SoftenedStacks.ToRoman();
			rare = ItemRarityID.Lime;
			string baseTooltip = Language.GetTextValueWith(
				"Mods.V2.StatusEffects.Voraria.Debuffs.Softened.Description.Base",
				new
				{
					SoftenedMaxHealthThreshold = MaxHealthDigestedForOneStack(Main.LocalPlayer).ToPercentage(1),
					SoftenedMaxStacks = MaxStacks,
					Main.LocalPlayer.AsFood().SoftenedStacks,
					SoftenedDefReduction = DefenseReductionPerStack.ToPercentage(1),
					SoftenedCurrentDefReduction = (Main.LocalPlayer.AsFood().SoftenedStacks * DefenseReductionPerStack).ToPercentage(1),
					SoftenedDigestiveAid = DigestionDamageIncreasePerStack.ToPercentage(1),
					SoftenedCurrentDigestiveAid = (Main.LocalPlayer.AsFood().SoftenedStacks * DigestionDamageIncreasePerStack).ToPercentage(1),
				}
			);
			string dynamicFlavorText = "'" + Language.GetTextValue("Mods.V2.StatusEffects.Voraria.Debuffs.Softened.Description.Flavor." + Main.LocalPlayer.AsFood().SoftenedStacks) + "'";
			tip = baseTooltip + "\n" + dynamicFlavorText;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.DefenseEffectiveness *= (float)(1.0 - (DefenseReductionPerStack * player.AsFood().SoftenedStacks));
			player.AsFood().TakenDigestionDamageModifier *= (float)(1.0 + (DigestionDamageIncreasePerStack * player.AsFood().SoftenedStacks));
			player.buffTime[buffIndex] = 3;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.defense = (int)Math.Round((double)npc.defense * (1.0 - (DefenseReductionPerStack * (double)npc.SoftenedStacks())));
			npc.AsFood().TakenDigestionDamageModifier *= (float)(1.0 + (DigestionDamageIncreasePerStack * npc.SoftenedStacks()));
			npc.buffTime[buffIndex] = 3;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
		{
			Texture2D buffTextureSheet = ModContent.Request<Texture2D>("V2/StatusEffects/Voraria/Debuffs/SoftenedSheet").Value;

			spriteBatch.Draw(
				buffTextureSheet,
				drawParams.Position,
				new Rectangle(34 * Main.LocalPlayer.AsFood().SoftenedStacks, 0, 32, 38),
				drawParams.DrawColor,
				0f,
				Vector2.Zero,
				1.0f,
				SpriteEffects.None,
				0f
			);

			double damageTowardsNextStack = Main.LocalPlayer.AsFood().SoftenedDigestionDamageTaken % (Main.LocalPlayer.statLifeMax * MaxHealthDigestedForOneStack(Main.LocalPlayer));
			double barFillRatio = damageTowardsNextStack / (Main.LocalPlayer.statLifeMax * MaxHealthDigestedForOneStack(Main.LocalPlayer));
			if (Main.LocalPlayer.AsFood().SoftenedStacks == MaxStacks)
				barFillRatio = 0.0;
			spriteBatch.Draw(
				buffTextureSheet,
				drawParams.Position + new Vector2(4f, 28f),
				new Rectangle(4, 40, (int)Math.Floor(24.0 * barFillRatio), 6),
				drawParams.DrawColor,
				0f,
				Vector2.Zero,
				1.0f,
				SpriteEffects.None,
				0f
			);
			spriteBatch.Draw(
				buffTextureSheet,
				drawParams.Position + new Vector2(4f + (float)Math.Floor(24.0 * barFillRatio), 28f),
				new Rectangle(4 + (int)Math.Floor(24.0 * barFillRatio), 46, (int)Math.Ceiling(24.0 * (1.0 - barFillRatio)), 6),
				drawParams.DrawColor,
				0f,
				Vector2.Zero,
				1.0f,
				SpriteEffects.None,
				0f
			);

			Vector2 stringSize = FontAssets.MouseText.Value.MeasureString("" + Main.LocalPlayer.AsFood().SoftenedStacks);
			ChatManager.DrawColorCodedStringWithShadow(
				spriteBatch,
				FontAssets.MouseText.Value,
				Main.LocalPlayer.AsFood().SoftenedStacks > 0 ? Main.LocalPlayer.AsFood().SoftenedStacks.ToRoman() : "0",
				drawParams.Position + new Vector2(30f, 25f),
				drawParams.DrawColor,
				0f,
				new Vector2(stringSize.X, stringSize.Y / 2f),
				new Vector2(0.8f)
			);
			return false;
		}
	}
}
