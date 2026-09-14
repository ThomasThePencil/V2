using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.NPCs;

namespace V2.StatusEffects.Voraria.Debuffs
{
	public class TimeStun : ModBuff
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.TimeStun.Name");
		public override LocalizedText Description => Language.GetText("Mods.V2.StatusEffects.Voraria.Debuffs.TimeStuff.Description");
		public override string Texture => "V2/StatusEffects/Voraria/Debuffs/DebuffPlaceholder";
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			if (npc.boss)
				npc.AsV2NPC().TimeStunCooldown = 300;
			else
				npc.AsV2NPC().TimeStunCooldown = 60;
		}
	}
	public class TimeStunImmune : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.SpecificDebuffImmunity[NPCID.Golem][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.GolemFistLeft][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.GolemFistRight][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.GolemHead][ModContent.BuffType<TimeStun>()] = true;

			NPCID.Sets.SpecificDebuffImmunity[NPCID.MoonLordCore][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.MoonLordHand][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.MoonLordHead][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.MoonLordLeechBlob][ModContent.BuffType<TimeStun>()] = true;

			NPCID.Sets.SpecificDebuffImmunity[NPCID.DukeFishron][ModContent.BuffType<TimeStun>()] = true; //duke was immune to dazed from heart arrows so yknow what sure

			//life is a fuck
			NPCID.Sets.SpecificDebuffImmunity[NPCID.EaterofWorldsHead][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.EaterofWorldsBody][ModContent.BuffType<TimeStun>()] = true;
			NPCID.Sets.SpecificDebuffImmunity[NPCID.EaterofWorldsTail][ModContent.BuffType<TimeStun>()] = true;
		}
	}
}
