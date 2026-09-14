using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Sounds.Vore;

namespace V2.Tiles.Vanilla.Relics
{
	public class EmpressOfLightRelic : ModTile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public const int FrameWidth = 18 * 3;
		public const int FrameHeight = 18 * 4;
		public const int HorizontalFrames = 1;
		public const int VerticalFrames = 1; // Optional: Increase this number to match the amount of relics you have on your extra sheet, if you choose to use the Item.placeStyle approach

		public Asset<Texture2D> RelicTexture;

		// Every relic has its own extra floating part, should be 50x50. Optional: Expand this sheet if you want to add more, stacked vertically
		// If you do not use the Item.placeStyle approach, and you extend from this class, you can override this to point to a different texture
		public virtual string RelicTextureName => "V2/Tiles/Vanilla/Relics/EmpressOfLightRelic_SpriteSheet";

		// All relics use the same pedestal texture, this one is copied from vanilla
		public override string Texture => "V2/Tiles/Vanilla/Relics/RelicPedestal";

		public override void Load()
		{
			if (!Main.dedServ)
			{
				// Cache the extra texture displayed on the pedestal
				RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
			}
		}

		public override void Unload()
		{
			// Unload the extra texture displayed on the pedestal
			RelicTexture = null;
		}

		public override void SetStaticDefaults()
		{
			Main.tileShine[Type] = 400; // Responsible for golden particles
			Main.tileFrameImportant[Type] = true; // Any multitile requires this

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4); // Relics are 3x4
			TileObjectData.newTile.LavaDeath = false; // Does not break when lava touches it
			TileObjectData.newTile.DrawYOffset = 2; // So the tile sinks into the ground
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft; // Player faces to the left
			TileObjectData.newTile.StyleHorizontal = false; // Based on how the alternate sprites are positioned on the sprite (by default, true)

