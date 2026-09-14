using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Biomes;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using V2.Core;
using V2.Core.MainDetours;
using V2.Core.WorldGeneration;
using V2.Items;
using V2.NPCs;
using V2.NPCs.Vanilla.TownNPCs.TravellingMerchant;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.StatusEffects.Voraria.Debuffs;
using V2.UI.PredStatsMenu;

namespace V2
{
	public partial class V2 : Mod
	{
		private delegate void orig_NPCAI(NPC npc);
		internal static Hook NPCLoader_NPCAI_Hook;
		private static readonly MethodInfo NPCLoader_NPCAI_MethodInfo =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.NPCLoader")!.GetMethod("NPCAI", BindingFlags.Public | BindingFlags.Static)!;

		private delegate void orig_ProjectileAI(Projectile projectile);
		internal static Hook ProjectileLoader_ProjectileAI_Hook;
		private static readonly MethodInfo ProjectileLoader_ProjectileAI_MethodInfo =
			typeof(Main).Assembly.GetType("Terraria.ModLoader.ProjectileLoader")!.GetMethod("ProjectileAI", BindingFlags.Public | BindingFlags.Static)!;

		public static void EngageVoraciousGameFuckery()
		{
			NPCLoader_NPCAI_Hook = new Hook(NPCLoader_NPCAI_MethodInfo, (orig_NPCAI orig, NPC npc) =>
			{
				GeneralNPC npcAsV2NPC = npc.AsV2NPC(risky: true);
				PreyNPC npcAsPrey = npc.AsFood(risky: true);
				if (!BasicMode && (npc.HasBuff<TimeStun>() || (npc.realLife > -1 && Main.npc[npc.realLife].active && Main.npc[npc.realLife].HasBuff<TimeStun>())))
				{
					npc.frameCounter = 0;
					if (npc.AsV2NPC().VelocityBeforeTimeStun == null)
						npc.AsV2NPC().VelocityBeforeTimeStun = npc.velocity;
					npc.velocity = Vector2.Zero;
					npc.GravityMultiplier *= 0;
					return;
				}
				if (npcAsV2NPC is null || npcAsPrey is null)
					orig(npc);
				else
				{
					if (npc.CurrentCaptor() is not null)
					{
						npc.velocity = Vector2.Zero;
						npc.position = npc.CurrentCaptor().Predator.position;
						npcAsPrey.SpecialPreyAI?.Invoke(npc, npc.CurrentCaptor().Predator);
					}
					else if (npcAsV2NPC.NewAIMethod is not null)
					{
						if (npcAsV2NPC.FirstFrame && npcAsV2NPC.FirstFramePreAIMethod is not null)
						{
							npcAsV2NPC.FirstFrame = false;
							npcAsV2NPC.FirstFramePreAIMethod.Invoke(npc);
						}

						npcAsV2NPC.CustomSprite?.Advance();

						if (npcAsV2NPC.NewAIMethod.Invoke(npc))
							orig(npc);
						else
							NPCLoader.PostAI(npc);
					}
					else
						orig(npc);
				}
			});
			NPCLoader_NPCAI_Hook.Apply();


			ProjectileLoader_ProjectileAI_Hook = new Hook(ProjectileLoader_ProjectileAI_MethodInfo, (orig_ProjectileAI orig, Projectile projectile) =>
			{
				GeneralProjectile projectileAsV2Projectile = projectile.AsV2Proj(risky: true);
				PreyProjectile projectileAsPrey = projectile.AsFood(risky: true);
				if (projectileAsV2Projectile is null || projectileAsPrey is null)
					orig(projectile);
				else
				{
					PredProjectile.ResetEffects(projectile);
					if (projectile.CurrentCaptor() is not null)
					{
						projectile.timeLeft += 1;
						projectile.velocity = Vector2.Zero;
						projectile.position = projectile.CurrentCaptor().Predator.position;
						projectileAsPrey.SpecialPreyAI?.Invoke(projectile, projectile.CurrentCaptor().Predator);
					}
					else if (projectileAsV2Projectile.NewAIMethod is not null)
					{
						projectileAsV2Projectile.CustomSprite?.Advance();

						if (projectileAsV2Projectile.NewAIMethod.Invoke(projectile))
							orig(projectile);
						else
							ProjectileLoader.PostAI(projectile);
					}
					else
						orig(projectile);
				}
			});
			ProjectileLoader_ProjectileAI_Hook.Apply();

			On_Chest.SetupTravelShop_GetItem += (On_Chest.orig_SetupTravelShop_GetItem orig, Player playerWithHighestLuck, int[] rarity, ref int it, int minimumRarity)
				=> TravellingMerchant.SetupTravelShop_GetItem(playerWithHighestLuck, rarity, ref it, minimumRarity);

			On_Main.UpdateAudio_DecideOnNewMusic += (orig, instance) => MainDetours.UpdateAudio_DecideOnNewMusic();
			On_Item.NewItem_Inner += MainDetours.SpillLootInToGutFromItemDrops;
			On_Main.DrawInterface_36_Cursor += (orig) =>
			{
				if (PredStatsMenuMouthUI.MouthState is not (PredStatsMenuMouthState.EatingCursor or PredStatsMenuMouthState.RegurgitatingCursor))
					orig();
			};

			On_NPC.CanBeChasedBy += (orig, npc, attacker, ignoreDontTakeDamage) =>
			{
				if (npc.active)
				{
					if (npc.CurrentCaptor() is not null)
						return false;
				}

				return orig(npc, attacker, ignoreDontTakeDamage);
			};
			On_NPC.checkDead += (orig, npc) => NPCDetours.CheckDead(npc);
			On_NPC.NPCLoot_DropHeals += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsFood().Digested)
					orig(npc, closestPlayer);
			};
			On_NPC.NPCLoot_DropMoney += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsFood().Digested)
					orig(npc, closestPlayer);
			};
			On_NPC.NPCLoot_DropItems += (orig, npc, closestPlayer) =>
			{
				if (!npc.AsFood().Digested)
					orig(npc, closestPlayer);
			};
			On_NPC.DoDeathEvents_DropBossPotionsAndHearts += NoPotionsOrHeartsIfDigested;
			On_NPC.DoDeathEvents_CelebrateBossDeath += (orig, npc, typeName) => NPCDetours.DoDeathEvents_CelebrateBossDeath(npc, typeName);

			On_Player.CheckDrowning += (orig, player) =>
			{
				if (player.CurrentCaptor() is null)
					orig(player);
			};
			On_Player.KillMe += (orig, player, damageSource, dmg, hitDirection, pvp) => PlayerDetours.KillMe(player, damageSource, dmg, hitDirection, pvp);
			On_Player.GrantArmorBenefits += (orig, player, armorPiece) => V2Player.GrantArmorBenefits(player, armorPiece);
			On_Player.ApplyEquipFunctional += (orig, player, item, hideVisual) =>
			{
				if (item.IsAir)
					return;

				if ((item.expertOnly && !Main.expertMode) || (item.masterOnly && !Main.masterMode))
					return;

				if (item.AsFood().MaxHealth != -1 && item.AsFood().Health <= 0)
					return;

				if (item.AsAnItem() is not null && item.AsAnItem().AccessoryEffectCode is not null)
					item.AsAnItem().AccessoryEffectCode.Invoke(item, player, hideVisual);
				else
					orig(player, item, hideVisual);
			};
			On_Player.UpdateArmorSets += (orig, player, i) =>
			{
				if (ArmorSetHandler.CheckDefinedArmorSets(player))
					player.ApplyArmorSoundAndDustChanges();
				else
					orig(player, i);
			};
			On_Player.UpdateLifeRegen += (orig, player) => PlayerDetours.Detour_UpdateLifeRegen(player);
			On_Player.UpdateManaRegen += (orig, player) => PlayerDetours.Detour_UpdateManaRegen(player);
			On_Player.UpdateBuffs += (orig, player, i) => PlayerDetours.Detour_UpdateBuffs(player);
			On_Player.DashMovement += (orig, player) =>
			{
				if (player.CurrentCaptor() is null)
					orig(player);
				else
				{
					player.dashDelay = 60;
					player.dashTime = 0;
				}
			};
			On_Player.ItemCheck_ReleaseCritter += (orig, player, item) => PlayerDetours.ItemCheck_ReleaseCritter(player, item);
			On_Player.ToggleInv += (orig, player) =>
			{
				if (!player.AsPred().InPredStatsMenu || Main.gamePaused)
					orig(player);
			};
			On_Player.PickupItem += PlayerDetours.Detour_PickupItem;
			On_Player.GrappleMovement += PlayerDetours.Detour_GrappleMovement;
			// On_Player.DropFromItem += PlayerDetours.Detour_DropFromItem;
			// On_Player.DropItemFromExtractinator += PlayerDetours.Detour_DropItemFromExtractinator;
			On_DeadMansChestBiome.TurnGoldChestIntoDeadMansChest += (orig, instance, position) => WorldGenDetours.TurnGoldChestIntoDeadMansChest(position);
			On_WorldGen.SpreadInfectionToNearbyTile += (orig, x, y, conversionType, range) => WorldGenDetours.SpreadInfectionToNearbyTile(orig, x, y, conversionType, range);
			On_WorldGen.SpreadGrass += (orig, i, j, dirt, grass, repeat, color) => WorldGenDetours.SpreadGrass(orig, i, j, dirt, grass, repeat, color);
			On_WorldGen.hardUpdateWorld += (orig, i, j) => WorldGenDetours.hardUpdateWorld(orig, i, j);
			//On_HiveBiome.Place += (orig, self, origin, structures) => WorldGenDetours.HiveBiome_Place(self, origin, structures);
		}

		public static void DisengageVoraciousGameFuckery()
		{
			if (NPCLoader_NPCAI_Hook is not null)
			{
				NPCLoader_NPCAI_Hook.Undo();
				NPCLoader_NPCAI_Hook = null;
			}
			if (ProjectileLoader_ProjectileAI_Hook is not null)
			{
				ProjectileLoader_ProjectileAI_Hook.Undo();
				ProjectileLoader_ProjectileAI_Hook = null;
			}
		}

		private static void NoPotionsOrHeartsIfDigested(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig, NPC npc, ref string typeName)
		{
			if (!npc.AsFood().Digested)
				orig(npc, ref typeName);
		}
	}
}