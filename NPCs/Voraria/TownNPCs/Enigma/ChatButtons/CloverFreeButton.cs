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

namespace V2.NPCs.Voraria.TownNPCs.Enigma.ChatButtons
{
	public class CloverFreeButton : ChatButton
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.V2.NPCs.Clover.FreeButton.DisplayName");

		public override double Priority => 3.4;

		public override bool IsActive(NPC npc, Player player) => npc.type == ModContent.NPCType<CloverBound>();

		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public override void OnClick(NPC npc, Player player)
		{
			ModContent.GetInstance<V2MasterSystem>().freedEnigma = true;
			npc.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Clover>());
			Main.npcChatText = "Oh you actually got me down. Uh, hi? What do i do now exactly?";
		}
	}
}
