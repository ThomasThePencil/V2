using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.StatusEffects.Voraria.Debuffs;

namespace V2.NPCs
{
	public static class NPCExtensions
	{
		public static bool IsFoodFor(this NPC npc, Entity pred)
		{
			if (npc.CurrentCaptor() is null)
				return false;

			if (pred is NPC predNPC)
			{
				if (PredNPC.GetStomachTracker(predNPC) is null)
					return false;

				return npc.CurrentCaptor() == PredNPC.GetStomachTracker(predNPC);
			}
			else if (pred is Player predPlayer)
			{
				if (predPlayer.AsPred().StomachTracker is null)
					return false;

				return npc.CurrentCaptor() == predPlayer.AsPred().StomachTracker;
			}
			else if (pred is Projectile predProjectile)
			{
				if (PredProjectile.GetStomachTracker(predProjectile) is null)
					return false;

				return npc.CurrentCaptor() == PredProjectile.GetStomachTracker(predProjectile);
			}
			return false;
		}

		public static void SwitchToPattern<T>(this NPC npc, Entity target) where T : NPCBehaviorPattern, new()
		{
			npc.AsV2NPC().BehaviorPattern = new T();
			npc.AsV2NPC().BehaviorPattern.DoBehavior(npc, target);
		}

		public static void TryFindNewTarget(this NPC npc, List<(TargetType, int, TargetPriorityLevel)> specificWhitelistInput = null)
		{
			List<(TargetType Type, int ID, TargetPriorityLevel PriorityLevel)> specificWhitelist = null;
			if (specificWhitelistInput is not null)
			{
				specificWhitelist = new List<(TargetType, int, TargetPriorityLevel)>(specificWhitelistInput);
				if (V2.BlacklistsActive)
				{
					specificWhitelist.RemoveAll(x => x.Type == TargetType.NPC && V2.VoreNPCBlacklist.Contains(x.ID));
					specificWhitelist.RemoveAll(x => x.Type == TargetType.Projectile && V2.VoreProjectileBlacklist.Contains(x.ID));
				}
			}

			List<(int index, TargetType type, int aggro, float dist, TargetPriorityLevel priority)> targetList = [];
			foreach (Player targetPlayer in Main.ActivePlayers)
			{
				if (targetPlayer.dead || targetPlayer.npcTypeNoAggro[npc.type] || targetPlayer.aggro <= -1000 || targetPlayer.CurrentCaptor() is not null)
					continue;

				TargetPriorityLevel priority = TargetPriorityLevel.Neutral;
				bool inSpecificWhitelist = false;
				if (specificWhitelist is not null)
				{
					foreach ((TargetType type, int ID, TargetPriorityLevel priorityLevel) in specificWhitelist)
					{
						if (type == TargetType.Player)
						{
							inSpecificWhitelist = true;
							priority = priorityLevel;
							break;
						}
					}
				}
				else
					inSpecificWhitelist = true;

				if (!inSpecificWhitelist)
					continue;

				float distanceToTarget = npc.Distance(targetPlayer.position);
				float negativeAggroDistMult = 1f;
				if (targetPlayer.aggro < 0)
					negativeAggroDistMult -= (float)Math.Abs(targetPlayer.aggro) / 1000f;
				bool canTarget = distanceToTarget <= npc.AsV2NPC().TargetRange * negativeAggroDistMult;
				if (npc.AsV2NPC().TargetRequiresLineOfSight)
					canTarget &= Collision.CanHitLine(npc.position, npc.width, npc.height, targetPlayer.position, targetPlayer.width, targetPlayer.height);

				if (canTarget)
					targetList.Add((targetPlayer.whoAmI, TargetType.Player, targetPlayer.aggro, distanceToTarget, priority));
			}
			foreach (NPC targetNPC in Main.ActiveNPCs)
			{
				if (targetNPC.life <= 0 || targetNPC.AsV2NPC().Aggro <= -1000 || targetNPC.CurrentCaptor() is not null)
					continue;

				TargetPriorityLevel priority = TargetPriorityLevel.Neutral;
				bool inSpecificWhitelist = false;
				if (specificWhitelist is not null)
				{
					foreach ((TargetType type, int ID, TargetPriorityLevel priorityLevel) in specificWhitelist)
					{
						if (type == TargetType.NPC && (ID == targetNPC.type || ID == targetNPC.netID))
						{
							inSpecificWhitelist = true;
							priority = priorityLevel;
							break;
						}
					}
				}
				else
					inSpecificWhitelist = true;

				if (!inSpecificWhitelist)
					continue;

				float distanceToTarget = npc.Distance(targetNPC.position);
				float negativeAggroDistMult = 1f;
				if (targetNPC.AsV2NPC().Aggro < 0)
					negativeAggroDistMult -= (float)Math.Abs(targetNPC.AsV2NPC().Aggro) / 1000f;
				bool canTarget = distanceToTarget <= npc.AsV2NPC().TargetRange * negativeAggroDistMult;
				if (npc.AsV2NPC().TargetRequiresLineOfSight)
					canTarget &= Collision.CanHitLine(npc.position, npc.width, npc.height, targetNPC.position, targetNPC.width, targetNPC.height);

				if (canTarget)
					targetList.Add((targetNPC.whoAmI, TargetType.NPC, targetNPC.AsV2NPC().Aggro, distanceToTarget, priority));
			}
			foreach (Projectile targetProjectile in Main.ActiveProjectiles)
			{
				if (targetProjectile.AsFood().Health <= 0 || targetProjectile.AsV2Proj().Aggro <= -1000 || targetProjectile.CurrentCaptor() is not null)
					continue;

				TargetPriorityLevel priority = TargetPriorityLevel.Neutral;
				bool inSpecificWhitelist = false;
				if (specificWhitelist is not null)
				{
					foreach ((TargetType type, int ID, TargetPriorityLevel priorityLevel) in specificWhitelist)
					{
						if (type == TargetType.Projectile && ID == targetProjectile.type)
						{
							inSpecificWhitelist = true;
							priority = priorityLevel;
							break;
						}
					}
				}
				else
					inSpecificWhitelist = true;

				if (!inSpecificWhitelist)
					continue;

				float distanceToTarget = npc.Distance(targetProjectile.position);
				float negativeAggroDistMult = 1f;
				if (targetProjectile.AsV2Proj().Aggro < 0)
					negativeAggroDistMult -= (float)Math.Abs(targetProjectile.AsV2Proj().Aggro) / 1000f;
				bool canTarget = distanceToTarget <= npc.AsV2NPC().TargetRange * negativeAggroDistMult;
				if (npc.AsV2NPC().TargetRequiresLineOfSight)
					canTarget &= Collision.CanHitLine(npc.position, npc.width, npc.height, targetProjectile.position, targetProjectile.width, targetProjectile.height);

				if (canTarget)
					targetList.Add((targetProjectile.whoAmI, TargetType.Projectile, targetProjectile.AsV2Proj().Aggro, distanceToTarget, priority));
			}

			if (targetList.Count > 0)
			{
				bool currentlyTargetingSomething = npc.AsV2NPC().TargetIndex != -1 && npc.AsV2NPC().TargetType != TargetType.None && npc.AsV2NPC().TargetPriority != TargetPriorityLevel.None;
				targetList = new List<(int index, TargetType type, int aggro, float dist, TargetPriorityLevel priority)>(targetList.OrderByDescending(x => x.priority));
				if (currentlyTargetingSomething && npc.AsV2NPC().TargetPriority >= targetList[0].priority)
					return;

				TargetPriorityLevel highestPriority = targetList[0].priority;
				targetList.RemoveAll(x => x.priority < highestPriority);
				targetList = new List<(int index, TargetType type, int aggro, float dist, TargetPriorityLevel priority)>(targetList.OrderByDescending(x => x.aggro));
				if (currentlyTargetingSomething)
				{
					switch (npc.AsV2NPC().TargetType)
					{
						case TargetType.Player:
							Player previousTargetPlayer = Main.player[npc.AsV2NPC().TargetIndex];
							if (previousTargetPlayer.aggro >= targetList[0].aggro)
								return;
							break;
						case TargetType.NPC:
							NPC previousTargetNPC = Main.npc[npc.AsV2NPC().TargetIndex];
							if (previousTargetNPC.AsV2NPC().Aggro >= targetList[0].aggro)
								return;
							break;
						case TargetType.Projectile:
							Projectile previousTargetProjectile = Main.projectile[npc.AsV2NPC().TargetIndex];
							if (previousTargetProjectile.AsV2Proj().Aggro >= targetList[0].aggro)
								return;
							break;
					}
				}

				int highestAggro = targetList[0].aggro;
				targetList.RemoveAll(x => x.aggro < highestAggro);
				targetList = new List<(int index, TargetType type, int aggro, float dist, TargetPriorityLevel priority)>(targetList.OrderBy(x => x.dist));
				npc.AsV2NPC().TargetIndex = targetList[0].index;
				npc.AsV2NPC().TargetType = targetList[0].type;
				npc.AsV2NPC().TargetPriority = targetList[0].priority;
			}
		}
 
