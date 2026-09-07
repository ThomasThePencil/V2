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

namespace V2.UI.PredStatsMenu
{
	public enum PredGoalsSortingOption
	{
		Default,
		Alphabetical,
		AlphabeticalBackwards,
		PointValue,
		PointValueBackwards,
		Completion,
		Incompletion,
	}
	public class PredStatsMenuUI : UIState
	{
		public static bool Visible { get; set; }

		public static bool GoalsMenuOpen { get; set; }
		public static ProgressionStage SelectedProgressionStage { get; set; }
		public static PredGoalsSortingOption SortStyle { get; set; }
		public static int GoalsPage { get; set; }

		private static readonly Asset<Texture2D> _predStatsMenuBackground = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Background", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsPizzaSlice_GLP = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_GLP", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsPizzaSlice_TUM = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_TUM", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsPizzaSlice_ACI = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ACI", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsPizzaSlice_ABS = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_PredStatSlice_ABS", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsOverviewPanel = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_StatOverviewPanel", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsAvailablePointsPanel = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_AvailableStatPointsPanel", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsGoalsMenuBook = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_GoalsBook", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsGoalsStageDeselected = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_SectionDeselected", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsGoalsStageSelected = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_SectionSelected", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsGoalsMenuHeader = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_Header", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsGoalsMenuFooter = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_Footer", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsGoalsSortingClipboard = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_SortingClipboard", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsGoalsMenuExitButton = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Goals_ExitButton", AssetRequestMode.ImmediateLoad);
		private static readonly Asset<Texture2D> _predStatsExitButton = ModContent.Request<Texture2D>("V2/UI/PredStatsMenu/PredStatsMenu_Exit", AssetRequestMode.ImmediateLoad);
		private static readonly SoundStyle AllocateSuccess = new SoundStyle("V2/Sounds/PredStatsMenu/AllocateSuccess", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };
		private static readonly SoundStyle AllocateFail = new SoundStyle("V2/Sounds/PredStatsMenu/AllocateFail", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };
		private static readonly SoundStyle SortStyleChange = new SoundStyle("V2/Sounds/PredStatsMenu/SortStyleChange", SoundType.Sound) with { MaxInstances = 0, PitchVariance = 0f };

		public override void OnInitialize()
		{
			SelectedProgressionStage = ModContent.GetInstance<StarterStage>();
			SortStyle = PredGoalsSortingOption.Default;
		}

		public override void Update(GameTime gameTime)
		{
			Visible = false;
			Player player = Main.LocalPlayer;

			if (!Main.playerInventory || V2.GetFooled)
				player.AsPred().InPredStatsMenu = false;

			if (player.AsPred().InPredStatsMenu)
				Visible = true;
			else
				GoalsMenuOpen = false;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!Visible)
				return;

			Main.LocalPlayer.mouseInterface = true;
			Vector2 backdropPos = new Vector2(
				(Main.screenWidth - _predStatsMenuBackground.Value.Width) / 2,
				(Main.screenHeight - _predStatsMenuBackground.Value.Height) / 2
			);
			spriteBatch.Draw(
				_predStatsMenuBackground.Value,
				backdropPos,
				_predStatsMenuBackground.Value.Bounds,
				Color.White,
				0f,
				Vector2.Zero,
				1f,
				SpriteEffects.None,
				0f
			);

			Rectangle backdropRect = _predStatsMenuBackground.Value.Bounds;
			backdropRect.X = (int)backdropPos.X;
			backdropRect.Y = (int)backdropPos.Y;

			Rectangle goalsMenuBookRect = new Rectangle(
				(int)backdropPos.X + 644,
				(int)backdropPos.Y + 394,
				30,
				30
			);

			float mouseTextColorFactor = (float)(int)Main.mouseTextColor / 255f;

