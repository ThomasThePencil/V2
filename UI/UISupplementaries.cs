using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using tModPorter;

namespace V2.UI
{
	public static class UISupplementaries
	{
		/// <summary>
		/// Draws a hovering tooltip with a tile-based bounding box.<br/>
		/// This exists to replicate the functions of <see cref="UICommon.TooltipMouseText"/>, except way better, because it:<br/>
		/// - Makes use of <see cref="TextSnippet"/>s instead of <see cref="string"/>s, preserving the effects of chat tags.<br/>
		/// - Allows you to choose what bounding box is utilized, unlike the original method.<br/>
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="boundingBox"></param>
		/// <param name="boundingBoxTileSize"></param>
		/// <param name="boundingBoxDrawColor"></param>
		/// <param name="text"></param>
		/// <param name="baseTextColor"></param>
		/// <param name="baseShadowColor"></param>
		/// <param name="maxWidth"></param>
		/// <param name="maxLines"></param>
		public static void DrawMouseTooltipWithTilesBoundingBox(SpriteBatch spriteBatch, Texture2D boundingBox, float boundingBoxTileSize,
			Color boundingBoxDrawColor, string text, Color baseTextColor, Color baseShadowColor, float maxWidth, int maxLines)
		{
			List<List<TextSnippet>> smartWrappedText = Utils.WordwrapStringSmart(
				text,
				Color.White * (Main.mouseTextColor / 255f),
				FontAssets.MouseText.Value,
				(int)maxWidth,
				maxLines
			);

			Vector2 largestLineSize = Vector2.Zero;
			foreach (List<TextSnippet> line in smartWrappedText)
			{
				Vector2 lineMeasurement = ChatManager.GetStringSize(FontAssets.MouseText.Value, [.. line], Vector2.One);
				// as the vertical (Y) measurement will always be the same, we only care about the horizontal length of the lines compared
				if (lineMeasurement.X > largestLineSize.X)
					largestLineSize = lineMeasurement;
			}

			Vector2 mouseOffset = new Vector2(14, 8);
			int maxWidthInTiles = (int)Math.Ceiling(maxWidth / boundingBoxTileSize);
			int widthInTiles = Math.Min((int)Math.Ceiling((largestLineSize.X + 20) / boundingBoxTileSize), maxWidthInTiles);
			float actualProjectedWidth = (float)widthInTiles * boundingBoxTileSize;
			int screenWidth = Main.screenWidth;
			int screenHeight = Main.screenHeight;
			if ((float)mouseOffset.X + Main.MouseScreen.X + (float)actualProjectedWidth > (float)screenWidth)
				mouseOffset.X = (int)((float)screenWidth - Main.MouseScreen.X - (float)actualProjectedWidth);

			if ((float)mouseOffset.Y + Main.MouseScreen.Y + (float)actualProjectedWidth > (float)screenHeight)
				mouseOffset.Y = (int)((float)screenHeight - Main.MouseScreen.Y - (float)actualProjectedWidth);

			for (int y = 0; y <= smartWrappedText.Count; y++)
			{
				for (int x = 0; x <= widthInTiles; x++)
				{
					int targetSheetTileX = x == 0 ? 0 : (x < widthInTiles ? (int)boundingBoxTileSize + 2 : (int)(boundingBoxTileSize * 2) + 4);
					int targetSheetTileY = y == 0 ? 0 : (y < smartWrappedText.Count ? 32 : 64);
					Main.spriteBatch.Draw(
						boundingBox,
						Main.MouseScreen + new Vector2(
							10 + (30 * x),
							10 + (30 * y)
						),
						new Rectangle(
							targetSheetTileX,
							targetSheetTileY,
							30,
							30
						),
						boundingBoxDrawColor,
						0f,
						default,
						1f,
						SpriteEffects.None,
						0f
					);
				}
			}
			int lineBeingDrawn = 0;
			foreach (List<TextSnippet> line in smartWrappedText)
			{
				ChatManager.DrawColorCodedStringWithShadow(
					spriteBatch,
					FontAssets.MouseText.Value,
					[.. line],
					Main.MouseScreen + new Vector2(12, 12 + (30 * lineBeingDrawn)),
					0f,
					baseTextColor,
					baseShadowColor,
					Vector2.Zero,
					Vector2.One,
					out _
				);
				lineBeingDrawn++;
			}
			Main.mouseText = true;
		}
	}
}
