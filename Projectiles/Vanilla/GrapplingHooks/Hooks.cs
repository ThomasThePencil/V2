using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Golf;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;

namespace V2.Projectiles.Vanilla.GrapplingHooks
{
	public class Hooks : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public float SetPullingStrength(Projectile projectile)
		{
			switch(projectile.type)
			{
				case ProjectileID.BatHook:
					return 0.001f;
				case ProjectileID.Web or ProjectileID.QueenSlimeHook:
					return 0.15f;
				case ProjectileID.SlimeHook:
					return 0.175f;
				case ProjectileID.IvyWhip:
					return 0.2f;
				case ProjectileID.Hook or ProjectileID.SquirrelHook or ProjectileID.SkeletronHand or ProjectileID.FishHook:
					return 0.33f;
				case ProjectileID.GemHookAmethyst or ProjectileID.GemHookTopaz:
					return 0.5f;
				case ProjectileID.GemHookSapphire or ProjectileID.GemHookEmerald or ProjectileID.IlluminantHook or ProjectileID.WormHook or ProjectileID.TendonHook:
					return 0.75f;
				case ProjectileID.LunarHookNebula or ProjectileID.LunarHookSolar or ProjectileID.LunarHookStardust or ProjectileID.LunarHookVortex:
					return 1.33f;
			}
			return 1;
		}
		public float SetPullingSpeed(Projectile projectile)
		{
			switch (projectile.type)
			{
				case ProjectileID.BatHook:
					return 1.75f;
				case ProjectileID.LunarHookNebula or ProjectileID.LunarHookSolar or ProjectileID.LunarHookStardust or ProjectileID.LunarHookVortex:
					return 1.67f;
				case ProjectileID.ThornHook or ProjectileID.WoodHook or ProjectileID.ChristmasHook:
					return 1.2f;
			}
			return 1;
		}
		public bool CanHookAttachTo(Projectile projectile, int x, int y)
		{
			Tile theTile = Main.tile[x, y];
			bool vanilla = Main.tileSolid[theTile.TileType] | theTile.TileType == 314 | (projectile.type == 865 && TileID.Sets.IsATreeTrunk[theTile.TileType]) | (projectile.type == 865 && theTile.TileType == 323);
			vanilla &= theTile.HasTile;
			bool? flag = ProjectileLoader.GrappleCanLatchOnTo(projectile, Main.player[projectile.owner], x, y);
			if (flag != null)
			{
				return flag.GetValueOrDefault();
			}
			return vanilla;
		}

