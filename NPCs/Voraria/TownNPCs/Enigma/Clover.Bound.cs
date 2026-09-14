using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;

namespace V2.NPCs.Voraria.TownNPCs.Enigma
{
	public class CloverBound : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
			NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;
		}

		public override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.dontTakeDamageFromHostiles = true;
			NPC.width = 8;
			NPC.height = 94;
			NPC.aiStyle = 0;
			NPC.lifeMax = 500;
			NPC.damage = 35;
			NPC.defense = 38;
			NPC.rarity = 1;
			NPC.knockBackResist = 0f;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.noGravity = true;

			NPC.AsFood().DefinedBaseSize = 1.15;
			NPC.AsPred().WeightGainRatio = 0.125;
			NPC.AsPred().MaxStomachCapacity = 2.2;
			NPC.AsPred().BaseStomachacheMeterCapacity = 90.0;
			NPC.AsFood().StruggleEffectiveness = 1; //have fun

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;
		}
		public override void ModifyTypeName(ref string typeName) => typeName = "Stuck Enigma";

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (ModContent.GetInstance<V2MasterSystem>().freedEnigma)
				return 0f;

			if (!spawnInfo.Player.ZoneRockLayerHeight)
				return 0f;

			if (!spawnInfo.Player.ZoneJungle)
				return 0f;

			if (!Main.hardMode)
				return 0f;

			if (NPC.AnyNPCs(ModContent.NPCType<CloverBound>()))
				return 0f;

			return 0.18f;
		}
		public override void AI()
		{
			if (NPC.ai[0] == 0f)
			{
				int TileCount = 0;
				bool FoundTile = false;
				Vector2 GoTo = Vector2.Zero;
				while (TileCount < 25 && !FoundTile)
				{
					TileCount++;
					if (Collision.IsWorldPointSolid(NPC.Center - new Vector2(0, TileCount * 16), true))
					{
						FoundTile = true;
						Point tilePos = (NPC.Center - new Vector2(0, TileCount * 16)).ToTileCoordinates();
						GoTo = tilePos.ToWorldCoordinates();
					}
				}
				if (GoTo == Vector2.Zero) NPC.type = NPCID.None;
				NPC.position = GoTo;
			}
			NPC.ai[0] += 0.1f;
			if (NPC.ai[0] >= 1f)
			{
				NPC.ai[0] = 1f;
				Vector2 tilePos = new Vector2(NPC.Center.X, NPC.position.Y - 4);
				tilePos.ToTileCoordinates();
				if (!Collision.IsWorldPointSolid(tilePos, true))
				{
					ModContent.GetInstance<V2MasterSystem>().freedEnigma = true;
					NPC.AI_000_TransformBoundNPC(Main.CurrentPlayer.whoAmI, ModContent.NPCType<Clover>());
				}
			}
		}
		public override bool CanChat() => true;

		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public override string GetChat()
		{
			List<string> possibleLines = [
				"Oh, hey! I'm, uh, stuck up here somehow. Don't ask how I did it, just get me down!",
				"So, how's it... hanging? Get it? ...okay, I won't do any more awful jokes if you get me down!",
				"...no chat, I'm not going to- OH IM NOT ALONE HERE Hi! Can you... help a gal out here?",
			];
			return Main.rand.NextFromCollection(possibleLines);
		}
		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle(
				(int)NPC.Center.X - 16,
				(int)NPC.Center.Y,
				32,
				40
			);
		}
	}
}
