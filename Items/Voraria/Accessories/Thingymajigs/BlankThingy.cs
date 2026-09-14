using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Accessories.Informational;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Voraria.Accessories.Thingymajigs
{
	public class BlankThingy : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Thingymajigs.FullNameForExternalUse.Blank");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.Thingymajigs.Blank");

		private static Asset<Texture2D> MainTexture;
		private static Asset<Texture2D> OutlineTexture;

		public override void Load()
		{
			MainTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Accessories/Thingymajigs/BlankThingy");
			OutlineTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Accessories/Thingymajigs/BlankThingyOutline");
		}

		public override void Unload()
		{
			MainTexture = null;
			OutlineTexture = null;
		}

		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			Item.ResearchUnlockCount = 1;
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, new Vector3(255,255,255) * 0.005f);
		}
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 38;
			Item.maxStack = Item.CommonMaxStack;

			Item.value = Item.buyPrice(0, 2, 50);
			Item.rare = ItemRarityID.Orange;
		}

		public static void DrawThingymajig(SpriteBatch spriteBatch, Vector2 position, float scale, float rotation,
			Asset<Texture2D> mainTexture, Asset<Texture2D> outlineTexture, Color colorA, Color colorB, bool inInventory = false)
		{
			Main.spriteBatch.End();
			if (inInventory)
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
			else
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			Vector2 Center = new Vector2(mainTexture.Value.Bounds.Width / 2,
			mainTexture.Value.Bounds.Height / 2);
			float angle = (Main.GlobalTimeWrappedHourly * 2) % 360;
			float ColorSequence = (Main.GlobalTimeWrappedHourly % 8) / 4f;
			if (ColorSequence > 1)
				ColorSequence = 2 - ColorSequence;

			Vector2 Offset = new Vector2((float)(1 * Math.Cos(angle) - 1 * Math.Sin(angle)), (float)(1 * Math.Cos(angle) + 1 * Math.Sin(angle)));

			Color actualColor = new Color(
				(int)Math.Round(colorA.R * ColorSequence + colorB.R * (1 - ColorSequence))
				, (int)Math.Round(colorA.G * ColorSequence + colorB.G * (1 - ColorSequence))
				, (int)Math.Round(colorA.B * ColorSequence + colorB.B * (1 - ColorSequence)));

			Color BGColor = new Color(actualColor.R, actualColor.G, actualColor.B, 100);

			if (inInventory)
			{
				Offset /= 1.25f;
				BGColor = new Color(BGColor.R, BGColor.G, BGColor.B, 65);
			}

			spriteBatch.Draw(outlineTexture.Value
				, position + Offset * 3, outlineTexture.Value.Bounds, BGColor, rotation, Center, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(outlineTexture.Value
				, position + Offset * -3, outlineTexture.Value.Bounds, BGColor, rotation, Center, scale, SpriteEffects.None, 0f);

			spriteBatch.Draw(outlineTexture.Value
				, position, outlineTexture.Value.Bounds, actualColor, rotation, Center, scale, SpriteEffects.None, 0f);

			spriteBatch.Draw(mainTexture.Value
				, position, mainTexture.Value.Bounds, Color.White, rotation, Center, scale, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			if (inInventory)
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
			else
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			DrawThingymajig(spriteBatch, position, scale, 0f, MainTexture, OutlineTexture, new Color(200, 200, 200), new Color(125, 125, 150), true);
			return false;
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			DrawThingymajig(spriteBatch, Item.Center - Main.screenPosition, scale, rotation, MainTexture, OutlineTexture, new Color(200, 200, 200), new Color(125, 125, 150));
			return false;
		}
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (Main.HoverItem.type == Item.type || Main.guideItem == Item || new Rectangle((int)position.X - frame.Width / 2 - 8, (int)position.Y - frame.Height / 2 - 8, frame.Width + 16, frame.Height + 16).Contains(Main.MouseScreen.ToPoint()) || Item == Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem])
				return;
			Item.SetNameOverride(Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Thingymajigs.Name.Blank")
				+ " " + Language.GetText("Mods.V2.ObjectNames." + Main.rand.Next(1, 36).ToString()).Value);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Accessories.Thingymajigs.Blank",
				new
				{

				}
			);
		}
	}
}