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

		private static readonly Asset<Texture2D> _voreBestiaryBackground = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_Main", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _DITabActive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionTab", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _DITabInactive = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DivineInterventionTabInactive", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _actualDescBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_DescriptionBox", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _snackAngelYapBox = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_FoodAngelYappingBox", AssetRequestMode.ImmediateLoad);

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

			switch (SelectedTab)
			{
				case AEMTab.DivineIntervention:
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

					spriteBatch.Draw(
						_actualDescBox.Value,
						backdropPos + new Vector2(922, 62),
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
						backdropPos + new Vector2(922, 502),
						_snackAngelYapBox.Value.Bounds,
						Color.White,
						0f,
						Vector2.Zero,
						1f,
						SpriteEffects.None,
						0f
					);
					break;
				case AEMTab.Starter:
				default:
					break;
			}
		}
	}
}