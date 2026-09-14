using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Vanilla.Placeables.Statues
{
	public class AlphabetStatues : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.AlphabetStatue0 or ItemID.AlphabetStatue1 or ItemID.AlphabetStatue2
			 or ItemID.AlphabetStatue3 or ItemID.AlphabetStatue4 or ItemID.AlphabetStatue5 or ItemID.AlphabetStatue6 or ItemID.AlphabetStatue7 or ItemID.AlphabetStatue8
			 or ItemID.AlphabetStatue9 or ItemID.AlphabetStatueA or ItemID.AlphabetStatueB or ItemID.AlphabetStatueC or ItemID.AlphabetStatueD or ItemID.AlphabetStatueE
			 or ItemID.AlphabetStatueF or ItemID.AlphabetStatueG or ItemID.AlphabetStatueH or ItemID.AlphabetStatueI or ItemID.AlphabetStatueJ or ItemID.AlphabetStatueK
			 or ItemID.AlphabetStatueL or ItemID.AlphabetStatueM or ItemID.AlphabetStatueN or ItemID.AlphabetStatueO or ItemID.AlphabetStatueP or ItemID.AlphabetStatueQ
			 or ItemID.AlphabetStatueR or ItemID.AlphabetStatueS or ItemID.AlphabetStatueT or ItemID.AlphabetStatueU or ItemID.AlphabetStatueV or ItemID.AlphabetStatueW
			 or ItemID.AlphabetStatueX or ItemID.AlphabetStatueY or ItemID.AlphabetStatueZ;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 825;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().Size = 1.75;
		}
	}
}
