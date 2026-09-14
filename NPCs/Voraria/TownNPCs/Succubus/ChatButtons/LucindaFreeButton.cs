using BetterDialogue.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.NPCs.Voraria.TownNPCs.Succubus.ChatButtons
{
	public class LucindaFreeButton : ChatButton
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.V2.NPCs.Lucinda.FreeButton.DisplayName");

		public override double Priority => 3.4;

		public override bool IsActive(NPC npc, Player player) => npc.type == ModContent.NPCType<LucindaBound>();

		public override void OnClick(NPC npc, Player player)
		{
			ModContent.GetInstance<V2MasterSystem>().freedSucc = true;
			npc.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Lucinda>());
			// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
			Main.npcChatText = "There ya go! Wasn't that hard. Now, c'mere so I can reward you with some time in my gut...or, y'know, just some old trinkets from your ol' pal Lucinda to help you be a great pred just like me.";
		}
	}
}
