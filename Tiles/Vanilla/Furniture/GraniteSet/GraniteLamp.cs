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
using V2.Items.Voraria.Placeables;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Sounds.Vore;

namespace V2.Tiles.Vanilla.Furniture.GraniteSet
{
	public class GraniteLamp : ModTile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void HitWire(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.TileFrameY == 18)
			{
				top--;
			}
			else if (tile.TileFrameY == 36)
			{
				top -= 2;
			}
			if (TileEntity.ByPosition.TryGetValue(new Point16(left, top), out TileEntity tileEntity))
			{
				if (tileEntity is GraniteLamp_TileEntity)
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 1f && npc.type == ModContent.ProjectileType<GraniteLamp_ProjectileEntity>())
						{
							if (npc.netUpdate == true) continue;
							if (npc.ai[0] == 0) npc.ai[0] = 1;
							else npc.ai[0] = 0;
							npc.netUpdate = true;
						}
					}
				}
			}
		}
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16};
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;

			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<GraniteLamp_TileEntity>().Hook_AfterPlacement, -1, 0, true);
			TileObjectData.newTile.UsesCustomCanPlace = true;

			TileObjectData.addTile(Type);

			AddMapEntry(new Color(220,200,10), Language.GetText("MapObject.FloorLamp"));
			DustType = 2;
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			ModContent.GetInstance<GraniteLamp_TileEntity>().Kill(i, j);
		}
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity))
			{
				if (tileEntity is GraniteLamp_TileEntity)
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 1f && npc.type == ModContent.ProjectileType<GraniteLamp_ProjectileEntity>())
						{
							int tumSize = GraniteLamp_ProjectileEntity.GetVisualBellySize(npc);
							int weightSize = GraniteLamp_ProjectileEntity.GetVisualWeightStage(npc);
							Texture2D texture = ModContent.Request<Texture2D>("V2/Tiles/Vanilla/Furniture/GraniteSet/GraniteLamp_SpriteSheet").Value;
							Rectangle sourceRect = new Rectangle((48 * (int)npc.ai[0]) + (96 * weightSize), 64 * tumSize, 48, 64);
							Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
							spriteBatch.Draw(
								texture,
								new Vector2(i * 16 - (int)Main.screenPosition.X - 16, j * 16 - (int)Main.screenPosition.Y - 14) + zero,
								sourceRect,
								Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
							Player plr = Main.LocalPlayer;
							if (plr.AsV2Player().HoldingPredToggleRod)
							{
								Texture2D cornerTextureA = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodInactiveCornerThin").Value;
								Texture2D cornerTextureB = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodInactiveCornerShort").Value;
								if (npc.ai[2] == 1)
								{
									cornerTextureA = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodActiveCornerThin").Value;
									cornerTextureB = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodActiveCornerShort").Value;
								}

								spriteBatch.Draw( // Upper Left
									cornerTextureA,
									new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
									new Rectangle(0, 0, 10, 10),
									Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Upper Right
									cornerTextureB,
									new Vector2(i * 16 - (int)Main.screenPosition.X + (npc.width), j * 16 - (int)Main.screenPosition.Y) + zero,
									new Rectangle(0, 0, 10, 10),
									Color.White, 1.5708f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Bottom Left
									cornerTextureB,
									new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + (npc.height)) + zero,
									new Rectangle(0, 0, 10, 10),
									Color.White, 4.71239f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Bottom Right
									cornerTextureA,
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
	public class GraniteLamp_TileEntity : ModTileEntity
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public Projectile connectedNPC = null;
		public double WeightOnLoad = 0;
		public bool CurrentlyEnabled = false;
		public override void Update()
		{
			if (connectedNPC is null)
			{
				Activate();
			}
			else if (!connectedNPC.active || connectedNPC.type != ModContent.ProjectileType<GraniteLamp_ProjectileEntity>())
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
				if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 1f && npc.type == ModContent.ProjectileType<GraniteLamp_ProjectileEntity>())
				{
					connectedNPC = npc;
					return;
				}
			}
			//ill be honest i dont exactly know the grounds for the offset for the npc but i *think* its like, half of the X tiles and all Y tiles
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num = Projectile.NewProjectile(new EntitySource_TileEntity(this, null), new Vector2((int)(Position.X * 16) + 8, (int)(Position.Y * 16) + 24), Vector2.Zero, ModContent.ProjectileType<GraniteLamp_ProjectileEntity>(), 0, 0, ai2: CurrentlyEnabled ? 1 : 0);
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
			return tile.HasTile && tile.TileType == ModContent.TileType<GraniteLamp>();
		}
		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				int width = 4;
				int height = 4;
				NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);
				NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
				return -1;
			}
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
	public class GraniteLamp_ProjectileEntity : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Tiles/InvisibleImage";
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 16;
			Projectile.height = 48;
			Projectile.aiStyle = -1;
			Projectile.damage = 0;
			Projectile.timeLeft = 6000;
			Projectile.tileCollide = false;

			Projectile.AsPred().IsPredTileEntity = true;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;

			Projectile.AsFood().DefinedSize = 4;
			Projectile.AsPred().WeightGainRatio = 0.1;
			Projectile.AsPred().MaxStomachCapacity = 1.5;
			Projectile.AsPred().BaseStomachacheMeterCapacity = 200.0;
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
			if (Projectile.ai[0] == 0)
				Lighting.AddLight(Projectile.Center + new Vector2(0,-16), new Vector3(180, 145, 214) * (0.005f + 0.0015f * GetVisualBellySize(Projectile)));
			Projectile.timeLeft = 6000;
			Projectile.velocity = Vector2.Zero;
			Tile Painting = Main.tile[Projectile.position.ToTileCoordinates()];
			if (!Painting.HasTile || Painting.TileType != ModContent.TileType<GraniteLamp>())
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
				(int)Math.Floor(2.8 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				3
			);
		}
		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(2 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				3
			);
		}
		public static double GetDigestionTickDamage(Projectile projectile, PreyData prey) => 16;
		public static double GetDigestionTickRate(Projectile projectile, PreyData prey) => 1.2;
		public static double GetPreyAbsorptionRate(Projectile projectile)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				seconds: 30
			);
			return baseAbsorptionRate;
		}
	}
}