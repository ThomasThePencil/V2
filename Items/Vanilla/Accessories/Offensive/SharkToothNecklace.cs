using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.Vanilla.Accessories.Offensive
{
	public class SharkToothNecklace : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int ArmorPenetration => 5;
		public static int GLPACIBonus => 1;
		public static float CritDamageBonus => 0.10f;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SharkToothNecklace;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 150;
			item.AsFood().Size = 0.06;
			item.AsFood().AcidResistTier = 0;

			item.AsAnItem().AccessoryEffectCode += UpdateSharkToothNecklace;

			item.AsFood().OnBreak += OnBreak;
		}

		public static void UpdateSharkToothNecklace(Item item, Player player, bool hideVisual)
		{
			player.GetArmorPenetration(DamageClass.Generic) += ArmorPenetration;
			player.GetModPlayer<PredPlayer>().GLP.Extra += GLPACIBonus;
			player.GetModPlayer<PredPlayer>().ACI.Extra += GLPACIBonus;
		}

		public static bool OnBreak(Item item, Entity pred, bool direct) => true;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip("Vanilla.Accessories.Offensive.SharkToothNecklace",
				new
				{
					
				}
			);
		}
	}
}