		public static void TryVerifyRemainingTarget(this NPC npc, List<(TargetType, int, TargetPriorityLevel)> specificWhitelistInput = null)
		{
			List<(TargetType Type, int ID, TargetPriorityLevel PriorityLevel)> specificWhitelist = null;
			if (specificWhitelistInput is not null)
			{
				specificWhitelist = new List<(TargetType, int, TargetPriorityLevel)>(specificWhitelistInput);
				if (V2.BlacklistsActive)
				{
					specificWhitelist.RemoveAll(x => x.Type == TargetType.NPC && V2.VoreNPCBlacklist.Contains(x.ID));
					specificWhitelist.RemoveAll(x => x.Type == TargetType.Projectile && V2.VoreProjectileBlacklist.Contains(x.ID));
				}
			}

			if (npc.AsV2NPC().TargetIndex != -1)
			{
				switch (npc.AsV2NPC().TargetType)
				{
					case TargetType.Player:
						Player targetPlayer = Main.player[npc.AsV2NPC().TargetIndex];
						if (!targetPlayer.active
						 || targetPlayer.dead
						 || targetPlayer.CurrentCaptor() is not null
						 || (npc.AsV2NPC().TargetRequiresLineOfSight && !Collision.CanHitLine(npc.position, npc.width, npc.height, targetPlayer.position, targetPlayer.width, targetPlayer.height))
						 || specificWhitelist.FindAll(x => x.Type == TargetType.Player).Count == 0)
						{
							npc.AsV2NPC().TargetType = TargetType.None;
							npc.AsV2NPC().TargetIndex = -1;
							npc.AsV2NPC().TargetPriority = TargetPriorityLevel.None;
						}
						break;
					case TargetType.NPC:
						NPC targetNPC = Main.npc[npc.AsV2NPC().TargetIndex];
						if (!targetNPC.active
						 || targetNPC.life <= 0
						 || targetNPC.CurrentCaptor() is not null
						 || (npc.AsV2NPC().TargetRequiresLineOfSight && !Collision.CanHitLine(npc.position, npc.width, npc.height, targetNPC.position, targetNPC.width, targetNPC.height))
						 || specificWhitelist.FindAll(x => x.Type == TargetType.NPC && x.ID == targetNPC.netID).Count == 0)
						{
							npc.AsV2NPC().TargetType = TargetType.None;
							npc.AsV2NPC().TargetIndex = -1;
							npc.AsV2NPC().TargetPriority = TargetPriorityLevel.None;
						}
						break;
					case TargetType.Projectile:
						Projectile targetProj = Main.projectile[npc.AsV2NPC().TargetIndex];
						if (!targetProj.active
						 || targetProj.AsFood().Health <= 0
						 || targetProj.CurrentCaptor() is not null
						 || (npc.AsV2NPC().TargetRequiresLineOfSight && !Collision.CanHitLine(npc.position, npc.width, npc.height, targetProj.position, targetProj.width, targetProj.height))
						 || specificWhitelist.FindAll(x => x.Type == TargetType.Projectile && x.ID == targetProj.type).Count == 0)
						{
							npc.AsV2NPC().TargetType = TargetType.None;
							npc.AsV2NPC().TargetIndex = -1;
							npc.AsV2NPC().TargetPriority = TargetPriorityLevel.None;
						}
						break;
					case TargetType.Other:
					case TargetType.None:
					default:
						break;
				}
			}
		}

