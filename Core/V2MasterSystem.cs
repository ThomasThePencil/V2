using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Tiles.Vanilla;
using V2.Tiles.Vanilla.Paintings;

namespace V2.Core
{
	public class V2MasterSystem : ModSystem
	{
		public bool freedSucc;
		public bool freedAngel;
		public bool freedEnigma;

		public List<VoreTracker> VoreTrackers { get; set; } = new List<VoreTracker>();

		public static RecipeGroup CounterweightRecipeGroup;
		public override void Unload()
		{
			CounterweightRecipeGroup = null;
		}

		public override void AddRecipeGroups()
		{
			// Create a recipe group and store it
			// Language.GetTextValue("LegacyMisc.37") is the word "Any" in english, and the corresponding word in other languages
			CounterweightRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} " + "Counterweight",
				ItemID.RedCounterweight, ItemID.BlackCounterweight, ItemID.BlueCounterweight, ItemID.GreenCounterweight, ItemID.PurpleCounterweight, ItemID.YellowCounterweight);

			// To avoid name collisions, when a modded items is the iconic or 1st item in a recipe group, name the recipe group: ModName:ItemName
			RecipeGroup.RegisterGroup("Voraria:Counterweights", CounterweightRecipeGroup);
		}

		public override void OnWorldLoad()
		{
			VoreTrackers = [];
			freedSucc = false;
			freedAngel = false;
			freedEnigma = false;
		}
		public override void OnWorldUnload()
		{
			VoreTrackers = [];
			freedSucc = false;
			freedAngel = false;
			freedEnigma = false;
		}

		public override void PostWorldGen()
		{
			if (V2.BasicMode)
				return;

			// going through every single tile in the world... Awesome...
			for (int x = 5; x < Main.maxTilesX - 5; x++)
			{
				for (int y = 5; y < Main.maxTilesY - 5; y++)
				{
					Tile tile = Main.tile[x, y];
					if (tile.TileType == TileID.Sunflower)
					{
						tile.TileType = (ushort)ModContent.TileType<Sunflower>();
						if (tile.TileFrameY == 0 && (tile.TileFrameX == 0 || tile.TileFrameX == 36 || tile.TileFrameX == 72))
						{
							tile.TileFrameX = 0;
							TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<Sunflower_TileEntity>());
						}
						else if (tile.TileFrameX == 36 || tile.TileFrameX == 72)
							tile.TileFrameX = 0;
						else if (tile.TileFrameX == 54 || tile.TileFrameX == 90)
							tile.TileFrameX = 18;
					}
					else if (tile.TileType == TileID.Painting6X4 && tile.TileFrameX >= 0 && tile.TileFrameX <= 90 && tile.TileFrameY >= 360 && tile.TileFrameY <= 414)
					{
						tile.TileType = (ushort)ModContent.TileType<Dryadisque>();
						if (tile.TileFrameX == 0 && tile.TileFrameY == 360)
						{
							tile.TileFrameY = 0;
							TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<Dryadisque_TileEntity>());
						}
						else
							tile.TileFrameY -= 360;
					}
				}
			}
		}

		public override void PreUpdateEntities()
		{
			foreach (VoreTracker tracker in VoreTrackers)
			{
				tracker.UpdatePrey();
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					tracker.UpdateProgress();
					tracker.HandleStruggleSystem();
				}
			}

			VoreTrackers.RemoveAll(x => x.CheckClearability());
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["freedSucc"] = freedSucc;
			tag["freedAngel"] = freedAngel;
			tag["freedEnigma"] = freedEnigma;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			freedSucc = tag.ContainsKey("freedSucc") && tag.GetBool("freedSucc");
			freedAngel = tag.ContainsKey("freedAngel") && tag.GetBool("freedAngel");
			freedEnigma = tag.ContainsKey("freedEnigma") && tag.GetBool("freedEnigma");
		}

		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte(
				freedSucc,
				freedAngel,
				freedEnigma,
				false,
				false,
				false,
				false,
				false
			);
			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			freedSucc = flags[0];
			freedAngel = flags[1];
			freedEnigma = flags[2];
		}

	}
}
