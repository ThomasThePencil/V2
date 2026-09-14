using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using V2.Core;
using V2.PlayerHandling;

namespace V2.NPCs.Voraria.TownNPCs.Succubus
{
	public class LucindaBound : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
		}

		public override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.width = 22;
			NPC.height = 36;
			NPC.aiStyle = 0;
			NPC.lifeMax = 700;
			NPC.damage = 35;
			NPC.defense = 22;
			NPC.rarity = 4;
			NPC.knockBackResist = 0f;
			NPC.HitSound = SoundID.NPCHit1;

			NPC.AsFood().DefinedBaseSize = 1.15;
			NPC.AsPred().WeightGainRatio = 0.125;
			NPC.AsPred().MaxStomachCapacity = 2.2;
			NPC.AsPred().BaseStomachacheMeterCapacity = 155.0;

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;

			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.OnFire3] = true;
			NPC.buffImmune[BuffID.Burning] = true;
			NPC.buffImmune[BuffID.ShadowFlame] = true;
			NPC.lavaImmune = true;
		}

		public override void ModifyTypeName(ref string typeName) => typeName = "Bound Succubus";

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (ModContent.GetInstance<V2MasterSystem>().freedSucc)
				return 0f;

			if (!spawnInfo.Player.ZoneUnderworldHeight)
				return 0f;

			if (NPC.AnyNPCs(ModContent.NPCType<LucindaBound>()))
				return 0f;

			return 0.15f;
		}

		public override bool CanChat() => true;

		public override string GetChat()
		{
			List<string> possibleLines = new List<string>
			{
				"Hey! You! Morsel! Mind lendin' me a hand? Been stuck here since last Tuesday, havin' to munch on imps and the chips off those serpents just to keep my gut quiet.",
				"Hey there, soon-to-be snack. I know you're not all that busy, so care to help a pred out? My gut and I will be MORE than happy to make it worth your while.",
				"So WHAT!? The Convocation says I'm out for a bit because the bimbo that one of 'em wanted made good gut fodder!? Dumbasses...hey, lunch! You can tear these tacky tightropes for me, yeah?",
			};
			return Main.rand.NextFromCollection(possibleLines);
		}
	}
}
