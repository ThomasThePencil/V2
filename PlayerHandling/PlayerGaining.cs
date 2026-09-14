using System;
using Terraria;
using V2.Core;
using V2.Items.Voraria.TransformationItems.Baelz;


namespace V2.PlayerHandling
{
	public class PlayerGaining
	{
		public static float DamageScale = 0.125f;
		public static float AttackSpeedScale = -0.06f;
		public static int MaxLifeScale = 40;
		public static void GetPlayerWeightGainStats(Player player, out float DamageMult, out float AttackSpeedMult, out int MaxLifeIncrease)
		{
			float mult = (float)GetPlayerWeight(player, false, false, true);

			DamageMult = DamageScale * mult + 1;
			AttackSpeedMult = Math.Max(AttackSpeedScale * mult, -0.75f) + 1;
			MaxLifeIncrease = (int)Math.Round(MaxLifeScale * mult);
		}
		public static double GetPlayerWeight(Player player, bool IncludeStomachWeight = false, bool IncludeWeightModifiers = true, bool ExcludeBaseSize = false)
		{
			double Size = 1;
			double transformationSize = 0;
			if (player.AsV2Player().BaeTransformation)
			{
				transformationSize += player.AsPred().BaeTransformation_ExtraWeight;
				Size = BaelzInfo.BaseWeight;
			}
			else if (player.AsV2Player().KroniiTransformation)
			{
				transformationSize += player.AsPred().KroniiTransformation_ExtraWeight;
				Size = BaelzInfo.BaseWeight;
			}
			else if (player.AsV2Player().OllieTransformation)
			{
				transformationSize += player.AsPred().OllieTransformation_ExtraWeight;
				Size = BaelzInfo.BaseWeight;
			}
			else if (player.AsV2Player().SoraTransformation)
			{
				transformationSize += player.AsPred().SoraTransformation_ExtraWeight;
				Size = BaelzInfo.BaseWeight;
			}
			else if (player.AsV2Player().MintTransformation)
			{
				transformationSize += player.AsPred().MintTransformation_ExtraWeight;
				Size = BaelzInfo.BaseWeight;
			}
			if (ExcludeBaseSize)
				Size = 0;
			if (IncludeStomachWeight)
			{
				if (IncludeWeightModifiers)
					Size += player.AsPred().StomachWeight;
				else
					Size += player.AsPred().StomachFullness;
			}
			if (IncludeWeightModifiers)
			{
				transformationSize = (double)player.AsPred().BodyWeightModifier.ApplyTo((float)transformationSize);
				return Size + Math.Max(transformationSize - player.AsPred().FlatBodyWeightModifier, 0);
			}
			else
				return Size + transformationSize;
		}
		public static void AddWeightToRightTransformation(Player pred, double amount)
		{
			if (pred.AsV2Player().BaeTransformation == true)
			{
				pred.AsPred().BaeTransformation_ExtraWeight = Math.Max(pred.AsPred().BaeTransformation_ExtraWeight += amount, 0);
			}
			else if (pred.AsV2Player().KroniiTransformation == true)
			{
				pred.AsPred().KroniiTransformation_ExtraWeight = Math.Max(pred.AsPred().KroniiTransformation_ExtraWeight += amount, 0);
			}
			else if (pred.AsV2Player().OllieTransformation == true)
			{
				pred.AsPred().OllieTransformation_ExtraWeight = Math.Max(pred.AsPred().OllieTransformation_ExtraWeight += amount, 0);
			}
			else if (pred.AsV2Player().SoraTransformation == true)
			{
				pred.AsPred().SoraTransformation_ExtraWeight = Math.Max(pred.AsPred().SoraTransformation_ExtraWeight += amount, 0);
			}
			else if (pred.AsV2Player().MintTransformation == true)
			{
				pred.AsPred().MintTransformation_ExtraWeight = Math.Max(pred.AsPred().MintTransformation_ExtraWeight += amount, 0);
			}
		}
		public static double CalculateGain(Player pred, double amount, PreyData prey)
		{
			double effectiveSize = GetPlayerWeight(pred, IncludeWeightModifiers: false);
			return amount * prey.CalorieMultiplier * (1 / effectiveSize) * pred.AsPred().BaseWeightGainRatio;
		}
		public static double CalculateGain(Player pred, double amount)
		{
			double effectiveSize = GetPlayerWeight(pred, IncludeWeightModifiers: false);
			return amount * (1 / effectiveSize) * pred.AsPred().BaseWeightGainRatio;
		}
		public static void AddWeight(Player pred, double amount, PreyData prey)
		{
			if (amount > 0 && pred.AsPred().ActuallyReasonableAmountOfFood < 0.1)
			{
				pred.AsPred().ActuallyReasonableAmountOfFood = Math.Max(pred.AsPred().ActuallyReasonableAmountOfFood + CalculateGain(pred, amount, prey), 0);
			}
			else
			{
				if (pred.AsV2Player().HasTransformation == true)
				{
					AddWeightToRightTransformation(pred, Math.Max(CalculateGain(pred, amount, prey) * pred.AsPred().WeightGainMultiplier, 0));
				}
			}
		}
		public static void AddWeight(Player pred, double amount)
		{
			if (amount > 0 && pred.AsPred().ActuallyReasonableAmountOfFood < 0.1)
			{
				pred.AsPred().ActuallyReasonableAmountOfFood = Math.Max(pred.AsPred().ActuallyReasonableAmountOfFood + CalculateGain(pred, amount), 0);
			}
			else
			{
				if (pred.AsV2Player().HasTransformation == true)
				{
					AddWeightToRightTransformation(pred, Math.Max(CalculateGain(pred, amount) * pred.AsPred().WeightGainMultiplier, 0));
				}
			}
		}
		public static void ReduceWeight(Player pred, double amount)
		{
			if (pred.AsPred().ActuallyReasonableAmountOfFood > 0)
			{
				pred.AsPred().ActuallyReasonableAmountOfFood = Math.Max(pred.AsPred().ActuallyReasonableAmountOfFood - amount * pred.AsPred().WeightLossMultiplier, 0);
			}
			else
			{
				if (pred.AsV2Player().HasTransformation == true)
				{
					AddWeightToRightTransformation(pred, Math.Min(amount * pred.AsPred().WeightLossMultiplier * -1, 0));
				}
			}
		}
	}
}
