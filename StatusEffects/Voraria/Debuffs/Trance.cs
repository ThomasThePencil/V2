using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.NPCs;
using V2.PlayerHandling;

namespace V2.StatusEffects.Voraria.Debuffs
{
	public class Trance : ModBuff
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/StatusEffects/Voraria/Debuffs/DebuffPlaceholder";
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.Trance.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.Trance.Description");

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
		{
			rare = ItemRarityID.LightPurple;
		}
	}
}