		//das a lotta hooks, jegus
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) =>
			entity.type == ProjectileID.Hook || entity.type == ProjectileID.GemHookAmethyst || entity.type == ProjectileID.GemHookTopaz
			|| entity.type == ProjectileID.GemHookSapphire || entity.type == ProjectileID.GemHookEmerald || entity.type == ProjectileID.GemHookRuby
			|| entity.type == ProjectileID.AmberHook || entity.type == ProjectileID.GemHookDiamond || entity.type == ProjectileID.SquirrelHook
			|| entity.type == ProjectileID.Web || entity.type == ProjectileID.SkeletronHand || entity.type == ProjectileID.SlimeHook
			|| entity.type == ProjectileID.FishHook || entity.type == ProjectileID.IvyWhip || entity.type == ProjectileID.BatHook
			|| entity.type == ProjectileID.CandyCaneHook || entity.type == ProjectileID.DualHookRed || entity.type == ProjectileID.DualHookBlue
			|| entity.type == ProjectileID.QueenSlimeHook || entity.type == ProjectileID.WormHook || entity.type == ProjectileID.TendonHook
			|| entity.type == ProjectileID.IlluminantHook || entity.type == ProjectileID.ThornHook || entity.type == ProjectileID.WoodHook
			|| entity.type == ProjectileID.ChristmasHook || entity.type == ProjectileID.LunarHookSolar || entity.type == ProjectileID.LunarHookNebula
			|| entity.type == ProjectileID.LunarHookVortex || entity.type == ProjectileID.LunarHookStardust;
		public override void SetDefaults(Projectile entity)
		{
			entity.AsV2Proj().GrappleStrength = SetPullingStrength(entity);
			entity.AsV2Proj().GrappleSpeed = SetPullingSpeed(entity);
		}
		/*
		public override bool PreAI(Projectile projectile)
		{
			projectile.aiStyle = 0;
			return base.PreAI(projectile);
		}
		public override void AI(Projectile projectile)
		{
			if (Main.player[projectile.owner].dead || Main.player[projectile.owner].stoned || Main.player[projectile.owner].webbed || Main.player[projectile.owner].frozen)
			{
				projectile.Kill();
				return;
			}
			Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
			Vector2 vector = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
			float num = mountedCenter.X - vector.X;
			float num2 = mountedCenter.Y - vector.Y;
			float num3 = (float)Math.Sqrt((double)(num * num + num2 * num2));
			projectile.rotation = (float)Math.Atan2((double)num2, (double)num) - 1.57f;
			if (projectile.ai[0] == 2f && projectile.type == 865)
			{
				float num4 = 1.5707964f;
				int num5 = (int)Math.Round((double)(projectile.rotation / num4));
				projectile.rotation = (float)num5 * num4;
			}
			if (Main.myPlayer == projectile.owner)
			{
				int num6 = (int)(projectile.Center.X / 16f);
				int num7 = (int)(projectile.Center.Y / 16f);
				if (num6 > 0 && num7 > 0 && num6 < Main.maxTilesX && num7 < Main.maxTilesY && Main.tile[num6, num7].HasTile && TileID.Sets.CrackedBricks[Main.tile[num6, num7].TileType] && Main.rand.NextBool(16))
				{
					WorldGen.KillTile(num6, num7, false, false, false);
					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 20, (float)num6, (float)num7, 0f, 0, 0, 0);
					}
				}
			}
			if (num3 > 2500f)
			{
				projectile.Kill();
			}
			if (projectile.type == 256)
			{
				projectile.rotation = (float)Math.Atan2((double)num2, (double)num) + 3.9250002f;
			}
			if (projectile.type == 446)
			{
				Lighting.AddLight(mountedCenter, 0f, 0.4f, 0.3f);
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] >= 28f)
				{
					projectile.localAI[0] = 0f;
				}
				DelegateMethods.v3_1 = new Vector3(0f, 0.4f, 0.3f);
				Vector2 center = projectile.Center;
				Vector2 end = mountedCenter;
				float width = 8f;
				/*Utils.TileActionAttempt plot;
				if ((plot = Projectile.<>O.<4>__CastLightOpen) == null)
				{
					plot = (Projectile.<> O.< 4 > __CastLightOpen = new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
				}
				Utils.PlotTileLine(center, end, width, new Utils.TileActionAttempt(DelegateMethods.CastLight));
			}
			if (projectile.type == 652)
			{
				int num20 = projectile.frameCounter + 1;
				projectile.frameCounter = num20;
				if (num20 >= 7)
				{
					projectile.frameCounter = 0;
					num20 = projectile.frame + 1;
					projectile.frame = num20;
					if (num20 >= Main.projFrames[projectile.type])
					{
						projectile.frame = 0;
					}
				}
			}
			if (projectile.type >= 646 && projectile.type <= 649)
			{
				Vector3 vector2 = Vector3.Zero;
				switch (projectile.type)
				{
					case 646:
						vector2 = new Vector3(0.7f, 0.5f, 0.1f);
						break;
					case 647:
						vector2 = new Vector3(0f, 0.6f, 0.7f);
						break;
					case 648:
						vector2 = new Vector3(0.6f, 0.2f, 0.6f);
						break;
					case 649:
						vector2 = new Vector3(0.6f, 0.6f, 0.9f);
						break;
				}
				Lighting.AddLight(mountedCenter, vector2);
				Lighting.AddLight(projectile.Center, vector2);
				DelegateMethods.v3_1 = vector2;
				Vector2 center2 = projectile.Center;
				Vector2 end2 = mountedCenter;
				float width2 = 8f;
				/*Utils.TileActionAttempt plot2;
				if ((plot2 = Projectile.<> O.< 4 > __CastLightOpen) == null)
				{
					plot2 = (Projectile.<> O.< 4 > __CastLightOpen = new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
				}
				Utils.PlotTileLine(center2, end2, width2, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
			}
			if (projectile.ai[0] == 0f)
			{
				if ((num3 > 300f && projectile.type == 13) || (num3 > 400f && projectile.type == 32) || (num3 > 440f && projectile.type == 73) || (num3 > 440f && projectile.type == 74) || (num3 > 375f && projectile.type == 165) || (num3 > 350f && projectile.type == 256) || (num3 > 500f && projectile.type == 315) || (num3 > 550f && projectile.type == 322) || (num3 > 400f && projectile.type == 331) || (num3 > 550f && projectile.type == 332) || (num3 > 400f && projectile.type == 372) || (num3 > 300f && projectile.type == 396) || (num3 > 550f && projectile.type >= 646 && projectile.type <= 649) || (num3 > 600f && projectile.type == 652) || (num3 > 300f && projectile.type == 865) || (num3 > 500f && projectile.type == 935) || (num3 > 480f && projectile.type >= 486 && projectile.type <= 489) || (num3 > 500f && projectile.type == 446))
				{
					projectile.ai[0] = 1f;
				}
				else if (projectile.type >= 230 && projectile.type <= 235)
				{
					int num8 = 300 + (projectile.type - 230) * 30;
					if (num3 > (float)num8)
					{
						projectile.ai[0] = 1f;
					}
				}
				else if (projectile.type == 753)
				{
					int num9 = 420;
					if (num3 > (float)num9)
					{
						projectile.ai[0] = 1f;
					}
				}
				else if (ProjectileLoader.GrappleOutOfRange(num3, projectile))
				{
					projectile.ai[0] = 1f;
				}
				Vector2 vector3 = projectile.Center - new Vector2(5f);
				Vector2 vector5 = projectile.Center + new Vector2(5f);
				Point point = (vector3 - new Vector2(16f)).ToTileCoordinates();
				Point point4 = (vector5 + new Vector2(32f)).ToTileCoordinates();
				int num10 = point.X;
				int num11 = point4.X;
				int num12 = point.Y;
				int num13 = point4.Y;
				if (num10 < 0)
				{
					num10 = 0;
				}
				if (num11 > Main.maxTilesX)
				{
					num11 = Main.maxTilesX;
				}
				if (num12 < 0)
				{
					num12 = 0;
				}
				if (num13 > Main.maxTilesY)
				{
					num13 = Main.maxTilesY;
				}
				Player player = Main.player[projectile.owner];
				List<Point> list = new List<Point>();
				for (int i = 0; i < player.grapCount; i++)
				{
					Projectile proj = Main.projectile[player.grappling[i]];
					if (proj.aiStyle == 7 && proj.ai[0] == 2f)
					{
						Point pt = proj.Center.ToTileCoordinates();
						Tile tileSafely = Framing.GetTileSafely(pt);
						if (tileSafely.TileType == 314 || TileID.Sets.Platforms[tileSafely.TileType])
						{
							for (int j = -2; j <= 2; j++)
							{
								for (int k = -2; k <= 2; k++)
								{
									Point point2 = new Point(pt.X + j, pt.Y + k);
									Tile tileSafely2 = Framing.GetTileSafely(point2);
									if (tileSafely2.TileType == 314 || TileID.Sets.Platforms[tileSafely2.TileType])
									{
										list.Add(point2);
									}
								}
							}
						}
					}
				}
				Vector2 vector4 = default(Vector2);
				for (int l = num10; l < num11; l++)
				{
					for (int m = num12; m < num13; m++)
					{
						if (Main.tile[l, m] == null)
						{
							//Main.tile[l, m] = default(Tile);
						}
						vector4.X = (float)(l * 16);
						vector4.Y = (float)(m * 16);
						if (vector3.X + 10f > vector4.X && vector3.X < vector4.X + 16f && vector3.Y + 10f > vector4.Y && vector3.Y < vector4.Y + 16f)
						{
							Tile tile = Main.tile[l, m];
							if (CanHookAttachTo(projectile, l, m) && !list.Contains(new Point(l, m)) && (projectile.type != 403 || tile.TileType == 314) && !Main.player[projectile.owner].IsBlacklistedForGrappling(new Point(l, m)))
							{
								if (Main.player[projectile.owner].grapCount < 10)
								{
									Main.player[projectile.owner].grappling[Main.player[projectile.owner].grapCount] = projectile.whoAmI;
									Main.player[projectile.owner].grapCount++;
								}
								if (Main.myPlayer == projectile.owner)
								{
									int num14 = 0;
									int num15 = -1;
									int num16 = 100000;
									if (projectile.type == 73 || projectile.type == 74)
									{
										for (int n = 0; n < 1000; n++)
										{
											if (n != projectile.whoAmI && Main.projectile[n].active && Main.projectile[n].owner == projectile.owner && Main.projectile[n].aiStyle == 7 && Main.projectile[n].ai[0] == 2f)
											{
												Main.projectile[n].Kill();
											}
										}
									}
									else
									{
										int num17 = 3;
										if (projectile.type == 165)
										{
											num17 = 8;
										}
										if (projectile.type == 256)
										{
											num17 = 2;
										}
										if (projectile.type == 372)
										{
											num17 = 2;
										}
										if (projectile.type == 652)
										{
											num17 = 1;
										}
										if (projectile.type >= 646 && projectile.type <= 649)
										{
											num17 = 4;
										}
										ProjectileLoader.NumGrappleHooks(projectile, Main.player[projectile.owner], ref num17);
										for (int num18 = 0; num18 < 1000; num18++)
										{
											if (Main.projectile[num18].active && Main.projectile[num18].owner == projectile.owner && Main.projectile[num18].aiStyle == 7)
											{
												if (Main.projectile[num18].timeLeft < num16)
												{
													num15 = num18;
													num16 = Main.projectile[num18].timeLeft;
												}
												num14++;
											}
										}
										if (num14 > num17)
										{
											Main.projectile[num15].Kill();
										}
									}
									WorldGen.KillTile(l, m, true, true, false);
									//SoundEngine.PlaySound(0, l * 16, m * 16, 1, 1f, 0f);
									projectile.velocity.X = 0f;
									projectile.velocity.Y = 0f;
									projectile.ai[0] = 2f;
									projectile.position.X = (float)(l * 16 + 8 - projectile.width / 2);
									projectile.position.Y = (float)(m * 16 + 8 - projectile.height / 2);
									Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(l, m);
									if (tileVisualHitbox != null)
									{
										projectile.Center = tileVisualHitbox.Value.Center.ToVector2();
									}
									projectile.damage = 0;
									projectile.netUpdate = true;
									if (Main.myPlayer == projectile.owner)
									{
										if (projectile.type == 935)
										{
											Main.player[projectile.owner].DoQueenSlimeHookTeleport(projectile.Center);
										}
										NetMessage.SendData(13, -1, -1, null, projectile.owner, 0f, 0f, 0f, 0, 0, 0);
										break;
									}
									break;
								}
							}
						}
					}
					if (projectile.ai[0] == 2f)
					{
						return;
					}
				}
				return;
			}
			if (projectile.ai[0] == 1f)
			{
				float num19 = 11f;
				if (projectile.type == 32)
				{
					num19 = 15f;
				}
				if (projectile.type == 73 || projectile.type == 74)
				{
					num19 = 17f;
				}
				if (projectile.type == 315)
				{
					num19 = 20f;
				}
				if (projectile.type == 322)
				{
					num19 = 22f;
				}
				if (projectile.type >= 230 && projectile.type <= 235)
				{
					num19 = 11f + (float)(projectile.type - 230) * 0.75f;
				}
				if (projectile.type == 753)
				{
					num19 = 15f;
				}
				if (projectile.type == 446)
				{
					num19 = 20f;
				}
				if (projectile.type >= 486 && projectile.type <= 489)
				{
					num19 = 18f;
				}
				if (projectile.type >= 646 && projectile.type <= 649)
				{
					num19 = 24f;
				}
				if (projectile.type == 652)
				{
					num19 = 24f;
				}
				if (projectile.type == 332)
				{
					num19 = 17f;
				}
				ProjectileLoader.GrappleRetreatSpeed(projectile, Main.player[projectile.owner], ref num19);
				if (num3 < 24f)
				{
					projectile.Kill();
				}
				num3 = num19 / num3;
				num *= num3;
				num2 *= num3;
				projectile.velocity.X = num;
				projectile.velocity.Y = num2;
				return;
			}
			if (projectile.ai[0] == 2f)
			{
				Point point3 = projectile.Center.ToTileCoordinates();
				if (Main.tile[point3.X, point3.Y] == null)
				{
					//Main.tile[point3.X, point3.Y] = default(Tile);
				}
				bool flag = true;
				if (CanHookAttachTo(projectile, point3.X, point3.Y))
				{
					flag = false;
				}
				if (flag)
				{
					projectile.ai[0] = 1f;
					return;
				}
				if (Main.player[projectile.owner].grapCount < 10)
				{
					Main.player[projectile.owner].grappling[Main.player[projectile.owner].grapCount] = projectile.whoAmI;
					Main.player[projectile.owner].grapCount++;
				}
			}
		}
		public override void PostAI(Projectile projectile)
		{
			projectile.aiStyle = 7;
		}

		public float SetPullingStrength(Projectile projectile)
		{
			switch(projectile.type)
			{
				case ProjectileID.BatHook:
					return 0.001f;
				case ProjectileID.Web or ProjectileID.QueenSlimeHook:
					return 0.15f;
				case ProjectileID.SlimeHook:
					return 0.175f;
				case ProjectileID.IvyWhip:
					return 0.2f;
				case ProjectileID.Hook or ProjectileID.SquirrelHook or ProjectileID.SkeletronHand or ProjectileID.FishHook:
					return 0.33f;
				case ProjectileID.GemHookAmethyst or ProjectileID.GemHookTopaz:
					return 0.5f;
				case ProjectileID.GemHookSapphire or ProjectileID.GemHookEmerald or ProjectileID.IlluminantHook or ProjectileID.WormHook or ProjectileID.TendonHook:
					return 0.75f;
				case ProjectileID.LunarHookNebula or ProjectileID.LunarHookSolar or ProjectileID.LunarHookStardust or ProjectileID.LunarHookVortex:
					return 1.33f;
			}
			return 1;
		}
		public float SetPullingSpeed(Projectile projectile)
		{
			switch (projectile.type)
			{
				case ProjectileID.BatHook:
					return 1.75f;
				case ProjectileID.LunarHookNebula or ProjectileID.LunarHookSolar or ProjectileID.LunarHookStardust or ProjectileID.LunarHookVortex:
					return 1.7f;
				case ProjectileID.ThornHook or ProjectileID.WoodHook or ProjectileID.ChristmasHook:
					return 1.35f;
				case ProjectileID.TendonHook or ProjectileID.WormHook or ProjectileID.IlluminantHook:
					return 1.2f;
				case ProjectileID.DualHookRed or ProjectileID.DualHookBlue:
					return 1.1f;
			}
			return 1;
		}
		public bool CanHookAttachTo(Projectile projectile, int x, int y)
		{
			Tile theTile = Main.tile[x, y];
			bool vanilla = Main.tileSolid[theTile.TileType] | theTile.TileType == 314 | (projectile.type == 865 && TileID.Sets.IsATreeTrunk[theTile.TileType]) | (projectile.type == 865 && theTile.TileType == 323);
			vanilla &= theTile.HasTile;
			bool? flag = ProjectileLoader.GrappleCanLatchOnTo(projectile, Main.player[projectile.owner], x, y);
			if (flag != null)
			{
				return flag.GetValueOrDefault();
			}
			return vanilla;
		}

		//das a lotta hooks, jegus
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) =>
			entity.type is ProjectileID.Hook or ProjectileID.GemHookAmethyst or ProjectileID.GemHookTopaz or ProjectileID.GemHookSapphire
			or ProjectileID.GemHookEmerald or ProjectileID.GemHookRuby or ProjectileID.AmberHook or ProjectileID.GemHookDiamond
			or ProjectileID.SquirrelHook or ProjectileID.Web or ProjectileID.SkeletronHand or ProjectileID.SlimeHook
			or ProjectileID.FishHook or ProjectileID.IvyWhip or ProjectileID.BatHook or ProjectileID.CandyCaneHook or ProjectileID.DualHookRed
			or ProjectileID.DualHookBlue or ProjectileID.QueenSlimeHook or ProjectileID.WormHook or ProjectileID.TendonHook
			or ProjectileID.IlluminantHook or ProjectileID.ThornHook or ProjectileID.WoodHook or ProjectileID.ChristmasHook
			or ProjectileID.LunarHookSolar or ProjectileID.LunarHookNebula or ProjectileID.LunarHookVortex or ProjectileID.LunarHookStardust;
		public override void SetDefaults(Projectile entity)
		{
			entity.AsV2Proj().GrappleStrength = SetPullingStrength(entity);
			entity.AsV2Proj().GrappleSpeed = SetPullingSpeed(entity);
		}
		/*
		public override bool PreAI(Projectile projectile)
		{
			projectile.aiStyle = 0;
			return base.PreAI(projectile);
		}
		public override void AI(Projectile projectile)
		{
			if (Main.player[projectile.owner].dead || Main.player[projectile.owner].stoned || Main.player[projectile.owner].webbed || Main.player[projectile.owner].frozen)
			{
				projectile.Kill();
				return;
			}
			Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
			Vector2 vector = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
			float num = mountedCenter.X - vector.X;
			float num2 = mountedCenter.Y - vector.Y;
			float num3 = (float)Math.Sqrt((double)(num * num + num2 * num2));
			projectile.rotation = (float)Math.Atan2((double)num2, (double)num) - 1.57f;
			if (projectile.ai[0] == 2f && projectile.type == 865)
			{
				float num4 = 1.5707964f;
				int num5 = (int)Math.Round((double)(projectile.rotation / num4));
				projectile.rotation = (float)num5 * num4;
			}
			if (Main.myPlayer == projectile.owner)
			{
				int num6 = (int)(projectile.Center.X / 16f);
				int num7 = (int)(projectile.Center.Y / 16f);
				if (num6 > 0 && num7 > 0 && num6 < Main.maxTilesX && num7 < Main.maxTilesY && Main.tile[num6, num7].HasTile && TileID.Sets.CrackedBricks[Main.tile[num6, num7].TileType] && Main.rand.NextBool(16))
				{
					WorldGen.KillTile(num6, num7, false, false, false);
					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 20, (float)num6, (float)num7, 0f, 0, 0, 0);
					}
				}
			}
			if (num3 > 2500f)
			{
				projectile.Kill();
			}
			if (projectile.type == 256)
			{
				projectile.rotation = (float)Math.Atan2((double)num2, (double)num) + 3.9250002f;
			}
			if (projectile.type == 446)
			{
				Lighting.AddLight(mountedCenter, 0f, 0.4f, 0.3f);
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] >= 28f)
				{
					projectile.localAI[0] = 0f;
				}
				DelegateMethods.v3_1 = new Vector3(0f, 0.4f, 0.3f);
				Vector2 center = projectile.Center;
				Vector2 end = mountedCenter;
				float width = 8f;
				/*Utils.TileActionAttempt plot;
				if ((plot = Projectile.<>O.<4>__CastLightOpen) == null)
				{
					plot = (Projectile.<> O.< 4 > __CastLightOpen = new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
				}
				Utils.PlotTileLine(center, end, width, new Utils.TileActionAttempt(DelegateMethods.CastLight));
			}
			if (projectile.type == 652)
			{
				int num20 = projectile.frameCounter + 1;
				projectile.frameCounter = num20;
				if (num20 >= 7)
				{
					projectile.frameCounter = 0;
					num20 = projectile.frame + 1;
					projectile.frame = num20;
					if (num20 >= Main.projFrames[projectile.type])
					{
						projectile.frame = 0;
					}
				}
			}
			if (projectile.type >= 646 && projectile.type <= 649)
			{
				Vector3 vector2 = Vector3.Zero;
				switch (projectile.type)
				{
					case 646:
						vector2 = new Vector3(0.7f, 0.5f, 0.1f);
						break;
					case 647:
						vector2 = new Vector3(0f, 0.6f, 0.7f);
						break;
					case 648:
						vector2 = new Vector3(0.6f, 0.2f, 0.6f);
						break;
					case 649:
						vector2 = new Vector3(0.6f, 0.6f, 0.9f);
						break;
				}
				Lighting.AddLight(mountedCenter, vector2);
				Lighting.AddLight(projectile.Center, vector2);
				DelegateMethods.v3_1 = vector2;
				Vector2 center2 = projectile.Center;
				Vector2 end2 = mountedCenter;
				float width2 = 8f;
				/*Utils.TileActionAttempt plot2;
				if ((plot2 = Projectile.<> O.< 4 > __CastLightOpen) == null)
				{
					plot2 = (Projectile.<> O.< 4 > __CastLightOpen = new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
				}
				Utils.PlotTileLine(center2, end2, width2, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
			}
			if (projectile.ai[0] == 0f)
			{
				if ((num3 > 300f && projectile.type == 13) || (num3 > 400f && projectile.type == 32) || (num3 > 440f && projectile.type == 73) || (num3 > 440f && projectile.type == 74) || (num3 > 375f && projectile.type == 165) || (num3 > 350f && projectile.type == 256) || (num3 > 500f && projectile.type == 315) || (num3 > 550f && projectile.type == 322) || (num3 > 400f && projectile.type == 331) || (num3 > 550f && projectile.type == 332) || (num3 > 400f && projectile.type == 372) || (num3 > 300f && projectile.type == 396) || (num3 > 550f && projectile.type >= 646 && projectile.type <= 649) || (num3 > 600f && projectile.type == 652) || (num3 > 300f && projectile.type == 865) || (num3 > 500f && projectile.type == 935) || (num3 > 480f && projectile.type >= 486 && projectile.type <= 489) || (num3 > 500f && projectile.type == 446))
				{
					projectile.ai[0] = 1f;
				}
				else if (projectile.type >= 230 && projectile.type <= 235)
				{
					int num8 = 300 + (projectile.type - 230) * 30;
					if (num3 > (float)num8)
					{
						projectile.ai[0] = 1f;
					}
				}
				else if (projectile.type == 753)
				{
					int num9 = 420;
					if (num3 > (float)num9)
					{
						projectile.ai[0] = 1f;
					}
				}
				else if (ProjectileLoader.GrappleOutOfRange(num3, projectile))
				{
					projectile.ai[0] = 1f;
				}
				Vector2 vector3 = projectile.Center - new Vector2(5f);
				Vector2 vector5 = projectile.Center + new Vector2(5f);
				Point point = (vector3 - new Vector2(16f)).ToTileCoordinates();
				Point point4 = (vector5 + new Vector2(32f)).ToTileCoordinates();
				int num10 = point.X;
				int num11 = point4.X;
				int num12 = point.Y;
				int num13 = point4.Y;
				if (num10 < 0)
				{
					num10 = 0;
				}
				if (num11 > Main.maxTilesX)
				{
					num11 = Main.maxTilesX;
				}
				if (num12 < 0)
				{
					num12 = 0;
				}
				if (num13 > Main.maxTilesY)
				{
					num13 = Main.maxTilesY;
				}
				Player player = Main.player[projectile.owner];
				List<Point> list = new List<Point>();
				for (int i = 0; i < player.grapCount; i++)
				{
					Projectile proj = Main.projectile[player.grappling[i]];
					if (proj.aiStyle == 7 && proj.ai[0] == 2f)
					{
						Point pt = proj.Center.ToTileCoordinates();
						Tile tileSafely = Framing.GetTileSafely(pt);
						if (tileSafely.TileType == 314 || TileID.Sets.Platforms[tileSafely.TileType])
						{
							for (int j = -2; j <= 2; j++)
							{
								for (int k = -2; k <= 2; k++)
								{
									Point point2 = new Point(pt.X + j, pt.Y + k);
									Tile tileSafely2 = Framing.GetTileSafely(point2);
									if (tileSafely2.TileType == 314 || TileID.Sets.Platforms[tileSafely2.TileType])
									{
										list.Add(point2);
									}
								}
							}
						}
					}
				}
				Vector2 vector4 = default(Vector2);
				for (int l = num10; l < num11; l++)
				{
					for (int m = num12; m < num13; m++)
					{
						if (Main.tile[l, m] == null)
						{
							//Main.tile[l, m] = default(Tile);
						}
						vector4.X = (float)(l * 16);
						vector4.Y = (float)(m * 16);
						if (vector3.X + 10f > vector4.X && vector3.X < vector4.X + 16f && vector3.Y + 10f > vector4.Y && vector3.Y < vector4.Y + 16f)
						{
							Tile tile = Main.tile[l, m];
							if (CanHookAttachTo(projectile, l, m) && !list.Contains(new Point(l, m)) && (projectile.type != 403 || tile.TileType == 314) && !Main.player[projectile.owner].IsBlacklistedForGrappling(new Point(l, m)))
							{
								if (Main.player[projectile.owner].grapCount < 10)
								{
									Main.player[projectile.owner].grappling[Main.player[projectile.owner].grapCount] = projectile.whoAmI;
									Main.player[projectile.owner].grapCount++;
								}
								if (Main.myPlayer == projectile.owner)
								{
									int num14 = 0;
									int num15 = -1;
									int num16 = 100000;
									if (projectile.type == 73 || projectile.type == 74)
									{
										for (int n = 0; n < 1000; n++)
										{
											if (n != projectile.whoAmI && Main.projectile[n].active && Main.projectile[n].owner == projectile.owner && Main.projectile[n].aiStyle == 7 && Main.projectile[n].ai[0] == 2f)
											{
												Main.projectile[n].Kill();
											}
										}
									}
									else
									{
										int num17 = 3;
										if (projectile.type == 165)
										{
											num17 = 8;
										}
										if (projectile.type == 256)
										{
											num17 = 2;
										}
										if (projectile.type == 372)
										{
											num17 = 2;
										}
										if (projectile.type == 652)
										{
											num17 = 1;
										}
										if (projectile.type >= 646 && projectile.type <= 649)
										{
											num17 = 4;
										}
										ProjectileLoader.NumGrappleHooks(projectile, Main.player[projectile.owner], ref num17);
										for (int num18 = 0; num18 < 1000; num18++)
										{
											if (Main.projectile[num18].active && Main.projectile[num18].owner == projectile.owner && Main.projectile[num18].aiStyle == 7)
											{
												if (Main.projectile[num18].timeLeft < num16)
												{
													num15 = num18;
													num16 = Main.projectile[num18].timeLeft;
												}
												num14++;
											}
										}
										if (num14 > num17)
										{
											Main.projectile[num15].Kill();
										}
									}
									WorldGen.KillTile(l, m, true, true, false);
									//SoundEngine.PlaySound(0, l * 16, m * 16, 1, 1f, 0f);
									projectile.velocity.X = 0f;
									projectile.velocity.Y = 0f;
									projectile.ai[0] = 2f;
									projectile.position.X = (float)(l * 16 + 8 - projectile.width / 2);
									projectile.position.Y = (float)(m * 16 + 8 - projectile.height / 2);
									Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(l, m);
									if (tileVisualHitbox != null)
									{
										projectile.Center = tileVisualHitbox.Value.Center.ToVector2();
									}
									projectile.damage = 0;
									projectile.netUpdate = true;
									if (Main.myPlayer == projectile.owner)
									{
										if (projectile.type == 935)
										{
											Main.player[projectile.owner].DoQueenSlimeHookTeleport(projectile.Center);
										}
										NetMessage.SendData(13, -1, -1, null, projectile.owner, 0f, 0f, 0f, 0, 0, 0);
										break;
									}
									break;
								}
							}
						}
					}
					if (projectile.ai[0] == 2f)
					{
						return;
					}
				}
				return;
			}
			if (projectile.ai[0] == 1f)
			{
				float num19 = 11f;
				if (projectile.type == 32)
				{
					num19 = 15f;
				}
				if (projectile.type == 73 || projectile.type == 74)
				{
					num19 = 17f;
				}
				if (projectile.type == 315)
				{
					num19 = 20f;
				}
				if (projectile.type == 322)
				{
					num19 = 22f;
				}
				if (projectile.type >= 230 && projectile.type <= 235)
				{
					num19 = 11f + (float)(projectile.type - 230) * 0.75f;
				}
				if (projectile.type == 753)
				{
					num19 = 15f;
				}
				if (projectile.type == 446)
				{
					num19 = 20f;
				}
				if (projectile.type >= 486 && projectile.type <= 489)
				{
					num19 = 18f;
				}
				if (projectile.type >= 646 && projectile.type <= 649)
				{
					num19 = 24f;
				}
				if (projectile.type == 652)
				{
					num19 = 24f;
				}
				if (projectile.type == 332)
				{
					num19 = 17f;
				}
				ProjectileLoader.GrappleRetreatSpeed(projectile, Main.player[projectile.owner], ref num19);
				if (num3 < 24f)
				{
					projectile.Kill();
				}
				num3 = num19 / num3;
				num *= num3;
				num2 *= num3;
				projectile.velocity.X = num;
				projectile.velocity.Y = num2;
				return;
			}
			if (projectile.ai[0] == 2f)
			{
				Point point3 = projectile.Center.ToTileCoordinates();
				if (Main.tile[point3.X, point3.Y] == null)
				{
					//Main.tile[point3.X, point3.Y] = default(Tile);
				}
				bool flag = true;
				if (CanHookAttachTo(projectile, point3.X, point3.Y))
				{
					flag = false;
				}
				if (flag)
				{
					projectile.ai[0] = 1f;
					return;
				}
				if (Main.player[projectile.owner].grapCount < 10)
				{
					Main.player[projectile.owner].grappling[Main.player[projectile.owner].grapCount] = projectile.whoAmI;
					Main.player[projectile.owner].grapCount++;
				}
			}
		}
		public override void PostAI(Projectile projectile)
		{
			projectile.aiStyle = 7;
		}
*/
	}
}
