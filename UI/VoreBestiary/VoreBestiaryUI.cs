using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals;

namespace V2.UI.VoreBestiary
{
	public enum AEMTab
	{
		Starter,
		You,
		ItemEntries,
		NPCEntries,
		WorldEntries,
		MiscEntries,
		DivineIntervention,
	}

	public enum DITab
	{
		Gender,
		Type,
	}

	public class VoreBestiaryUI : UIState
	{
		public static bool Visible { get; set; }
		public static AEMTab SelectedTab { get; set; }
		public static DITab SelectedDITab { get; set; }
		public static DISwitch SelectedDISwitch { get; set; }

		public static string StPromYapLocal { get; internal set; }
		private static bool StPromYapLocalChanged { get; set; }
		public static int StPromYapDelay => 2;
		public static int StPromYapTimer { get; internal set; }
		public static int StPromYapChars { get; internal set; }
		private static bool StPromYapNewCharDisplayed { get; set; }

		private static readonly Asset<Texture2D> _voreBestiaryBackground = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_Main");
		private static readonly Asset<Texture2D> _actualDescBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DescriptionBox");
		private static readonly Asset<Texture2D> _snackAngelYapBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_FoodAngelYappingBox");
		private static readonly Asset<Texture2D> _DITabActive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionTab");
		private static readonly Asset<Texture2D> _DITabInactive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionTabInactive");
		private static readonly Asset<Texture2D> _DISwitch = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionSwitch");
		private static readonly Asset<Texture2D> _HoverBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_GeneralHoverBox");

		public override void OnInitialize()
		{
			
		}

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;

			if (V2.GetFooled)
				player.AsV2Player().LookingAtAEM = false;

