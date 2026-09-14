using Humanizer;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs.Vanilla.TownNPCs.Nurse;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs
{
	public partial class GeneralNPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (V2.BasicMode)
				return;
			npcLoot.RemoveWhere(x =>
				x is DropBasedOnExpertMode expertDependentRule
			 && expertDependentRule.ruleForNormalMode is CommonDropWithRerolls normalBandageRule
			 && normalBandageRule.itemId == ItemID.AdhesiveBandage
			);
		}
	}
}
