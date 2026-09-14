using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using V2.NPCs;

namespace V2.Core
{
	public static class V2CommonDropRules
	{
		public class DifficultyScalingDrop : IItemDropRule, INestedItemDropRule
		{
			public IItemDropRule ruleForNormalMode;
			public IItemDropRule ruleForExpertMode;
			public IItemDropRule ruleForMasterMode;

			public List<IItemDropRuleChainAttempt> ChainedRules
			{
				get;
				private set;
			}

			public DifficultyScalingDrop(IItemDropRule ruleForNormalMode, IItemDropRule ruleForExpertMode, IItemDropRule ruleForMasterMode)
			{
				this.ruleForNormalMode = ruleForNormalMode;
				this.ruleForExpertMode = ruleForExpertMode;
				this.ruleForMasterMode = ruleForMasterMode;
				ChainedRules = new List<IItemDropRuleChainAttempt>();
			}

			public bool CanDrop(DropAttemptInfo info)
			{
				if (info.IsMasterMode)
					return ruleForMasterMode.CanDrop(info);
				else if (info.IsExpertMode)
					return ruleForExpertMode.CanDrop(info);

				return ruleForNormalMode.CanDrop(info);
			}

			public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
			{
				if (info.IsMasterMode)
					return ruleForMasterMode.TryDroppingItem(info);
				else if (info.IsExpertMode)
					return ruleForExpertMode.TryDroppingItem(info);

				return ruleForNormalMode.TryDroppingItem(info);
			}

			public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
			{
				if (info.IsMasterMode)
					return resolveAction(ruleForMasterMode, info);
				else if (info.IsExpertMode)
					return resolveAction(ruleForExpertMode, info);

				return resolveAction(ruleForNormalMode, info);
			}

			public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
			{
				DropRateInfoChainFeed masterRatesInfo = ratesInfo.With(1f);
				masterRatesInfo.AddCondition(new Conditions.IsMasterMode());
				ruleForMasterMode.ReportDroprates(drops, masterRatesInfo);
				DropRateInfoChainFeed expertRatesInfo = ratesInfo.With(1f);
				expertRatesInfo.AddCondition(new Conditions.IsExpert());
				expertRatesInfo.AddCondition(new Conditions.NotMasterMode());
				ruleForExpertMode.ReportDroprates(drops, expertRatesInfo);
				DropRateInfoChainFeed normalRatesInfo = ratesInfo.With(1f);
				normalRatesInfo.AddCondition(new Conditions.NotExpert());
				ruleForNormalMode.ReportDroprates(drops, normalRatesInfo);
				Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
			}
		}

		public class RerollsBasedOnWeightLevelRule(int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1, int minimumWeightLevel = 0) : CommonDrop(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
		{
			public int minimumWeightLevel = minimumWeightLevel;

			public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
			{
				bool flag = false;
				if (info.npc.AsPred().GetVisualWeightStage is not null && info.npc.AsPred().GetVisualWeightStage.Invoke(info.npc) >= minimumWeightLevel)
				{
					for (int i = 0; i < info.npc.AsPred().GetVisualWeightStage.Invoke(info.npc) - minimumWeightLevel; i++)
					{
						flag = true; // flag || info.player.RollLuck(chanceDenominator) < chanceNumerator;
					}
				}
				ItemDropAttemptResult result;
				if (flag)
				{
					CommonCode.DropItem(info, itemId, info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1));
					result = default(ItemDropAttemptResult);
					result.State = ItemDropAttemptResultState.Success;
					return result;
				}

				result = default(ItemDropAttemptResult);
				result.State = ItemDropAttemptResultState.FailedRandomRoll;
				return result;
			}

			public override void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
			{
				float num = (float)chanceNumerator / (float)chanceDenominator;
				float num2 = 1f - num;
				float num3 = 1f;
				for (int i = 0; i < 1; i++)
				{
					num3 *= num2;
				}

				float num4 = 1f - num3;
				float dropRate = num4 * ratesInfo.parentDroprateChance;
				drops.Add(new DropRateInfo(itemId, amountDroppedMinimum, amountDroppedMaximum, dropRate, ratesInfo.conditions));
				Chains.ReportDroprates(base.ChainedRules, num4, drops, ratesInfo);
			}
		}
	}
}
