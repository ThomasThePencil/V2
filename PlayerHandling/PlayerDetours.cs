using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;
using V2.Core;
using V2.Items;
using V2.Items.Voraria.Tools;
using V2.Projectiles;
using V2.StatusEffects.Voraria.Debuffs;
using V2.Tiles;

namespace V2.PlayerHandling
{
	public static class PlayerDetours
	{
		public static void Detour_UpdateLifeRegen(Player player)
		{
			bool shinyStoneShouldEverFuckingWork = false;
			if (player.shinyStone && player.velocity.Length() < 0.05f && player.itemAnimation == 0)
				shinyStoneShouldEverFuckingWork = true;

			player.AsV2Player().healthRegenTime += 1.0;
			foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
			{
				healthRegenEffect.modifyHealthRegenTimeMethod?.Invoke(
					player,
					ref player.AsV2Player().healthRegenTime
				);
			}
			double oneMinuteFrameCount = (double)V2Utils.SensibleTime(
				minutes: 1
			);
			if (player.AsV2Player().healthRegenTime >= oneMinuteFrameCount)
				player.AsV2Player().healthRegenTime = oneMinuteFrameCount;

			player.AsV2Player().HealthRegenNatural.baseRegen = 0.0;
			player.AsV2Player().HealthRegenNatural.additiveRegenModifier = 1.0;
			player.AsV2Player().HealthRegenNatural.flatRegenBonus = 0.0;
			player.AsV2Player().HealthRegenArtificial.baseRegen = 0.0;
			player.AsV2Player().HealthRegenArtificial.additiveRegenModifier = 1.0;
			player.AsV2Player().HealthRegenArtificial.flatRegenBonus = 0.0;
			foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
			{
				if (healthRegenEffect.natural)
					player.AsV2Player().HealthRegenNatural.baseRegen += (float)healthRegenEffect.healthPerSecond.Invoke(player);
				else
					player.AsV2Player().HealthRegenArtificial.baseRegen += (float)healthRegenEffect.healthPerSecond.Invoke(player);
			}

			foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
			{
				healthRegenEffect.modifyTotalHealthRegenMethod?.Invoke(
					player,
					ref player.AsV2Player().HealthRegenNatural.additiveRegenModifier,
					ref player.AsV2Player().HealthRegenNatural.multiplicativeRegenModifier,
					ref player.AsV2Player().HealthRegenArtificial.additiveRegenModifier,
					ref player.AsV2Player().HealthRegenArtificial.multiplicativeRegenModifier
				);
			}

			double naturalHealthRegenCount =
				(player.AsV2Player().HealthRegenNatural.baseRegen * player.AsV2Player().HealthRegenNatural.additiveRegenModifier)
			   + player.AsV2Player().HealthRegenNatural.flatRegenBonus;
			double artificialHealthRegenCount =
				(player.AsV2Player().HealthRegenArtificial.baseRegen * player.AsV2Player().HealthRegenArtificial.additiveRegenModifier)
			   + player.AsV2Player().HealthRegenArtificial.flatRegenBonus;
			player.AsV2Player().healthRegenCount += naturalHealthRegenCount + artificialHealthRegenCount;
			while (player.AsV2Player().healthRegenCount >= 60.0)
			{
				player.AsV2Player().healthRegenCount -= 60.0;
				if (player.statLife < player.statLifeMax2)
				{
					player.statLife++;
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, 1);
					}
				}

				if (player.statLife > player.statLifeMax2)
					player.statLife = player.statLifeMax2;
			}

