using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items;
using V2.NPCs;
using V2.Projectiles;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.PlayerHandling
{
	public static class PreyPlayerDigestionSounds
	{
		public static readonly SoundStyle PlayerDigestingMale = new SoundStyle("V2/PlayerHandling/MaleHit_FromDigestTick", 0, 3, SoundType.Sound) { Volume = 1f, PitchVariance = 0f };
		public static readonly SoundStyle PlayerDigestingFemale = new SoundStyle("V2/PlayerHandling/FemaleHit_FromDigestTick", 0, 3, SoundType.Sound) { Volume = 1f, PitchVariance = 0f };
	}

	public partial class PreyPlayer : ModPlayer
	{
		// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
		// public int[] HasBeenDigestedByNPC { get; set; }
		// public int[] HasBeenDigestedByNPCTotal { get; set; }

		public bool Digested { get; set; }

		public (int _swallowCount, int _gurgleCount) _timesEaten;
		public int TotalTimesSwallowed
		{
			get => _timesEaten._swallowCount;
			set => _timesEaten._swallowCount = value;
		}
		public int TotalTimesDigested
		{
			get => _timesEaten._gurgleCount;
			set => _timesEaten._gurgleCount = value;
		}

		public StatModifier TakenDigestionDamageModifier;

		public double SoftenedDigestionDamageTaken { get; set; }
		public StatModifier SoftenedDigestionDamageModifier;
		public StatModifier SoftenedDigestionDamageThresholdModifier;
		public int SoftenedWearoffDelay { get; set; }
		public static int SoftenedWearoffMaxDelay => V2Utils.SensibleTime(seconds: 2, frames: 30);
		public StatModifier SoftenedWearoffRateModifier;
		public int SoftenedStacks => Math.Min(Softened.MaxStacks, (int)Math.Floor((double)SoftenedDigestionDamageTaken / (Player.statLifeMax * Softened.MaxHealthDigestedForOneStack(Player))));

		public bool PredScanner { get; set; }
		public bool PerfectMeal { get; set; }

		public bool GuttedGaze { get; set; }
		public Entity GuttedGazePred { get; set; }

		public override void Initialize()
		{
			Digested = false;
			// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
			// Player.AsPrey().HasBeenDigestedByNPC = new int[NPCLoader.NPCCount];
			// Player.AsPrey().HasBeenDigestedByNPCTotal = new int[NPCLoader.NPCCount];

			SoftenedDigestionDamageTaken = 0;
			SoftenedWearoffDelay = 0;
		}

		public override void OnEnterWorld()
		{
			Digested = false;

			SoftenedDigestionDamageTaken = 0;
			SoftenedWearoffDelay = 0;
		}

		public override void ResetEffects()
		{
			Digested = false;

			StruggleDamageModifier = StatModifier.Default;

			TakenDigestionDamageModifier = StatModifier.Default;

			if (!Player.HasBuff(ModContent.BuffType<Softened>()))
				Player.AddBuff(ModContent.BuffType<Softened>(), 3);
			SoftenedDigestionDamageModifier = StatModifier.Default;
			SoftenedWearoffRateModifier = StatModifier.Default;
			if (SoftenedWearoffDelay > 0)
				SoftenedWearoffDelay--;

			PredScanner = false;

			PerfectMeal = false;

			GuttedGaze = false;
		}

		public override void UpdateDead()
		{
			SoftenedDigestionDamageTaken = 0;
			SoftenedWearoffDelay = 0;
		}

		public override void ModifyScreenPosition()
		{
			if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer || !GuttedGaze)
				return;

			if (GuttedGazePred is not null && GuttedGazePred.active)
				Main.screenPosition = GuttedGazePred.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

			bool bPressed = V2.RespawnAfterDigestionHotkey.JustPressed;
			if (bPressed)
				Player.respawnTimer = 0;
			else
				Player.respawnTimer = 888;
		}

		public override void PreUpdateMovement()
		{
			if (Player.wet)
				SoftenedWearoffRateModifier *= 2.0f;

			if (SoftenedWearoffDelay <= 0 && SoftenedDigestionDamageTaken > 0)
			{
				SoftenedDigestionDamageTaken -= SoftenedWearoffRateModifier.ApplyTo((float)(25.0 / 60.0));
				if (SoftenedDigestionDamageTaken < 0)
					SoftenedDigestionDamageTaken = 0;
			}
		}

		public override void PostUpdateBuffs()
		{
			Player.statDefense *= (float)(1.0 - (Softened.DefenseReductionPerStack * SoftenedStacks));
			Player.AsFood().TakenDigestionDamageModifier *= (float)(1.0 + (Softened.DigestionDamageIncreasePerStack * SoftenedStacks));
		}

		public override void PostItemCheck()
		{
			if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer && V2.FeedHotkey.JustPressed)
			{
				if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
					Main.NewText("Attempting to force-feed " + Player.name + " to nearby predators...");
				if (Player.CurrentCaptor() is not null)
				{
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
						Main.NewText("Force-feed attempt failed; " + Player.name + " is already busy being food.");
					return;
				}
				string predType = "none";
				int predIndex = -1;
				Vector2 playerLocation = Player.MountedCenter;
				Vector2 cursorLocation = Main.MouseWorld;
				double maxDistanceFromPlayer = V2Utils.TileCountAsPixelCount(4.25);
				double maxDistanceFromCursor = 2000;
				for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
				{
					NPC potentialPred = Main.npc[npcIndex];
					if (!potentialPred.active)
						continue;

					if (potentialPred.CurrentCaptor() is not null)
						continue;

					switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
					{
						default:
							// do absolutely fucking nothing lmao
							break;
						case "No Male":
							if (potentialPred.AsV2NPC().Gender == EntityGender.Male)
								continue;
							break;
						case "No Female":
							if (potentialPred.AsV2NPC().Gender == EntityGender.Female)
								continue;
							break;
						case "No M or F...but why?":
							if (potentialPred.AsV2NPC().Gender != EntityGender.Other)
								continue;
							break;
					}

					if (!potentialPred.AsPred().CanBeForceFed.Invoke(potentialPred))
						continue;

					if (potentialPred.Distance(playerLocation) >= maxDistanceFromPlayer)
						continue;

					if (potentialPred.Distance(cursorLocation) < maxDistanceFromCursor)
					{
						predIndex = npcIndex;
						predType = "NPC";
						maxDistanceFromCursor = potentialPred.Distance(cursorLocation);
					}
				}
				for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
				{
					Player potentialPred = Main.player[playerIndex];
					if (!potentialPred.active || potentialPred.dead || potentialPred.whoAmI == Player.whoAmI)
						continue;

					if (potentialPred.CurrentCaptor() is not null)
						continue;

					switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
					{
						default:
							// do absolutely fucking nothing lmao
							break;
						case "No Male":
							if (potentialPred.Male)
								continue;
							break;
						case "No Female":
							if (!potentialPred.Male)
								continue;
							break;
						case "No M or F...but why?":
							continue;
					}

					if (potentialPred.Distance(playerLocation) >= maxDistanceFromPlayer)
						continue;

					if (potentialPred.Distance(cursorLocation) < maxDistanceFromCursor)
					{
						predIndex = playerIndex;
						predType = "player";
						maxDistanceFromCursor = potentialPred.Distance(cursorLocation);
					}
				}
				for (int projectileIndex = 0; projectileIndex < Main.maxProjectiles; projectileIndex++)
				{
					Projectile potentialPred = Main.projectile[projectileIndex];
					if (!potentialPred.active)
						continue;

					if (potentialPred.CurrentCaptor() is not null)
						continue;

					switch (ModContent.GetInstance<V2ServerConfig>().GenderBlacklist)
					{
						default:
							// do absolutely fucking nothing lmao
							break;
						case "No Male":
							if (potentialPred.AsV2Proj().Gender == EntityGender.Male)
								continue;
							break;
						case "No Female":
							if (potentialPred.AsV2Proj().Gender == EntityGender.Female)
								continue;
							break;
						case "No M or F...but why?":
							if (potentialPred.AsV2Proj().Gender != EntityGender.Other)
								continue;
							break;
					}

					if (potentialPred.Distance(playerLocation) >= maxDistanceFromPlayer)
						continue;

					if (potentialPred.Distance(cursorLocation) < maxDistanceFromCursor)
					{
						predIndex = projectileIndex;
						predType = "projectile";
						maxDistanceFromCursor = potentialPred.Distance(cursorLocation);
					}
				}

				if (predType != "none" && predIndex != -1)
				{
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
						Main.NewText("Pred found! Pred type: " + predType + ". Pred index: " + predIndex + ".\n"
								   + "Cramming " + Player.name + " into the chosen stomach...");
					string foodFor = "";
					switch (predType)
					{
						case "NPC":
							NPC predNPC = Main.npc[predIndex];
							if (!PredNPC.CanSwallow(predNPC, Player))
								return;

							PredNPC.Swallow(predNPC, Player);
							predNPC.AsPred().OnForceFed.Invoke(predNPC, Player);
							foodFor = predNPC.FullName;
							break;
						case "player":
							Player predPlayer = Main.player[predIndex];
							if (!PredPlayer.CanSwallow(predPlayer, Player))
								return;

							PredPlayer.Swallow(predPlayer, Player);
							foodFor = predPlayer.name;
							break;
						case "projectile":
							Projectile predProjectile = Main.projectile[predIndex];
							if (!PredProjectile.CanSwallow(predProjectile, Player))
								return;

							PredProjectile.Swallow(predProjectile, Player);
							predProjectile.AsPred().OnForceFed.Invoke(predProjectile, Player);
							foodFor = predProjectile.Name;
							break;
					}
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
					{
						string debugText = "Force-feed action successful; " + Player.name + " is now food for " + foodFor + ".";
						if (Main.netMode == NetmodeID.SinglePlayer)
							Main.NewText(debugText, Color.PaleVioletRed);
						else if (Main.netMode == NetmodeID.Server)
							ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugText), Color.PaleVioletRed);
					}
				}
				else
				{
					if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
					{
						string debugText = "Force-feed action failed; there are no suitable preds nearby to turn " + Player.name + " into a snack for.";
						if (Main.netMode == NetmodeID.SinglePlayer)
							Main.NewText(debugText, Color.PaleVioletRed);
						else if (Main.netMode == NetmodeID.Server)
							ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugText), Color.PaleVioletRed);
					}
					return;
				}
			}
		}

		public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
		{
			if (npc.CurrentCaptor() is not null || Player.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public override bool CanBeHitByProjectile(Projectile proj)
		{
			if (Player.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public override void NaturalLifeRegen(ref float regen)
		{
			if (Player.CurrentCaptor() is not null)
			{
				Player.lifeRegen = 0;
				Player.lifeRegenTime = 0;
				Player.lifeRegenCount = 0;
				regen = 0;
			}
		}

		/// <summary>
		/// Deals the given amount of digestion damage to the player, respecting damage variation and luck.
		/// </summary>
		/// <param name="pred">The pred currently digesting this player.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick kills the player.</returns>
		public bool TakeDigestionDamage(Entity pred, double digestionDamage)
		{
			int trueDigestionDamage = Main.DamageVar((float)digestionDamage, Player.luck);

			for (int i = 0; i < 10; i++)
			{
				Item churnableEquip = Player.armor[i];
				if (churnableEquip.IsAir)
					continue;

				if (churnableEquip.AsFood().MaxHealth == -1 || churnableEquip.AsFood().Health <= 0)
					continue;

				churnableEquip.TakeDigestionDamage(pred, trueDigestionDamage, false, Player.whoAmI);
			}
			if (ModContent.GetInstance<V2ServerConfig>().DefenseInDigestionCalcs)
				trueDigestionDamage -= Player.statDefense;
			trueDigestionDamage = (int)Math.Round((float)trueDigestionDamage * (1f - Player.endurance));
			trueDigestionDamage = (int)Math.Round(TakenDigestionDamageModifier.ApplyTo(trueDigestionDamage));
			if (trueDigestionDamage < 1)
				trueDigestionDamage = 1;
			SoftenedDigestionDamageTaken += SoftenedDigestionDamageModifier.ApplyTo(trueDigestionDamage);
			SoftenedWearoffDelay = SoftenedWearoffMaxDelay;
			Player.statLife -= trueDigestionDamage;
			switch (Main.netMode)
			{
				case NetmodeID.SinglePlayer:
					if (!ModContent.GetInstance<V2ClientConfig>().ShowChurnDamageNumbers)
						break;

					CombatText digestionDamageText = Main.combatText[CombatText.NewText(
						Player.Hitbox,
						Color.DarkGreen,
						trueDigestionDamage,
						false,
						true
					)];
					digestionDamageText.position.X = pred.Center.X + (pred.direction * 28);
					digestionDamageText.position.Y = Player.Center.Y + (Player.height / 5f);
					digestionDamageText.velocity.X = pred.direction * 2.5f;
					digestionDamageText.velocity.Y = -4f;
					break;
				case NetmodeID.Server:
					ModPacket digestionDamageTextPacket = V2.Instance.GetPacket();
					digestionDamageTextPacket.Write((byte)V2.MessageType.SyncDigestionCombatTextForPreyPlayer);
					digestionDamageTextPacket.Write(Player.whoAmI);
					digestionDamageTextPacket.Write(trueDigestionDamage);
					digestionDamageTextPacket.Write(pred.Center.X + (pred.direction * 28));
					digestionDamageTextPacket.Write(Player.Center.Y + (Player.height / 5f));
					digestionDamageTextPacket.Write(pred.direction * 2.5f);
					digestionDamageTextPacket.Write(-4f);
					digestionDamageTextPacket.Send();
					break;
				case NetmodeID.MultiplayerClient:
					// here we do nothing because the packet takes care of this
					break;
			}
			SoundEngine.PlaySound(Player.Male ? PreyPlayerDigestionSounds.PlayerDigestingMale : PreyPlayerDigestionSounds.PlayerDigestingFemale, pred.position);
			if (Player.statLife <= 0)
			{
				Digested = true;
				TotalTimesDigested += 1;
				if (pred is NPC predNPC)
				{
					// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
					// Player.AsPrey().HasBeenDigestedByNPC[predNPC.type] += 1;
					// Player.AsPrey().HasBeenDigestedByNPCTotal[predNPC.type] += 1;
					Player.KillMe(
						PlayerDeathReason.ByCustomReason(PredNPC.GetDigestedPlayerDeathReason(predNPC, Player)),
						trueDigestionDamage,
						0
					);
				}
				else if (pred is Player predPlayer)
				{
					Player.KillMe(
						PlayerDeathReason.ByCustomReason(PredPlayer.GetDigestedPlayerDeathReason(predPlayer, Player)),
						trueDigestionDamage,
						0
					);
				}
				else if (pred is Projectile predProjectile)
				{
					Player.KillMe(
						PlayerDeathReason.ByCustomReason(PredProjectile.GetDigestedPlayerDeathReason(predProjectile, Player)),
						trueDigestionDamage,
						0
					);
				}
				else
				{
					Player.KillMe(
						PlayerDeathReason.ByCustomReason(Player.name + " was digested."),
						trueDigestionDamage,
						0
					);
				}
				if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer && ModContent.GetInstance<V2ClientConfig>().TheGutSlutVisionOMatic)
				{
					GuttedGaze = true;
					GuttedGazePred = pred;
				}
				return true;
			}
			return false;
		}

		public override bool PreKill(
			double damage,
			int hitDirection,
			bool pvp,
			ref bool playSound,
			ref bool genGore,
			ref PlayerDeathReason damageSource
		)
		{
			if (Digested)
			{
				playSound = false;
				genGore = false;
			}
			if (damageSource.SourceOtherIndex == 1)
			{
				if (TotalTimesDigested >= 20)
				{
					damageSource.CustomReason = NetworkText.FromKey(
						Main.rand.NextFromList(
							"Mods.V2.Death.DrownedPlayer.GutSlut.1",
							"Mods.V2.Death.DrownedPlayer.GutSlut.2",
							"Mods.V2.Death.DrownedPlayer.GutSlut.3"
						),
						Player.name
					);
				}
			}
			if (Player.CurrentCaptor() is not null && !Digested)
				Player.CurrentCaptor().Prey.RemoveAll(x => x.Type == PreyType.Player && x.Instance.whoAmI == Player.whoAmI);

			return true;
		}

		public override void HideDrawLayers(PlayerDrawSet drawInfo)
		{
			foreach (PlayerDrawLayer drawLayer in PlayerDrawLayerLoader.Layers)
			{
				if (!Main.gameMenu && (Player.CurrentCaptor() is not null || Digested))
					drawLayer.Hide();
			}
		}

		public override void SaveData(TagCompound tag)
		{
			// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
			// tag["hasBeenEatenBy"] = Player.AsPrey().HasBeenDigestedByNPC.ToList();
			// tag["hasBeenEatenByTotal"] = Player.AsPrey().HasBeenDigestedByNPCTotal.ToList();
		}

		public override void LoadData(TagCompound tag)
		{
			// uncomment once achievements are available so the Ascended Acolyte race is...relatively fair for everyone
			// Player.AsPrey().HasBeenDigestedByNPC = tag.GetList<int>("hasBeenEatenBy").ToArray();
			// Player.AsPrey().HasBeenDigestedByNPCTotal = tag.GetList<int>("hasBeenEatenByTotal").ToArray();
		}
	}
}