		public static List<NPC> GetNearbyResidentNPCs(this NPC npc, out int npcsWithinHouse, out int npcsWithinVillage)
		{
			List<NPC> list = [];
			npcsWithinHouse = 0;
			npcsWithinVillage = 0;
			Vector2 value = new Vector2(npc.homeTileX, npc.homeTileY);
			if (npc.homeless)
				value = new Vector2(npc.Center.X / 16f, npc.Center.Y / 16f);

			for (int i = 0; i < 200; i++)
			{
				if (i == npc.whoAmI)
					continue;

				NPC nPC = Main.npc[i];
				if (nPC.active && nPC.townNPC && !npc.IsNotReallyTownNPC() && !WorldGen.TownManager.CanNPCsLiveWithEachOther_ShopHelper(npc, nPC))
				{
					Vector2 value2 = new Vector2(nPC.homeTileX, nPC.homeTileY);
					if (nPC.homeless)
						value2 = nPC.Center / 16f;

					float num = Vector2.Distance(value, value2);
					if (num < 25f)
					{
						list.Add(nPC);
						npcsWithinHouse++;
					}
					else if (num < 120f)
					{
						npcsWithinVillage++;
					}
				}
			}

			return list;
		}

		public static bool IsNotReallyTownNPC(this NPC npc)
		{
			int type = npc.type;
			if (type == 37 || type == 368 || NPCID.Sets.ActsLikeTownNPC[type])
				return true;

			return false;
		}

