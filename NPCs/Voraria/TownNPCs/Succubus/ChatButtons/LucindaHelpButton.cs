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
using V2.PlayerHandling;

namespace V2.NPCs.Voraria.TownNPCs.Succubus.ChatButtons
{
	public class LucindaHelpButton : ChatButton
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		internal static int HelpIndex = -1;
		internal static int HelpIndexMax = 7;
		public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.DisplayName");

		public override double Priority => 3.5;

		public override bool IsActive(NPC npc, Player player) => npc.type == ModContent.NPCType<Lucinda>();

		public override Color? OverrideColor(NPC npc, Player player)
		{
			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense)
				return Color.Gray;

			return null;
		}

		public override void OnClick(NPC npc, Player player)
		{
			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense)
			{
				HelpIndex = 0;
				Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.AlreadyDinnerForADemon");
			}
			else
			{
				HelpIndex = HelpIndex++ % HelpIndexMax;
				Main.npcChatText = HelpIndex switch
				{
					0 => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.Intro"),
					1 => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.PredStats"),
					2 => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.SwallowSize"),
					3 => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.StomachCapacity"),
					4 => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.Struggles"),
					5 => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.Stomachache"),
					6 => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.Absorption"),
					_ => Language.GetTextValue("Mods.V2.NPCs.Lucinda.HelpButton.Tips.Invalid")
				};
			}
		}
	}
}
