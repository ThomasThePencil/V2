using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Sounds.Vore;

namespace V2.Tiles.Vanilla.Paintings
{
	public class DoNotEatTheVileMushroom : ModTile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 4;

			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<DoNotEatTheVileMushroom_TileEntity>().Hook_AfterPlacement, -1, 0, true);
			TileObjectData.newTile.UsesCustomCanPlace = true;

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			ModContent.GetInstance<DoNotEatTheVileMushroom_TileEntity>().Kill(i, j);
		}
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity))
			{
				if (tileEntity is DoNotEatTheVileMushroom_TileEntity)
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<DoNotEatTheVileMushroom_ProjectileEntity>())
						{
							int XOffset = 0;
							int YSize = 64;
							if (npc.ai[0] <= 6) XOffset = 192;
							else if (npc.ai[0] <= 12) XOffset = 96;
							int tumSize = DoNotEatTheVileMushroom_ProjectileEntity.GetVisualBellySize(npc);
							switch (tumSize)
							{
								case 7: YSize = 74; break;
								case 8: YSize = 82; break;
							}
							if (tumSize == 7) YSize = 74;
							Texture2D texture = ModContent.Request<Texture2D>("V2/Tiles/Vanilla/Paintings/DoNotEatTheVileMushroom_SpriteSheet").Value;
							Rectangle sourceRect = new Rectangle(XOffset, 64 * tumSize, 96, YSize);
							switch (tumSize)
							{
								case 8: sourceRect.Y = 522; break;
							}
							Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
							spriteBatch.Draw(
								texture,
								new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
								sourceRect,
								Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
							Player plr = Main.LocalPlayer;
							if (plr.AsV2Player().HoldingPredToggleRod)
							{
								Texture2D cornerTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodInactiveCorner").Value;
								if (npc.ai[2] == 1)
									cornerTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodActiveCorner").Value;

								spriteBatch.Draw( // Upper Left
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
									new Rectangle(0, 0, 10, 10),
									Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Upper Right
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X + (npc.width), j * 16 - (int)Main.screenPosition.Y) + zero,
									new Rectangle(0, 0, 10, 10),
									Color.White, 1.5708f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Bottom Left
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + (npc.height)) + zero,
									new Rectangle(0, 0, 10, 10),
									Color.White, 4.71239f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Bottom Right
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X + (npc.width), j * 16 - (int)Main.screenPosition.Y + (npc.height)) + zero,
									new Rectangle(0, 0, 10, 10),
									Color.White, 3.14159f, default, 1f, SpriteEffects.None, 0f);
							}
						}
					}
				}
			}
			return false;
		}
	}
	public class DoNotEatTheVileMushroom_TileEntity : ModTileEntity
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public Projectile connectedNPC = null;
		public double WeightOnLoad = 0;
		public bool CurrentlyEnabled = true;

		public override void Update()
		{
			if (connectedNPC is null)
			{
				Activate();
			}
			else if (!connectedNPC.active || connectedNPC.type != ModContent.ProjectileType<DoNotEatTheVileMushroom_ProjectileEntity>())
			{
				Activate();
			}
			else
			{
				CurrentlyEnabled = connectedNPC.ai[2] == 1 ? true : false;
			}

		}
		public void Activate()
		{
			foreach (var npc in Main.ActiveProjectiles)
			{
				if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<DoNotEatTheVileMushroom_ProjectileEntity>())
				{
					connectedNPC = npc;
					return;
				}
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num = Projectile.NewProjectile(new EntitySource_TileEntity(this, null), new Vector2((int)(Position.X * 16) + 48, (int)(Position.Y * 16) + 32), Vector2.Zero, ModContent.ProjectileType<DoNotEatTheVileMushroom_ProjectileEntity>(), 0, 0, ai2: CurrentlyEnabled ? 1 : 0);
				connectedNPC = Main.projectile[num];
				connectedNPC.AsPred().ExtraWeight = WeightOnLoad;
				Main.projectile[num].netUpdate = true;
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, (float)Position.X, (float)Position.Y);
				}
			}
		}
		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.HasTile && tile.TileType == ModContent.TileType<DoNotEatTheVileMushroom>();
		}
		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				// Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
				int width = 6;
				int height = 4;
				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

				// Sync the placement of the tile entity with other clients
				// The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
				NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
				return -1;
			}

			// ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
			int placedEntity = Place(i, j);
			return placedEntity;
		}
		public override void OnNetPlace()
		{
			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
			}
		}
		public override void SaveData(TagCompound tag)
		{
			if (connectedNPC is not null)
				tag.Add("ExtraWeight", connectedNPC.AsPred().ExtraWeight);
			tag.Add("CurrentlyEnabled", CurrentlyEnabled);
		}

		public override void LoadData(TagCompound tag)
		{
			WeightOnLoad = tag.GetDouble("ExtraWeight");
			CurrentlyEnabled = tag.GetBool("CurrentlyEnabled");
		}
	}
	public class DoNotEatTheVileMushroom_ProjectileEntity : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Tiles/InvisibleImage";
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 96;
			Projectile.height = 64;
			Projectile.aiStyle = -1;
			Projectile.damage = 0;
			Projectile.timeLeft = 6000;
			Projectile.tileCollide = false;

			Projectile.AsPred().IsPredTileEntity = true;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;

			Projectile.AsFood().DefinedSize = 24;
			Projectile.AsPred().WeightGainRatio = 0;
			Projectile.AsPred().MaxStomachCapacity = 9;
			Projectile.AsPred().BaseStomachacheMeterCapacity = 750.0;
			Projectile.AsPred().CanSwallowBosses = false;
			Projectile.AsFood().MaxHealth = 7500;
			Projectile.AsFood().Health = 7500;

			Projectile.AsPred().MouthSoundRawOffset = new Vector2(0f, -14f);
			Projectile.AsPred().SmallGulps = Gulps.Short;
			Projectile.AsPred().SmallGulpThreshold = 0.1;
			Projectile.AsPred().BigGulps = Gulps.Standard;
			Projectile.AsPred().CanBeForceFed = CanPaintingBeForceFed;
			Projectile.AsPred().MaxSwallowRange = V2Utils.TileCountAsPixelCount(12.5);

			Projectile.AsPred().DigestionType = EntityDigestionType.Acidic;
			Projectile.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			Projectile.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			Projectile.AsPred().SmallBurps = Burps.Humanoid.Small;
			Projectile.AsPred().StandardBurps = Burps.Humanoid.Standard;
			Projectile.AsPred().BurpPitchOffset = 0f;

			Projectile.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			Projectile.AsPred().GetVisualBellySize = GetVisualBellySize;
			Projectile.AsPred().GetVisualWeightStage = GetVisualWeightStage;
		}
		public override bool? CanHitNPC(NPC target) => false;
		public override bool CanHitPlayer(Player target) => false;
		public override bool CanHitPvp(Player target) => false;
		public static bool CanPaintingBeForceFed(Projectile projectile) => true;
		public override void AI()
		{
			Projectile.ai[0]--;
			if (Projectile.ai[0] <= 0) Projectile.ai[0] = Main.rand.Next(350, 650);
			Projectile.timeLeft = 6000;
			Projectile.velocity = Vector2.Zero;
			Tile Painting = Main.tile[Projectile.position.ToTileCoordinates()];
			if (!Painting.HasTile || Painting.TileType != ModContent.TileType<DoNotEatTheVileMushroom>())
			{
				Projectile.active = false;
			}
			if (Main.rand.NextBool(100) && Projectile.ai[2] == 1) Projectile.DoContactGulpage();
		}
		public override void PostAI()
		{
			Projectile.velocity = Vector2.Zero;
		}
		public static int GetVisualBellySize(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(3 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				8
			);
		}
		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(2 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				0
			);
		}
		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey) => 22;
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey) => 1.2;
		public static double GetPreyAbsorptionRate(Projectile projectile)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				seconds: 45
			);
			return baseAbsorptionRate;
		}
	}
}
