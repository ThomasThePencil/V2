using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using V2.PlayerHandling.PredPlayerGoals.Beginner;

namespace V2.Items.Voraria.Charms
{
	public static class CharmHelpers
	{
		public static CharmGlobalItem AsCharm(this Item item) => item.GetGlobalItem<CharmGlobalItem>();

		public static List<int> ImplementedCharms =>
		[
			ModContent.ItemType<CharmBetterDigestion>(),
			ModContent.ItemType<CharmFatass>(),
			ModContent.ItemType<CharmLessStomachWeight>(),
			ModContent.ItemType<CharmPreyItemTheft>(),
			ModContent.ItemType<CharmRegenFromAbsorption>(),
		];
	}

	public class CharmGlobalItem : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => CharmHelpers.ImplementedCharms.Contains(entity.type);
		/// <summary>
		/// Whether or not this item is a charm and thus able to grant the charm goal on equip.<br/>
		/// Defaults to <see langword="false"/>.
		/// </summary>
		public bool IsCharm { get; set; } = false;

		public override bool InstancePerEntity => true;

		public override void SetDefaults(Item item)
		{
			item.AsCharm().IsCharm = true;
			item.AsAnItem().AccessoryEffectCode += UpdateCharm;
		}

		public static void UpdateCharm(Item item, Player player, bool visual)
		{
			ModContent.GetInstance<EquipCharm>().TrySetCompletion(player);
		}
	}
}