		public static void DoContactGulpage(this NPC npc, List<(TargetType, int, TargetPriorityLevel)> specificWhitelistInput = null, List<(TargetType, int)> specificPredWhitelistInput = null)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			List<(TargetType Type, int ID, TargetPriorityLevel PriorityLevel)> specificWhitelist = null;
			List<(TargetType Type, int ID)> specificPredWhitelist = null;
			if (specificWhitelistInput is not null)
			{
				specificWhitelist = [.. specificWhitelistInput];
				if (V2.BlacklistsActive)
				{
					specificWhitelist.RemoveAll(x => x.Type == TargetType.NPC && V2.VoreNPCBlacklist.Contains(x.ID));
					specificWhitelist.RemoveAll(x => x.Type == TargetType.Projectile && V2.VoreProjectileBlacklist.Contains(x.ID));
				}
			}
			if (specificPredWhitelistInput is not null)
			{
				specificPredWhitelist = [.. specificPredWhitelistInput];
				if (V2.BlacklistsActive)
				{
					specificPredWhitelist.RemoveAll(x => x.Type == TargetType.NPC && V2.VoreNPCBlacklist.Contains(x.ID));
					specificPredWhitelist.RemoveAll(x => x.Type == TargetType.Projectile && V2.VoreProjectileBlacklist.Contains(x.ID));
				}
			}
			foreach (var prey in Main.ActiveNPCs)
			{
				NPC preyNPC = prey;
				if (preyNPC.active && preyNPC.life > 0 && preyNPC.whoAmI != npc.whoAmI)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.NPC && ID == preyNPC.netID)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyNPC.Hitbox))
					{
						bool turnTables = specificPredWhitelist is not null && specificPredWhitelist.Contains((TargetType.NPC, preyNPC.type));
						if (turnTables)
							PredNPC.Swallow(preyNPC, npc);
						else
							PredNPC.Swallow(npc, preyNPC);
					}
				}
			}
			foreach (var prey in Main.ActivePlayers)
			{
				Player preyPlayer = prey;
				if (preyPlayer.active && !preyPlayer.dead)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.Player)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyPlayer.Hitbox))
						PredNPC.Swallow(npc, preyPlayer);
				}
			}
			foreach (var prey in Main.ActiveProjectiles)
			{
				Projectile preyProjectile = prey;
				if (preyProjectile.active)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.Projectile && ID == preyProjectile.type)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyProjectile.Hitbox))
						PredNPC.Swallow(npc, preyProjectile);
				}
			}
			/*
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC preyNPC = Main.npc[i];
				if (preyNPC.active && preyNPC.life > 0 && preyNPC.whoAmI != npc.whoAmI)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.NPC && ID == preyNPC.netID)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyNPC.Hitbox))
					{
						bool empressGetsGulped = preyNPC.type == NPCID.PartyGirl;
						if (ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
						{
							empressGetsGulped |= new List<int>
							{
								NPCID.Dryad,
								NPCID.Stylist,
								NPCID.TheBride,
								NPCID.EmpressButterfly,
							}.Contains(preyNPC.type);
						}
						if (npc.type == NPCID.HallowBoss && empressGetsGulped)
							PredNPC.Swallow(preyNPC, npc);
						else
							PredNPC.Swallow(npc, preyNPC);
					}
				}
			}
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player preyPlayer = Main.player[i];
				if (preyPlayer.active && !preyPlayer.dead)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.Player)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyPlayer.Hitbox))
						PredNPC.Swallow(npc, preyPlayer);
				}
			}
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile preyProjectile = Main.projectile[i];
				if (preyProjectile.active)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.Projectile && ID == preyProjectile.type)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(preyProjectile.Hitbox))
						PredNPC.Swallow(npc, preyProjectile);
				}
			}
			*/
		}
		public static void DoContactFeed(this NPC npc, List<(TargetType, int, TargetPriorityLevel)> specificWhitelistInput = null)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			List<(TargetType Type, int ID, TargetPriorityLevel PriorityLevel)> specificWhitelist = null;
			if (specificWhitelistInput is not null)
			{
				specificWhitelist = new List<(TargetType, int, TargetPriorityLevel)>(specificWhitelistInput);
				if (V2.BlacklistsActive)
				{
					specificWhitelist.RemoveAll(x => x.Type == TargetType.NPC && V2.VoreNPCBlacklist.Contains(x.ID));
					specificWhitelist.RemoveAll(x => x.Type == TargetType.Projectile && V2.VoreProjectileBlacklist.Contains(x.ID));
				}
			}
			foreach (var pred in Main.ActiveNPCs)
			{
				NPC predNPC = pred;
				if (predNPC.active && predNPC.life > 0 && predNPC.whoAmI != npc.whoAmI)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.NPC && ID == predNPC.netID)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(predNPC.Hitbox))
					{
						PredNPC.Swallow(predNPC, npc);
					}
				}
			}
			foreach (var pred in Main.ActivePlayers)
			{
				Player predPlayer = pred;
				if (predPlayer.active && !predPlayer.dead)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.Player)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(predPlayer.Hitbox))
						PredPlayer.Swallow(predPlayer, npc);
				}
			}
			foreach (var pred in Main.ActiveProjectiles)
			{
				Projectile predProjectile = pred;
				if (predProjectile.active)
				{
					bool inSpecificWhitelist = false;
					if (specificWhitelist is not null)
					{
						foreach ((TargetType type, int ID, TargetPriorityLevel priority) in specificWhitelist)
						{
							if (type == TargetType.Projectile && ID == predProjectile.type)
							{
								inSpecificWhitelist = true;
								break;
							}
						}
					}
					else
						inSpecificWhitelist = true;

					if (!inSpecificWhitelist)
						continue;

					if (npc.Hitbox.Intersects(predProjectile.Hitbox))
						PredProjectile.Swallow(predProjectile, npc);
				}
			}
		}

		public static int SoftenedStacks(this NPC npc) => Math.Min(
			Softened.MaxStacks,
			(int)Math.Floor((double)npc.AsFood().SoftenedDigestionDamageTaken / (npc.lifeMax * Softened.MaxHealthDigestedForOneStack(npc)))
		);

		public static bool CanItemsBeThievedBy(this NPC npc, Entity pred)
		{
			if (pred is Player playerPred)
			{
				if (playerPred.AsPred().charmStealPreyLoot)
					return true;
			}
			return false;
		}
	}

	public static class NPCChatHelper
	{
		public static void AddHumanoidPredMessages(this List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.1",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.2",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.3",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.4",
				"Mods.V2.Death.DigestedPlayer.HumanoidPred.5",
			});
		}
	}
}