			if (player.AsV2Player().LookingAtAEM)
				Visible = true;
			else
			{
				SelectedTab = AEMTab.Starter;
				SelectedDITab = DITab.Gender;
				SelectedDISwitch = null;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			Main.LocalPlayer.mouseInterface = true;
			Vector2 backdropPos = new Vector2(
				(Main.screenWidth - _voreBestiaryBackground.Value.Width) / 2,
				(Main.screenHeight - _voreBestiaryBackground.Value.Height) / 2
			);
			Vector2 descBoxPos = backdropPos + new Vector2(922, 62);
			Vector2 snackAngelYapPos = backdropPos + new Vector2(922, 502);
			spriteBatch.Draw(
				_voreBestiaryBackground.Value,
				backdropPos,
				_voreBestiaryBackground.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);
			spriteBatch.Draw(
				_actualDescBox.Value,
				descBoxPos,
				_actualDescBox.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);
			spriteBatch.Draw(
				_snackAngelYapBox.Value,
				snackAngelYapPos,
				_snackAngelYapBox.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);

			List<VoreBestiaryTab> tabsList = [.. VoreBestiaryTabHandling.Tabs.OrderBy(x => x.Priority)];
			tabsList.RemoveAll(x => x.Priority <= 0);
			for (int i = 0; i < tabsList.Count; i++)
			{
				VoreBestiaryTab tabToDraw = tabsList[i];
				Texture2D tabTexture = tabToDraw.Texture;
				spriteBatch.Draw(
					tabTexture,
					backdropPos + new Vector2(60 * tabToDraw.Priority, 20),
					tabTexture.Bounds,
					Color.White,
					0f,
					tabTexture.Size() / 2f,
					1f,
					SpriteEffects.None,
					0f
				);

				Rectangle hoverRect = new Rectangle(
					(int)backdropPos.X + (60 * tabToDraw.Priority),
					(int)backdropPos.Y + 20,
					tabTexture.Width,
					tabTexture.Height
				);
				if (hoverRect.Contains(Main.MouseScreen.ToPoint()))
				{
					Color gilded = new Color(255, 204, 0);
					// this is the most dogshit way to set up this string ever but whatever
					string titleAndDescOnHover = "\n[c/"
						+ gilded.Hex3()
						+ ":"
						+ Language.GetTextValue(tabToDraw.LocalizeKey + ".Title")
						+ "]\n[c/"
						+ Color.CornflowerBlue.Hex3()
						+ ":"
						+ Language.GetTextValue(tabToDraw.LocalizeKey + ".TabOverview")
						+ "]\n"
						+ Language.GetTextValue("Mods.V2.AEM.Generic." + (tabToDraw.IsActiveTab ? "AlreadyOnTab" : "OpenTabByClicking"));
					UISupplementaries.DrawMouseTooltipWithTilesBoundingBox(
						spriteBatch,
						_HoverBox.Value,
						30,
						Color.White,
						titleAndDescOnHover,
						Color.White,
						new Color(0, 9, 38),
						500,
						8
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Main.mouseLeftRelease = false;
						if (!tabToDraw.IsActiveTab)
							SelectedTab = tabToDraw.Signifier;
						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}
			}

			switch (SelectedTab)
			{
				case AEMTab.DivineIntervention:
					Vector2 tabGenderPos = backdropPos + new Vector2(128, 128);
					Vector2 tabTypePos = backdropPos + new Vector2(200, 128);
					Vector2 switchBasePos = backdropPos + new Vector2(160, 200);
					int row = 0;
					int column = 0;
					void DrawCategoryToggles(DISwitchCategory category)
					{
						foreach (DISwitch toggle in DISwitchHandling.Switches)
						{
							if (toggle.Category != category)
								continue;
							Vector2 properSwitchPos = switchBasePos + new Vector2(
								row * 40,
								column * 60
							);
							spriteBatch.Draw(
								_DISwitch.Value,
								properSwitchPos,
								toggle.GetToggleState() ? new Rectangle(16, 0, 16, 16) : new Rectangle(0, 0, 16, 16),
								Color.White,
								0f,
								new Vector2(8, 8),
								1f,
								SpriteEffects.None,
								0f
							);

							toggle.GetTitleAndDescription(out string title, out string stPrommentary);

							Vector2 titleSize = ChatManager.GetStringSize(
								FontAssets.MouseText.Value,
								title,
								Vector2.One
							);

							ChatManager.DrawColorCodedStringWithShadow(
								spriteBatch,
								FontAssets.MouseText.Value,
								title,
								properSwitchPos + new Vector2(20, 0),
								SelectedDISwitch == toggle ? Color.LightGreen : new Color(255, 204, 0),
								new Color(0, 9, 38),
								0f,
								new Vector2(0, titleSize.Y / 2f),
								Vector2.One
							);

							Rectangle switchHoverBox = new Rectangle(
								(int)properSwitchPos.X,
								(int)properSwitchPos.Y,
								16,
								16
							);

							if (switchHoverBox.Contains(Main.MouseScreen.ToPoint()))
							{
								if (Main.mouseLeft & Main.mouseLeftRelease)
								{
									Main.mouseLeftRelease = false;
									toggle.SetToggleState(toggle.GetToggleState());
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
							switchHoverBox.Width += 20 + (int)Math.Ceiling(titleSize.X);
							switchHoverBox.Height += (int)Math.Ceiling(titleSize.X);

							if (switchHoverBox.Contains(Main.MouseScreen.ToPoint()))
							{
								if (Main.mouseLeft & Main.mouseLeftRelease)
								{
									Main.mouseLeftRelease = false;
									if (SelectedDISwitch == toggle)
										SelectedDISwitch = null;
									else
										SelectedDISwitch = toggle;
									SoundEngine.PlaySound(SoundID.MenuTick);
								}
							}
							row += 1;
							if (row >= 7)
							{
								row = 0;
								column += 1;
							}
						}
					}
					switch (SelectedDITab)
					{
						case DITab.Gender:
						default:
							spriteBatch.Draw(
								_DITabActive.Value,
								tabGenderPos,
								_DITabActive.Value.Bounds,
								Color.White,
								0f,
								Vector2.Zero,
								1f,
								SpriteEffects.None,
								0f
							);
							spriteBatch.Draw(
								_DITabInactive.Value,
								tabTypePos,
								_DITabInactive.Value.Bounds,
								Color.White,
								0f,
								Vector2.Zero,
								1f,
								SpriteEffects.None,
								0f
							);

							DrawCategoryToggles(DISwitchCategory.Gender);
							break;
						case DITab.Type:
							spriteBatch.Draw(
								_DITabInactive.Value,
								tabGenderPos,
								_DITabInactive.Value.Bounds,
								Color.White,
								0f,
								Vector2.Zero,
								1f,
								SpriteEffects.None,
								0f
							);
							spriteBatch.Draw(
								_DITabActive.Value,
								tabTypePos,
								_DITabActive.Value.Bounds,
								Color.White,
								0f,
								Vector2.Zero,
								1f,
								SpriteEffects.None,
								0f
							);

							DrawCategoryToggles(DISwitchCategory.Type);
							break;
					}
					// note to self: add case for the AEM to eat the player if all DI toggles are off simultaneously
					break;
				case AEMTab.Starter:
				default:
					break;
			}

			if (StPromYapLocalChanged)
			{
				StPromYapTimer = 0;
				StPromYapChars = 0;
			}

			if (StPromYapLocal != "")
			{
				StPromYapTimer++;
				StPromYapNewCharDisplayed = false;
				if (StPromYapTimer >= StPromYapDelay)
				{
					StPromYapTimer = 0;
					StPromYapChars++;
					StPromYapNewCharDisplayed = true;
				}

				List<List<TextSnippet>> parsedSnippets = Utils.WordwrapStringSmart(
					Language.GetTextValue("Mods.V2.AEM." + StPromYapLocal),
					Color.White,
					FontAssets.MouseText.Value,
					690,
					10
				);
				List<TextSnippet> displayableSnippets = [];
				int charDisplayTrackerCount = 0;
				foreach (List<TextSnippet> snippetsBatch in parsedSnippets)
				{
					foreach (TextSnippet snippet in snippetsBatch)
					{
						foreach (char character in snippet.Text)
						{
							TextSnippet charSnippet = new TextSnippet(character.ToString(), snippet.Color, snippet.Scale);
							displayableSnippets.Add(snippet);
							charDisplayTrackerCount++;
							if (charDisplayTrackerCount >= StPromYapChars)
								break;
						}
						if (charDisplayTrackerCount >= StPromYapChars)
							break;
					}
					if (charDisplayTrackerCount >= StPromYapChars)
						break;
				}
				ChatManager.DrawColorCodedStringWithShadow(
					spriteBatch,
					FontAssets.MouseText.Value,
					[.. displayableSnippets],
					snackAngelYapPos + new Vector2(15, 15),
					0f,
					new Color(255, 229, 127),
					new Color(0, 9, 38),
					Vector2.Zero,
					Vector2.One,
					out _
				);
				if (StPromYapNewCharDisplayed)
				{
					List<char> skipSoundsForTheseChars = [
						' '
					];
					if (!skipSoundsForTheseChars.Contains(displayableSnippets.Last().Text.Last()))
						SoundEngine.PlaySound(SoundID.MenuTick);
				}
			}
		}
	}
}