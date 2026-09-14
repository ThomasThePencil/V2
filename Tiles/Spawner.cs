using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using V2.Core;
using V2.NPCs;
using V2.NPCs.Voraria.Jungle;
using V2.NPCs.Voraria.TownNPCs.Ghost;
using V2.Projectiles;
using V2.Sounds.Vore;

namespace V2.Tiles.Voraria
{
	public class Spawner : ModTile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Tiles/InvisibleImage";
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			AddMapEntry(new Color(255, 255, 255), Language.GetText("MapObject.Painting"));
		}
		public override void RandomUpdate(int i, int j)
		{
			NPC npc = NPC.NewNPCDirect(
				NPC.GetSource_NaturalSpawn(),
				i * 16,
				j * 16,
				ModContent.NPCType<JungleFairy>()
			);
			npc.netUpdate = true;
			WorldGen.KillTile(i, j);
		}
	}
}
