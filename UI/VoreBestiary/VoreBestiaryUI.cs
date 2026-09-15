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

		public static int YappySnackAngelTalkingTime = 0;

		private static readonly Asset<Texture2D> _voreBestiaryBackground = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_Main");
		private static readonly Asset<Texture2D> _actualDescBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DescriptionBox");
		private static readonly Asset<Texture2D> _snackAngelYapBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_FoodAngelYappingBox");
		private static readonly Asset<Texture2D> _genericTabActive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_GenericTabActive");
		private static readonly Asset<Texture2D> _genericTabInactive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_GenericTabInactive");
		private static readonly Asset<Texture2D> _DITabActive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionTab");
		private static readonly Asset<Texture2D> _DITabInactive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionTabInactive");
		private static readonly Asset<Texture2D> _DISwitch = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionSwitch");
		private static readonly Asset<Texture2D> _DIHoverBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_GeneralHoverBox");

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
				SelectedTab = AEMTab.Starter;
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
			void DrawAEMTab(AEMTab tabToDraw, int offX,)
			{
				Texture2D tabTexture = (SelectedTab switch {
					_ => _genericTabInactive
				}).Value;
				spriteBatch.Draw(
					tabTexture,
					backdropPos + new Vector2(offX, 20),
					tabTexture.Bounds,
					Color.White,
					0f,
					tabTexture.Size() / 2f,
					1f,
					SpriteEffects.None,
					0f
				);

				Rectangle hoverRect = new Rectangle(
					(int)backdropPos.X + offX,
					(int)backdropPos.Y + 20,
					tabTexture.Width,
					tabTexture.Height
				);
				if (hoverRect.Contains(Main.MouseScreen.ToPoint()))
				{
					string 
				}
			}

			switch (SelectedTab)
			{
				case AEMTab.DivineIntervention:
					Vector2 tabGenderPos = backdropPos + new Vector2(128, 128);
					Vector2 tabTypePos = backdropPos + new Vector2(200, 128);
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

							int row = 0;
							int column = 0;
							Vector2 switchBasePos = backdropPos + new Vector2(160, 200);
							foreach (DISwitch toggle in DISwitchHandling.Switches)
							{
								if (toggle.Category != DISwitchCategory.Gender)
									continue;

								Vector2 properSwitchPos = switchBasePos + new Vector2(
									row * 40,
									column * 60
								);
								spriteBatch.Draw(
									_DISwitch.Value,
									properSwitchPos,
									_DISwitch.Value.Bounds,
									Color.White,
									0f,
									Vector2.Zero,
									1f,
									SpriteEffects.None,
									0f
								);

								toggle.GetTitleAndDescription(out string title, out string stPrommentary);

								ChatManager.DrawColorCodedStringWithShadow(
									spriteBatch,
									FontAssets.MouseText.Value,
									title,
									properSwitchPos + new Vector2(20, 0),
									new Color(255, 204, 255),
									new Color(0, 9, 38),
									0f,
									Vector2.Zero,
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
									
								}
								row += 1;
								if (row >= 7)
								{
									row = 0;
									column += 1;
								}
							}
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
							break;
					}
					break;
				case AEMTab.Starter:
				default:
					break;
			}
		}
	}
}