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
	public class LucindaDontFreeBecauseYoureAnAssholeButton : ChatButton
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.V2.NPCs.Lucinda.DontFreeBecauseYoureAnAssholeButton.DisplayName");

		public override double Priority => 3.6;

		public override bool IsActive(NPC npc, Player player) => npc.type == ModContent.NPCType<LucindaBound>();

		public override void OnClick(NPC npc, Player player)
		{
			ModContent.GetInstance<V2MasterSystem>().freedSucc = true;
			npc.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Lucinda>());
			// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
			PredNPC.SwallowWithTextIfApplicable(
				npc,
				Main.CurrentPlayer,
				"[c/7F7F7F:<As a scowl quickly crosses her face, the succubus whips her tail around your form and uses it to guide you headfirst into her mouth, soon letting her newly-filled stomach break the bindings for you.>]\n"
			  + "Fine! If you're gonna be such a " + (Main.CurrentPlayer.Male ? "dick" : "bitch") + " about it, you can AT LEAST be helpful enough to fatten up my thighs! Haven't had some REAL good eats in a while..."
			);
		}
	}
}