			while (player.AsV2Player().healthRegenCount <= -60.0)
			{
				if (player.AsV2Player().healthRegenCount <= -240.0)
				{
					player.AsV2Player().healthRegenCount += 240.0;
					player.statLife -= 4;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 4, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -4);
					}
				}
				else if (player.AsV2Player().healthRegenCount <= -180.0)
				{
					player.AsV2Player().healthRegenCount += 180.0;
					player.statLife -= 3;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 3, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -3);
					}
				}
				else if (player.AsV2Player().healthRegenCount <= -120.0)
				{
					player.AsV2Player().healthRegenCount += 120.0;
					player.statLife -= 2;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 2, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -2);
					}
				}
				else
				{
					player.AsV2Player().healthRegenCount += 60.0;
					player.statLife--;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 1, dramatic: false, dot: true);
					foreach (HealthRegenEffect healthRegenEffect in player.AsV2Player().HealthRegenEffects)
					{
						healthRegenEffect.onHealthAdjustmentMethod?.Invoke(player, -1);
					}
				}

				if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
				{
					if (player.poisoned || player.venom)
						player.KillMe(PlayerDeathReason.ByOther(9), 10.0, 0);
					else if (player.electrified)
						player.KillMe(PlayerDeathReason.ByOther(10), 10.0, 0);
					else
						player.KillMe(PlayerDeathReason.ByOther(8), 10.0, 0);

					return;
				}
			}

			// compatibility with vanilla-style health regen effects
			PlayerLoader.UpdateBadLifeRegen(player);

			player.lifeRegenTime++;
			if (player.lifeRegenTime >= 3600)
				player.lifeRegenTime = 3600;

			PlayerLoader.UpdateLifeRegen(player);
			float num5 = 0f;
			PlayerLoader.NaturalLifeRegen(player, ref num5);
			float num7 = (float)player.statLifeMax2 / 400f * 0.85f + 0.15f;
			num5 *= num7;
			player.lifeRegen += (int)Math.Round(num5);
			player.lifeRegenCount += player.lifeRegen;

			if (shinyStoneShouldEverFuckingWork && player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
			{
				player.lifeRegenCount++;
				if (shinyStoneShouldEverFuckingWork && (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30)))
				{
					int num8 = Dust.NewDust(player.position, player.width, player.height, DustID.Pixie, 0f, 0f, 200, default(Color), 0.5f);
					Main.dust[num8].noGravity = true;
					Main.dust[num8].velocity *= 0.75f;
					Main.dust[num8].fadeIn = 1.3f;
					Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
					vector.Normalize();
					vector *= (float)Main.rand.Next(50, 100) * 0.04f;
					Main.dust[num8].velocity = vector;
					vector.Normalize();
					vector *= 34f;
					Main.dust[num8].position = player.Center - vector;
				}
			}

			while (player.lifeRegenCount >= 120)
			{
				player.lifeRegenCount -= 120;
				if (player.statLife < player.statLifeMax2)
				{
					player.statLife++;
					if (player.crimsonRegen)
					{
						for (int i = 0; i < 10; i++)
						{
							int num9 = Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 0f, 0f, 175, default(Color), 1.75f);
							Main.dust[num9].noGravity = true;
							Main.dust[num9].velocity *= 0.75f;
							int num10 = Main.rand.Next(-40, 41);
							int num11 = Main.rand.Next(-40, 41);
							Main.dust[num9].position.X += num10;
							Main.dust[num9].position.Y += num11;
							Main.dust[num9].velocity.X = (float)(-num10) * 0.075f;
							Main.dust[num9].velocity.Y = (float)(-num11) * 0.075f;
						}
					}
				}

				if (player.statLife > player.statLifeMax2)
					player.statLife = player.statLifeMax2;
			}

			if (player.burned || player.suffocating || (player.tongued && Main.expertMode))
			{
				while (player.lifeRegenCount <= -600)
				{
					player.lifeRegenCount += 600;
					player.statLife -= 5;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 5, dramatic: false, dot: true);
					if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
					{
						if (player.suffocating)
							player.KillMe(PlayerDeathReason.ByOther(7), 10.0, 0);
						else
							player.KillMe(PlayerDeathReason.ByOther(8), 10.0, 0);
					}
				}

				return;
			}

			if (player.starving)
			{
				int num12 = player.statLifeMax2 / 50;
				if (num12 < 2)
					num12 = 2;

				int num13 = (player.ZoneDesert || player.ZoneSnow) ? (num12 * 2) : num12;
				int num14 = 120 * num12;
				while (player.lifeRegenCount <= -num14)
				{
					player.lifeRegenCount += num14;
					player.statLife -= num13;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, num13, dramatic: false, dot: true);
					if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
						player.KillMe(PlayerDeathReason.ByOther(18), 10.0, 0);
				}

				return;
			}

			while (player.lifeRegenCount <= -120)
			{
				if (player.lifeRegenCount <= -480)
				{
					player.lifeRegenCount += 480;
					player.statLife -= 4;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 4, dramatic: false, dot: true);
				}
				else if (player.lifeRegenCount <= -360)
				{
					player.lifeRegenCount += 360;
					player.statLife -= 3;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 3, dramatic: false, dot: true);
				}
				else if (player.lifeRegenCount <= -240)
				{
					player.lifeRegenCount += 240;
					player.statLife -= 2;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 2, dramatic: false, dot: true);
				}
				else
				{
					player.lifeRegenCount += 120;
					player.statLife--;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.LifeRegen, 1, dramatic: false, dot: true);
				}

				if (player.statLife <= 0 && player.whoAmI == Main.myPlayer)
				{
					if (player.poisoned || player.venom)
						player.KillMe(PlayerDeathReason.ByOther(9), 10.0, 0);
					else if (player.electrified)
						player.KillMe(PlayerDeathReason.ByOther(10), 10.0, 0);
					else
						player.KillMe(PlayerDeathReason.ByOther(8), 10.0, 0);
				}
			}
		}



		public static void Detour_UpdateManaRegen(Player player)
		{
			player.AsV2Player().manaRegenDelay -= 1.0;
			foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
			{
				manaRegenEffect.modifyManaRegenDelayMethod?.Invoke(
					player,
					ref player.AsV2Player().manaRegenDelay
				);
			}
			double oneMinuteFrameCount = (double)V2Utils.SensibleTime(
				minutes: 1
			);
			if (player.AsV2Player().manaRegenDelay >= oneMinuteFrameCount)
				player.AsV2Player().manaRegenDelay = oneMinuteFrameCount;

			player.AsV2Player().ManaRegenNatural.baseRegen = 0.0;
			player.AsV2Player().ManaRegenNatural.additiveRegenModifier = 1.0;
			player.AsV2Player().ManaRegenNatural.flatRegenBonus = 0.0;
			player.AsV2Player().ManaRegenNatural.multiplicativeRegenModifier = 1.0;
			player.AsV2Player().ManaRegenArtificial.baseRegen = 0.0;
			player.AsV2Player().ManaRegenArtificial.additiveRegenModifier = 1.0;
			player.AsV2Player().ManaRegenArtificial.flatRegenBonus = 0.0;
			player.AsV2Player().ManaRegenArtificial.multiplicativeRegenModifier = 1.0;
			foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
			{
				if (manaRegenEffect.natural)
					player.AsV2Player().ManaRegenNatural.baseRegen += (float)manaRegenEffect.manaPerSecond.Invoke(player);
				else
					player.AsV2Player().ManaRegenArtificial.baseRegen += (float)manaRegenEffect.manaPerSecond.Invoke(player);
			}

			foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
			{
				manaRegenEffect.modifyTotalManaRegenMethod?.Invoke(
					player,
					ref player.AsV2Player().ManaRegenNatural.additiveRegenModifier,
					ref player.AsV2Player().ManaRegenNatural.multiplicativeRegenModifier,
					ref player.AsV2Player().ManaRegenArtificial.additiveRegenModifier,
					ref player.AsV2Player().ManaRegenArtificial.multiplicativeRegenModifier
				);
			}

			double naturalManaRegenCount =
				(player.AsV2Player().ManaRegenNatural.baseRegen * player.AsV2Player().ManaRegenNatural.additiveRegenModifier)
			   + player.AsV2Player().ManaRegenNatural.flatRegenBonus;
			double artificialManaRegenCount =
				(player.AsV2Player().ManaRegenArtificial.baseRegen * player.AsV2Player().ManaRegenArtificial.additiveRegenModifier)
			   + player.AsV2Player().ManaRegenArtificial.flatRegenBonus;
			player.AsV2Player().manaRegenCount += naturalManaRegenCount + artificialManaRegenCount;
			while (player.AsV2Player().manaRegenCount >= 60.0)
			{
				player.AsV2Player().manaRegenCount -= 60.0;
				if (player.statMana < player.statManaMax2)
				{
					player.statMana++;
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, 1);
					}
				}

				if (player.statMana > player.statManaMax2)
					player.statMana = player.statManaMax2;
			}

			while (player.AsV2Player().manaRegenCount <= -60.0)
			{
				if (player.AsV2Player().manaRegenCount <= -240.0)
				{
					player.AsV2Player().manaRegenCount += 240.0;
					player.statMana -= 4;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 4, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -4);
					}
				}
				else if (player.AsV2Player().manaRegenCount <= -180.0)
				{
					player.AsV2Player().manaRegenCount += 180.0;
					player.statMana -= 3;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 3, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -3);
					}
				}
				else if (player.AsV2Player().manaRegenCount <= -120.0)
				{
					player.AsV2Player().manaRegenCount += 120.0;
					player.statMana -= 2;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 2, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -2);
					}
				}
				else
				{
					player.AsV2Player().manaRegenCount += 60.0;
					player.statMana--;
					CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, 1, dramatic: false, dot: true);
					foreach (ManaRegenEffect manaRegenEffect in player.AsV2Player().ManaRegenEffects)
					{
						manaRegenEffect.onManaAdjustmentMethod?.Invoke(player, -1);
					}
				}
			}

			// the followin' is for compatibility with vanilla-style mana regen effects
			if (player.nebulaLevelMana > 0)
			{
				int num = 6;
				player.nebulaManaCounter += player.nebulaLevelMana;
				if (player.nebulaManaCounter >= num)
				{
					player.nebulaManaCounter -= num;
					player.statMana++;
					if (player.statMana >= player.statManaMax2)
						player.statMana = player.statManaMax2;
				}
			}
			else
			{
				player.nebulaManaCounter = 0;
			}

			if (player.manaRegenDelay > 0f)
			{
				player.manaRegenDelay -= 1f;
				player.manaRegenDelay -= player.manaRegenDelayBonus;
				if (player.IsStandingStillForSpecialEffects || player.grappling[0] >= 0 || player.manaRegenBuff)
					player.manaRegenDelay -= 1f;

				if (player.usedArcaneCrystal)
					player.manaRegenDelay -= 0.05f;
			}

			if (player.manaRegenBuff && player.manaRegenDelay > 20f)
				player.manaRegenDelay = 20f;

			if (player.manaRegenDelay <= 0f)
			{
				player.manaRegenDelay = 0f;
				player.manaRegen = player.statManaMax2 / 3 + 1 + player.manaRegenBonus;
				if (player.IsStandingStillForSpecialEffects || player.grappling[0] >= 0 || player.manaRegenBuff)
					player.manaRegen += player.statManaMax2 / 3;

				if (player.usedArcaneCrystal)
					player.manaRegen += player.statManaMax2 / 50;

				float num2 = (float)player.statMana / (float)player.statManaMax2 * 0.8f + 0.2f;
				if (player.manaRegenBuff)
					num2 = 1f;

				player.manaRegen = (int)((double)((float)player.manaRegen * num2) * 1.15);
			}
			else
			{
				player.manaRegen = 0;
			}

			player.manaRegenCount += player.manaRegen;
			while (player.manaRegenCount >= 120)
			{
				bool flag = false;
				player.manaRegenCount -= 120;
				if (player.statMana < player.statManaMax2)
				{
					player.statMana++;
					flag = true;
				}

				if (player.statMana < player.statManaMax2)
					continue;

				if (player.whoAmI == Main.myPlayer && flag)
				{
					SoundEngine.PlaySound(SoundID.MaxMana);
					for (int i = 0; i < 5; i++)
					{
						int num3 = Dust.NewDust(player.position, player.width, player.height, DustID.ManaRegeneration, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
						Main.dust[num3].noLight = true;
						Main.dust[num3].noGravity = true;
						Main.dust[num3].velocity *= 0.5f;
					}
				}

				player.statMana = player.statManaMax2;
			}
		}

		public static void Detour_UpdateBuffs(Player player)
		{
			if (player.soulDrain > 0 && player.whoAmI == Main.myPlayer)
				player.AddBuff(151, 2);

			if (Main.dontStarveWorld)
				player.UpdateStarvingState(withEmote: true);

			for (int j = 0; j < Player.MaxBuffs; j++)
			{
				if (player.buffType[j] <= 0 || player.buffTime[j] <= 0)
					continue;

				if (player.whoAmI == Main.myPlayer && !BuffID.Sets.TimeLeftDoesNotDecrease[player.buffType[j]])
					player.buffTime[j]--;

				bool actuallyModifiedByVSC = V2.ModifiedStatusEffects.ContainsKey(player.buffType[j]);
				if (actuallyModifiedByVSC)
				{
					GlobalBuff buffReplacement = V2.ModifiedStatusEffects[player.buffType[j]];
					buffReplacement.Update(player.buffType[j], player, ref j);
					continue;
				}

				//TML: This will be used at the very end of player scope.
				int originalIndex = j;

				if (player.buffType[j] == 1)
				{
					player.lavaImmune = true;
					player.fireWalk = true;
					player.buffImmune[24] = true;
				}
				else if (BuffID.Sets.BasicMountData[player.buffType[j]] != null)
				{
					BuffID.Sets.BuffMountData buffMountData = BuffID.Sets.BasicMountData[player.buffType[j]];
					player.mount.SetMount(buffMountData.mountID, player, buffMountData.faceLeft);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 158)
				{
					player.manaRegenDelayBonus += 0.5f;
					player.manaRegenBonus += 10;
				}
				else if (player.buffType[j] == 159)
				{
					player.GetArmorPenetration(DamageClass.Melee) += 12;
				}
				else if (player.buffType[j] == 192)
				{
					player.pickSpeed -= 0.2f;
					player.moveSpeed += 0.2f;
				}
				else if (player.buffType[j] == 321)
				{
					player.GetCritChance(DamageClass.Generic) += 10;
					player.GetDamage(DamageClass.Summon) += 0.1f;
				}
				else if (player.buffType[j] == 3)
				{
					player.moveSpeed += 0.25f;
				}
				else if (player.buffType[j] == 4)
				{
					player.gills = true;
				}
				else if (player.buffType[j] == 5)
				{
					player.statDefense += 8;
				}
				else if (player.buffType[j] == 6)
				{
					player.manaRegenBuff = true;
				}
				else if (player.buffType[j] == 7)
				{
					player.GetDamage(DamageClass.Magic) += 0.2f;
				}
				else if (player.buffType[j] == 8)
				{
					player.slowFall = true;
				}
				else if (player.buffType[j] == 9)
				{
					player.findTreasure = true;
				}
				else if (player.buffType[j] == 343)
				{
					player.biomeSight = true;
				}
				else if (player.buffType[j] == 10)
				{
					player.invis = true;
				}
				else if (player.buffType[j] == 11)
				{
					Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 0.8f, 0.95f, 1f);
				}
				else if (player.buffType[j] == 12)
				{
					player.nightVision = true;
				}
				else if (player.buffType[j] == 13)
				{
					player.enemySpawns = true;
				}
				else if (player.buffType[j] == 14)
				{
					if (player.thorns < 1f)
						player.thorns = 1f;
				}
				else if (player.buffType[j] == 15)
				{
					player.waterWalk = true;
				}
				else if (player.buffType[j] == 16)
				{
					player.archery = true;

					//TML: Moved from PickAmmo, as StatModifier allows multiplicative buffs to be 'registered' alongside additive ones.
					player.arrowDamage *= 1.1f;
				}
				else if (player.buffType[j] == 17)
				{
					player.detectCreature = true;
				}
				else if (player.buffType[j] == 18)
				{
					player.gravControl = true;
				}
				else if (player.buffType[j] == 30)
				{
					player.bleed = true;
				}
				else if (player.buffType[j] == 31)
				{
					player.confused = true;
				}
				else if (player.buffType[j] == 32)
				{
					player.slow = true;
				}
				else if (player.buffType[j] == 35)
				{
					player.silence = true;
				}
				else if (player.buffType[j] == 160)
				{
					player.dazed = true;
				}
				else if (player.buffType[j] == 46)
				{
					player.chilled = true;
				}
				else if (player.buffType[j] == 47)
				{
					player.frozen = true;
				}
				else if (player.buffType[j] == 156)
				{
					player.stoned = true;
				}
				else if (player.buffType[j] == 69)
				{
					player.ichor = true;
					player.statDefense -= 15;
				}
				else if (player.buffType[j] == 36)
				{
					player.brokenArmor = true;
				}
				else if (player.buffType[j] == 48)
				{
					player.honey = true;
				}
				else if (player.buffType[j] == 59)
				{
					player.shadowDodge = true;
				}
				else if (player.buffType[j] == 93)
				{
					player.ammoBox = true;
				}
				else if (player.buffType[j] == 58)
				{
					player.palladiumRegen = true;
				}
				else if (player.buffType[j] == 306)
				{
					player.hasTitaniumStormBuff = true;
				}
				else if (player.buffType[j] == 88)
				{
					player.chaosState = true;
				}
				else if (player.buffType[j] == 215)
				{
					player.statDefense += 5;
				}
				else if (player.buffType[j] == 311)
				{
					player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.35f;
				}
				else if (player.buffType[j] == 308)
				{
					player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.25f;
				}
				else if (player.buffType[j] == 314)
				{
					player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
				}
				else if (player.buffType[j] == 312)
				{
					player.coolWhipBuff = true;
				}
				else if (player.buffType[j] == 63)
				{
					player.moveSpeed += 1f;
				}
				else if (player.buffType[j] == 104)
				{
					player.pickSpeed -= 0.25f;
				}
				else if (player.buffType[j] == 105)
				{
					player.lifeMagnet = true;
				}
				else if (player.buffType[j] == 106)
				{
					player.calmed = true;
				}
				else if (player.buffType[j] == 121)
				{
					player.fishingSkill += 15;
				}
				else if (player.buffType[j] == 122)
				{
					player.sonarPotion = true;
				}
				else if (player.buffType[j] == 123)
				{
					player.cratePotion = true;
				}
				else if (player.buffType[j] == 107)
				{
					player.tileSpeed += 0.25f;
					player.wallSpeed += 0.25f;
					player.blockRange++;
				}
				else if (player.buffType[j] == 108)
				{
					player.kbBuff = true;
				}
				else if (player.buffType[j] == 109)
				{
					player.ignoreWater = true;
					player.accFlipper = true;
				}
				else if (player.buffType[j] == 110)
				{
					player.maxMinions++;
				}
				else if (player.buffType[j] == 150)
				{
					player.maxMinions++;
				}
				else if (player.buffType[j] == 348)
				{
					player.maxTurrets++;
				}
				else if (player.buffType[j] == 111)
				{
					player.dangerSense = true;
				}
				else if (player.buffType[j] == 112)
				{
					player.ammoPotion = true;
				}
				else if (player.buffType[j] == 113)
				{
					player.lifeForce = true;
					player.statLifeMax2 += player.statLifeMax / 5 / 20 * 20;
				}
				else if (player.buffType[j] == 114)
				{
					player.endurance += 0.1f;
				}
				else if (player.buffType[j] == 115)
				{
					player.GetCritChance(DamageClass.Generic) += 10;
				}
				else if (player.buffType[j] == 116)
				{
					player.inferno = true;
					Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.65f, 0.4f, 0.1f);
					int num2 = 323;
					float num3 = 200f;
					bool flag = player.infernoCounter % 60 == 0;
					int damage = 20;
					if (player.whoAmI != Main.myPlayer)
						continue;

					for (int k = 0; k < 200; k++)
					{
						NPC nPC = Main.npc[k];
						if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num2] && player.CanNPCBeHitByPlayerOrPlayerProjectile(nPC) && Vector2.Distance(player.Center, nPC.Center) <= num3)
						{
							if (nPC.FindBuffIndex(num2) == -1)
								nPC.AddBuff(num2, 120);

							if (flag)
								player.ApplyDamageToNPC(nPC, damage, 0f, 0, crit: false);
						}
					}

					if (!player.hostile)
						continue;

					for (int l = 0; l < 255; l++)
					{
						Player otherPlayer = Main.player[l];
						if (otherPlayer == player || !otherPlayer.active || otherPlayer.dead || !otherPlayer.hostile || otherPlayer.buffImmune[num2] || (otherPlayer.team == player.team && otherPlayer.team != 0) || !(Vector2.Distance(player.Center, otherPlayer.Center) <= num3))
							continue;

						if (otherPlayer.FindBuffIndex(num2) == -1)
							otherPlayer.AddBuff(num2, 120);

						if (flag)
						{
							PlayerDeathReason reason = PlayerDeathReason.ByOther(16, otherPlayer.whoAmI);
							otherPlayer.Hurt(reason, damage, 0, pvp: true);
						}
					}
				}
				else if (player.buffType[j] == 117)
				{
					player.GetDamage(DamageClass.Generic) += 0.1f;
				}
				else if (player.buffType[j] == 119)
				{
					player.loveStruck = true;
				}
				else if (player.buffType[j] == 120)
				{
					player.stinky = true;
				}
				else if (player.buffType[j] == 124)
				{
					player.resistCold = true;
				}
				else if (player.buffType[j] == 257)
				{
					if (Main.myPlayer == player.whoAmI)
					{
						if (player.buffTime[j] > 36000)
							player.luckPotion = 3;
						else if (player.buffTime[j] > 18000)
							player.luckPotion = 2;
						else
							player.luckPotion = 1;
					}
				}
				else if (player.buffType[j] == 144)
				{
					player.electrified = true;
					Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.3f, 0.8f, 1.1f);
				}
				else if (player.buffType[j] == 94)
				{
					player.manaSick = true;
					player.manaSickReduction = Player.manaSickLessDmg * ((float)player.buffTime[j] / (float)Player.manaSickTime);
				}
				else if (player.buffType[j] >= 95 && player.buffType[j] <= 97)
				{
					player.buffTime[j] = 5;
					int num4 = (byte)(1 + player.buffType[j] - 95);
					if (player.beetleOrbs > 0 && player.beetleOrbs != num4)
					{
						if (player.beetleOrbs > num4)
						{
							player.DelBuff(j);
							j--;
						}
						else
						{
							for (int m = 0; m < Player.MaxBuffs; m++)
							{
								if (player.buffType[m] >= 95 && player.buffType[m] <= 95 + num4 - 1)
								{
									player.DelBuff(m);
									m--;
								}
							}
						}
					}

					player.beetleOrbs = num4;
					if (!player.beetleDefense)
					{
						player.beetleOrbs = 0;
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.beetleBuff = true;
					}
				}
				else if (player.buffType[j] >= 170 && player.buffType[j] <= 172)
				{
					player.buffTime[j] = 5;
					int num5 = (byte)(1 + player.buffType[j] - 170);
					if (player.solarShields > 0 && player.solarShields != num5)
					{
						if (player.solarShields > num5)
						{
							player.DelBuff(j);
							j--;
						}
						else
						{
							for (int n = 0; n < Player.MaxBuffs; n++)
							{
								if (player.buffType[n] >= 170 && player.buffType[n] <= 170 + num5 - 1)
								{
									player.DelBuff(n);
									n--;
								}
							}
						}
					}

					player.solarShields = num5;
					if (!player.setSolar)
					{
						player.solarShields = 0;
						player.DelBuff(j);
						j--;
					}
				}
				else if (player.buffType[j] >= 98 && player.buffType[j] <= 100)
				{
					int num6 = (byte)(1 + player.buffType[j] - 98);
					if (player.beetleOrbs > 0 && player.beetleOrbs != num6)
					{
						if (player.beetleOrbs > num6)
						{
							player.DelBuff(j);
							j--;
						}
						else
						{
							for (int num7 = 0; num7 < Player.MaxBuffs; num7++)
							{
								if (player.buffType[num7] >= 98 && player.buffType[num7] <= 98 + num6 - 1)
								{
									player.DelBuff(num7);
									num7--;
								}
							}
						}
					}

					player.beetleOrbs = num6;
					player.GetDamage(DamageClass.Melee) += 0.1f * (float)player.beetleOrbs;
					player.GetAttackSpeed(DamageClass.Melee) += 0.1f * (float)player.beetleOrbs;
					if (!player.beetleOffense)
					{
						player.beetleOrbs = 0;
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.beetleBuff = true;
					}
				}
				else if (player.buffType[j] >= 176 && player.buffType[j] <= 178)
				{
					int num8 = player.nebulaLevelMana;
					int num9 = (byte)(1 + player.buffType[j] - 176);
					if (num8 > 0 && num8 != num9)
					{
						if (num8 > num9)
						{
							player.DelBuff(j);
							j--;
						}
						else
						{
							for (int num10 = 0; num10 < Player.MaxBuffs; num10++)
							{
								if (player.buffType[num10] >= 176 && player.buffType[num10] <= 178 + num9 - 1)
								{
									player.DelBuff(num10);
									num10--;
								}
							}
						}
					}

					player.nebulaLevelMana = num9;
					if (player.buffTime[j] == 2 && player.nebulaLevelMana > 1)
					{
						player.nebulaLevelMana--;
						player.buffType[j]--;
						player.buffTime[j] = 480;
					}
				}
				else if (player.buffType[j] >= 173 && player.buffType[j] <= 175)
				{
					int num11 = player.nebulaLevelLife;
					int num12 = (byte)(1 + player.buffType[j] - 173);
					if (num11 > 0 && num11 != num12)
					{
						if (num11 > num12)
						{
							player.DelBuff(j);
							j--;
						}
						else
						{
							for (int num13 = 0; num13 < Player.MaxBuffs; num13++)
							{
								if (player.buffType[num13] >= 173 && player.buffType[num13] <= 175 + num12 - 1)
								{
									player.DelBuff(num13);
									num13--;
								}
							}
						}
					}

					player.nebulaLevelLife = num12;
					if (player.buffTime[j] == 2 && player.nebulaLevelLife > 1)
					{
						player.nebulaLevelLife--;
						player.buffType[j]--;
						player.buffTime[j] = 480;
					}

					player.AddHealthRegenEffect(
						healthPerSecond: 3 * player.nebulaLevelLife
					);
				}
				else if (player.buffType[j] >= 179 && player.buffType[j] <= 181)
				{
					int num14 = player.nebulaLevelDamage;
					int num15 = (byte)(1 + player.buffType[j] - 179);
					if (num14 > 0 && num14 != num15)
					{
						if (num14 > num15)
						{
							player.DelBuff(j);
							j--;
						}
						else
						{
							for (int num16 = 0; num16 < Player.MaxBuffs; num16++)
							{
								if (player.buffType[num16] >= 179 && player.buffType[num16] <= 181 + num15 - 1)
								{
									player.DelBuff(num16);
									num16--;
								}
							}
						}
					}

					player.nebulaLevelDamage = num15;
					if (player.buffTime[j] == 2 && player.nebulaLevelDamage > 1)
					{
						player.nebulaLevelDamage--;
						player.buffType[j]--;
						player.buffTime[j] = 480;
					}

					player.GetDamage(DamageClass.Generic) += 0.15f * (float)player.nebulaLevelDamage;
				}
				else if (player.buffType[j] == 62)
				{
					if ((double)player.statLife <= (double)player.statLifeMax2 * 0.5)
					{
						Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.1f, 0.2f, 0.45f);
						player.iceBarrier = true;
						player.endurance += 0.25f;
						player.iceBarrierFrameCounter++;
						if (player.iceBarrierFrameCounter > 2)
						{
							player.iceBarrierFrameCounter = 0;
							player.iceBarrierFrame++;
							if (player.iceBarrierFrame >= 12)
								player.iceBarrierFrame = 0;
						}
					}
					else
					{
						player.DelBuff(j);
						j--;
					}
				}
				else if (player.buffType[j] == 49)
				{
					for (int num18 = 191; num18 <= 194; num18++)
					{
						if (player.ownedProjectileCounts[num18] > 0)
							player.pygmy = true;
					}

					if (!player.pygmy)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 83)
				{
					if (player.ownedProjectileCounts[317] > 0)
						player.raven = true;

					if (!player.raven)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 64)
				{
					if (player.ownedProjectileCounts[266] > 0)
						player.slime = true;

					if (!player.slime)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 125)
				{
					if (player.ownedProjectileCounts[373] > 0)
						player.hornetMinion = true;

					if (!player.hornetMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 126)
				{
					if (player.ownedProjectileCounts[375] > 0)
						player.impMinion = true;

					if (!player.impMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 133)
				{
					if (player.ownedProjectileCounts[390] > 0 || player.ownedProjectileCounts[391] > 0 || player.ownedProjectileCounts[392] > 0)
						player.spiderMinion = true;

					if (!player.spiderMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 134)
				{
					if (player.ownedProjectileCounts[387] > 0 || player.ownedProjectileCounts[388] > 0)
						player.twinsMinion = true;

					if (!player.twinsMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 135)
				{
					if (player.ownedProjectileCounts[393] > 0 || player.ownedProjectileCounts[394] > 0 || player.ownedProjectileCounts[395] > 0)
						player.pirateMinion = true;

					if (!player.pirateMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 214)
				{
					if (player.ownedProjectileCounts[758] > 0)
						player.vampireFrog = true;

					if (!player.vampireFrog)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 139)
				{
					if (player.ownedProjectileCounts[407] > 0)
						player.sharknadoMinion = true;

					if (!player.sharknadoMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 140)
				{
					if (player.ownedProjectileCounts[423] > 0)
						player.UFOMinion = true;

					if (!player.UFOMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 182)
				{
					if (player.ownedProjectileCounts[613] > 0)
						player.stardustMinion = true;

					if (!player.stardustMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 213)
				{
					if (player.ownedProjectileCounts[755] > 0)
						player.batsOfLight = true;

					if (!player.batsOfLight)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 216)
				{
					bool flag2 = true;
					if (player.ownedProjectileCounts[759] > 0)
					{
						player.babyBird = true;
					}
					else if (player.whoAmI == Main.myPlayer)
					{
						if (player.numMinions < player.maxMinions)
						{
							int num19 = player.FindItem(4281);
							if (num19 != -1)
							{
								Item item = player.inventory[num19];
								int num20 = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Top, Vector2.Zero, item.shoot, item.damage, item.knockBack, player.whoAmI);
								Main.projectile[num20].originalDamage = item.damage;
								player.babyBird = true;
							}
						}

						if (!player.babyBird)
						{
							player.DelBuff(j);
							j--;
							flag2 = false;
						}
					}

					if (flag2)
						player.buffTime[j] = 18000;
				}
				else if (player.buffType[j] == 325)
				{
					if (player.ownedProjectileCounts[951] > 0)
						player.flinxMinion = true;

					if (!player.flinxMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 335)
				{
					if (player.ownedProjectileCounts[970] > 0)
						player.abigailMinion = true;

					if (!player.abigailMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}

					if (player.whoAmI == Main.myPlayer)
						UpdateAbigailStatus(player);
				}
				else if (player.buffType[j] == 263)
				{
					if (player.ownedProjectileCounts[831] > 0)
						player.stormTiger = true;

					if (!player.stormTiger)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}

					if (player.whoAmI == Main.myPlayer)
						UpdateStormTigerStatus(player);
				}
				else if (player.buffType[j] == 271)
				{
					if (player.ownedProjectileCounts[864] > 0)
						player.smolstar = true;

					if (!player.smolstar)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 322)
				{
					if (player.ownedProjectileCounts[946] > 0)
						player.empressBlade = true;

					if (!player.empressBlade)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 187)
				{
					if (player.ownedProjectileCounts[623] > 0)
						player.stardustGuardian = true;

					if (!player.stardustGuardian)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 188)
				{
					if (player.ownedProjectileCounts[625] > 0)
						player.stardustDragon = true;

					if (!player.stardustDragon)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 161)
				{
					if (player.ownedProjectileCounts[533] > 0)
						player.DeadlySphereMinion = true;

					if (!player.DeadlySphereMinion)
					{
						player.DelBuff(j);
						j--;
					}
					else
					{
						player.buffTime[j] = 18000;
					}
				}
				else if (player.buffType[j] == 90)
				{
					player.mount.SetMount(0, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 128)
				{
					player.mount.SetMount(1, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 129)
				{
					player.mount.SetMount(2, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 130)
				{
					player.mount.SetMount(3, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 131)
				{
					player.ignoreWater = true;
					player.accFlipper = true;
					player.mount.SetMount(4, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 132)
				{
					player.mount.SetMount(5, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 168)
				{
					player.ignoreWater = true;
					player.accFlipper = true;
					player.mount.SetMount(12, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 141)
				{
					player.mount.SetMount(7, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 142)
				{
					player.mount.SetMount(8, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 143)
				{
					player.mount.SetMount(9, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 162)
				{
					player.mount.SetMount(10, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 193)
				{
					player.mount.SetMount(14, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 212)
				{
					player.mount.SetMount(17, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 230)
				{
					player.mount.SetMount(23, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 265)
				{
					player.canFloatInWater = true;
					player.accFlipper = true;
					player.mount.SetMount(37, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 275)
				{
					player.mount.SetMount(40, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 276)
				{
					player.mount.SetMount(41, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 277)
				{
					player.mount.SetMount(42, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 278)
				{
					player.mount.SetMount(43, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 279)
				{
					player.ignoreWater = true;
					player.accFlipper = true;
					player.mount.SetMount(44, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 280)
				{
					player.mount.SetMount(45, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 281)
				{
					player.mount.SetMount(46, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 282)
				{
					player.mount.SetMount(47, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 283)
				{
					player.mount.SetMount(48, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 305)
				{
					player.ignoreWater = true;
					player.accFlipper = true;
					player.lavaImmune = true;
					player.mount.SetMount(49, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 318)
				{
					player.mount.SetMount(50, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == 342)
				{
					player.mount.SetMount(52, player);
					player.buffTime[j] = 10;
				}
				else if (player.buffType[j] == BuffID.Horrified)
				{
					if (Main.wofNPCIndex >= 0 && Main.npc[Main.wofNPCIndex].type == NPCID.WallofFlesh)
					{
						player.gross = true;
						player.buffTime[j] = 10;
					}
					else
					{
						player.DelBuff(j);
						j--;
					}
				}
				else if (player.buffType[j] == BuffID.TheTongue)
				{
					player.buffTime[j] = 10;
					player.tongued = true;
				}
				else if (player.buffType[j] == BuffID.Sunflower)
				{
					player.moveSpeed += 0.1f;
					player.moveSpeed *= 1.1f;
					player.sunflower = true;
				}
				else if (player.buffType[j] == 19)
				{
					player.buffTime[j] = 18000;
					player.lightOrb = true;
					bool flag3 = true;
					if (player.ownedProjectileCounts[18] > 0)
						flag3 = false;

					if (flag3 && player.whoAmI == Main.myPlayer)
						Projectile.NewProjectile(player.GetSource_Buff(j), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 18, 0, 0f, player.whoAmI);
				}
				else if (player.buffType[j] == 155)
				{
					player.buffTime[j] = 18000;
					player.crimsonHeart = true;
					bool flag4 = true;
					if (player.ownedProjectileCounts[500] > 0)
						flag4 = false;

					if (flag4 && player.whoAmI == Main.myPlayer)
						Projectile.NewProjectile(player.GetSource_Buff(j), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 500, 0, 0f, player.whoAmI);
				}
				else if (player.buffType[j] == 191)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.companionCube, 653);
				}
				else if (player.buffType[j] == 202)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDD2Dragon, 701);
				}
				else if (player.buffType[j] == 217)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagUpbeatStar, 764);
				}
				else if (player.buffType[j] == 219)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagBabyShark, 774);
				}
				else if (player.buffType[j] == 258)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagLilHarpy, 815);
				}
				else if (player.buffType[j] == 259)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagFennecFox, 816);
				}
				else if (player.buffType[j] == 260)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagGlitteryButterfly, 817);
				}
				else if (player.buffType[j] == 261)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagBabyImp, 821);
				}
				else if (player.buffType[j] == 262)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagBabyRedPanda, 825);
				}
				else if (player.buffType[j] == 264)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagPlantero, 854);
				}
				else if (player.buffType[j] == 266)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDynamiteKitten, 858);
				}
				else if (player.buffType[j] == 267)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagBabyWerewolf, 859);
				}
				else if (player.buffType[j] == 268)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagShadowMimic, 860);
				}
				else if (player.buffType[j] == 274)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagVoltBunny, 875);
				}
				else if (player.buffType[j] == 284)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagKingSlimePet, 881);
				}
				else if (player.buffType[j] == 285)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagEyeOfCthulhuPet, 882);
				}
				else if (player.buffType[j] == 286)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagEaterOfWorldsPet, 883);
				}
				else if (player.buffType[j] == 287)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagBrainOfCthulhuPet, 884);
				}
				else if (player.buffType[j] == 288)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagSkeletronPet, 885);
				}
				else if (player.buffType[j] == 289)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagQueenBeePet, 886);
				}
				else if (player.buffType[j] == 290)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDestroyerPet, 887);
				}
				else if (player.buffType[j] == 291)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagTwinsPet, 888);
				}
				else if (player.buffType[j] == 292)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagSkeletronPrimePet, 889);
				}
				else if (player.buffType[j] == 293)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagPlanteraPet, 890);
				}
				else if (player.buffType[j] == 294)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagGolemPet, 891);
				}
				else if (player.buffType[j] == 295)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDukeFishronPet, 892);
				}
				else if (player.buffType[j] == 296)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagLunaticCultistPet, 893);
				}
				else if (player.buffType[j] == 297)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagMoonLordPet, 894);
				}
				else if (player.buffType[j] == 298)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagFairyQueenPet, 895);
				}
				else if (player.buffType[j] == 299)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagPumpkingPet, 896);
				}
				else if (player.buffType[j] == 300)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagEverscreamPet, 897);
				}
				else if (player.buffType[j] == 301)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagIceQueenPet, 898);
				}
				else if (player.buffType[j] == 302)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagMartianPet, 899);
				}
				else if (player.buffType[j] == 303)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDD2OgrePet, 900);
				}
				else if (player.buffType[j] == 304)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDD2BetsyPet, 901);
				}
				else if (player.buffType[j] == 317)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagQueenSlimePet, 934);
				}
				else if (player.buffType[j] == 327)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagBerniePet, 956);
				}
				else if (player.buffType[j] == 328)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagGlommerPet, 957);
				}
				else if (player.buffType[j] == 329)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDeerclopsPet, 958);
				}
				else if (player.buffType[j] == 330)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagPigPet, 959);
				}
				else if (player.buffType[j] == 331)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagChesterPet, 960);
				}
				else if (player.buffType[j] == 341)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagKingSlimePet, 881);
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagQueenSlimePet, 934);
				}
				else if (player.buffType[j] == 345)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagJunimoPet, 994);
				}
				else if (player.buffType[j] == 349)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagBlueChickenPet, 998);
				}
				else if (player.buffType[j] == 351)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagSpiffo, 1003);
				}
				else if (player.buffType[j] == 352)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagCaveling, 1004);
				}
				else if (player.buffType[j] == 354)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDirtiestBlock, 1018);
				}
				else if (player.buffType[j] == 200)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDD2Gato, 703);
				}
				else if (player.buffType[j] == 201)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagDD2Ghost, 702);
				}
				else if (player.buffType[j] == 218)
				{
					player.BuffHandle_SpawnPetIfNeededAndSetTime(j, ref player.petFlagSugarGlider, 765);
				}
				else if (player.buffType[j] == 190)
				{
					player.buffTime[j] = 18000;
					player.suspiciouslookingTentacle = true;
					bool flag5 = true;
					if (player.ownedProjectileCounts[650] > 0)
						flag5 = false;

					if (flag5 && player.whoAmI == Main.myPlayer)
						Projectile.NewProjectile(player.GetSource_Buff(j), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 650, 0, 0f, player.whoAmI);
				}
				else if (player.buffType[j] == 27 || player.buffType[j] == 101 || player.buffType[j] == 102)
				{
					player.buffTime[j] = 18000;
					bool flag6 = true;
					int num21 = 72;
					if (player.buffType[j] == 27)
						player.blueFairy = true;

					if (player.buffType[j] == 101)
					{
						num21 = 86;
						player.redFairy = true;
					}

					if (player.buffType[j] == 102)
					{
						num21 = 87;
						player.greenFairy = true;
					}

					if (player.head == 45 && player.body == 26 && player.legs == 25)
						num21 = 72;

					if (player.ownedProjectileCounts[num21] > 0)
						flag6 = false;

					if (flag6 && player.whoAmI == Main.myPlayer)
						Projectile.NewProjectile(player.GetSource_Buff(j), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, num21, 0, 0f, player.whoAmI);
				}
				else if (player.buffType[j] == 40)
				{
					player.buffTime[j] = 18000;
					player.bunny = true;
					bool flag7 = true;
					if (player.ownedProjectileCounts[111] > 0)
						flag7 = false;

					if (flag7 && player.whoAmI == Main.myPlayer)
						Projectile.NewProjectile(player.GetSource_Buff(j), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ProjectileID.Bunny, 0, 0f, player.whoAmI);
				}
				else if (player.buffType[j] == 148)
				{
					player.rabid = true;
					if (Main.rand.NextBool(1200))
					{
						int num22 = Main.rand.Next(6);
						float num23 = (float)Main.rand.Next(60, 100) * 0.01f;
						switch (num22)
						{
							case 0:
								player.AddBuff(BuffID.Darkness, (int)(60f * num23 * 3f));
								break;
							case 1:
								player.AddBuff(BuffID.Cursed, (int)(60f * num23 * 0.75f));
								break;
							case 2:
								player.AddBuff(BuffID.Confused, (int)(60f * num23 * 1.5f));
								break;
							case 3:
								player.AddBuff(BuffID.Slow, (int)(60f * num23 * 3.5f));
								break;
							case 4:
								player.AddBuff(BuffID.Weak, (int)(60f * num23 * 5f));
								break;
							case 5:
								player.AddBuff(BuffID.Silenced, (int)(60f * num23 * 1f));
								break;
						}
					}

					player.GetDamage(DamageClass.Generic) += 0.2f;
				}
				else if (player.buffType[j] == BuffID.BabyPenguin) player.DoPetHandlerBuff(j, ref player.penguin, ProjectileID.Penguin);
				else if (player.buffType[j] == BuffID.MagicLantern) player.DoPetHandlerBuff(j, ref player.magicLantern, ProjectileID.MagicLantern);
				else if (player.buffType[j] == BuffID.Puppy) player.DoPetHandlerBuff(j, ref player.puppy, ProjectileID.Puppy);
				else if (player.buffType[j] == BuffID.BabyGrinch) player.DoPetHandlerBuff(j, ref player.grinch, ProjectileID.BabyGrinch);
				else if (player.buffType[j] == BuffID.BlackCat) player.DoPetHandlerBuff(j, ref player.blackCat, ProjectileID.BlackCat);
				else if (player.buffType[j] == BuffID.BabyDinosaur) player.DoPetHandlerBuff(j, ref player.dino, ProjectileID.BabyDino);
				else if (player.buffType[j] == BuffID.BabyFaceMonster) player.DoPetHandlerBuff(j, ref player.babyFaceMonster, ProjectileID.BabyFaceMonster);
				else if (player.buffType[j] == BuffID.EyeballSpring) player.DoPetHandlerBuff(j, ref player.eyeSpring, ProjectileID.EyeSpring);
				else if (player.buffType[j] == BuffID.BabySnowman) player.DoPetHandlerBuff(j, ref player.snowman, ProjectileID.BabySnowman);
				else if (player.buffType[j] == BuffID.PetTurtle) player.DoPetHandlerBuff(j, ref player.turtle, ProjectileID.Turtle);
				else if (player.buffType[j] == BuffID.BabyEater) player.DoPetHandlerBuff(j, ref player.eater, ProjectileID.BabyEater);
				else if (player.buffType[j] == BuffID.BabySkeletronHead) player.DoPetHandlerBuff(j, ref player.skeletron, ProjectileID.BabySkeletronHead);
				else if (player.buffType[j] == BuffID.BabyHornet) player.DoPetHandlerBuff(j, ref player.hornet, ProjectileID.BabyHornet);
				else if (player.buffType[j] == BuffID.TikiSpirit) player.DoPetHandlerBuff(j, ref player.tiki, ProjectileID.TikiSpirit);
				else if (player.buffType[j] == BuffID.PetLizard) player.DoPetHandlerBuff(j, ref player.lizard, ProjectileID.PetLizard);
				else if (player.buffType[j] == BuffID.PetParrot) player.DoPetHandlerBuff(j, ref player.parrot, ProjectileID.Parrot);
				else if (player.buffType[j] == BuffID.BabyTruffle) player.DoPetHandlerBuff(j, ref player.truffle, ProjectileID.Truffle);
				else if (player.buffType[j] == BuffID.PetSapling) player.DoPetHandlerBuff(j, ref player.sapling, ProjectileID.Sapling);
				else if (player.buffType[j] == BuffID.CursedSapling) player.DoPetHandlerBuff(j, ref player.cSapling, ProjectileID.CursedSapling);
				else if (player.buffType[j] == BuffID.PetSpider) player.DoPetHandlerBuff(j, ref player.spider, ProjectileID.Spider);
				else if (player.buffType[j] == BuffID.Squashling) player.DoPetHandlerBuff(j, ref player.squashling, ProjectileID.Squashling);
				else if (player.buffType[j] == BuffID.Wisp) player.DoPetHandlerBuff(j, ref player.wisp, ProjectileID.Wisp);
				else if (player.buffType[j] == 60)
				{
					player.buffTime[j] = 18000;
					player.crystalLeaf = true;
					bool flag29 = true;
					for (int num24 = 0; num24 < 1000; num24++)
					{
						if (Main.projectile[num24].active && Main.projectile[num24].owner == player.whoAmI && Main.projectile[num24].type == ProjectileID.CrystalLeaf)
						{
							if (!flag29)
								Main.projectile[num24].Kill();

							flag29 = false;
						}
					}

					if (flag29 && player.whoAmI == Main.myPlayer)
						Projectile.NewProjectile(player.GetSource_Buff(j), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ProjectileID.CrystalLeaf, 0, 0f, player.whoAmI);
				}
				else if (player.buffType[j] == BuffID.ZephyrFish) player.DoPetHandlerBuff(j, ref player.zephyrfish, ProjectileID.ZephyrFish);
				else if (player.buffType[j] == BuffID.MiniMinotaur) player.DoPetHandlerBuff(j, ref player.miniMinotaur, ProjectileID.MiniMinotaur);
				else if (player.buffType[j] == 70)
				{
					player.venom = true;
				}
				else if (player.buffType[j] == 20)
				{
					player.poisoned = true;
				}
				else if (player.buffType[j] == 21)
				{
					player.potionDelay = player.buffTime[j];
				}
				else if (player.buffType[j] == 22)
				{
					player.blind = true;
				}
				else if (player.buffType[j] == 80)
				{
					player.blackout = true;
				}
				else if (player.buffType[j] == 23)
				{
					player.noItems = true;
					player.cursed = true;
				}
				else if (player.buffType[j] == 24)
				{
					player.onFire = true;
				}
				else if (player.buffType[j] == 103)
				{
					player.dripping = true;
				}
				else if (player.buffType[j] == 137)
				{
					player.drippingSlime = true;
				}
				else if (player.buffType[j] == 320)
				{
					player.drippingSparkleSlime = true;
				}
				else if (player.buffType[j] == 67)
				{
					player.burned = true;
				}
				else if (player.buffType[j] == 68)
				{
					player.suffocating = true;
				}
				else if (player.buffType[j] == 39)
				{
					player.onFire2 = true;
				}
				else if (player.buffType[j] == 323)
				{
					player.onFire3 = true;
				}
				else if (player.buffType[j] == 44)
				{
					player.onFrostBurn = true;
				}
				else if (player.buffType[j] == 324)
				{
					player.onFrostBurn2 = true;
				}
				else if (player.buffType[j] == 353)
				{
					player.shimmering = true;
					player.frozen = true;
					player.fallStart = (int)(player.position.Y / 16f);
					if (Main.myPlayer != player.whoAmI)
						continue;

					if (player.position.Y / 16f > (float)Main.UnderworldLayer)
					{
						if (Main.myPlayer == player.whoAmI)
							player.DelBuff(j);

						continue;
					}

					if (player.shimmerWet)
					{
						player.buffTime[j] = 60;
						continue;
					}

					bool flag32 = false;
					for (int num25 = (int)(player.position.X / 16f); (float)num25 <= (player.position.X + (float)player.width) / 16f; num25++)
					{
						for (int num26 = (int)(player.position.Y / 16f); (float)num26 <= (player.position.Y + (float)player.height) / 16f; num26++)
						{
							if (WorldGen.SolidTile3(num25, num26))
								flag32 = true;
						}
					}

					if (flag32)
						player.buffTime[j] = 6;
					else
						player.DelBuff(j);
				}
				else if (player.buffType[j] == 163)
				{
					player.headcovered = true;
					player.bleed = true;
				}
				else if (player.buffType[j] == 164)
				{
					player.vortexDebuff = true;
				}
				else if (player.buffType[j] == 194)
				{
					player.windPushed = true;
				}
				else if (player.buffType[j] == 195)
				{
					player.witheredArmor = true;
				}
				else if (player.buffType[j] == 205)
				{
					player.ballistaPanic = true;
				}
				else if (player.buffType[j] == 196)
				{
					player.witheredWeapon = true;
				}
				else if (player.buffType[j] == 197)
				{
					player.slowOgreSpit = true;
				}
				else if (player.buffType[j] == 198)
				{
					player.parryDamageBuff = true;
				}
				else if (player.buffType[j] == 145)
				{
					player.moonLeech = true;
				}
				else if (player.buffType[j] == 149)
				{
					player.webbed = true;
					if (player.velocity.Y != 0f)
						player.velocity = new Vector2(0f, 1E-06f);
					else
						player.velocity = Vector2.Zero;

					Player.jumpHeight = 0;
					player.gravity = 0f;
					player.moveSpeed = 0f;
					player.dash = 0;
					player.dashType = 0;
					player.noKnockback = true;
					player.RemoveAllGrapplingHooks();
				}
				else if (player.buffType[j] == 43)
				{
					player.defendedByPaladin = true;
				}
				else if (player.buffType[j] == 29)
				{
					player.GetCritChance(DamageClass.Magic) += 2;
					player.GetDamage(DamageClass.Magic) += 0.05f;
					player.statManaMax2 += 20;
					player.manaCost -= 0.02f;
				}
				else if (player.buffType[j] == 28)
				{
					if (!Main.dayTime && player.wolfAcc && !player.merman)
					{
						player.AddHealthRegenEffect(
							healthPerSecond: 0.5
						);
						player.wereWolf = true;
						player.GetCritChance(DamageClass.Melee) += 2;
						player.GetDamage(DamageClass.Melee) += 0.051f;
						player.GetAttackSpeed(DamageClass.Melee) += 0.051f;
						player.statDefense += 3;
						player.moveSpeed += 0.05f;
					}
					else
					{
						player.DelBuff(j);
						j--;
					}
				}
				else if (player.buffType[j] == 33)
				{
					player.GetDamage(DamageClass.Melee) -= 0.051f;
					player.GetAttackSpeed(DamageClass.Melee) -= 0.051f;
					player.statDefense -= 4;
					player.moveSpeed -= 0.1f;
				}
				else if (player.buffType[j] == 25)
				{
					player.tipsy = true;
					player.statDefense -= 4;
					player.GetCritChance(DamageClass.Melee) += 2;
					player.GetDamage(DamageClass.Melee) += 0.1f;
					player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
				}
				else if (player.buffType[j] == 26)
				{
					player.wellFed = true;
					player.statDefense += 2;
					player.GetCritChance(DamageClass.Generic) += 2;
					player.GetDamage(DamageClass.Generic) += 0.05f;
					player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
					player.GetKnockback(DamageClass.Summon) += 0.5f;
					player.moveSpeed += 0.2f;
					player.pickSpeed -= 0.05f;
				}
				else if (player.buffType[j] == 206)
				{
					player.wellFed = true;
					player.statDefense += 3;
					player.GetCritChance(DamageClass.Generic) += 3;
					player.GetDamage(DamageClass.Generic) += 0.075f;
					player.GetAttackSpeed(DamageClass.Melee) += 0.075f;
					player.GetKnockback(DamageClass.Summon) += 0.75f;
					player.moveSpeed += 0.3f;
					player.pickSpeed -= 0.1f;
				}
				else if (player.buffType[j] == 207)
				{
					player.wellFed = true;
					player.statDefense += 4;
					player.GetCritChance(DamageClass.Generic) += 4;
					player.GetDamage(DamageClass.Generic) += 0.1f;
					player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
					player.GetKnockback(DamageClass.Summon) += 1f;
					player.moveSpeed += 0.4f;
					player.pickSpeed -= 0.15f;
				}
				else if (player.buffType[j] == 333)
				{
					player.hungry = true;
					player.statDefense -= 2;
					player.GetCritChance(DamageClass.Generic) -= 2;
					player.GetDamage(DamageClass.Generic) -= 0.05f;
					player.GetAttackSpeed(DamageClass.Melee) -= 0.05f;
					player.GetKnockback(DamageClass.Summon) -= 0.5f;
					player.pickSpeed += 0.05f;
				}
				else if (player.buffType[j] == 334)
				{
					player.starving = true;
					player.statDefense -= 4;
					player.GetCritChance(DamageClass.Generic) -= 4;
					player.GetDamage(DamageClass.Generic) -= 0.1f;
					player.GetAttackSpeed(DamageClass.Melee) -= 0.1f;
					player.GetKnockback(DamageClass.Summon) -= 1f;
					player.pickSpeed += 0.15f;
				}
				else if (player.buffType[j] == 336)
				{
					player.heartyMeal = true;
				}
				else if (player.buffType[j] == 71)
				{
					player.meleeEnchant = 1;
				}
				else if (player.buffType[j] == 73)
				{
					player.meleeEnchant = 2;
				}
				else if (player.buffType[j] == 74)
				{
					player.meleeEnchant = 3;
				}
				else if (player.buffType[j] == 75)
				{
					player.meleeEnchant = 4;
				}
				else if (player.buffType[j] == 76)
				{
					player.meleeEnchant = 5;
				}
				else if (player.buffType[j] == 77)
				{
					player.meleeEnchant = 6;
				}
				else if (player.buffType[j] == 78)
				{
					player.meleeEnchant = 7;
				}
				else if (player.buffType[j] == 79)
				{
					player.meleeEnchant = 8;
				}

				if (j == originalIndex)
					BuffLoader.Update(player.buffType[j], player, ref j);
			}

			player.UpdateHungerBuffs();
			if (player.whoAmI == Main.myPlayer && player.luckPotion != player.oldLuckPotion)
			{
				player.luckNeedsSync = true;
				player.oldLuckPotion = player.luckPotion;
			}
		}
		private static void UpdateAbigailStatus(Player player)
		{
			int num = 963;
			if (player.ownedProjectileCounts[970] < 1)
			{
				for (int i = 0; i < 1000; i++)
				{
					Projectile projectile = Main.projectile[i];
					if (projectile.active && projectile.owner == player.whoAmI && projectile.type == num)
						projectile.Kill();
				}
			}
			else if (player.ownedProjectileCounts[num] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Misc("AbigailTierSwap"), player.Center, Vector2.Zero, num, 0, 0f, player.whoAmI);
			}
		}

		private static void UpdateStormTigerStatus(Player player)
		{
			var num = GetDesiredStormTigerMinionRank(player) switch
			{
				1 => 833,
				2 => 834,
				3 => 835,
				_ => -1,
			};
			bool flag = false;
			if (num == -1)
				flag = true;

			for (int i = 0; i < ProjectileID.Sets.StormTigerIds.Length; i++)
			{
				int num2 = ProjectileID.Sets.StormTigerIds[i];
				if (num2 != num && player.ownedProjectileCounts[num2] >= 1)
				{
					flag = true;
					break;
				}
			}

			if (flag)
			{
				for (int j = 0; j < 1000; j++)
				{
					Projectile projectile = Main.projectile[j];
					if (projectile.active && projectile.owner == player.whoAmI && projectile.type != num && ProjectileID.Sets.StormTiger[projectile.type])
						projectile.Kill();
				}
			}
			else if (player.ownedProjectileCounts[num] < 1)
			{
				int num3 = Projectile.NewProjectile(player.GetSource_Misc("StormTigerTierSwap"), player.Center, Vector2.Zero, num, 0, 0f, player.whoAmI, 0f, 1f);
				Main.projectile[num3].localAI[0] = 60f;
			}
		}

		private static int GetDesiredStormTigerMinionRank(Player player)
		{
			int result = 0;
			int num = player.ownedProjectileCounts[831];
			if (num > 0)
				result = 1;

			if (num > 3)
				result = 2;

			if (num > 6)
				result = 3;

			return result;
		}

		public static void ItemCheck_ReleaseCritter(Player player, Item sItem)
		{
			if (sItem.makeNPC == NPCID.ExplosiveBunny)
			{
				player.ApplyItemTime(sItem);
				int releasedCritterIndex = NPC.ReleaseNPC((int)player.Center.X, (int)player.Bottom.Y, sItem.makeNPC, sItem.placeStyle, player.whoAmI);
				NPC releasedCritter = Main.npc[releasedCritterIndex];
				if (sItem.AsFood().MaxHealth != 0 && sItem.AsFood().MaxHealth == releasedCritter.lifeMax)
					releasedCritter.life = sItem.AsFood().Health;
				if (Main.myPlayer == player.whoAmI && V2.SwallowHotkey.Current && PredPlayer.CanSwallow(player, releasedCritter))
					PredPlayer.Swallow(player, releasedCritter);
			}
			else if (player.position.X / 16f - (float)Player.tileRangeX - (float)sItem.tileBoost <= (float)Player.tileTargetX
				 && (player.position.X + (float)player.width) / 16f + (float)Player.tileRangeX + (float)sItem.tileBoost - 1f >= (float)Player.tileTargetX
				 && player.position.Y / 16f - (float)Player.tileRangeY - (float)sItem.tileBoost <= (float)Player.tileTargetY
				 && (player.position.Y + (float)player.height) / 16f + (float)Player.tileRangeY + (float)sItem.tileBoost - 2f >= (float)Player.tileTargetY)
			{
				int num = Main.mouseX + (int)Main.screenPosition.X;
				int num2 = Main.mouseY + (int)Main.screenPosition.Y;
				int i = num / 16;
				int j = num2 / 16;
				if (!WorldGen.SolidTile(i, j))
				{
					player.ApplyItemTime(sItem);
					int releasedCritterIndex = NPC.ReleaseNPC(num, num2, sItem.makeNPC, sItem.placeStyle, player.whoAmI);
					NPC releasedCritter = Main.npc[releasedCritterIndex];
					if (sItem.AsAnItem().ReleasedNPCNetID < 0)
						releasedCritter.SetDefaults(sItem.AsAnItem().ReleasedNPCNetID);
					if (sItem.AsFood().MaxHealth != 0 && sItem.AsFood().MaxHealth == releasedCritter.lifeMax)
						releasedCritter.life = sItem.AsFood().Health;
					if (Main.myPlayer == player.whoAmI && V2.SwallowHotkey.Current && PredPlayer.CanSwallow(player, releasedCritter))
						PredPlayer.Swallow(player, releasedCritter);
				}
			}
		}

		private static void GetGrapplingForces(Player self, Vector2 fromPosition, out int? preferredPlayerDirectionToSet, out float preferedPlayerVelocityX, out float preferedPlayerVelocityY)
		{
			bool noGravity = false;
			float num = 0f;
			float num2 = 0f;
			preferredPlayerDirectionToSet = null;
			int num3 = 0;
			for (int i = 0; i < self.grapCount; i++)
			{
				Projectile projectile = Main.projectile[self.grappling[i]];
				if (projectile.ai[0] == 2f && !projectile.position.HasNaNs())
				{
					int type = projectile.type;
					bool flag = projectile.ModProjectile != null && projectile.ModProjectile.AIType > ProjectileID.None;
					if (flag)
					{
						projectile.type = projectile.ModProjectile.AIType;
					}
					num += projectile.position.X + (float)(projectile.width / 2);
					num2 += projectile.position.Y + (float)(projectile.height / 2);
					num3++;
					if (projectile.type == 446)
					{
						Vector2 vector;
						vector = new((float)(self.controlRight.ToInt() - self.controlLeft.ToInt()), (float)(self.controlDown.ToInt() - self.controlUp.ToInt()) * self.gravDir);
						if (vector != Vector2.Zero)
						{
							vector.Normalize();
						}
						vector *= 100f;
						Vector2 vec = Vector2.Normalize(self.Center - projectile.Center + vector);
						if (vec.HasNaNs())
						{
							vec = -Vector2.UnitY;
						}
						float num4 = 200f;
						num += vec.X * num4;
						num2 += vec.Y * num4;
						noGravity = true;
					}
					else if (projectile.type == 652)
					{
						Vector2 vector2 = new Vector2((float)(self.controlRight.ToInt() - self.controlLeft.ToInt()), (float)(self.controlDown.ToInt() - self.controlUp.ToInt()) * self.gravDir).SafeNormalize(Vector2.Zero);
						Vector2 vector3 = projectile.Center - self.Center;
						Vector2 vector4 = vector3.SafeNormalize(Vector2.Zero);
						Vector2 value = Vector2.Zero;
						if (vector2 != Vector2.Zero)
						{
							value = vector4 * Vector2.Dot(vector4, vector2);
						}
						float num5 = 6f;
						if (Vector2.Dot(value, vector3) < 0f && vector3.Length() >= 600f)
						{
							num5 = 0f;
						}
						num += 0f - vector3.X + value.X * num5;
						num2 += 0f - vector3.Y + value.Y * num5;
						noGravity = true;
					}
					else if (projectile.type == 865)
					{
						Vector2 vector5 = (projectile.rotation - 1.5707964f).ToRotationVector2().SafeNormalize(Vector2.UnitY);
						Vector2 vector6 = -vector5 * 28f;
						num += vector6.X;
						num2 += vector6.Y;
						if (vector5.X != 0f)
						{
							preferredPlayerDirectionToSet = new int?(Math.Sign(vector5.X));
						}
					}
					if (flag)
					{
						projectile.type = type;
					}
					ProjectileLoader.GrappleTargetPoint(projectile, self, ref num, ref num2);
				}
			}
			if (num3 == 0)
			{
				preferedPlayerVelocityX = self.velocity.X;
				preferedPlayerVelocityY = self.velocity.Y;
				return;
			}
			float num6 = num / (float)num3;
			float num7 = num2 / (float)num3;
			preferedPlayerVelocityX = num6 - fromPosition.X;
			preferedPlayerVelocityY = num7 - fromPosition.Y;
			float num8 = (float)Math.Sqrt((double)(preferedPlayerVelocityX * preferedPlayerVelocityX + preferedPlayerVelocityY * preferedPlayerVelocityY));
			float num9 = 11f;
			if (Main.projectile[self.grappling[0]].type == 315)
			{
				num9 = 14f;
			}
			if (Main.projectile[self.grappling[0]].type == 487)
			{
				num9 = 12f;
			}
			if (Main.projectile[self.grappling[0]].type >= 646 && Main.projectile[self.grappling[0]].type <= 649)
			{
				num9 = 16f;
			}
			ProjectileLoader.GrapplePullSpeed(Main.projectile[self.grappling[0]], self, ref num9);
			float num10 = (num8 <= num9) ? 1f : (num9 / num8);
			preferedPlayerVelocityX *= num10;
			preferedPlayerVelocityY *= num10;

			//got all that? okay, now forget it.

			if (!noGravity)
			{
				double PlayerWeight = self.AsPred().StomachWeight + 1.0;
				if (self.AsV2Player().BaeTransformation)
					PlayerWeight += self.AsPred().BaeTransformation_ExtraWeight;
				else if (self.AsV2Player().KroniiTransformation)
					PlayerWeight += self.AsPred().KroniiTransformation_ExtraWeight;
				else if (self.AsV2Player().OllieTransformation)
					PlayerWeight += self.AsPred().OllieTransformation_ExtraWeight;
				else if (self.AsV2Player().SoraTransformation)
					PlayerWeight += self.AsPred().SoraTransformation_ExtraWeight;
				else if (self.AsV2Player().MintTransformation)
					PlayerWeight += self.AsPred().MintTransformation_ExtraWeight;
				float additionalWeight = ((float)Math.Max(0, PlayerWeight - 1) / 4f);
				Vector2 TargetForce = Vector2.Zero;
				float HookStrength = 0f;
				float HighestGrappleSpeed = 0f;
				bool onlyOneHook = true;
				int saidOneHook = -1;
				int hookCount = 0;
				bool isBatHook = false; //this mf
				bool Close = false;
				for (int i = 0; i < self.grapCount; i++)
				{
					Projectile projectile = Main.projectile[self.grappling[i]];
					if (projectile.ai[0] == 2f && !projectile.position.HasNaNs())
					{
						saidOneHook = projectile.whoAmI;
						hookCount++;
						int type = projectile.type;
						bool flag = projectile.ModProjectile != null && projectile.ModProjectile.AIType > ProjectileID.None;
						if (flag)
						{
							projectile.type = projectile.ModProjectile.AIType;
						}
						float distance = 1 + self.Center.Distance(projectile.Center) / 160f;
						float distance2 = 1 + self.Center.Distance(projectile.Center) / 500f;
						float adddedForce = Math.Clamp(distance2, 0, 1.25f);
						if (i > 0)
							onlyOneHook = false;
						if (distance < 1.25f)
						{
							Close = true;
						}
						TargetForce += self.Center.DirectionTo(projectile.Center) * projectile.AsV2Proj().GrappleSpeed * adddedForce;
						HookStrength += projectile.AsV2Proj().GrappleStrength;
						if (projectile.AsV2Proj().GrappleSpeed > HighestGrappleSpeed)
							HighestGrappleSpeed = projectile.AsV2Proj().GrappleSpeed;
						if (projectile.type == ProjectileID.BatHook)
							isBatHook = true;
					}
				}
				TargetForce /= 0.5f + (hookCount * 0.5f);
				HookStrength /= 0.5f + (hookCount * 0.5f);
				if (HookStrength < additionalWeight)
				{
					float modifier = (additionalWeight - HookStrength + 1) * (1 + 0.1f * additionalWeight);
					if (isBatHook)
						TargetForce /= Math.Min(modifier * 1.25f, 2);
					if (TargetForce.X != 0)
						TargetForce.X /= modifier;
					if (TargetForce.Y != 0)
						TargetForce.X /= modifier;
					TargetForce.Y += modifier;
				}
				self.velocity *= 0.95f;
				if (Close && onlyOneHook)
				{
					TargetForce *= 0.85f;
					self.velocity *= 0.85f;
				}
				Vector2 totalVelocity = self.velocity + TargetForce;
				if (onlyOneHook && saidOneHook >= 0)
				{
					float distance = self.Center.Distance(Main.projectile[saidOneHook].Center) - (TargetForce.Length() / 8);
					if (distance < 14)
					{
						totalVelocity = totalVelocity * (distance / 16);
					}
				}
				if (totalVelocity.Length() > HighestGrappleSpeed * 8)
				{
					totalVelocity.Normalize();
					totalVelocity *= HighestGrappleSpeed * 8;
				}
				if (totalVelocity.Length() < 0.03f)
					totalVelocity = Vector2.Zero;
				preferedPlayerVelocityX = totalVelocity.X;
				preferedPlayerVelocityY = totalVelocity.Y;
			}

		}
		public static void Detour_GrappleMovement(On_Player.orig_GrappleMovement orig, Player self)
		{
			if (self.grappling[0] < 0)
			{
				self.AsV2Player().GrappleLastSpeed = Vector2.Zero;
				return;
			}
			self.StopVanityActions(true);
			if (Main.myPlayer == self.whoAmI && self.mount.Active)
			{
				self.mount.Dismount(self);
			}
			self.canCarpet = true;
			self.carpetFrame = -1;
			self.wingFrame = 1;
			if (self.velocity.Y == 0f || (self.wet && (double)self.velocity.Y > -0.02 && (double)self.velocity.Y < 0.02))
			{
				self.wingFrame = 0;
			}
			if (self.wings == 4)
			{
				self.wingFrame = 3;
			}
			if (self.wings == 30)
			{
				self.wingFrame = 0;
			}
			self.RefreshMovementAbilities(true);
			self.rocketFrame = false;
			self.canRocket = false;
			self.rocketRelease = false;
			self.fallStart = (int)(self.position.Y / 16f);
			int num = -1;
			for (int i = 0; i < self.grapCount; i++)
			{
				if (Main.projectile[self.grappling[i]].type == 403)
				{
					num = i;
				}
			}
			int? preferredPlayerDirectionToSet;
			float preferedPlayerVelocityX;
			float preferedPlayerVelocityY;
			GetGrapplingForces(self, self.Center, out preferredPlayerDirectionToSet, out preferedPlayerVelocityX, out preferedPlayerVelocityY);
			if (preferedPlayerVelocityY > 0f)
			{
				self.GoingDownWithGrapple = true;
			}
			self.velocity.X = preferedPlayerVelocityX;
			self.velocity.Y = preferedPlayerVelocityY;
			if (num != -1)
			{
				Projectile projectile = Main.projectile[self.grappling[num]];
				if (projectile.position.X < self.position.X + (float)self.width && projectile.position.X + (float)projectile.width >= self.position.X && projectile.position.Y < self.position.Y + (float)self.height && projectile.position.Y + (float)projectile.height >= self.position.Y)
				{
					int num2 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
					int num3 = (int)(projectile.position.Y + (float)(projectile.height / 2)) / 16;
					self.velocity = Vector2.Zero;
					if (Main.tile[num2, num3].TileType == 314)
					{
						Vector2 Position = default(Vector2);
						Position.X = projectile.position.X + (float)(projectile.width / 2) - (float)(self.width / 2);
						Position.Y = projectile.position.Y + (float)(projectile.height / 2) - (float)(self.height / 2);
						self.RemoveAllGrapplingHooks();
						int num4 = 13;
						if (self.miscEquips[2].stack > 0 && self.miscEquips[2].mountType >= MountID.Rudolph && MountID.Sets.Cart[self.miscEquips[2].mountType] && (!self.miscEquips[2].expertOnly || Main.expertMode) && (!self.miscEquips[2].masterOnly || Main.masterMode))
						{
							num4 = self.miscEquips[2].mountType;
						}
						int num5 = self.height + Mount.GetHeightBoost(num4);
						if (Minecart.GetOnTrack(num2, num3, ref Position, self.width, num5) && !Collision.SolidCollision(Position, self.width, num5 - 20))
						{
							self.position = Position;
							DelegateMethods.Minecart.rotation = self.fullRotation;
							DelegateMethods.Minecart.rotationOrigin = self.fullRotationOrigin;
							self.mount.SetMount(num4, self, self.minecartLeft);
							Minecart.WheelSparks(self.mount.Delegations.MinecartDust, self.position, self.width, self.height, 25);
						}
					}
				}
			}
			if (self.itemAnimation == 0)
			{
				if (self.velocity.X == 0f && preferredPlayerDirectionToSet != null)
				{
					self.ChangeDir(preferredPlayerDirectionToSet.Value);
				}
				if (self.velocity.X > 0f)
				{
					self.ChangeDir(1);
				}
				if (self.velocity.X < 0f)
				{
					self.ChangeDir(-1);
				}
			}
			if (self.controlJump)
			{
				if (self.releaseJump)
				{
					if (self.velocity.Y > -0.1 && self.velocity.Y < 0.1 && !self.controlDown)
					{
						self.velocity.Y = 0f - Player.jumpSpeed;
						self.jump = Player.jumpHeight / 2;
						self.releaseJump = false;
					}
					else
					{
						self.velocity.Y = self.velocity.Y + 0.01f;
						self.releaseJump = false;
					}
					self.RefreshExtraJumps();
					self.RemoveAllGrapplingHooks();
					return;
				}
			}
			else
			{
				self.releaseJump = true;
			}
		}

		public static Item Detour_PickupItem(On_Player.orig_PickupItem orig, Player self, int playerIndex, int worldItemArrayIndex, Item itemToPickUp)
		{
			if (ItemID.Sets.NebulaPickup[itemToPickUp.type])
			{
				SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
				int num = itemToPickUp.buffType;
				itemToPickUp = new Item();
				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.NebulaLevelupRequest, -1, -1, null, playerIndex, (float)num, self.Center.X, self.Center.Y, 0, 0, 0);
				else
					self.NebulaLevelup(num);
			}
			if (itemToPickUp.type == ItemID.Heart || itemToPickUp.type == ItemID.CandyApple || itemToPickUp.type == ItemID.CandyCane)
			{
				SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
				self.Heal(20);
				itemToPickUp = new Item();
			}
			else if (itemToPickUp.type == ItemID.Star || itemToPickUp.type == ItemID.SoulCake || itemToPickUp.type == ItemID.SugarPlum)
			{
				SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
				self.statMana += 100;
				if (Main.myPlayer == self.whoAmI)
				{
					self.ManaEffect(100);
				}
				if (self.statMana > self.statManaMax2)
				{
					self.statMana = self.statManaMax2;
				}
				itemToPickUp = new Item();
			}
			else if (itemToPickUp.type == ItemID.ManaCloakStar)
			{
				SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
				self.statMana += 50;
				if (Main.myPlayer == self.whoAmI)
				{
					self.ManaEffect(50);
				}
				if (self.statMana > self.statManaMax2)
				{
					self.statMana = self.statManaMax2;
				}
				itemToPickUp = new Item();
			}
			else if (self.HasBuff<Trance>() && itemToPickUp.AsFood().Health > 0)
			{
				if (itemToPickUp.stack > 25)
				{
					itemToPickUp.stack -= 25;
					Item eatenItem = new Item();
					eatenItem.SetDefaults(itemToPickUp.type);
					eatenItem.stack = 25;
					self.ForceDropItem(self.Center, ref eatenItem, out Item itemDrop);
					PredPlayer.Swallow(self, itemDrop, ForceSwallow: true);
					itemToPickUp = self.GetItem(playerIndex, itemToPickUp, GetItemSettings.PickupItemFromWorld);
				}
				else
					PredPlayer.Swallow(self, itemToPickUp, ForceSwallow: true);
			}
			else
			{
				itemToPickUp = self.GetItem(playerIndex, itemToPickUp, GetItemSettings.PickupItemFromWorld);
			}
			Main.item[worldItemArrayIndex] = itemToPickUp;
			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, worldItemArrayIndex, 0f, 0f, 0f, 0, 0, 0);
			return itemToPickUp;
		}

		public static void KillMe(Player player, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp = false)
		{
			if (player.creativeGodMode || player.dead)
				return;

			player.StopVanityActions();
			bool playSound = true;
			bool genGore = true;
			if (!PlayerLoader.PreKill(player, dmg, hitDirection, pvp, ref playSound, ref genGore, ref damageSource))
				return;

			if (pvp)
				player.pvpDeath = true;

			if (player.trapDebuffSource)
				AchievementsHelper.HandleSpecialEvent(player, 4);

			if (Main.myPlayer == player.whoAmI)
			{
				if (player._framesLeftEligibleForDeadmansChestDeathAchievement > 0)
					AchievementsHelper.HandleSpecialEvent(player, 23);

				Main.NotifyOfEvent(GameNotificationType.SpawnOrDeath);
			}
			player.lastDeathPostion = player.Center;
			player.lastDeathTime = DateTime.Now;
			player.showLastDeath = true;
			bool overFlowing;
			int coinsOwned = (int)Utils.CoinsCount(out overFlowing, player.inventory);
			if (Main.myPlayer == player.whoAmI)
			{
				player.lostCoins = coinsOwned;
				player.lostCoinString = Main.ValueToCoins(player.lostCoins);

				MethodInfo endOngoingTorchGodEventMethod = player.GetType().GetMethod("EndOngoingTorchGodEvent", BindingFlags.Instance | BindingFlags.NonPublic);
				endOngoingTorchGodEventMethod.Invoke(player, null);

				Main.mapFullscreen = false;

				player.trashItem.SetDefaults();
				if (player.difficulty == PlayerDifficultyID.SoftCore || player.difficulty == PlayerDifficultyID.Creative)
				{
					for (int i = 0; i < 59; i++)
					{
						if (player.inventory[i].stack > 0 && ((player.inventory[i].type >= ItemID.LargeAmethyst && player.inventory[i].type <= ItemID.LargeDiamond) || player.inventory[i].type == ItemID.LargeAmber))
						{
								int num = Item.NewItem(player.GetSource_Death(), (int)player.position.X, (int)player.position.Y, player.width, player.height, player.inventory[i].type);
								Main.item[num].netDefaults(player.inventory[i].netID);
								Main.item[num].Prefix(player.inventory[i].prefix);
								Main.item[num].stack = player.inventory[i].stack;
								Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
								Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
								Main.item[num].noGrabDelay = 100;
								Main.item[num].favorited = false;
								Main.item[num].newAndShiny = false;
								if (player.CurrentCaptor() is not null)
									player.CurrentCaptor().QueueNewPrey(PreyData.NewData(Main.item[num], player.CurrentCaptor()));

								if (Main.netMode == NetmodeID.MultiplayerClient)
									NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num);

							player.inventory[i].SetDefaults();
						}
					}
				}
				else if (player.difficulty == 1)
				{
					player.DropItems();
				}
				else if (player.difficulty == 2)
				{
					player.DropItems();
					player.KillMeForGood();
				}
			}

			if (playSound)
			{
				SoundEngine.PlaySound(
					SoundID.PlayerKilled,
					player.Center
				);
			}

			player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			if (player.stoned || !genGore || player.AsFood().Digested)
			{
				player.headPosition = Vector2.Zero;
				player.bodyPosition = Vector2.Zero;
				player.legPosition = Vector2.Zero;
			}

			if (genGore && !player.AsFood().Digested)
			{
				for (int j = 0; j < 100; j++)
				{
					if (player.stoned)
					{
						Dust.NewDust(player.position, player.width, player.height, DustID.Stone, 2 * hitDirection, -2f);
					}
					else if (player.frostArmor)
					{
						int num2 = Dust.NewDust(player.position, player.width, player.height, DustID.IceTorch, 2 * hitDirection, -2f);
						Main.dust[num2].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
					}
					else if (player.boneArmor)
					{
						int num3 = Dust.NewDust(player.position, player.width, player.height, DustID.Bone, 2 * hitDirection, -2f);
						Main.dust[num3].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
					}
					else
					{
						Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 2 * hitDirection, -2f);
					}
				}
			}

			player.mount.Dismount(player);
			player.dead = true;
			player.respawnTimer = 600;
			bool flag = false;
			if (Main.netMode != NetmodeID.SinglePlayer && !pvp)
			{
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && (Main.npc[k].boss || Main.npc[k].type == NPCID.EaterofWorldsHead || Main.npc[k].type == NPCID.EaterofWorldsBody || Main.npc[k].type == NPCID.EaterofWorldsTail) && Math.Abs(player.Center.X - Main.npc[k].Center.X) + Math.Abs(player.Center.Y - Main.npc[k].Center.Y) < 4000f)
					{
						flag = true;
						break;
					}
				}
			}

			if (flag)
				player.respawnTimer += 600;

			if (Main.expertMode)
				player.respawnTimer = (int)((double)player.respawnTimer * 1.5);

			PlayerLoader.Kill(player, dmg, hitDirection, pvp, damageSource);
			player.immuneAlpha = 0;
			if (!ChildSafety.Disabled)
				player.immuneAlpha = 255;

			player.palladiumRegen = false;
			player.iceBarrier = false;
			player.crystalLeaf = false;
			NetworkText deathText = damageSource.GetDeathText(player.name);
			if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(deathText, new Color(225, 25, 25));
			else if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(deathText.ToString(), 225, 25, 25);

			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
				NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)dmg, hitDirection, pvp);

			if (player.whoAmI == Main.myPlayer && (player.difficulty == 0 || player.difficulty == 3))
			{
				if (!pvp)
				{
					player.DropCoins();
				}
				else
				{
					player.lostCoins = 0;
					player.lostCoinString = Main.ValueToCoins(player.lostCoins);
				}
			}

			if (!player.AsFood().Digested)
				player.DropTombstone(coinsOwned, deathText, hitDirection);

			if (player.whoAmI == Main.myPlayer)
			{
				try
				{
					WorldGen.saveToonWhilePlaying();
				}
				catch
				{
				}
			}
		}

		/*
		this doesn't really matter yet but is kept here for ease of access
		the Extractinator's eventual rework will make it into something you can feed extractibles to and fuck off from for a while and come back to free loot (and a fatter Extractinator)
		when this happens, I will need to redo this entire method to have a, frankly, even remotely sensible - extensible - comprehensible loot table system compared to the current approach
		DropItemFromExtractinator is not used in the final product at all but will be kept for compatibility when the time comes to rig up the detour properly
		*/
		public static void ExtractinatorUse(Player player, int extractType, int extractinatorBlockType)
		{
			int num = 5000;
			int num2 = 25;
			int num3 = 50;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			int num7 = 1;
			switch (extractType)
			{
				case ItemID.DesertFossil:
					num /= 3;
					num2 *= 2;
					num3 = 20;
					num4 = 10;
					break;
				case ItemID.OldShoe:
					num = -1;
					num2 = -1;
					num3 = -1;
					num4 = -1;
					num5 = 1;
					num7 = -1;
					break;
				case ItemID.LavaMoss:
					num = -1;
					num2 = -1;
					num3 = -1;
					num4 = -1;
					num5 = -1;
					num7 = -1;
					num6 = 1;
					break;
			}

			int extractItem = -1;
			int extractStack = 1;
			if (num4 != -1 && Main.rand.Next(num4) == 0)
			{
				extractItem = 3380;
				if (Main.rand.NextBool(5))
					extractStack += Main.rand.Next(2);

				if (Main.rand.NextBool(10))
					extractStack += Main.rand.Next(3);

				if (Main.rand.NextBool(15))
					extractStack += Main.rand.Next(4);
			}
			else if (num7 != -1 && Main.rand.NextBool(2))
			{
				if (Main.rand.NextBool(12000))
				{
					extractItem = 74;
					if (Main.rand.NextBool(14))
						extractStack += Main.rand.Next(0, 2);

					if (Main.rand.NextBool(14))
						extractStack += Main.rand.Next(0, 2);

					if (Main.rand.NextBool(14))
						extractStack += Main.rand.Next(0, 2);
				}
				else if (Main.rand.NextBool(800))
				{
					extractItem = 73;
					if (Main.rand.NextBool(6))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(6))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(6))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(6))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(6))
						extractStack += Main.rand.Next(1, 20);
				}
				else if (Main.rand.NextBool(60))
				{
					extractItem = 72;
					if (Main.rand.NextBool(4))
						extractStack += Main.rand.Next(5, 26);

					if (Main.rand.NextBool(4))
						extractStack += Main.rand.Next(5, 26);

					if (Main.rand.NextBool(4))
						extractStack += Main.rand.Next(5, 26);

					if (Main.rand.NextBool(4))
						extractStack += Main.rand.Next(5, 25);
				}
				else
				{
					extractItem = 71;
					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(10, 26);

					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(10, 26);

					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(10, 26);

					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(10, 25);
				}
			}
			else if (num != -1 && Main.rand.Next(num) == 0)
			{
				extractItem = 1242;
			}
			else if (num5 != -1)
			{
				extractItem = ((Main.rand.NextBool(4)) ? 2674 : ((Main.rand.NextBool(3)) ? 2006 : ((Main.rand.NextBool(3)) ? 2675 : 2002)));
			}
			else if (num6 != -1 && extractinatorBlockType == 642)
			{
				if (Main.rand.NextBool(10))
				{
					extractItem = Main.rand.Next(5) switch
					{
						0 => 4354,
						1 => 4389,
						2 => 4377,
						3 => 5127,
						_ => 4378,
					};
				}
				else
				{
					extractItem = Main.rand.Next(5) switch
					{
						0 => 4349,
						1 => 4350,
						2 => 4351,
						3 => 4352,
						_ => 4353,
					};
				}
			}
			else if (num6 != -1)
			{
				extractItem = Main.rand.Next(5) switch
				{
					0 => 4349,
					1 => 4350,
					2 => 4351,
					3 => 4352,
					_ => 4353,
				};
			}
			else if (num2 != -1 && Main.rand.Next(num2) == 0)
			{
				extractItem = Main.rand.Next(6) switch
				{
					0 => 181,
					1 => 180,
					2 => 177,
					3 => 179,
					4 => 178,
					_ => 182,
				};
				if (Main.rand.NextBool(20))
					extractStack += Main.rand.Next(0, 2);

				if (Main.rand.NextBool(30))
					extractStack += Main.rand.Next(0, 3);

				if (Main.rand.NextBool(40))
					extractStack += Main.rand.Next(0, 4);

				if (Main.rand.NextBool(50))
					extractStack += Main.rand.Next(0, 5);

				if (Main.rand.NextBool(60))
					extractStack += Main.rand.Next(0, 6);
			}
			else if (num3 != -1 && Main.rand.Next(num3) == 0)
			{
				extractItem = 999;
				if (Main.rand.NextBool(20))
					extractStack += Main.rand.Next(0, 2);

				if (Main.rand.NextBool(30))
					extractStack += Main.rand.Next(0, 3);

				if (Main.rand.NextBool(40))
					extractStack += Main.rand.Next(0, 4);

				if (Main.rand.NextBool(50))
					extractStack += Main.rand.Next(0, 5);

				if (Main.rand.NextBool(60))
					extractStack += Main.rand.Next(0, 6);
			}
			else if (Main.rand.NextBool(3))
			{
				if (Main.rand.NextBool(5000))
				{
					extractItem = 74;
					if (Main.rand.NextBool(10))
						extractStack += Main.rand.Next(0, 3);

					if (Main.rand.NextBool(10))
						extractStack += Main.rand.Next(0, 3);

					if (Main.rand.NextBool(10))
						extractStack += Main.rand.Next(0, 3);

					if (Main.rand.NextBool(10))
						extractStack += Main.rand.Next(0, 3);

					if (Main.rand.NextBool(10))
						extractStack += Main.rand.Next(0, 3);
				}
				else if (Main.rand.NextBool(400))
				{
					extractItem = 73;
					if (Main.rand.NextBool(5))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(5))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(5))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(5))
						extractStack += Main.rand.Next(1, 21);

					if (Main.rand.NextBool(5))
						extractStack += Main.rand.Next(1, 20);
				}
				else if (Main.rand.NextBool(30))
				{
					extractItem = 72;
					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(5, 26);

					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(5, 26);

					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(5, 26);

					if (Main.rand.NextBool(3))
						extractStack += Main.rand.Next(5, 25);
				}
				else
				{
					extractItem = 71;
					if (Main.rand.NextBool(2))
						extractStack += Main.rand.Next(10, 26);

					if (Main.rand.NextBool(2))
						extractStack += Main.rand.Next(10, 26);

					if (Main.rand.NextBool(2))
						extractStack += Main.rand.Next(10, 26);

					if (Main.rand.NextBool(2))
						extractStack += Main.rand.Next(10, 25);
				}
			}
			else if (extractinatorBlockType == 642)
			{
				extractItem = Main.rand.Next(14) switch
				{
					0 => 12,
					1 => 11,
					2 => 14,
					3 => 13,
					4 => 699,
					5 => 700,
					6 => 701,
					7 => 702,
					8 => 364,
					9 => 1104,
					10 => 365,
					11 => 1105,
					12 => 366,
					_ => 1106,
				};
				if (Main.rand.NextBool(20))
					extractStack += Main.rand.Next(0, 2);

				if (Main.rand.NextBool(30))
					extractStack += Main.rand.Next(0, 3);

				if (Main.rand.NextBool(40))
					extractStack += Main.rand.Next(0, 4);

				if (Main.rand.NextBool(50))
					extractStack += Main.rand.Next(0, 5);

				if (Main.rand.NextBool(60))
					extractStack += Main.rand.Next(0, 6);
			}
			else
			{
				extractItem = Main.rand.Next(8) switch
				{
					0 => 12,
					1 => 11,
					2 => 14,
					3 => 13,
					4 => 699,
					5 => 700,
					6 => 701,
					_ => 702,
				};
				if (Main.rand.NextBool(20))
					extractStack += Main.rand.Next(0, 2);

				if (Main.rand.NextBool(30))
					extractStack += Main.rand.Next(0, 3);

				if (Main.rand.NextBool(40))
					extractStack += Main.rand.Next(0, 4);

				if (Main.rand.NextBool(50))
					extractStack += Main.rand.Next(0, 5);

				if (Main.rand.NextBool(60))
					extractStack += Main.rand.Next(0, 6);
			}

			ItemLoader.ExtractinatorUse(ref extractItem, ref extractStack, extractType, extractinatorBlockType);

			if (extractItem > 0)
				DropItemFromExtractinator(player, extractItem, extractStack);
		}

		public static void DropItemFromExtractinator(Player player, int itemType, int stack)
		{
			Vector2 vector = Main.ReverseGravitySupport(Main.MouseScreen) + Main.screenPosition;
			if (Main.SmartCursorIsUsed || PlayerInput.UsingGamepad)
				vector = player.Center;

			int number = Item.NewItem(player.GetSource_TileInteraction(Player.tileTargetX, Player.tileTargetY), (int)vector.X, (int)vector.Y, 1, 1, itemType, stack, noBroadcast: false, -1);
			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
		}
	}
}