			if (GoalsMenuOpen)
			{
				List<PredPlayerGoal> selectedStageGoals = [.. ModContent.GetContent<PredPlayerGoal>()];
				List<ProgressionStage> stagesOrdered = [.. PredPlayerGoalLoader.ProgressionStages.OrderBy(x => x.Order)];

				selectedStageGoals.RemoveAll(x => x.Stage != SelectedProgressionStage || !x.Available(Main.LocalPlayer));

				switch (SortStyle)
				{
					default:
						break;
					case PredGoalsSortingOption.Alphabetical:
						selectedStageGoals = [.. selectedStageGoals.OrderBy(x => Language.GetTextValue(x.DisplayName(Main.LocalPlayer)))];
						break;
					case PredGoalsSortingOption.AlphabeticalBackwards:
						selectedStageGoals = [.. selectedStageGoals.OrderByDescending(x => Language.GetTextValue(x.DisplayName(Main.LocalPlayer)))];
						break;
					case PredGoalsSortingOption.PointValue:
						selectedStageGoals = [.. selectedStageGoals.OrderBy(x => x.StatPointsFromCompletion)];
						break;
					case PredGoalsSortingOption.PointValueBackwards:
						selectedStageGoals = [.. selectedStageGoals.OrderByDescending(x => x.StatPointsFromCompletion)];
						break;
					case PredGoalsSortingOption.Completion:
						selectedStageGoals = [.. selectedStageGoals.OrderBy(x => !x.Complete(Main.LocalPlayer))];
						break;
					case PredGoalsSortingOption.Incompletion:
						selectedStageGoals = [.. selectedStageGoals.OrderByDescending(x => !x.Complete(Main.LocalPlayer))];
						break;
				}

				int columnCountPerPage = 10;
				int rowCountPerPage = 4;
				int totalGoalsPerPage = columnCountPerPage * rowCountPerPage;
				int goalPages = 1 + (int)Math.Floor((double)(selectedStageGoals.Count - 1) / (double)totalGoalsPerPage);
				int firstIndexForThisPage = (GoalsPage - 1) * totalGoalsPerPage;
				int lastIndexForThisPage = firstIndexForThisPage + (columnCountPerPage * rowCountPerPage) - 1;
				if (lastIndexForThisPage >= selectedStageGoals.Count)
					lastIndexForThisPage = selectedStageGoals.Count - 1;

				for (int i = firstIndexForThisPage; i <= lastIndexForThisPage; i++)
				{
					int placementIndex = i % totalGoalsPerPage;
					int x = placementIndex % columnCountPerPage;
					int y = (int)Math.Floor((double)placementIndex / (double)columnCountPerPage);
					PredPlayerGoal goalToDraw = selectedStageGoals[i];
					Vector2 goalDrawPos = backdropPos + new Vector2(80f, 140f) + new Vector2((PredPlayerGoal.TextureBounds.Width + 2) * x, (PredPlayerGoal.TextureBounds.Height + 2) * y);
					spriteBatch.Draw(
						goalToDraw.Complete(Main.LocalPlayer)
						? goalToDraw.CompleteTexture
						: goalToDraw.IncompleteTexture,
						goalDrawPos,
						PredPlayerGoal.TextureBounds,
						Color.White,
						0f,
						Vector2.Zero,
						1f,
						SpriteEffects.None,
						0f
					);

					Rectangle goalHoverRect = new Rectangle(
						(int)goalDrawPos.X,
						(int)goalDrawPos.Y,
						PredPlayerGoal.TextureBounds.Width,
						PredPlayerGoal.TextureBounds.Height
					);
					if (PredStatsMenuMouthUI.MouthState == PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot && goalHoverRect.Contains(Main.MouseScreen.ToPoint()))
					{
						string goalFullHoverText =
							"[c/"
						  + (goalToDraw.DisplayNameColor(Main.LocalPlayer) * mouseTextColorFactor).Hex3()
						  + ":"
						  + Language.GetTextValue(goalToDraw.DisplayName(Main.LocalPlayer))
						  + "]\n[c/"
						  + (new Color(127, 127, 127) * mouseTextColorFactor).Hex3()
						  + ":"
						  + (goalToDraw.Complete(Main.LocalPlayer) ? "Complete" : "Incomplete")
						  + "; worth "
						  + goalToDraw.StatPointsFromCompletion
						  + " stat point"
						  + (goalToDraw.StatPointsFromCompletion == 1 ? "" : "s") + "]\n";
						string descriptionKey = goalToDraw.Description(Main.LocalPlayer);
						if (Main.keyState.IsKeyDown(Keys.LeftShift))
							descriptionKey += ".Clarification";
						else
							descriptionKey += ".Default";
						string[] goalFullHoverTextLines = Utils.WordwrapString(
							Language.GetTextValue(descriptionKey),
							FontAssets.MouseText.Value,
							720,
							15,
							out _
						);
						foreach (string piece in goalFullHoverTextLines)
						{
							if (piece is not null && piece != "")
							{
								goalFullHoverText += piece;
								if (!piece.Contains('\n'))
									goalFullHoverText += "\n";
							}
						}

						if (!Main.keyState.IsKeyDown(Keys.LeftShift))
							goalFullHoverText += "[c/7F7F7F:" + Language.GetTextValue("Mods.V2.PredPlayerGoals.GenericText.HoldToLearnMore." + (goalToDraw.Complete(Main.LocalPlayer) ? "Complete" : "Incomplete")) + "]";

						UICommon.TooltipMouseText(goalFullHoverText);
						Main.mouseText = true;
					}
				}

				int selectedStageIndex = stagesOrdered.IndexOf(SelectedProgressionStage);
				for (int i = 0; i < stagesOrdered.Count; i++)
				{
					ProgressionStage stageToDraw = stagesOrdered[i];
					List<PredPlayerGoal> stageGoals = [.. ModContent.GetContent<PredPlayerGoal>()];
					stageGoals.RemoveAll(x => x.Stage != stageToDraw);
					List<PredPlayerGoal> stageGoalsCompleted = stageGoals.FindAll(x => x.Complete(Main.LocalPlayer));
					float goalCompletionRatio = (float)stageGoalsCompleted.Count / (float)stageGoals.Count;
					int pointsPossibleFromStage = 0;
					int pointsGainedFromStage = 0;
					foreach (PredPlayerGoal goal in stageGoals)
					{
						pointsPossibleFromStage += goal.StatPointsFromCompletion;
						if (goal.Complete(Main.LocalPlayer))
							pointsGainedFromStage += goal.StatPointsFromCompletion;
					}
					float pointsCompletionRatio = (float)pointsGainedFromStage / (float)pointsPossibleFromStage;

					Vector2 stageTabDrawPos = backdropPos + new Vector2(10f, 140f);
					if (i <= selectedStageIndex)
						stageTabDrawPos.Y += i * 20;
					else
						stageTabDrawPos.Y += (i + 1) * 20;

					spriteBatch.Draw(
						i == selectedStageIndex
						? _predStatsGoalsStageSelected.Value
						: _predStatsGoalsStageDeselected.Value,
						stageTabDrawPos,
						i == selectedStageIndex
						? _predStatsGoalsStageSelected.Value.Bounds
						: _predStatsGoalsStageDeselected.Value.Bounds,
						Color.White,
						0f,
						Vector2.Zero,
						1f,
						SpriteEffects.None,
						0f
					);

					Rectangle stageHoverRect = new Rectangle(
						(int)stageTabDrawPos.X,
						(int)stageTabDrawPos.Y,
						i == selectedStageIndex ? _predStatsGoalsStageSelected.Value.Width : _predStatsGoalsStageDeselected.Value.Width,
						i == selectedStageIndex ? _predStatsGoalsStageSelected.Value.Height : _predStatsGoalsStageDeselected.Value.Height
					);
					if (stageHoverRect.Contains(Main.MouseScreen.ToPoint()))
					{
						Color nameColor = Color.LimeGreen;
						Color subtitleColor = new Color(127, 127, 127);
						string stageFullHoverText =
							"[c/" + (nameColor * mouseTextColorFactor).Hex3() + ":" + stageToDraw.DisplayName + "]\n"
						  + "[c/" + (subtitleColor * mouseTextColorFactor).Hex3() + ":" + stageToDraw.DisplaySubtitle + "]\n";
						string[] stageFullHoverTextLines = Utils.WordwrapString(stageToDraw.Description, FontAssets.MouseText.Value, 600, 10, out _);
						foreach (string piece in stageFullHoverTextLines)
						{
							if (piece is not null && piece != "")
							{
								stageFullHoverText += piece;
								if (!piece.Contains('\n'))
									stageFullHoverText += "\n";
							}
						}
						if (!stageToDraw.Available(Main.LocalPlayer))
						{
							stageFullHoverText += "[c/" + (V2Colors.Basic.Red * mouseTextColorFactor).Hex3() + ":You haven't unlocked this progression stage yet!]\n";
							stageFullHoverText += "[c/" + (V2Colors.Basic.Red * mouseTextColorFactor).Hex3() + ":You have to " + stageToDraw.UnlockCondition + " to unlock this progression stage!]\n";
						}

						Color completionRatiosBaseColor = new Color(145, 155, 215);
						Color goalsCompleteColor = Color.Lerp(new Color(150, 50, 50), new Color(70, 210, 70), goalCompletionRatio.CastToDecimalPlaces(1));
						Color pointsGainedColor = Color.Lerp(new Color(150, 50, 50), new Color(70, 210, 70), pointsCompletionRatio.CastToDecimalPlaces(1));
						int currentHiddenGoalsInStage = stageGoals.FindAll(x => !x.Available(Main.LocalPlayer)).Count;
						stageFullHoverText += "[c/" + (completionRatiosBaseColor * mouseTextColorFactor).Hex3() + ":Stage Goals Completed:] [c/" + (goalsCompleteColor * mouseTextColorFactor).Hex3() + ":" + stageGoalsCompleted.Count + " / " + stageGoals.Count + " (" + goalCompletionRatio.ToPercentage(1) + ")] [c/" + (subtitleColor * mouseTextColorFactor).Hex3() + ":(" + (currentHiddenGoalsInStage == 1 ? "1 goal is" : (currentHiddenGoalsInStage + " goals are")) + " currently hidden)]\n";
						stageFullHoverText += "[c/" + (completionRatiosBaseColor * mouseTextColorFactor).Hex3() + ":Points Gained From Stage:] [c/" + (pointsGainedColor * mouseTextColorFactor).Hex3() + ":" + pointsGainedFromStage + " / " + pointsPossibleFromStage + " (" + pointsCompletionRatio.ToPercentage(1) + ")]";

						UICommon.TooltipMouseText(stageFullHoverText);
						Main.mouseText = true;
						if (stageToDraw.Available(Main.LocalPlayer) && Main.mouseLeft && Main.mouseLeftRelease)
						{
							SoundEngine.PlaySound(SoundID.MenuTick);
							SelectedProgressionStage = stageToDraw;
						}
					}
				}

				spriteBatch.Draw(
					_predStatsGoalsMenuHeader.Value,
					backdropPos + new Vector2(_predStatsMenuBackground.Value.Width / 2f, 10f),
					_predStatsGoalsMenuHeader.Value.Bounds,
					Color.White,
					0f,
					new Vector2(
						_predStatsGoalsMenuHeader.Value.Width / 2f,
						0
					),
					1f,
					SpriteEffects.None,
					0f
				);
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					FontAssets.MouseText.Value,
					Language.GetTextValue("Mods.V2.PredPlayerGoals.GenericText.Header.Title"),
					backdropPos + new Vector2(_predStatsMenuBackground.Value.Width / 2f, 12f),
					Color.White,
					0f,
					new Vector2(
						ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.PredPlayerGoals.GenericText.Header.Title"), new Vector2(1.2f)).X / 2f,
						0f
					),
					new Vector2(1.2f)
				);
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					FontAssets.MouseText.Value,
					Language.GetTextValue("Mods.V2.PredPlayerGoals.GenericText.Header.Description"),
					backdropPos + new Vector2(
						(_predStatsMenuBackground.Value.Width / 2f) - (_predStatsGoalsMenuHeader.Value.Width / 2f) + 8f,
						12f + ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("Mods.V2.PredPlayerGoals.GenericText.Header.Title"), new Vector2(1.25f)).Y + 2f
					),
					Color.White,
					0f,
					Vector2.Zero,
					new Vector2(0.75f),
					_predStatsGoalsMenuHeader.Value.Width - 16f
				);

				spriteBatch.Draw(
					_predStatsGoalsMenuFooter.Value,
					backdropPos + new Vector2((_predStatsMenuBackground.Value.Width / 2f) - 30f, _predStatsMenuBackground.Value.Height - 10f),
					_predStatsGoalsMenuFooter.Value.Bounds,
					Color.White,
					0f,
					new Vector2(
						_predStatsGoalsMenuFooter.Value.Width / 2f,
						_predStatsGoalsMenuFooter.Value.Height
					),
					1f,
					SpriteEffects.None,
					0f
				);
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					FontAssets.MouseText.Value,
					SelectedProgressionStage.FooterAdvice,
					backdropPos + new Vector2(
						(_predStatsMenuBackground.Value.Width / 2f) - 30f - (_predStatsGoalsMenuFooter.Value.Width / 2f) + 8f,
						_predStatsMenuBackground.Value.Height - _predStatsGoalsMenuFooter.Value.Height
					),
					Color.White,
					0f,
					Vector2.Zero,
					new Vector2(0.75f),
					_predStatsGoalsMenuFooter.Value.Width - 16f
				);

				Rectangle sortingClipboardHoverRect = new Rectangle(
					(int)backdropPos.X + 644,
					(int)backdropPos.Y + 154,
					_predStatsGoalsSortingClipboard.Value.Width,
					_predStatsGoalsSortingClipboard.Value.Height
				);
				spriteBatch.Draw(
					_predStatsGoalsSortingClipboard.Value,
					backdropPos + new Vector2(644f, 154f),
					sortingClipboardHoverRect.Contains(Main.MouseScreen.ToPoint())
					? new Rectangle(32, 0, 30, 48)
					: new Rectangle(0, 0, 30, 48),
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				if (sortingClipboardHoverRect.Contains(Main.MouseScreen.ToPoint()))
				{
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						SortStyle = SortStyle.NextEnum();
						SoundEngine.PlaySound(SortStyleChange);
					}
					if (Main.mouseRight && Main.mouseRightRelease)
					{
						SortStyle = SortStyle.PreviousEnum();
						SoundEngine.PlaySound(SortStyleChange);
					}
					UICommon.TooltipMouseText(
						Language.GetTextValueWith(
							"Mods.V2.PredPlayerGoals.SortingText",
							new
							{
								HeaderColor = (V2Colors.Basic.Gold * mouseTextColorFactor).Hex3(),
								DefaultSortColor = ((SortStyle == PredGoalsSortingOption.Default ? V2Colors.Basic.Aqua : V2Colors.Basic.LightGray) * mouseTextColorFactor).Hex3(),
								AlphabeticalSortColor = ((SortStyle == PredGoalsSortingOption.Alphabetical ? V2Colors.Basic.Aqua : V2Colors.Basic.LightGray) * mouseTextColorFactor).Hex3(),
								AlphabeticalBackwardsSortColor = ((SortStyle == PredGoalsSortingOption.AlphabeticalBackwards ? V2Colors.Basic.Aqua : V2Colors.Basic.LightGray) * mouseTextColorFactor).Hex3(),
								PointValueSortColor = ((SortStyle == PredGoalsSortingOption.PointValue ? V2Colors.Basic.Aqua : V2Colors.Basic.LightGray) * mouseTextColorFactor).Hex3(),
								PointValueBackwardsSortColor = ((SortStyle == PredGoalsSortingOption.PointValueBackwards ? V2Colors.Basic.Aqua : V2Colors.Basic.LightGray) * mouseTextColorFactor).Hex3(),
								CompletionSortColor = ((SortStyle == PredGoalsSortingOption.Completion ? V2Colors.Basic.Aqua : V2Colors.Basic.LightGray) * mouseTextColorFactor).Hex3(),
								IncompletionSortColor = ((SortStyle == PredGoalsSortingOption.Incompletion ? V2Colors.Basic.Aqua : V2Colors.Basic.LightGray) * mouseTextColorFactor).Hex3(),
								CycleColor = (V2Colors.Basic.Aqua * mouseTextColorFactor).Hex3(),
							}
						)
					);
				}

				spriteBatch.Draw(
					_predStatsGoalsMenuExitButton.Value,
					backdropPos + new Vector2(644f, 394f),
					goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint())
					? new Rectangle(32, 0, 30, 30)
					: new Rectangle(0, 0, 30, 30),
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				if (PredStatsMenuMouthUI.MouthState == PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot && goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint()))
				{
					Main.instance.MouseText(
						"Return to the main pred stats menu"
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						SoundEngine.PlaySound(SoundID.MenuClose);
						GoalsMenuOpen = false;
						return;
					}
				}

				if (PredStatsMenuMouthUI.MouthState == PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot && Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
				{
					SoundEngine.PlaySound(SoundID.MenuClose);
					GoalsMenuOpen = false;
					return;
				}
			}
			else
			{
				string hoveredStatSlice = "none";
				void SliceHoverLogic(Rectangle sliceRect, PredStat stat, string statFullName, string statShorthand)
				{
					if (PredStatsMenuMouthUI.MouthState != PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot || !sliceRect.Contains(Main.MouseScreen.ToPoint()))
						return;

					hoveredStatSlice = statShorthand;
					string pointsAllocationText;
					switch (Main.keyState.IsKeyDown(Keys.LeftShift), Main.keyState.IsKeyDown(Keys.LeftControl))
					{
						case (false, false):
							pointsAllocationText = "Mods.V2.PredStatsMenu.StatSliceHover.Default";
							break;
						case (true, false):
							pointsAllocationText = "Mods.V2.PredStatsMenu.StatSliceHover.Bulk10";
							break;
						case (true, true):
							pointsAllocationText = "Mods.V2.PredStatsMenu.StatSliceHover.Bulk100";
							break;
						case (false, true):
							pointsAllocationText = "Mods.V2.PredStatsMenu.StatSliceHover.BulkMaximum";
							break;
					}
					UICommon.TooltipMouseText(
						Language.GetTextValueWith(
							pointsAllocationText,
							new
							{
								PredStatName = statFullName,
								PredStatShorthand = statShorthand,
								SpentPoints = stat.Spent,
							}
						)
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						if (Main.LocalPlayer.AsPred().AvailableStatPoints == 0)
						{
							SoundEngine.PlaySound(AllocateFail);
						}
						else
						{
							switch (Main.keyState.IsKeyDown(Keys.LeftShift), Main.keyState.IsKeyDown(Keys.LeftControl))
							{
								case (false, false):
									stat.Spent++;
									break;
								case (true, false):
									stat.Spent += Math.Min(10, Main.LocalPlayer.AsPred().AvailableStatPoints);
									break;
								case (true, true):
									stat.Spent += Math.Min(100, Main.LocalPlayer.AsPred().AvailableStatPoints);
									break;
								case (false, true):
									stat.Spent += Main.LocalPlayer.AsPred().AvailableStatPoints;
									break;
							}
							SoundEngine.PlaySound(AllocateSuccess);
							if (Main.netMode == NetmodeID.MultiplayerClient)
							{
								ModPacket deliveryPacket = V2.Instance.GetPacket();
								deliveryPacket.Write((byte)V2.MessageType.RequestPlayerPredStatSync);
								deliveryPacket.Write((byte)Main.myPlayer);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().GLP.Spent);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().TUM.Spent);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().ACI.Spent);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().ABS.Spent);
								deliveryPacket.Send(ignoreClient: Main.myPlayer);
							}
						}
					}
					else if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (stat.Spent == 0)
						{
							SoundEngine.PlaySound(AllocateFail);
						}
						else
						{
							switch (Main.keyState.IsKeyDown(Keys.LeftShift), Main.keyState.IsKeyDown(Keys.LeftControl))
							{
								case (false, false):
									stat.Spent--;
									break;
								case (true, false):
									stat.Spent -= Math.Min(10, stat.Spent);
									break;
								case (true, true):
									stat.Spent -= Math.Min(100, stat.Spent);
									break;
								case (false, true):
									stat.Spent -= stat.Spent;
									break;
							}
							SoundEngine.PlaySound(AllocateSuccess with { Pitch = -0.15f });
							if (Main.netMode == NetmodeID.MultiplayerClient)
							{
								ModPacket deliveryPacket = V2.Instance.GetPacket();
								deliveryPacket.Write((byte)V2.MessageType.RequestPlayerPredStatSync);
								deliveryPacket.Write((byte)Main.myPlayer);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().GLP.Spent);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().TUM.Spent);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().ACI.Spent);
								deliveryPacket.Write(Main.LocalPlayer.AsPred().ABS.Spent);
								deliveryPacket.Send(ignoreClient: Main.myPlayer);
							}
						}
					}
				}

				#region GLP
				spriteBatch.Draw(
					_predStatsPizzaSlice_GLP.Value,
					backdropPos + new Vector2(560f, 40f),
					_predStatsPizzaSlice_GLP.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectGLP = new Rectangle(
					backdropRect.X + 560,
					backdropRect.Y + 40,
					_predStatsPizzaSlice_GLP.Value.Width,
					_predStatsPizzaSlice_GLP.Value.Height
				);
				SliceHoverLogic(sliceRectGLP, Main.LocalPlayer.AsPred().GLP, "Swallow strength", "GLP");
				#endregion
				#region TUM
				spriteBatch.Draw(
					_predStatsPizzaSlice_TUM.Value,
					backdropPos + new Vector2(620f, 40f),
					_predStatsPizzaSlice_TUM.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectTUM = new Rectangle(
					backdropRect.X + 620,
					backdropRect.Y + 40,
					_predStatsPizzaSlice_TUM.Value.Width,
					_predStatsPizzaSlice_TUM.Value.Height
				);
				SliceHoverLogic(sliceRectTUM, Main.LocalPlayer.AsPred().TUM, "Stomach strength", "TUM");
				#endregion
				#region ACI
				spriteBatch.Draw(
					_predStatsPizzaSlice_ACI.Value,
					backdropPos + new Vector2(560f, 100f),
					_predStatsPizzaSlice_ACI.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectACI = new Rectangle(
					backdropRect.X + 560,
					backdropRect.Y + 100,
					_predStatsPizzaSlice_ACI.Value.Width,
					_predStatsPizzaSlice_ACI.Value.Height
				);
				SliceHoverLogic(sliceRectACI, Main.LocalPlayer.AsPred().ACI, "Acid strength", "ACI");
				#endregion
				#region ABS
				spriteBatch.Draw(
					_predStatsPizzaSlice_ABS.Value,
					backdropPos + new Vector2(620f, 100f),
					_predStatsPizzaSlice_ABS.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle sliceRectABS = new Rectangle(
					backdropRect.X + 620,
					backdropRect.Y + 100,
					_predStatsPizzaSlice_ABS.Value.Width,
					_predStatsPizzaSlice_ABS.Value.Height
				);
				SliceHoverLogic(sliceRectABS, Main.LocalPlayer.AsPred().ABS, "Absorption strength", "ABS");
				#endregion

				spriteBatch.Draw(
					_predStatsOverviewPanel.Value,
					backdropPos + new Vector2(20f, 20f),
					_predStatsOverviewPanel.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);

				if (hoveredStatSlice != "none")
				{
					string explainHoveredStatHow = "RelevantStats";
					if (Main.keyState.IsKeyDown(Keys.LeftShift))
						explainHoveredStatHow = "Description";

					string acidTierKey = "Mods.V2.PredStatsMenu.StatDescription.ACI.AcidTiers.Normal";

					ChatManager.DrawColorCodedStringWithShadow(
						spriteBatch,
						FontAssets.MouseText.Value,
						Language.GetTextValueWith(
							"Mods.V2.PredStatsMenu.StatDescription." + hoveredStatSlice + "." + explainHoveredStatHow,
							new
							{
								GLPTotal = Main.LocalPlayer.AsPred().GLP.Total,
								GLPSpent = Main.LocalPlayer.AsPred().GLP.Spent,
								GLPBase = Main.LocalPlayer.AsPred().GLP.Base,
								GLPExtra = Main.LocalPlayer.AsPred().GLP.Extra,
								PreySwallowSize = Main.LocalPlayer.AsPred().SwallowCapacity != -1 ? ("" + Main.LocalPlayer.AsPred().SwallowCapacity.CastToDecimalPlaces(2)) : "Infinite",
								LiquidSwallowRate = (Main.LocalPlayer.AsPred().LiquidSwallowSize / 255.0 * PredPlayer.LiquidSwallowRatePerMinute).CastToDecimalPlaces(2),
								StruggleGracePeriod = Main.LocalPlayer.AsPred().StruggleGraceTimeReadable,
								PredPlayer.BaseSwallowSize,
								PredPlayer.SwallowSizePerLevel,
								BaseDrinkRate = (PredPlayer.BaseLiquidSwallowSize / 255.0 * 60.0).CastToDecimalPlaces(2),
								DrinkRatePer5Levels = (PredPlayer.LiquidSwallowSizePer5Levels / 255.0 * PredPlayer.LiquidSwallowRatePerMinute).CastToDecimalPlaces(2),
								PredPlayer.BaseStruggleGraceTime,
								PredPlayer.StruggleGraceTimePer5Levels,
								TUMTotal = Main.LocalPlayer.AsPred().TUM.Total,
								TUMSpent = Main.LocalPlayer.AsPred().TUM.Spent,
								TUMBase = Main.LocalPlayer.AsPred().TUM.Base,
								TUMExtra = Main.LocalPlayer.AsPred().TUM.Extra,
								StomachCapacity = Main.LocalPlayer.AsPred().StomachCapacity != -1 ? ("" + Main.LocalPlayer.AsPred().StomachCapacity.CastToDecimalPlaces(2)) : "Infinite",
								StomachacheMeterCapacity = Main.LocalPlayer.AsPred().StomachacheMeterCapacity != -1 ? ("" + Main.LocalPlayer.AsPred().StomachacheMeterCapacity.CastToDecimalPlaces(2)) : "Infinite",
								StruggleChartEstimatedDifficulty = "Something for me to figure out later",
								PredPlayer.BaseStomachCapacity,
								PredPlayer.StomachCapacityPerLevel,
								PredPlayer.BaseStomachacheMeterCapacity,
								PredPlayer.StomachacheMeterCapacityPer5Levels,
								ACITotal = Main.LocalPlayer.AsPred().ACI.Total,
								ACISpent = Main.LocalPlayer.AsPred().ACI.Spent,
								ACIBase = Main.LocalPlayer.AsPred().ACI.Base,
								ACIExtra = Main.LocalPlayer.AsPred().ACI.Extra,
								DigestionDamage = Main.LocalPlayer.AsPred().DigestionTickDamage.CastToDecimalPlaces(2),
								DigestionRate = Main.LocalPlayer.AsPred().DigestionTickRate.CastToDecimalPlaces(2),
								AcidTierWithDescription = Language.GetTextValue(acidTierKey),
								BaseDigestionDamage = PredPlayer.BaseDigestionTickDamage,
								DigestionDamagePerLevel = PredPlayer.DigestionTickDamagePerLevel,
								BaseDigestionRate = PredPlayer.BaseDigestionTickRate,
								DigestionRatePer5Levels = PredPlayer.DigestionTickRatePer5Levels,
								ABSTotal = Main.LocalPlayer.AsPred().ABS.Total,
								ABSSpent = Main.LocalPlayer.AsPred().ABS.Spent,
								ABSBase = Main.LocalPlayer.AsPred().ABS.Base,
								ABSExtra = Main.LocalPlayer.AsPred().ABS.Extra,
								AbsorptionRate = Main.LocalPlayer.AsPred().PreyAbsorptionRate.CastToDecimalPlaces(2),
								BuffExtensionTime = Main.LocalPlayer.AsPred().BuffExtensionFactor.ToPercentage(2),
								DebuffDisextensionTime = (1.0 / Main.LocalPlayer.AsPred().DebuffDisextensionFactor).ToPercentage(2),
								BaseAbsorbRate = PredPlayer.BasePreyAbsorptionRate.CastToDecimalPlaces(2),
								AbsorbRatePerLevel = PredPlayer.PreyAbsorptionRatePerLevel.CastToDecimalPlaces(2),
								BuffExtendPer5Levels = PredPlayer.BuffExtensionTimePer5Levels.CastToDecimalPlaces(2),
								DebuffLossPer5Levels = PredPlayer.DebuffDisextensionTimePer5Levels.CastToDecimalPlaces(2),
							}
						),
						backdropPos + new Vector2(30f, 30f),
						Color.White,
						0f,
						Vector2.Zero,
						new Vector2(0.8f),
						480
					);
				}

				Rectangle availablePointsRect = new Rectangle(
					(int)backdropPos.X + 560,
					(int)backdropPos.Y + 192,
					_predStatsAvailablePointsPanel.Value.Width,
					_predStatsAvailablePointsPanel.Value.Height
				);
				spriteBatch.Draw(
					_predStatsAvailablePointsPanel.Value,
					backdropPos + new Vector2(560f, 192f),
					_predStatsAvailablePointsPanel.Value.Bounds,
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					FontAssets.MouseText.Value,
					"Points Available",
					backdropPos + new Vector2(564f, 198f),
					Color.White,
					0f,
					Vector2.Zero,
					new Vector2(0.8f),
					112
				);
				ChatManager.DrawColorCodedStringWithShadow(
					Main.spriteBatch,
					FontAssets.DeathText.Value,
					Main.LocalPlayer.AsPred().AvailableStatPoints.ToString(),
					backdropPos + new Vector2(620f, 252f),
					Color.White,
					0f,
					ChatManager.GetStringSize(FontAssets.DeathText.Value, Main.LocalPlayer.AsPred().AvailableStatPoints.ToString(), new Vector2(0.8f)) / 2f,
					new Vector2(0.8f)
				);
				if (availablePointsRect.Contains(Main.MouseScreen.ToPoint()))
				{
					string mouseText = Language.GetTextValueWith(
						"Mods.V2.PredStatsMenu.StatPointHover.Default",
						new
						{
							StatPointCount = Main.LocalPlayer.AsPred().AvailableStatPoints,
							StatPointTotal = Main.LocalPlayer.AsPred().TotalStatPoints,
							GLPSpent = Main.LocalPlayer.AsPred().GLP.Spent,
							TUMSpent = Main.LocalPlayer.AsPred().TUM.Spent,
							ACISpent = Main.LocalPlayer.AsPred().ACI.Spent,
							ABSSpent = Main.LocalPlayer.AsPred().ABS.Spent,
							CycleColor = (V2Colors.Basic.Aqua * mouseTextColorFactor).Hex3(),
						}
					);
					if (Main.LocalPlayer.AsPred().CheatedStatPointsWork)
					{
						mouseText += "\n\n" + Language.GetTextValueWith(
							"Mods.V2.PredStatsMenu.StatPointHover.RoseAddon",
							new
							{
								RoseFlowerColor = new Color((byte)(255f * mouseTextColorFactor), (byte)(Main.masterColor * 200f * mouseTextColorFactor), 0, Main.mouseTextColor).Hex3(),
								CheatedPointAllocColor = (V2Colors.Basic.Aqua * mouseTextColorFactor).Hex3(),
								CheatedPointModifier = Main.LocalPlayer.AsPred().CheatedStatPoints,
							}
						);
					}
					UICommon.TooltipMouseText(mouseText);
					if (Main.LocalPlayer.AsPred().CheatedStatPointsWork && Main.mouseLeft && Main.mouseLeftRelease)
					{
						switch (Main.keyState.IsKeyDown(Keys.LeftShift), Main.keyState.IsKeyDown(Keys.LeftControl))
						{
							case (false, false):
								Main.LocalPlayer.AsPred().CheatedStatPoints++;
								break;
							case (true, false):
								Main.LocalPlayer.AsPred().CheatedStatPoints += 10;
								break;
							case (true, true):
								Main.LocalPlayer.AsPred().CheatedStatPoints += 100;
								break;
							case (false, true):
								Main.LocalPlayer.AsPred().CheatedStatPoints += Main.LocalPlayer.AsPred().LegitStatPoints;
								break;
						}
						SoundEngine.PlaySound(AllocateSuccess);
					}
					else if (Main.LocalPlayer.AsPred().CheatedStatPointsWork && Main.mouseRight && Main.mouseRightRelease)
					{
						if (Main.LocalPlayer.AsPred().CheatedStatPoints <= 0)
						{
							SoundEngine.PlaySound(AllocateFail);
						}
						else
						{
							switch (Main.keyState.IsKeyDown(Keys.LeftShift), Main.keyState.IsKeyDown(Keys.LeftControl))
							{
								case (false, false):
									Main.LocalPlayer.AsPred().CheatedStatPoints--;
									break;
								case (true, false):
									Main.LocalPlayer.AsPred().CheatedStatPoints -= Math.Min(10, Main.LocalPlayer.AsPred().CheatedStatPoints);
									break;
								case (true, true):
									Main.LocalPlayer.AsPred().CheatedStatPoints -= Math.Min(100, Main.LocalPlayer.AsPred().CheatedStatPoints);
									break;
								case (false, true):
									Main.LocalPlayer.AsPred().CheatedStatPoints -= Math.Min(Main.LocalPlayer.AsPred().LegitStatPoints, Main.LocalPlayer.AsPred().CheatedStatPoints);
									break;
							}
							if (Main.LocalPlayer.AsPred().CheatedStatPoints < 0)
								Main.LocalPlayer.AsPred().CheatedStatPoints = 0;

							SoundEngine.PlaySound(AllocateSuccess with { Pitch = -0.15f });
						}
					}
				}

				spriteBatch.Draw(
					_predStatsGoalsMenuBook.Value,
					backdropPos + new Vector2(644f, 394f),
					goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint())
					? new Rectangle(32, 0, 30, 30)
					: new Rectangle(0, 0, 30, 30),
					Color.White,
					0f,
					Vector2.Zero,
					1f,
					SpriteEffects.None,
					0f
				);
				if (PredStatsMenuMouthUI.MouthState == PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot && goalsMenuBookRect.Contains(Main.MouseScreen.ToPoint()))
				{
					Main.instance.MouseText(
						"Open the pred goals menu"
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						SoundEngine.PlaySound(SoundID.MenuOpen);
						GoalsMenuOpen = true;
						GoalsPage = 1;
						SelectedProgressionStage = ModContent.GetInstance<StarterStage>();
					}
				}

				spriteBatch.Draw(
					_predStatsExitButton.Value,
					backdropPos + new Vector2(350f, 10f),
					_predStatsExitButton.Value.Bounds,
					Color.White,
					0f,
					new Vector2(29, 0),
					1f,
					SpriteEffects.None,
					0f
				);
				Rectangle exitGulletRect = new Rectangle(
					(int)backdropPos.X + 343,
					(int)backdropPos.Y + 20,
					14,
					14
				);
				if (PredStatsMenuMouthUI.MouthState == PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot && exitGulletRect.Contains(Main.MouseScreen.ToPoint()))
				{
					Main.instance.MouseText(
						"Close the pred stats menu\n"
					  + "(Are you sure your cursor can't stay a little longer?)"
					);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						PredStatsMenuMouthUI.MouthState = PredStatsMenuMouthState.RegurgitatingCursor;
						PredStatsMenuMouthUI.CanSkipThisFrame = false;
					}
				}

				if (PredStatsMenuMouthUI.MouthState == PredStatsMenuMouthState.YourCursorGotFuckingGulpedIdiot && Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
				{
					PredStatsMenuMouthUI.MouthState = PredStatsMenuMouthState.RegurgitatingCursor;
					PredStatsMenuMouthUI.CanSkipThisFrame = false;
				}
			}
		}
	}
}