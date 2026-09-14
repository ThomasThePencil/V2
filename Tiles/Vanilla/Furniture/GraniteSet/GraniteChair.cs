using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
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
	public class GraniteChair : ModTile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.CanBeSatOnForNPCs[Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
			TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
			TileID.Sets.DisableSmartCursor[Type] = true;

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

			AdjTiles = [TileID.Chairs];

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = [16, 18];
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			// The following 3 lines are needed if you decide to add more styles and stack them vertically
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;

			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<GraniteChair_TileEntity>().Hook_AfterPlacement, -1, 0, true);
			TileObjectData.newTile.UsesCustomCanPlace = true;

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1); // Facing right will use the second texture style
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(220, 200, 10), Language.GetText("MapObject.Chair"));
			DustType = 2;
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			ModContent.GetInstance<GraniteChair_TileEntity>().Kill(i, j);
		}
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity))
			{
				if (tileEntity is GraniteChair_TileEntity)
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 1f && npc.type == ModContent.ProjectileType<GraniteChair_ProjectileEntity>())
						{
							int tumSize = GraniteChair_ProjectileEntity.GetVisualBellySize(npc);
							int weightSize = GraniteChair_ProjectileEntity.GetVisualWeightStage(npc);
							Texture2D texture = ModContent.Request<Texture2D>("V2/Tiles/Vanilla/Furniture/GraniteSet/GraniteChair_SpriteSheet").Value;
							Rectangle sourceRect = new Rectangle(62 * weightSize, 34 * tumSize, 62, 34);
							Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
							//zero.X += 24;
							//zero.Y += 34;
							spriteBatch.Draw(
								texture,
								new Vector2(i * 16 - (int)Main.screenPosition.X - 16, j * 16 - (int)Main.screenPosition.Y - 14) + zero +
								new Vector2(tile.TileFrameX != 0 ? 24 : -16, 36),
								sourceRect,
								Lighting.GetColor(i, j),
								0f,
								new Vector2(12f, 22f),
								1f,
								tile.TileFrameX != 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
								0f
							);
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

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range
		}

		public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
		{
			// It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for example, use info.restingEntity
			Tile tile = Framing.GetTileSafely(i, j);

			//info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
			//info.visualOffset = Vector2.Zero; // Defaults to (0,0)

			info.TargetDirection = -1;
			if (tile.TileFrameX != 0)
			{
				info.TargetDirection = 1; // Facing right if sat down on the right alternate (added through addAlternate in SetStaticDefaults earlier)
			}

			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j - 1), out TileEntity tileEntity))
			{
				if (tileEntity is GraniteChair_TileEntity)
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 1f && npc.type == ModContent.ProjectileType<GraniteChair_ProjectileEntity>())
						{
							int tumSize = GraniteChair_ProjectileEntity.GetVisualBellySize(npc);
							int weightSize = GraniteChair_ProjectileEntity.GetVisualWeightStage(npc);
							Vector2 Offset = Vector2.Zero;
							switch (tumSize)
							{
								case 0:
									Offset = new Vector2(0, 0);
									break;
								case 1:
									Offset = new Vector2(0, -2);
									break;
								case 2:
									Offset = new Vector2(2, -4);
									break;
								case 3:
									Offset = new Vector2(4, -4);
									break;
								case 4:
								case 5:
									Offset = new Vector2(8, -4);
									break;
								case 6:
									Offset = new Vector2(12, -6);
									break;
								case 7:
									Offset = new Vector2(20, -8);
									break;
							}
							if (weightSize >= 2 && tumSize == 0)
								Offset = new Vector2(0, 0);
							info.VisualOffset = Offset;
						}
					}
				}
			}

			// The anchor represents the bottom-most tile of the chair. This is used to align the entity hitbox
			// Since i and j may be from any coordinate of the chair, we need to adjust the anchor based on that
			info.AnchorTilePosition.X = i; // Our chair is only 1 wide, so nothing special required
			info.AnchorTilePosition.Y = j;
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;

			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
			{ // Avoid being able to trigger it from long range
				player.GamepadEnableGrappleCooldown();
				player.sitting.SitDown(player, i, j);
			}

			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
			{ // Match condition in RightClick. Interaction should only show if clicking it does something
				return;
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ItemID.GraniteChair;

			if (Main.tile[i, j].TileFrameX != 0)
			{
				player.cursorItemIconReversed = true;
			}
		}
	}
	public class GraniteChair_TileEntity : ModTileEntity
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
			else if (!connectedNPC.active || connectedNPC.type != ModContent.ProjectileType<GraniteChair_ProjectileEntity>())
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
				if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 1f && npc.type == ModContent.ProjectileType<GraniteChair_ProjectileEntity>())
				{
					connectedNPC = npc;
					return;
				}
			}
			//ill be honest i dont exactly know the grounds for the offset for the npc but i *think* its like, half of the X tiles and all Y tiles
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num = Projectile.NewProjectile(new EntitySource_TileEntity(this, null), new Vector2((int)(Position.X * 16) + 8, (int)(Position.Y * 16) + 16), Vector2.Zero, ModContent.ProjectileType<GraniteChair_ProjectileEntity>(), 0, 0, ai2: CurrentlyEnabled ? 1 : 0);
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
			return tile.HasTile && tile.TileType == ModContent.TileType<GraniteChair>();
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
	public class GraniteChair_ProjectileEntity : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Tiles/InvisibleImage";
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 16;
			Projectile.height = 32;
			Projectile.aiStyle = -1;
			Projectile.damage = 0;
			Projectile.timeLeft = 6000;
			Projectile.tileCollide = false;

			Projectile.AsPred().IsPredTileEntity = true;

			Projectile.AsFood().CannotBeEatenDueToShenanigans = true;

			Projectile.AsFood().DefinedSize = 0.80;
			Projectile.AsPred().WeightGainRatio = 0.185;
			Projectile.AsPred().MaxStomachCapacity = 4.5;
			Projectile.AsPred().BaseStomachacheMeterCapacity = 400.0;
			Projectile.AsPred().CanSwallowBosses = false;
			Projectile.AsFood().MaxHealth = 7500;
			Projectile.AsFood().Health = 7500;

			Projectile.AsPred().MouthSoundRawOffset = new Vector2(0f, -14f);
			Projectile.AsPred().SmallGulps = Gulps.Short;
			Projectile.AsPred().SmallGulpThreshold = 0.1;
			Projectile.AsPred().BigGulps = Gulps.Standard;
			Projectile.AsPred().CanBeForceFed = CanGraniteChairBeForceFed;
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
		public static bool CanGraniteChairBeForceFed(Projectile projectile) => true;
		public override void AI()
		{
			Projectile.timeLeft = 6000;
			Projectile.velocity = Vector2.Zero;
			Tile Painting = Main.tile[Projectile.position.ToTileCoordinates()];
			if (!Painting.HasTile || Painting.TileType != ModContent.TileType<GraniteChair>())
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
				(int)Math.Floor(5.35 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				7
			);
		}
		public static int GetVisualWeightStage(Projectile projectile)
		{
			return Math.Min(
				(int)Math.Floor(4.5 * Math.Sqrt(projectile.AsPred().ExtraWeight)),
				2
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