using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Vanilla.Currency
{
	public class CopperCoin : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CopperCoin;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 4;
			item.AsFood().Size = 0.0025;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = 0.01;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Currency.Coins.CopperCoin",
				new
				{
					
				}
			);
		}
	}
}