			// This controls how styles are laid out in the texture file. This tile is special in that all styles will use the same texture section to draw the pedestal.
			TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.styleLineSkipVisualOverride = 0; // This forces the tile preview to draw as if drawing the 1st style.

			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<EmpressOfLightRelic_TileEntity>().Hook_AfterPlacement, -1, 0, true);
			TileObjectData.newTile.UsesCustomCanPlace = true;

			// Register an alternate tile data with flipped direction
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile); // Copy everything from above, saves us some code
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; // Player faces to the right

			TileObjectData.addAlternate(1);

			// Register the tile data itself
			TileObjectData.addTile(Type);

			// Register map name and color
			// "MapObject.Relic" refers to the translation key for the vanilla "Relic" text
			AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			ModContent.GetInstance<EmpressOfLightRelic_TileEntity>().Kill(i, j);
		}
		public override bool CreateDust(int i, int j, ref int type)
		{
			return false;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			// This forces the tile to draw the pedestal even if the placeStyle differs. 
			tileFrameX %= FrameWidth; // Clamps the frameX
			tileFrameY %= FrameHeight * 2; // Clamps the frameY (two horizontally aligned place styles, hence * 2)
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			// Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
			// Therefore we register the top-left of the tile as a "special point"
			// This allows us to draw things in SpecialDraw
			if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
			{
				Main.instance.TilesRenderer.AddSpecialPoint(i, j, Terraria.GameContent.Drawing.TileDrawing.TileCounterType.CustomNonSolid);
			}
		}
		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			// Take the tile, check if it actually exists
			Point p = new Point(i, j);
			Tile tile = Main.tile[p.X, p.Y];
			if (!tile.HasTile)
			{
				return;
			}

			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity))
			{
				if (tileEntity is EmpressOfLightRelic_TileEntity)
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<EmpressOfLightRelic_ProjectileEntity>())
						{

							// Get the initial draw parameters
							Texture2D texture = RelicTexture.Value;

							int frameY = tile.TileFrameX / FrameWidth; // Picks the frame on the sheet based on the placeStyle of the item
							int tumSize = EmpressOfLightRelic_ProjectileEntity.GetVisualBellySize(npc);
							Rectangle frame = new Rectangle(0, 88 * tumSize, 88, 84);

							Vector2 origin = frame.Size() / 2f;
							Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

							Color color = Lighting.GetColor(p.X, p.Y);

							bool direction = tile.TileFrameY / FrameHeight != 0; // This is related to the alternate tile data we registered before
							SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

							// Some math magic to make it smoothly move up and down over time
							const float TwoPi = (float)Math.PI * 2f;
							float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
							Vector2 drawPos = worldPos - Main.screenPosition + new Vector2(0f, -30f) + new Vector2(0f, offset * 4f);

							// Draw the main texture
							spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

							// Draw the periodic glow effect
							float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
							Color effectColor = color;
							effectColor.A = 0;
							effectColor = effectColor * 0.1f * scale;
							for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
							{
								spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
							}

							Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
							Player plr = Main.LocalPlayer;
							if (plr.AsV2Player().HoldingPredToggleRod)
							{
								Texture2D cornerTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodInactiveCorner").Value;
								if (npc.ai[2] == 1)
									cornerTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Tools/PredToggleRodActiveCorner").Value;

								spriteBatch.Draw( // Upper Left
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero -
									new Vector2(192, 192),
									new Rectangle(0, 0, 10, 10),
									Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Upper Right
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X + (npc.width), j * 16 - (int)Main.screenPosition.Y) + zero -
									new Vector2(192, 192),
									new Rectangle(0, 0, 10, 10),
									Color.White, 1.5708f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Bottom Left
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + (npc.height)) + zero -
									new Vector2(192, 192),
									new Rectangle(0, 0, 10, 10),
									Color.White, 4.71239f, default, 1f, SpriteEffects.None, 0f);
								spriteBatch.Draw( // Bottom Right
									cornerTexture,
									new Vector2(i * 16 - (int)Main.screenPosition.X + (npc.width), j * 16 - (int)Main.screenPosition.Y + (npc.height)) + zero -
									new Vector2(192, 192),
									new Rectangle(0, 0, 10, 10),
									Color.White, 3.14159f, default, 1f, SpriteEffects.None, 0f);
							}
						}
					}
				}
			}
		}
	}

	public class EmpressOfLightRelic_TileEntity : ModTileEntity
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
			else if (!connectedNPC.active || connectedNPC.type != ModContent.ProjectileType<EmpressOfLightRelic_ProjectileEntity>())
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
				if (npc.active && (npc.position / 16).Distance(Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<EmpressOfLightRelic_ProjectileEntity>())
				{
					connectedNPC = npc;
					return;
				}
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num = Projectile.NewProjectile(new EntitySource_TileEntity(this, null), new Vector2((int)(Position.X * 16) + 24, (int)(Position.Y * 16) + 32), Vector2.Zero, ModContent.ProjectileType<EmpressOfLightRelic_ProjectileEntity>(), 0, 0, ai2: CurrentlyEnabled ? 1 : 0);
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
			return tile.HasTile && tile.TileType == ModContent.TileType<EmpressOfLightRelic>();
		}
		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				// Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
				int width = 3;
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
	public class EmpressOfLightRelic_ProjectileEntity : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/Tiles/InvisibleImage";
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 48;
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
			if (Projectile.ai[0] <= 0) Projectile.ai[0] = Main.rand.Next(300, 600);
			Projectile.timeLeft = 6000;
			Projectile.velocity = Vector2.Zero;
			Tile Painting = Main.tile[Projectile.position.ToTileCoordinates()];
			if (!Painting.HasTile || Painting.TileType != ModContent.TileType<EmpressOfLightRelic>())
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
				(int)Math.Floor(6 * Math.Sqrt(PredProjectile.GetCurrentBellyWeight(projectile))),
				5
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
