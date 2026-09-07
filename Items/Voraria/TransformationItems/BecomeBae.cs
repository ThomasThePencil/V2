using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Items.Vanilla.Accessories;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Items.Voraria.TransformationItems.Baelz
{
	public class A_BaeHeadDrawLayer : PlayerDrawLayer
	{
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			return drawInfo.drawPlayer.AsV2Player().HasTransformation;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.NeckAcc, PlayerDrawLayers.Head);
		public override bool IsHeadLayer => true;
		public override string Name => "Baelz_Head";
		public int LastIdleFrame = 0;
		public int LastRunFrame = 0;
		public Rectangle GetFrameForThisHead(Player player)
		{
			bool OnSelectScreen = Main.gameMenu;
			Rectangle rect = new Rectangle(0, 0, 82, 60);
			int idleFrame = LastIdleFrame;
			int runFrame = LastRunFrame;
			if (!Main.gamePaused)
			{
				idleFrame = (int)(Main.GlobalTimeWrappedHourly * 6) % 4;
				runFrame = (int)(Main.GlobalTimeWrappedHourly * 8) % 6;
				LastIdleFrame = idleFrame;
				LastRunFrame = runFrame;
			}
			bool inAir = player.IsAirborne() || player.sleeping.isSleeping || player.sitting.isSitting || player.grappling[0] >= 0;
			if (!inAir)
			{
				if (player.velocity.X != 0) //running
				{
					rect.Y = 60;
					rect.X = 82 * runFrame;
				}
				else if (OnSelectScreen && player.legFrame.Y > 100)
				{
					rect.Y = 60;
					rect.X = 82 * runFrame;
				}
				else
				{
					rect.Y = 0;
					rect.X = 82 * idleFrame;
				}
			}
			else
			{
				rect.Y = 120;
				if (player.velocity.Y >= 0) rect.X = 82;
				rect.Height += 8;
			}
			return rect;
		}
		public string GetExpression(Player player)
		{
			string expr = "Neutral";
			if (player.HasBuff(BuffID.Tipsy))
				expr = "Drunk";
			else if (player.HasBuff(BuffID.Weak))
				expr = "Exhausted";
			else if (player.eyeHelper.CurrentEyeFrame != PlayerEyeHelper.EyeFrame.EyeOpen)
				expr = "Squint";

			return expr;
		}
		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			var player = drawInfo.drawPlayer;
			int AdditionalOffset = (player.width - 20) / 2;
			string expression = GetExpression(player);
			//head
			Vector2 HeadPos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - 31 + AdditionalOffset), (int)(drawInfo.Position.Y - Main.screenPosition.Y - 12));
			Texture2D Head = ModContent.Request<Texture2D>("V2/Items/Voraria/TransformationItems/Baelz/Bae_" + expression).Value;
			Rectangle HeadSourceRect = GetFrameForThisHead(player);
			DrawData HeadDraw = new DrawData(Head, HeadPos, HeadSourceRect, drawInfo.colorMount, player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
			HeadDraw.shader = player.cMount;
			drawInfo.DrawDataCache.Add(HeadDraw);
		}
	}

	public class B_BaeDrawLayer : PlayerDrawLayer
	{
		public int LastIdleFrame = 0;
		public int LastRunFrame = 0;
		public override string Name => "Baelz_Body";
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.NeckAcc, PlayerDrawLayers.Head);
		public Rectangle GetFrameForThisBody(Player player, int weightSize)
		{
			bool OnSelectScreen = Main.gameMenu;
			Rectangle rect = new Rectangle(0, 0, 88, 60);
			Vector2 offsets = WeightSpriteOffset[weightSize];
			rect.Width += (int)offsets.X;
			int idleFrame = LastIdleFrame;
			int runFrame = LastRunFrame;
			if (!Main.gamePaused)
			{
				idleFrame = (int)(Main.GlobalTimeWrappedHourly * 6) % 4;
				runFrame = (int)(Main.GlobalTimeWrappedHourly * 8) % 6;
				LastIdleFrame = idleFrame;
				LastRunFrame = runFrame;
			}
			bool inAir = player.IsAirborne() || player.sleeping.isSleeping || player.sitting.isSitting || player.grappling[0] >= 0;
			if (!inAir)
			{
				if (player.velocity.X != 0) //running
				{
					rect.Y = 60;
					rect.X = 82 * runFrame + (int)offsets.X * runFrame;
				}
				else if (OnSelectScreen && player.legFrame.Y > 100)
				{
					rect.Y = 60;
					rect.X = 82 * runFrame + (int)offsets.X * runFrame;
				}
				else
				{
					rect.Y = 0;
					rect.X = 82 * idleFrame + (int)offsets.X * idleFrame;
				}
			}
			else
			{
				rect.Y = 120;
				if (player.velocity.Y >= 0) rect.X = 82 + (int)offsets.X;
				rect.Height += (int)offsets.Y;
			}
			return rect;
		}
		public Rectangle GetFrameForThisTum(Player player, int tumSize)
		{
			Rectangle rect = new Rectangle(0, 0, 82, 60);
			rect.Width += TumSpriteOffset[tumSize - 1];
			int idleFrame = LastIdleFrame;
			int runFrame = LastRunFrame;
			bool inAir = player.IsAirborne() || player.sleeping.isSleeping || player.sitting.isSitting || player.grappling[0] >= 0;
			if (!inAir)
			{
				if (player.velocity.X != 0) //running
				{
					if (runFrame == 2 || runFrame == 5)
						rect.Y = 60;
					else
						rect.Y = 0;
				}
				else
				{
					if (idleFrame == 1 || idleFrame == 2)
						rect.Y = 120;
					else
						rect.Y = 60;
				}
			}
			else
			{
				rect.Y = 180;
				rect.Height = 70;
			}
			rect.X = 82 * (tumSize - 1);
			rect.X += TumSpriteOffset2[tumSize - 1];
			return rect;
		}
		public bool UseAltTexture(Player player)
		{
			bool inAir = player.IsAirborne() || player.sleeping.isSleeping || player.sitting.isSitting || player.grappling[0] >= 0;
			if (inAir && player.velocity.Y < 0)
				return true;
			return false;
		}
		public static readonly IReadOnlyList<int> TumSpriteOffset =
		[
			0,
			0,
			0,
			0,
			0,
			0,
			4,
			16,
			26,
			48
		];
		public static readonly IReadOnlyList<int> TumSpriteOffset2 =
		[
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			4,
			20,
			48
		];
		public static readonly IReadOnlyList<Vector2> WeightSpriteOffset =
		[
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero,
			new Vector2(0, 8),
			new Vector2(0, 16),
			new Vector2(6, 24)
		];
		public Vector2 GetTumPosition(Player player, Vector2 BodyPosition, int Size)
		{
			Vector2 vect = BodyPosition;
			//42
			//vect.Y += 30;
			if (player.direction == -1)
			{
				vect.X -= TumSpriteOffset[Size - 1] - 6;
			}
			else
			{
			}
			bool inAir = player.IsAirborne() || player.sleeping.isSleeping || player.sitting.isSitting || player.grappling[0] >= 0;
			if (inAir)
			{
				vect.Y += 32;
			}
			return vect;
		}
		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			var player = drawInfo.drawPlayer;
			if (player.AsV2Player().HasTransformation == true)
			{
				int AdditionalOffset = (player.width - 20) / 2;
				var tumSize = Math.Min(BaelzInfo.GetVisualTum(player), BaelzInfo.MaxTumSize);
				var weightSize = BaelzInfo.GetVisualWeightStage(player);
				//body
				Rectangle BodySourceRect = GetFrameForThisBody(player, weightSize);
				if (player.direction == -1)
				{
					AdditionalOffset -= BodySourceRect.Width - 82;
				}
				Vector2 BodyPos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - 31 + AdditionalOffset), (int)(drawInfo.Position.Y - Main.screenPosition.Y - 14));
				Texture2D Body = ModContent.Request<Texture2D>("V2/Items/Voraria/TransformationItems/Baelz/Bae_Weight" + weightSize).Value;
				DrawData BodyDraw = new DrawData(Body, BodyPos, BodySourceRect, drawInfo.colorMount, player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
				BodyDraw.shader = player.cMount;
				drawInfo.DrawDataCache.Add(BodyDraw);

				//tum
				if (tumSize > 0)
				{
					Vector2 TumPos = GetTumPosition(player, BodyPos, tumSize);
					string altText = "";
					if (weightSize >= 6 && UseAltTexture(player))
					{
						altText = "_Alt";
					}
					Texture2D Tum = ModContent.Request<Texture2D>("V2/Items/Voraria/TransformationItems/Baelz/Bae_Tum_Weight" + weightSize + altText).Value;
					Rectangle TumSourceRect = GetFrameForThisTum(player, tumSize);
					DrawData TumDraw = new DrawData(Tum, TumPos, TumSourceRect, drawInfo.colorMount, player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
					TumDraw.shader = player.cMount;
					drawInfo.DrawDataCache.Add(TumDraw);
				}
			}
		}
	}

	public class C_BaeHairDrawLayer : PlayerDrawLayer
	{
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			return drawInfo.drawPlayer.AsV2Player().HasTransformation;
		}
		public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.NeckAcc, PlayerDrawLayers.Head);
		public override bool IsHeadLayer => true;
		public override string Name => "Baelz_Accessories";
		public int LastIdleFrame = 0;
		public int LastRunFrame = 0;
		public static string GetAccessory(Item item)
		{
			string acc = "";
			switch (item.type)
			{
				case ItemID.Sunglasses or ItemID.AviatorSunglasses:
					acc = "Shades";
					break;
			}
			return acc;
		}
		public Rectangle GetFrameForThisHead(Player player)
		{
			bool OnSelectScreen = Main.gameMenu;
			Rectangle rect = new Rectangle(0, 0, 82, 60);
			int idleFrame = LastIdleFrame;
			int runFrame = LastRunFrame;
			if (!Main.gamePaused)
			{
				idleFrame = (int)(Main.GlobalTimeWrappedHourly * 6) % 4;
				runFrame = (int)(Main.GlobalTimeWrappedHourly * 8) % 6;
				LastIdleFrame = idleFrame;
				LastRunFrame = runFrame;
			}
			bool inAir = player.IsAirborne() || player.sleeping.isSleeping || player.sitting.isSitting || player.grappling[0] >= 0;
			if (!inAir)
			{
				if (player.velocity.X != 0) //running
				{
					rect.Y = 60;
					rect.X = 82 * runFrame;
				}
				else if (OnSelectScreen && player.legFrame.Y > 100)
				{
					rect.Y = 60;
					rect.X = 82 * runFrame;
				}
				else
				{
					rect.Y = 0;
					rect.X = 82 * idleFrame;
				}
			}
			else
			{
				rect.Y = 120;
				if (player.velocity.Y >= 0) rect.X = 82;
				rect.Height += 8;
			}
			return rect;
		}
		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			var player = drawInfo.drawPlayer;
			int AdditionalOffset = (player.width - 20) / 2;
			//side hair
			Vector2 HeadPos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - 31 + AdditionalOffset), (int)(drawInfo.Position.Y - Main.screenPosition.Y - 12));
			Rectangle HeadSourceRect = GetFrameForThisHead(player);
			Texture2D Hair = ModContent.Request<Texture2D>("V2/Items/Voraria/TransformationItems/Baelz/Bae_HairThingymajig").Value;
			DrawData HairDraw = new DrawData(Hair, HeadPos, HeadSourceRect, drawInfo.colorMount, player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
			HairDraw.shader = player.cMount;
			drawInfo.DrawDataCache.Add(HairDraw);
			//extra
			string extraAcc = "";
			if (!player.armor[10].IsAir && player.armor[10].type != ItemID.FamiliarShirt)
				extraAcc = GetAccessory(player.armor[10]);
			else if (!player.armor[0].IsAir && player.armor[0].type != ItemID.FamiliarShirt)
				extraAcc = GetAccessory(player.armor[0]);
			if (extraAcc != "")
			{
				Texture2D Hat = ModContent.Request<Texture2D>("V2/Items/Voraria/TransformationItems/Baelz/Bae_Acc_" + extraAcc).Value;
				DrawData HatDraw = new DrawData(Hat, HeadPos, HeadSourceRect, drawInfo.colorMount, player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
				HatDraw.shader = player.cMount;
				drawInfo.DrawDataCache.Add(HatDraw);
			}
		}
	}

	public class BaelzDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			bool isCyan = Main.rand.NextBool();
			int Number = Main.rand.Next(6);
			dust.frame = new Rectangle(isCyan ? 10 : 0, 10 * Number, 10, 10);
			dust.customData = Main.rand.Next(-10, 11) / 10f;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(4, 3), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += (float)dust.customData / 5f;
			dust.customData = (float)dust.customData * 0.95f;
			dust.scale *= 0.99f;
			dust.velocity.X *= 0.95f;
			dust.velocity.Y += 0.08f;
			dust.alpha += 8;
			float light = (255 - dust.alpha) / 255f;

			if (dust.frame.X == 0)
				Lighting.AddLight(dust.position, new Vector3(light, 0, 0));
			else
				Lighting.AddLight(dust.position, new Vector3(0, light, light));

			if (dust.alpha >= 255)
			{
				dust.active = false;
			}

			return false;
		}
	}
	public class BaelzSparkleDustRed : ModDust
	{
		public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/BaelzSparkleDust";
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 6, 6);
			dust.customData = Main.rand.Next(-10, 11) / 10f;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(3, 3), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity.X *= 0.95f;
			dust.rotation += (float)dust.customData / 5f;
			dust.customData = (float)dust.customData * 0.95f;
			dust.scale *= 0.9f;
			float light = dust.scale * 2;

			Lighting.AddLight(dust.position, new Vector3(light, 0, 0));
			if (dust.scale <= 0.05f)
			{
				dust.active = false;
			}

			return false;
		}
	}
	public class BaelzSparkleDustCyan : ModDust
	{
		public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/BaelzSparkleDust";
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(6, 0, 6, 6);
			dust.customData = Main.rand.Next(-10, 11) / 10f;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(3, 3), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity.X *= 0.95f;
			dust.rotation += (float)dust.customData / 5f;
			dust.customData = (float)dust.customData * 0.95f;
			dust.scale *= 0.9f;
			float light = dust.scale * 2;

			Lighting.AddLight(dust.position, new Vector3(0, light, light));
			if (dust.scale <= 0.05f)
			{
				dust.active = false;
			}

			return false;
		}
	}
	public class BaelzSparkleDustYellow : ModDust
	{
		public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/BaelzSparkleDust";
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(12, 0, 6, 6);
			dust.customData = Main.rand.Next(-10, 11) / 10f;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(3, 3), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity.X *= 0.95f;
			dust.rotation += (float)dust.customData / 5f;
			dust.customData = (float)dust.customData * 0.95f;
			dust.scale *= 0.9f;
			float light = dust.scale * 2;

			Lighting.AddLight(dust.position, new Vector3(light, light, 0));
			if (dust.scale <= 0.05f)
			{
				dust.active = false;
			}

			return false;
		}
	}
	public class BaelzSparkleDustBlack : ModDust
	{
		public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/BaelzSparkleDust";
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(18, 0, 6, 6);
			dust.customData = Main.rand.Next(-10, 11) / 10f;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(3, 3), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity.X *= 0.95f;
			dust.rotation += (float)dust.customData / 5f;
			dust.customData = (float)dust.customData * 0.95f;
			dust.scale *= 0.9f;
			if (dust.scale <= 0.05f)
			{
				dust.active = false;
			}

			return false;
		}
	}
	public class DeadBaelz : ModDust
	{
		public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/Baelz_Fuckin_Dies";
		public override void OnSpawn(Dust dust)
		{
			dust.frame = new Rectangle(0, 0, 80, 56);
			dust.noGravity = true;
			dust.customData = Main.rand.Next(-10, 11) / 50f;
		}
		public override bool PreDraw(Dust dust)
		{
			Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.FromNonPremultiplied(255, 255, 255, 255 - dust.alpha), dust.rotation, new Vector2(40, 25.5f), dust.scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity.Y += 0.2f;
			dust.velocity.X *= 0.98f;
			dust.rotation += (float)dust.customData / 5f;
			dust.alpha += 2;
			if (dust.alpha >= 255)
			{
				dust.active = false;
			}

			return false;
		}
	}
	public class BaelzInfo
	{
		public static int GetVisualWeightStage(Player player)
		{
			return Math.Min(
				(int)Math.Floor(2.8 * Math.Sqrt(player.AsPred().BaeTransformation_ExtraWeight) + player.AsPred().BaeTransformation_ExtraWeight / 1.4),
				7
			);
		}
		public static int GetVisualTum(Player player)
		{
			int tummySize = (int)Math.Floor(5.0 * Math.Sqrt(player.AsPred().StomachFullness + player.AsPred().BaeTransformation_ExtraWeight / 3));
			tummySize = (int)Math.Round(tummySize * player.AsPred().PercentBellySizeModifier);
			tummySize += player.AsPred().FlatBellySizeModifier;
			return tummySize;
		}
		public static int MaxTumSize => 10;
		public static double WeightGainRatio => 0.1;
		public static double BaseWeight => 0.9;
	}

	public class BaeTransformationItem : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.BaelzTransformationItem.ActiveName");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.BaelzTransformationItem.Short");
		public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDie";
		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(8, 4);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 34;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(
				gold: 3,
				silver: 15
			);
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, new Vector3(95, 255, 255) * 0.005f);
		}
		public override void UpdateInventory(Player player)
		{
			if (!player.AsV2Player().HasTransformation)
			{
				player.AsV2Player().HasTransformation = true;
				player.AsV2Player().BaeTransformation = true;
				player.AddBuff(ModContent.BuffType<BaelzTransformation>(), V2Utils.SensibleTime(frames: 4));
			}
		}
		public override bool CanRightClick() => true;
		public override void RightClick(Player player)
		{
			bool Favourited = Item.favorited;
			Item.SetDefaults(ModContent.ItemType<InactiveBaeTransformationItem>());
			Item.favorited = Favourited;
			Item.stack++;
			SoundEngine.PlaySound(SoundID.Unlock);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Rat)
				.AddRecipeGroup(V2MasterSystem.CounterweightRecipeGroup)
				.AddIngredient(ItemID.GravitationPotion)
				.AddIngredient(ItemID.Ruby, 6)
				.AddTile(TileID.Anvils)
				.Register();
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.TransformationItems.BaelzTransformationItem",
				new
				{
				}
			);
		}
	}
	public class InactiveBaeTransformationItem : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.BaelzTransformationItem.InactiveName");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.BaelzTransformationItem.Short");
		public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDieInactive";

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 34;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(
				gold: 3,
				silver: 15
			);
		}
		public override bool CanRightClick() => true;
		public override void RightClick(Player player)
		{
			bool Favourited = Item.favorited;
			Item.SetDefaults(ModContent.ItemType<BaeTransformationItem>());
			Item.favorited = Favourited;
			Item.stack++;
			SoundEngine.PlaySound(SoundID.Unlock);
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.TransformationItems.BaelzTransformationItem",
				new
				{

				}
			);
		}
	}
}
