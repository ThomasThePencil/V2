using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Gamepad;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.Stylist;
using V2.PlayerHandling;

namespace V2.UI
{
	public static partial class UIOverrides
	{
		public static void DrawInterface_21_HairWindow(NPC stylist)
		{
			if (!Main.hairWindow)
				return;

			if (Main.npcChatText != "" || Main.playerInventory || Main.player[Main.myPlayer].chest != -1 || Main.npcShop != 0 || Main.player[Main.myPlayer].talkNPC == -1 || Main.InGuideCraftMenu)
			{
				Main.CancelHairWindow();
				return;
			}

			// I don't like havin' to do this but the relevant fields are private and I can't really weasel a PR to tML in that makes them not so here we go
			FieldInfo grabColorSliderInfo = typeof(Main).GetField("grabColorSlider", BindingFlags.NonPublic | BindingFlags.Instance);
			int GetGrabColorSlider() => (int)grabColorSliderInfo.GetValue(Main.instance);
			void SetGrabColorSlider(int value) => grabColorSliderInfo.SetValue(Main.instance, value);

			Main.Hairstyles.UpdateUnlocks();
			int hairstyleCount = Main.Hairstyles.AvailableHairstyles.Count;
			int hairWindowYCoordA = Main.screenHeight / 2 + 60;
			int hairWindowXCoordA = Main.screenWidth / 2 - TextureAssets.HairStyleBack.Width() / 2;
			int hairWindowYCoordB = hairWindowYCoordA + 42;
			int hairWindowXCoordB = hairWindowXCoordA + 22;
			int hairWindowXCoordC = hairWindowXCoordA + 234;
			int hairWindowYCoordC = hairWindowYCoordA + 18;
			Main.selColor = Main.player[Main.myPlayer].hairColor;
			Main.spriteBatch.Draw(TextureAssets.HairStyleBack.Value, new Vector2(hairWindowXCoordA, hairWindowYCoordA), new Rectangle(0, 0, TextureAssets.HairStyleBack.Width(), TextureAssets.HairStyleBack.Height()), new Color(200, 200, 200, 200), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			if (new Rectangle(hairWindowXCoordA, hairWindowYCoordA, TextureAssets.HairStyleBack.Width(), TextureAssets.HairStyleBack.Height()).Contains(Main.MouseScreen.ToPoint()))
			{
				int num7 = PlayerInput.ScrollWheelDelta / 120;
				num7 = -num7;
				int num8 = Math.Sign(num7);
				while (num7 != 0)
				{
					if (num7 < 0)
					{
						Main.hairStart -= 5;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
					else
					{
						Main.hairStart += 5;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}

					num7 -= num8;
				}
			}

			if (Main.mouseX > hairWindowXCoordA && Main.mouseX < hairWindowXCoordA + TextureAssets.HairStyleBack.Width() && Main.mouseY > hairWindowYCoordA && Main.mouseY < hairWindowYCoordA + TextureAssets.HairStyleBack.Height())
				Main.player[Main.myPlayer].mouseInterface = true;

			int num9 = hairWindowXCoordC - 18;
			int num10 = hairWindowYCoordC + 74;
			if (Main.hairStart > 1)
			{
				if (Main.mouseX >= num9 && Main.mouseX <= num9 + TextureAssets.CraftUpButton.Width() && Main.mouseY >= num10 && Main.mouseY <= num10 + TextureAssets.CraftUpButton.Height())
				{
					Main.player[Main.myPlayer].mouseInterface = true;
					if (Main.mouseLeftRelease && Main.mouseLeft)
					{
						Main.mouseLeftRelease = false;
						Main.hairStart -= 15;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}

				Main.spriteBatch.Draw(TextureAssets.ScrollLeftButton.Value, new Vector2(num9, num10), new Rectangle(0, 0, TextureAssets.CraftUpButton.Width(), TextureAssets.CraftUpButton.Height()), new Color(200, 200, 200, 200), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}

			if (Main.hairStart + 15 < hairstyleCount)
			{
				num9 += 296;
				if (Main.mouseX >= num9 && Main.mouseX <= num9 + TextureAssets.CraftUpButton.Width() && Main.mouseY >= num10 && Main.mouseY <= num10 + TextureAssets.CraftUpButton.Height())
				{
					Main.player[Main.myPlayer].mouseInterface = true;
					if (Main.mouseLeftRelease && Main.mouseLeft)
					{
						Main.mouseLeftRelease = false;
						Main.hairStart += 15;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}

				Main.spriteBatch.Draw(TextureAssets.ScrollRightButton.Value, new Vector2(num9, num10), new Rectangle(0, 0, TextureAssets.CraftUpButton.Width(), TextureAssets.CraftUpButton.Height()), new Color(200, 200, 200, 200), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}

			if (Main.hairStart + 15 >= hairstyleCount)
				Main.hairStart = hairstyleCount - 15;

			if (Main.hairStart < 0)
				Main.hairStart = 0;

			int num17 = (Main.mouseTextColor * 2 + 255) / 3;
			Color color = new Color(num17, (int)((double)num17 / 1.1), num17 / 2, num17);
			int normalCutPrice = 0;
			int gutCutPrice = Item.buyPrice(gold: 10);

			int savingsLocationX = hairWindowXCoordA + 18;
			int savingsLocationY = hairWindowYCoordA + 86;

			DrawNormalCutOption();
			DrawCancelOption();
			DrawGutCutOption();

			void DrawNormalCutOption()
			{
				FieldInfo oldHairStyleInfo = typeof(Main).GetField("oldHairStyle", BindingFlags.NonPublic | BindingFlags.Static);
				int oldHairStyle = (int)oldHairStyleInfo.GetValue(null);
				if (oldHairStyle != Main.player[Main.myPlayer].hair)
					normalCutPrice = (Main.player[Main.myPlayer].hair <= 51) ? (normalCutPrice + 20000) : (normalCutPrice + 200000);

				FieldInfo oldHairColorInfo = typeof(Main).GetField("oldHairColor", BindingFlags.NonPublic | BindingFlags.Static);
				Color oldHairColor = (Color)oldHairColorInfo.GetValue(null);
				if (oldHairColor != Main.player[Main.myPlayer].hairColor)
					normalCutPrice += 20000;

				normalCutPrice = (int)((double)normalCutPrice * Main.player[Main.myPlayer].currentShoppingSettings.PriceAdjustment);
				normalCutPrice = (int)Math.Round((float)normalCutPrice / 10000f) * 10000;
				string normalCutText = "";
				string normalCutTextCopy = "";
				int normalCutPlatinum = 0;
				int normalCutGold = 0;
				int normalCutSilver = 0;
				int normalCutCopper = 0;
				int normalCutPriceLeft = normalCutPrice;
				_ = 0;
				if (normalCutPriceLeft < 0)
					normalCutPriceLeft = 0;

				normalCutPrice = normalCutPriceLeft;
				if (normalCutPriceLeft >= 1000000)
				{
					normalCutPlatinum = normalCutPriceLeft / 1000000;
					normalCutPriceLeft -= normalCutPlatinum * 1000000;
				}

				if (normalCutPriceLeft >= 10000)
				{
					normalCutGold = normalCutPriceLeft / 10000;
					normalCutPriceLeft -= normalCutGold * 10000;
				}

				if (normalCutPriceLeft >= 100)
				{
					normalCutSilver = normalCutPriceLeft / 100;
					normalCutPriceLeft -= normalCutSilver * 100;
				}

				if (normalCutPriceLeft >= 1)
					normalCutCopper = normalCutPriceLeft;

				if (normalCutPlatinum > 0)
					normalCutTextCopy = normalCutTextCopy + normalCutPlatinum + " " + Lang.inter[15].Value + " ";

				if (normalCutGold > 0)
					normalCutTextCopy = normalCutTextCopy + normalCutGold + " " + Lang.inter[16].Value + " ";

				if (normalCutSilver > 0)
					normalCutTextCopy = normalCutTextCopy + normalCutSilver + " " + Lang.inter[17].Value + " ";

				if (normalCutCopper > 0)
					normalCutTextCopy = normalCutTextCopy + normalCutCopper + " " + Lang.inter[18].Value + " ";

				normalCutText = Language.GetTextValue("GameUI.BuyWithValue", normalCutTextCopy);
				if (normalCutPrice == 0)
					normalCutText = Language.GetTextValue("GameUI.Buy");

				float scale = 0.9f;
				string text3 = normalCutText;
				int normalCutLocationX = hairWindowXCoordA + 18;
				int normalCutLocationY = hairWindowYCoordA + 156;
				bool flag = false;
				if (normalCutPrice > 0)
					ItemSlot.DrawSavings(Main.spriteBatch, savingsLocationX, savingsLocationY, horizontal: true);

				if (normalCutPrice > 0 && Main.mouseX > normalCutLocationX && (float)Main.mouseX < (float)normalCutLocationX + FontAssets.MouseText.Value.MeasureString(text3).X && Main.mouseY > normalCutLocationY && (float)Main.mouseY < (float)normalCutLocationY + FontAssets.MouseText.Value.MeasureString(text3).Y)
				{
					flag = true;
					scale = 1.1f;
					if (!Main.npcChatFocus1)
						SoundEngine.PlaySound(SoundID.MenuTick);

					Main.npcChatFocus1 = true;
					Main.player[Main.myPlayer].releaseUseItem = false;
				}
				else
				{
					if (Main.npcChatFocus1)
						SoundEngine.PlaySound(SoundID.MenuTick);

					Main.npcChatFocus1 = false;
				}

				Vector2 vector = FontAssets.MouseText.Value.MeasureString(text3);
				vector *= 0.5f;
				UILinkPointNavigator.SetPosition(2603, new Vector2(normalCutLocationX, normalCutLocationY) + vector);
				for (int i = 0; i < 5; i++)
				{
					int num20 = normalCutLocationX;
					int num21 = normalCutLocationY;
					Color color2 = Color.Black;
					if (flag)
						color2 = Color.Brown;

					if (i == 0)
						num20 -= 2;

					if (i == 1)
						num20 += 2;

					if (i == 2)
						num21 -= 2;

					if (i == 3)
						num21 += 2;

					if (i == 4)
						color2 = ((normalCutPrice != 0) ? color : new Color(100, 100, 100));

					Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text3, new Vector2((float)num20 + vector.X, (float)num21 + vector.Y), color2, 0f, vector, scale, SpriteEffects.None, 0f);
				}
			}

			void DrawCancelOption()
			{
				float scale = 0.9f;
				string cancelText = Language.GetTextValue("GameUI.Cancel");
				int num18 = hairWindowXCoordA + 148;
				int num19 = hairWindowYCoordA + 156;
				bool flag = false;
				if (Main.mouseX > num18 && (float)Main.mouseX < (float)num18 + FontAssets.MouseText.Value.MeasureString(cancelText).X && Main.mouseY > num19 && (float)Main.mouseY < (float)num19 + FontAssets.MouseText.Value.MeasureString(cancelText).Y)
				{
					flag = true;
					scale = 1.1f;
					if (!Main.npcChatFocus2)
						SoundEngine.PlaySound(SoundID.MenuTick);

					Main.npcChatFocus2 = true;
					Main.player[Main.myPlayer].releaseUseItem = false;
				}
				else
				{
					if (Main.npcChatFocus2)
						SoundEngine.PlaySound(SoundID.MenuTick);

					Main.npcChatFocus2 = false;
				}

				Vector2 cancelVector = FontAssets.MouseText.Value.MeasureString(cancelText);
				cancelVector *= 0.5f;
				UILinkPointNavigator.SetPosition(2604, new Vector2(num18, num19) + cancelVector);
				for (int j = 0; j < 5; j++)
				{
					int num22 = num18;
					int num23 = num19;
					Color color3 = Color.Black;
					if (flag)
						color3 = Color.Brown;

					if (j == 0)
						num22 -= 2;

					if (j == 1)
						num22 += 2;

					if (j == 2)
						num23 -= 2;

					if (j == 3)
						num23 += 2;

					if (j == 4)
						color3 = color;

					Main.spriteBatch.DrawString(FontAssets.MouseText.Value, cancelText, new Vector2((float)num22 + cancelVector.X, (float)num23 + cancelVector.Y), color3, 0f, cancelVector, scale, SpriteEffects.None, 0f);
				}
			}

			void DrawGutCutOption()
			{
				gutCutPrice = (int)((double)gutCutPrice * Main.player[Main.myPlayer].currentShoppingSettings.PriceAdjustment);
				gutCutPrice = (int)Math.Round((float)gutCutPrice / 100000f) * 100000;
				string gutCutText = "";
				string gutCutTextCopy = "";
				int gutCutPlatinum = 0;
				int gutCutGold = 0;
				int gutCutSilver = 0;
				int gutCutCopper = 0;
				int gutCutPriceLeft = gutCutPrice;
				if (gutCutPriceLeft < 0)
					gutCutPriceLeft = 0;

				gutCutPrice = gutCutPriceLeft;
				if (gutCutPriceLeft >= 1000000)
				{
					gutCutPlatinum = gutCutPriceLeft / 1000000;
					gutCutPriceLeft -= gutCutPlatinum * 1000000;
				}

				if (gutCutPriceLeft >= 10000)
				{
					gutCutGold = gutCutPriceLeft / 10000;
					gutCutPriceLeft -= gutCutGold * 10000;
				}

				if (gutCutPriceLeft >= 100)
				{
					gutCutSilver = gutCutPriceLeft / 100;
					gutCutPriceLeft -= gutCutSilver * 100;
				}

				if (gutCutPriceLeft >= 1)
					gutCutCopper = gutCutPriceLeft;

				if (gutCutPlatinum > 0)
					gutCutTextCopy = gutCutTextCopy + gutCutPlatinum + " " + Lang.inter[15].Value + " ";

				if (gutCutGold > 0)
					gutCutTextCopy = gutCutTextCopy + gutCutGold + " " + Lang.inter[16].Value + " ";

				if (gutCutSilver > 0)
					gutCutTextCopy = gutCutTextCopy + gutCutSilver + " " + Lang.inter[17].Value + " ";

				if (gutCutCopper > 0)
					gutCutTextCopy = gutCutTextCopy + gutCutCopper + " " + Lang.inter[18].Value + " ";

				gutCutText = Language.GetTextValueWith(
					"Mods.V2.Menu.MiscVanilla.HairWindowGutCut",
					new
					{
						Price = gutCutTextCopy
					}
				);
				if (gutCutPrice == 0)
					gutCutText = Language.GetTextValue("Mods.V2.Menu.MiscVanilla.HairWindowGutCutFree");

				int num17V = (Main.mouseTextColor * 2 + 255) / 3;
				Color colorV = new Color(num17, (int)((double)num17V / 1.1), num17V / 2, num17V);
				float scaleV = 0.9f;
				string gutCutText3 = gutCutText;
				int gutCutLocationX = hairWindowXCoordA + (gutCutPrice > 0 ? 40 : 70);
				int gutCutLocationY = hairWindowYCoordA + 16;
				bool flagV = false;
				if (normalCutPrice == 0 && gutCutPrice > 0)
					ItemSlot.DrawSavings(Main.spriteBatch, savingsLocationX, savingsLocationY, horizontal: true);

				if (Main.mouseX > gutCutLocationX && (float)Main.mouseX < (float)gutCutLocationX + FontAssets.MouseText.Value.MeasureString(gutCutText3).X && Main.mouseY > gutCutLocationY && (float)Main.mouseY < (float)gutCutLocationY + FontAssets.MouseText.Value.MeasureString(gutCutText3).Y)
				{
					flagV = true;
					scaleV = 1.1f;
					if (!Main.npcChatFocus3)
						SoundEngine.PlaySound(SoundID.MenuTick);

					Main.npcChatFocus3 = true;
					Main.player[Main.myPlayer].releaseUseItem = false;
				}
				else
				{
					if (Main.npcChatFocus3)
						SoundEngine.PlaySound(SoundID.MenuTick);

					Main.npcChatFocus3 = false;
				}

				Vector2 gutCutVector = FontAssets.MouseText.Value.MeasureString(gutCutText3);
				gutCutVector *= 0.5f;
				// I'll figure this out later
				// UILinkPointNavigator.SetPosition(88000, new Vector2(gutCutLocationX, gutCutLocationY) + gutCutVector);
				for (int i = 0; i < 5; i++)
				{
					int num20 = gutCutLocationX;
					int num21 = gutCutLocationY;
					Color color2 = Color.Black;
					if (flagV)
						color2 = Color.Brown;

					if (i == 0)
						num20 -= 2;

					if (i == 1)
						num20 += 2;

					if (i == 2)
						num21 -= 2;

					if (i == 3)
						num21 += 2;

					if (i == 4)
						color2 = ((gutCutPrice != 0) ? colorV : new Color(100, 100, 100));

					Main.spriteBatch.DrawString(FontAssets.MouseText.Value, gutCutText3, new Vector2((float)num20 + gutCutVector.X, (float)num21 + gutCutVector.Y), color2, 0f, gutCutVector, scaleV, SpriteEffects.None, 0f);
				}
			}

			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				Main.mouseLeftRelease = false;
				if (Main.npcChatFocus1)
				{
					if (Main.player[Main.myPlayer].BuyItem(normalCutPrice))
					{
						Main.BuyHairWindow();
						return;
					}
				}
				else if (Main.npcChatFocus2)
				{
					Main.CancelHairWindow();
					return;
				}
				else if (Main.npcChatFocus3)
				{
					if (PredNPC.CanSwallow(stylist, Main.LocalPlayer))
					{
						if (Main.player[Main.myPlayer].BuyItem(gutCutPrice))
						{
							SoundEngine.PlaySound(SoundID.Coins);
							PredNPC.SwallowWithTextIfApplicable(
								stylist,
								Main.LocalPlayer,
								"Sure thing, hun! One Gut Cut experience, comin' right up! I hope you enjoy your new acid-worn look!\n"
							  + "[c/7F7F7F:<After preparing for a moment and giving you a friendly smile, " + stylist.GivenName + " steadily guides you down her throat, headfirst. She gives a pleasant hum as you settle into her stomach, the acids getting to work on your scalp.>]\n"
							  + "De~licious! Now, just tell me when you're satisfied with your Gut Cut, and remember: I run a STRICT no-refunds policy for these!"
							);
						}
						else
						{
							PredNPC.SetChatboxText(
								stylist,
								Main.LocalPlayer,
								"Sorry, hun, but you don't seem to have enough on you right now. I can't just give out the Gut Cut experience for free, y'know!"
							);
						}
					}
					else
					{
						PredNPC.SetChatboxText(
							stylist,
							Main.LocalPlayer,
							"Sorry, hun, but I just don't have enough room right now. Tell you what, though; come back later, and I'll have a snazzy new acid-worn cut with your name on it!"
						);
					}
				}
			}

			if (!Main.mouseLeft)
			{
				SetGrabColorSlider(0);
				Main.blockMouse = false;
			}

			int num24 = 167;
			Vector3 vector2 = Main.rgbToHsl(Main.selColor);
			float num25 = vector2.X;
			float num26 = vector2.Y;
			float z = vector2.Z;
			float num27 = (float)(int)Main.selColor.A / 255f;
			if (Main.hBar == -1f || Main.sBar == -1f || Main.lBar == -1f || Main.aBar == -1f)
			{
				Main.hBar = num25;
				Main.sBar = num26;
				Main.lBar = z;
				Main.aBar = (float)(int)Main.selColor.A / 255f;
			}
			else
			{
				num25 = Main.hBar;
				num26 = Main.sBar;
				z = Main.lBar;
				Main.aBar = num27;
			}

			UILinkPointNavigator.SetPosition(2600, new Vector2(hairWindowXCoordB, hairWindowYCoordB) + TextureAssets.Hue.Value.Size() / 2f);
			Main.spriteBatch.Draw(TextureAssets.Hue.Value, new Vector2(hairWindowXCoordB, hairWindowYCoordB), Color.White);
			if ((Main.mouseX > hairWindowXCoordB - 4 && Main.mouseX < hairWindowXCoordB + TextureAssets.Hue.Width() + 4 && Main.mouseY > hairWindowYCoordB - 4 && Main.mouseY < hairWindowYCoordB + TextureAssets.Hue.Height() + 4) || GetGrabColorSlider() == 1)
				Main.spriteBatch.Draw(TextureAssets.ColorHighlight.Value, new Vector2(hairWindowXCoordB, hairWindowYCoordB), Main.OurFavoriteColor);

			Main.spriteBatch.Draw(TextureAssets.ColorSlider.Value, new Vector2((float)hairWindowXCoordB + (float)(TextureAssets.Hue.Width() - 2) * Main.hBar - (float)(TextureAssets.ColorSlider.Width() / 2), hairWindowYCoordB - TextureAssets.ColorSlider.Height() / 2 + TextureAssets.Hue.Height() / 2), Color.White);
			if (((Main.mouseX > hairWindowXCoordB - 4 && Main.mouseX < hairWindowXCoordB + TextureAssets.Hue.Width() + 4 && Main.mouseY > hairWindowYCoordB - 4 && Main.mouseY < hairWindowYCoordB + TextureAssets.Hue.Height() + 4) || GetGrabColorSlider() == 1) && Main.mouseLeft && !Main.blockMouse)
			{
				SetGrabColorSlider(1);
				num25 = Main.mouseX - hairWindowXCoordB;
				num25 /= (float)TextureAssets.Hue.Width();
				if (num25 < 0f)
					num25 = 0f;

				if (num25 > 1f)
					num25 = 1f;

				Main.hBar = num25;
			}

			hairWindowYCoordB += 26;
			UILinkPointNavigator.SetPosition(2601, new Vector2(hairWindowXCoordB, hairWindowYCoordB) + TextureAssets.ColorBar.Value.Size() / 2f);
			Main.spriteBatch.Draw(TextureAssets.ColorBar.Value, new Vector2(hairWindowXCoordB, hairWindowYCoordB), Color.White);
			for (int k = 0; k <= num24; k++)
			{
				float saturation = (float)k / (float)num24;
				Color color4 = Main.hslToRgb(num25, saturation, z);
				Main.spriteBatch.Draw(TextureAssets.ColorBlip.Value, new Vector2(hairWindowXCoordB + k + 5, hairWindowYCoordB + 4), color4);
			}

			if ((Main.mouseX > hairWindowXCoordB - 4 && Main.mouseX < hairWindowXCoordB + TextureAssets.Hue.Width() + 4 && Main.mouseY > hairWindowYCoordB - 4 && Main.mouseY < hairWindowYCoordB + TextureAssets.Hue.Height() + 4) || GetGrabColorSlider() == 2)
				Main.spriteBatch.Draw(TextureAssets.ColorHighlight.Value, new Vector2(hairWindowXCoordB, hairWindowYCoordB), Main.OurFavoriteColor);

			Main.spriteBatch.Draw(TextureAssets.ColorSlider.Value, new Vector2((float)hairWindowXCoordB + (float)(TextureAssets.Hue.Width() - 2) * Main.sBar - (float)(TextureAssets.ColorSlider.Width() / 2), hairWindowYCoordB - TextureAssets.ColorSlider.Height() / 2 + TextureAssets.Hue.Height() / 2), Color.White);
			if (((Main.mouseX > hairWindowXCoordB - 4 && Main.mouseX < hairWindowXCoordB + TextureAssets.Hue.Width() + 4 && Main.mouseY > hairWindowYCoordB - 4 && Main.mouseY < hairWindowYCoordB + TextureAssets.Hue.Height() + 4) || GetGrabColorSlider() == 2) && Main.mouseLeft && !Main.blockMouse)
			{
				SetGrabColorSlider(2);
				num26 = Main.mouseX - hairWindowXCoordB;
				num26 /= (float)TextureAssets.Hue.Width();
				if (num26 < 0f)
					num26 = 0f;

				if (num26 > 1f)
					num26 = 1f;

				Main.sBar = num26;
			}

			hairWindowYCoordB += 26;
			UILinkPointNavigator.SetPosition(2602, new Vector2(hairWindowXCoordB, hairWindowYCoordB) + TextureAssets.ColorBar.Value.Size() / 2f);
			Main.spriteBatch.Draw(TextureAssets.ColorBar.Value, new Vector2(hairWindowXCoordB, hairWindowYCoordB), Color.White);
			float num28 = 0.15f;
			for (int l = 0; l <= num24; l++)
			{
				float luminosity = (float)l / (float)num24;
				Color color5 = Main.hslToRgb(num25, num26, luminosity);
				Main.spriteBatch.Draw(TextureAssets.ColorBlip.Value, new Vector2(hairWindowXCoordB + l + 5, hairWindowYCoordB + 4), color5);
			}

			if ((Main.mouseX > hairWindowXCoordB - 4 && Main.mouseX < hairWindowXCoordB + TextureAssets.Hue.Width() + 4 && Main.mouseY > hairWindowYCoordB - 4 && Main.mouseY < hairWindowYCoordB + TextureAssets.Hue.Height() + 4) || GetGrabColorSlider() == 3)
				Main.spriteBatch.Draw(TextureAssets.ColorHighlight.Value, new Vector2(hairWindowXCoordB, hairWindowYCoordB), Main.OurFavoriteColor);

			Main.spriteBatch.Draw(TextureAssets.ColorSlider.Value, new Vector2((float)hairWindowXCoordB + (float)(TextureAssets.Hue.Width() - 2) * ((Main.lBar - num28) / (1f - num28)) - (float)(TextureAssets.ColorSlider.Width() / 2), hairWindowYCoordB - TextureAssets.ColorSlider.Height() / 2 + TextureAssets.Hue.Height() / 2), Color.White);
			if (((Main.mouseX > hairWindowXCoordB - 4 && Main.mouseX < hairWindowXCoordB + TextureAssets.Hue.Width() + 4 && Main.mouseY > hairWindowYCoordB - 4 && Main.mouseY < hairWindowYCoordB + TextureAssets.Hue.Height() + 4) || GetGrabColorSlider() == 3) && Main.mouseLeft && !Main.blockMouse)
			{
				SetGrabColorSlider(3);
				z = Main.mouseX - hairWindowXCoordB;
				z /= (float)TextureAssets.Hue.Width();
				if (z < 0f)
					z = 0f;

				if (z > 1f)
					z = 1f;

				z = (Main.lBar = z * (1f - num28) + num28);
			}

			Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
			Main.player[Main.myPlayer].hairColor = Main.selColor;
			int num29 = hairWindowXCoordC;
			int num30 = hairWindowYCoordC;
			_ = Main.hairStart;
			int num31 = 0;
			int num32 = 0;
			for (int m = 0; m < 15; m++)
			{
				int num33 = Main.Hairstyles.AvailableHairstyles[Main.hairStart + m];
				UILinkPointNavigator.SetPosition(2605 + m, new Vector2(num29, num30) + TextureAssets.InventoryBack.Value.Size() * 0.75f);
				if (Main.player[Main.myPlayer].hair == num33)
					Main.spriteBatch.Draw(TextureAssets.InventoryBack14.Value, new Vector2(num29, num30), new Microsoft.Xna.Framework.Rectangle(0, 0, TextureAssets.InventoryBack.Width(), TextureAssets.InventoryBack.Height()), new Color(200, 200, 200, 200), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				else
					Main.spriteBatch.Draw(TextureAssets.InventoryBack8.Value, new Vector2(num29, num30), new Microsoft.Xna.Framework.Rectangle(0, 0, TextureAssets.InventoryBack.Width(), TextureAssets.InventoryBack.Height()), new Color(200, 200, 200, 200), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

				if (Main.mouseX > num29 && Main.mouseX < num29 + TextureAssets.InventoryBack.Width() && Main.mouseY > num30 && Main.mouseY < num30 + TextureAssets.InventoryBack.Height())
				{
					Asset<Texture2D> asset = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
					Utils.DrawSplicedPanel(Main.spriteBatch, asset.Value, num29, num30, TextureAssets.InventoryBack.Width(), TextureAssets.InventoryBack.Height(), asset.Width() / 2 - 1, asset.Width() / 2 - 1, asset.Height() / 2 - 1, asset.Height() / 2 - 1, Main.OurFavoriteColor);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Main.mouseLeftRelease = false;
						Main.player[Main.myPlayer].hair = num33;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}

				Main.instance.LoadHair(num33);
				float x = num29 + TextureAssets.InventoryBack.Width() / 2 - TextureAssets.PlayerHair[num33].Width() / 2;
				float y = num30 + 4;
				Main.spriteBatch.Draw(TextureAssets.Players[num31, 0].Value, new Vector2(x, y), new Rectangle(0, 0, TextureAssets.PlayerHair[num33].Width(), 56), Main.player[Main.myPlayer].skinColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(TextureAssets.Players[num31, 1].Value, new Vector2(x, y), new Rectangle(0, 0, TextureAssets.PlayerHair[num33].Width(), 56), new Color(255, 255, 255, 255), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(TextureAssets.Players[num31, 2].Value, new Vector2(x, y), new Rectangle(0, 0, TextureAssets.PlayerHair[num33].Width(), 56), Main.player[Main.myPlayer].eyeColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				Vector2 value = Main.player[Main.myPlayer].GetHairDrawOffset(num33, hatHair: false) * Main.player[Main.myPlayer].Directions;
				Main.spriteBatch.Draw(TextureAssets.PlayerHair[num33].Value, new Vector2(x, y) + value, new Rectangle(0, 0, TextureAssets.PlayerHair[num33].Width(), 56), Main.selColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				num32++;
				num29 += 56;
				if (num32 >= 5)
				{
					num32 = 0;
					num29 = hairWindowXCoordC;
					num30 += 56;
				}
			}
		}
	}
}