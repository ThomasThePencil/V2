using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Accessories
{
	public class AdhesiveBandage : GlobalItem
	{
		public static float SoftenedBuildupReduction => 0.075f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.AdhesiveBandage;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 90;
			item.AsFood().Size = 0.07;
			item.AsFood().AcidResistTier = 0;

			item.AsAnItem().AccessoryEffectCode += UpdateAdhesiveBandage;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void UpdateAdhesiveBandage(Item item, Player player, bool hideVisual)
		{
			player.AsV2Player().StatusDurationResistance[BuffID.Bleeding] *= 0.5;
			player.AsFood().SoftenedDigestionDamageModifier *= 1f - SoftenedBuildupReduction;
		}

		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
		{
			return true;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => true;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.player[Main.myPlayer];
			tooltips.AddVorariaDynamicItemTooltip(
				"Vanilla.Accessories.AdhesiveBandage",
				new
				{
					AdhesiveBandageSoftenedBuildupReduction = SoftenedBuildupReduction.ToPercentage(2),
				}
			);
		}
	}
}
