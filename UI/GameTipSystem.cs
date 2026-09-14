using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace V2.UI
{
	public class GameTipSystem : ModSystem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void ModifyGameTipVisibility(IReadOnlyList<GameTipData> gameTips)
		{
			gameTips[GameTipID.MagicMirror].Hide();
			gameTips[GameTipID.LavaAndObsidianSkinPotion].Hide();
			gameTips[GameTipID.WiresFromMechanic].Hide();
			gameTips[GameTipID.PartyGirlNeedsOtherNPCsToMoveIn].Hide();
		}
	}
}
