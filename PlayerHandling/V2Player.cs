using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using V2.Core;
using V2.Items;
using V2.Items.Voraria.Armor;
using V2.Items.Voraria.TransformationItems.Baelz;
using V2.NPCs;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.PlayerHandling.PredPlayerGoals.Intermediate;
using V2.PlayerHandling.PredPlayerGoals.Skilled;
using V2.Projectiles;
using V2.Projectiles.Voraria.Other;
using V2.StatusEffects.Voraria.Buffs;
using V2.StatusEffects.Voraria.Debuffs;
using V2.UI;
using V2.UI.MintWispSummonMeter;

namespace V2.PlayerHandling
{
	public partial class V2Player : ModPlayer
	{
		public List<DelegateGeneralItemDrawingUI> generalItemUIDrawMethods;

		public int GuideHelpText = 0;

		public int IsAtCrushingSpeed { get; set; }
		public int CrushingDamage { get; set; }

		public Vector2 GrappleLastSpeed { get; set; }

		public bool HungryCursor { get; set; }
		public bool ShroomNecklace { get; set; }
		public bool HoldingPredToggleRod { get; set; }
		//various transformation bools, and one if you have one in general
		public bool HasTransformation { get; set; }
		public bool BaeTransformation { get; set; }
		public bool KroniiTransformation { get; set; }
		public bool OllieTransformation { get; set; }
		public bool SoraTransformation { get; set; }
		public bool MintTransformation { get; set; }

		/// <summary>
		/// Used on the player select screen to render the transformation the player had upon leaving.<br/>
		/// </summary>
		public string LastTransformation = "None";

		public int KroniiBuffCooldown { get; set; }
		public int OllieDashDuration { get; set; }
		public Vector2 OllieDashDirection { get; set; }
		public List<(Vector2, bool)> OllieAfterimage = new List<(Vector2, bool)>();
		public double MintWispSummonMeter { get; set; }
		public double MintWispSummonMeterMax { get; set; }
		public SlotId LastSound { get; set; }

		public float ManaRegenOverallMod { get; set; }
		public float ManaRegenStillMod { get; set; }
		public float ManaRegenMovingMod { get; set; }
		
		public Dictionary<string, bool> LocationsVisited { get; set; }

		public Dictionary<int, double> StatusDurationResistance { get; set; }

		bool resetWidthOnce;

		public override void Initialize()
		{
			ResetHealthRegenTime();
			ResetHealthRegenEffectList();
			ResetManaRegenTime();
			ResetManaRegenEffectList();

			GrappleLastSpeed = Vector2.Zero;

			LocationsVisited = [];
			StatusDurationResistance = [];
			for (int i = 0; i < BuffLoader.BuffCount; i++)
			{
				if (!StatusDurationResistance.ContainsKey(i))
					StatusDurationResistance.TryAdd(i, 1.0);
			}
		}

		public void ResetStatusResistances()
		{
			for (int i = 0; i < BuffLoader.BuffCount; i++)
			{
				if (!StatusDurationResistance.ContainsKey(i))
					StatusDurationResistance.TryAdd(i, 1.0);

				StatusDurationResistance[i] = 1.0;
			}
		}
		public override void ResetEffects()
		{
			generalItemUIDrawMethods = [];
			setBonusActive = false;
			setBonusShouldBeDisplayed = false;
			ShroomNecklace = false;
			HoldingPredToggleRod = false;

			HasTransformation = false;

			BaeTransformation = false;
			KroniiTransformation = false;
			OllieTransformation = false;
			SoraTransformation = false;
			MintTransformation = false;
		
			if (Main.gameMenu)
			{
				switch(LastTransformation)
				{
					case "Baelz":
						HasTransformation = true;
						BaeTransformation = true;
						break;
					case "Kronii":
						HasTransformation = true;
						KroniiTransformation = true;
						break;
					case "Ollie":
						HasTransformation = true;
						OllieTransformation = true;
						break;
					case "Sora":
						HasTransformation = true;
						SoraTransformation = true;
						break;
					case "Mint":
						HasTransformation = true;
						MintTransformation = true;
						break;
				}
			}

			if (KroniiBuffCooldown > 0)
			{
				KroniiBuffCooldown--;
			}

			if (Player.name.ToLower() is "baelz" or "hakosbaelz" or "hakos baelz" or "baelzhakos" or "baelz hakos")
			{
				HasTransformation = true;
				BaeTransformation = true;
				Player.AddBuff(ModContent.BuffType<BaelzTransformation>(), V2Utils.SensibleTime(frames: 4));
			}
			/*else if (Player.name.ToLower() is "kronii" or "ourokronii" or "ouro kronii" or "kroniiouro" or "kronii ouro")
			{
				KroniiTransformation = true;
				Player.AddBuff(ModContent.BuffType<KroniiTransformation>(), V2Utils.SensibleTime(frames: 4));
			}
			else if (Player.name.ToLower() is "ollie" or "kureijiollie" or "kureiji ollie" or "olliekureiji" or "ollie kureiji")
			{
				OllieTransformation = true;
				Player.AddBuff(ModContent.BuffType<OllieTransformation>(), V2Utils.SensibleTime(frames: 4));
			}
			else if (Player.name.ToLower() is "sora" or "tokinosora" or "tokino sora" or "soratokino" or "sora tokino")
			{
				SoraTransformation = true;
				Player.AddBuff(ModContent.BuffType<SoraTransformation>(), V2Utils.SensibleTime(frames: 4));
			}
			else if (Player.name.ToLower() is "mint" or "mintfantome" or "mint fantome")
			{
				MintTransformation = true;
				Player.AddBuff(ModContent.BuffType<MintTransformation>(), V2Utils.SensibleTime(frames: 4));
			}*/

			if (Player.whoAmI != Main.myPlayer)
				return;

			if (Player.talkNPC != -1)
			{
				NPC npc = Player.TalkNPC;
				if (npc.CurrentCaptor() is not null)
					Main.CloseNPCChatOrSign();
			}
			ResetHealthRegenEffectList();
			ResetManaRegenEffectList();
			ResetEnvironmentEffects();
			ResetStatusResistances();
		}

		public override void ModifyLuck(ref float luck)
		{
			if (Player.armor[0].type == ModContent.ItemType<CloverHeadAccessories>())
				luck += 0.3f;
			if (Player.armor[1].type == ModContent.ItemType<CloverSweater>())
				luck += 0.1f;
			if (Player.armor[2].type == ModContent.ItemType<CloverStockings>())
				luck += 0.1f;
		}

		public override void UpdateDead()
		{
			ResetHealthRegenTime();
			ResetHealthRegenEffectList();
			ResetManaRegenTime();
			ResetManaRegenEffectList();
		}
		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			if (Player.AsV2Player().BaeTransformation)
			{
				Dust.NewDustPerfect(Player.position, ModContent.DustType<DeadBaelz>(), new Vector2(Main.rand.Next(-100, 101) / 15f, Main.rand.Next(-100, -50) / 15f));
			}
		}
		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (Player.AsV2Player().HasTransformation && OllieDashDuration <= 0)
			{
				modifiers.DisableSound();
				SoundEngine.PlaySound(
					SoundID.NPCHit1,
					Player.TrueCenter()
				);
			}
		}

		public override void OnHurt(Player.HurtInfo info)
		{
			if (Player.HasBuff<KroniiSpeed>())
			{
				int buffIndex = Player.FindBuffIndex(ModContent.BuffType<KroniiSpeed>());
				Player.buffTime[buffIndex] = Math.Max(Player.buffTime[buffIndex] + 900, 1);
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (BaeTransformation)
			{
				modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) => {
					if (hitInfo.Crit)
					{
						//check double crit
						int chance = Main.rand.Next(101);
						int currentCrit = (int)Player.GetTotalCritChance(hitInfo.DamageType) + Player.HeldItem.crit;

						if (chance <= currentCrit)
						{
							hitInfo.Damage *= 2;
							hitInfo.HideCombatText = true;
							CombatText.NewText(target.Hitbox, Color.FromNonPremultiplied(255, 25, 100, 255), hitInfo.Damage, true);
						}
					}
				};
			}
		}
		public static int GetStunTimeForNPC(NPC npc)
		{
			int StunTime = 120;
			int ReduceAmount = Math.Max(npc.AsV2NPC().TimeStunCounter - 2, 0);
			return Math.Max(StunTime - 30 * ReduceAmount, 30);
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (KroniiTransformation) //change to kronii later
			{
				if (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)
				{
					if (KroniiBuffCooldown == 0)
					{
						Player.AddBuff(ModContent.BuffType<KroniiSpeed>(), V2Utils.SensibleTime(frames: 75));
						KroniiBuffCooldown = 13;
					}
					if (target.AsV2NPC().TimeStunCooldown <= 0)
					{
						bool Success = Main.rand.NextBool(10);
						if (!Success && Main.rand.NextFloat() < Player.luck)
						{
							Success = Main.rand.NextBool(10);
						}
						if (!Success && Main.rand.NextFloat() < Player.luck - 0.5f)
						{
							Success = Main.rand.NextBool(10);
						}
						if (Success)
							if (target.realLife > -1 && Main.npc[target.realLife].active)
							{
								if (Main.npc[target.realLife].AsV2NPC().TimeStunCooldown <= 0)
								{
									Main.npc[target.realLife].AddBuff(ModContent.BuffType<TimeStun>(), GetStunTimeForNPC(Main.npc[target.realLife]));
									Main.npc[target.realLife].AsV2NPC().TimeStunCounter++;
								}
									
							}
							else
							{
								target.AddBuff(ModContent.BuffType<TimeStun>(), GetStunTimeForNPC(target));
								target.AsV2NPC().TimeStunCounter++;
							}
								
					}
				}
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			if (Player.AsPred().isAtCrushingSpeed > 0 || OllieDashDuration > 0)
			{
				modifiers.Cancel();
				return;
			}
		}
		public override void ModifyHitByProjectile(Projectile projectile, ref Player.HurtModifiers modifiers)
		{
			if (OllieDashDuration > 0)
			{
				modifiers.Cancel();
				return;
			}
		}

		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			ResetHealthRegenTime();
		}

		public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
		{
			ResetHealthRegenTime();
		}
		public override void PostUpdateEquips()
		{
			if (Player.AsV2Player().HasTransformation)
			{
				ModContent.GetInstance<BecomeSomeoneElse>().TrySetCompletion(Player);
				double PlayerWeight = PlayerGaining.GetPlayerWeight(Player, IncludeWeightModifiers: false);
				if (PlayerWeight >= 1.2)
					ModContent.GetInstance<MinorConsequences>().TrySetCompletion(Player);
				if (PlayerWeight >= 2)
					ModContent.GetInstance<Chunky>().TrySetCompletion(Player);
				if (PlayerWeight >= 3.5)
					ModContent.GetInstance<Fatty>().TrySetCompletion(Player);
				if (PlayerWeight >= 10)
					ModContent.GetInstance<MajorConsequences>().TrySetCompletion(Player);
				if (PlayerWeight >= 40)
					ModContent.GetInstance<HaveWeGoneTooFar>().TrySetCompletion(Player);
				if (PlayerWeight >= 100)
					ModContent.GetInstance<HowDidWeGetHere>().TrySetCompletion(Player);

			}

			int lastWidth = Player.width;
			if (Player.AsV2Player().BaeTransformation == true)
			{
				switch (BaelzInfo.GetVisualWeightStage(Player))
				{
					case 0 or 1:
						Player.width = 18;
						break;
					case 2:
						Player.width = 20;
						break;
					case 3:
						Player.width = 22;
						break;
					case 4:
						Player.width = 26;
						break;
					case 5:
						Player.width = 34;
						break;
					case 6:
						Player.width = 40;
						break;
					case 7:
						Player.width = 48;
						break;
				}
				resetWidthOnce = true;
			}
			else if (resetWidthOnce) // Vanilla resets hitbox changes only once.
			{
				Player.width = Player.defaultWidth;
				resetWidthOnce = false;
			}
			if (Player.width != lastWidth)
			{
				int difference = Player.width - lastWidth;
				Player.position.X -= difference / 2;
			}
		}
		public override void PostUpdateMiscEffects()
		{
			HandleSittingAndSleepingHealthRegenEffect();

			void AddLocationVisitMark(string place)
			{
				if (LocationsVisited.ContainsKey(place))
					LocationsVisited[place] = true;
				else
					LocationsVisited.TryAdd(place, true);
			}

			if (Player.ZoneSkyHeight)
				AddLocationVisitMark("sky");
			if (Player.ZoneForest)
				AddLocationVisitMark("forest");
			if (Player.ZoneDirtLayerHeight)
				AddLocationVisitMark("underground");
			if (Player.ZoneRockLayerHeight)
				AddLocationVisitMark("cavern");
			if (Player.ZoneUnderworldHeight)
				AddLocationVisitMark("hell");
			if (Player.ZoneSnow && Player.ZoneOverworldHeight)
				AddLocationVisitMark("tundra");
			if (Player.ZoneSnow && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight))
				AddLocationVisitMark("underground_tundra");
			if (Player.ZoneDesert)
				AddLocationVisitMark("desert");
			if (Player.ZoneUndergroundDesert)
				AddLocationVisitMark("underground_desert");
			if (Player.ZoneCorrupt)
				AddLocationVisitMark("corruption");
			if (Player.ZoneCrimson)
				AddLocationVisitMark("crimson");
			if (Player.ZoneBeach)
				AddLocationVisitMark("beach");
			if (Player.ZoneJungle && Player.ZoneOverworldHeight)
				AddLocationVisitMark("jungle");
			if (Player.ZoneJungle && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight))
				AddLocationVisitMark("underground_jungle");
			if (Player.ZoneGraveyard)
				AddLocationVisitMark("graveyard");
			if (Player.ZoneGranite)
				AddLocationVisitMark("granite");
			if (Player.ZoneMarble)
				AddLocationVisitMark("marble");
			if (Player.ZoneMeteor)
				AddLocationVisitMark("meteorite");
			if (Player.ZoneDungeon)
				AddLocationVisitMark("dungeon");
			if (Player.ZoneLihzhardTemple)
				AddLocationVisitMark("temple");
			if (!Main.dayTime)
				AddLocationVisitMark("nighttime");
			if (Player.ZoneSandstorm)
				AddLocationVisitMark("sandstorm");
			if (Main.IsItAHappyWindyDay)
				AddLocationVisitMark("windy_day");
			if (Main.IsItRaining && (Player.ZoneOverworldHeight || Player.ZoneSkyHeight))
			{
				if (Player.ZoneSnow)
					AddLocationVisitMark("snowing");
				else if (Main.IsItStorming)
					AddLocationVisitMark("thunderstorm");
				else
					AddLocationVisitMark("raining");
			}
			if (Main.IsItAHappyWindyDay)
				AddLocationVisitMark("windy_day");
			if (Main.bloodMoon)
				AddLocationVisitMark("blood_moon");
			if (Main.eclipse)
				AddLocationVisitMark("eclipse");


			if (V2.OllieDashHotkey.JustPressed && !Player.HasBuff<OllieDashCooldown>())
			{
				if (Player == Main.LocalPlayer && OllieTransformation)
				{
					OllieDashDirection = Player.Center.DirectionTo(Main.MouseWorld);
					OllieDashDuration = 20;
					Player.AddBuff(ModContent.BuffType<OllieDashCooldown>(), 600);
					Player.AddBuff(ModContent.BuffType<OllieShootSpeed>(), 300);
				}
			}
		}

		public int GetDamageForMintWisp()
		{
			int damage = 3;
			if (NPC.downedSlimeKing)
				damage += 2;

			if (NPC.downedBoss1)
				damage += 3;

			if (NPC.downedBoss2)
				damage += 4;

			if (NPC.downedBoss3)
				damage += 2;

			if (NPC.downedQueenBee)
				damage += 2;

			if (NPC.downedDeerclops)
				damage += 2;

			if (Main.hardMode)
				damage += 7;

			if (NPC.downedQueenSlime)
				damage += 3;

			if (NPC.downedMechBoss1)
				damage += 2;

			if (NPC.downedMechBoss2)
				damage += 2;

			if (NPC.downedMechBoss3)
				damage += 2;

			if (NPC.downedPlantBoss)
				damage += 6;

			if (NPC.downedFishron)
				damage += 2;

			if (NPC.downedEmpressOfLight)
				damage += 2;

			if (NPC.downedGolemBoss)
				damage += 3;

			if (NPC.downedAncientCultist)
				damage += 5;

			if (NPC.downedMoonlord)
				damage += 10;

			return damage;
		}
		public override void PostUpdate()
		{
			MintWispSummonMeterMax = 2 + (int)Math.Floor(Player.maxMinions * 1.5f);
			if (MintWispSummonMeter > MintWispSummonMeterMax)
				MintWispSummonMeter = MintWispSummonMeterMax;
			if (V2.MintWispHotkey.JustPressed && MintWispSummonMeter >= 1)
			{
				MintWispSummonMeter -= 1;
				SoundEngine.PlaySound(
					Player.AsPred().SmallBurps,
					Player.TrueCenter() + new Vector2(Player.direction * 8f, -14f)
				);
				if (Player == Main.LocalPlayer)
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(0, -2), new Vector2(Player.direction * 11f, Main.rand.Next(-35, 36) / 10f), ModContent.ProjectileType<MintWisp>(), GetDamageForMintWisp(), 0f, Main.myPlayer);
			}
		}
		public override void PostUpdateRunSpeeds()
		{
			if (OllieDashDuration > 0)
			{
				OllieAfterimage.Add((Player.position, true));
				OllieDashDuration--;
				Player.maxFallSpeed = 100f;
				Player.velocity = OllieDashDuration == 0 ? OllieDashDirection * 15f : OllieDashDirection * 30f;
			}
			else
				OllieAfterimage.Add((Player.position, false));
		}
		public override void HideDrawLayers(PlayerDrawSet drawInfo)
		{
			foreach (PlayerDrawLayer drawLayer in PlayerDrawLayerLoader.Layers)
			{
				if ((Player.AsV2Player().HasTransformation == true) && !drawInfo.headOnlyRender)
				{
					if ((drawLayer != PlayerDrawLayers.HeldItem && drawLayer != PlayerDrawLayers.Carpet && drawLayer != PlayerDrawLayers.Pulley && drawLayer != PlayerDrawLayers.ForbiddenSetRing
						&& drawLayer != PlayerDrawLayers.CaptureTheGem && drawLayer != PlayerDrawLayers.BeetleBuff && drawLayer != PlayerDrawLayers.ElectrifiedDebuffFront
						&& drawLayer != PlayerDrawLayers.ElectrifiedDebuffFront && drawLayer != PlayerDrawLayers.PortableStool && drawLayer != PlayerDrawLayers.SafemanSun
						&& drawLayer != PlayerDrawLayers.SolarShield && drawLayer != PlayerDrawLayers.WebbedDebuffBack && drawLayer != PlayerDrawLayers.FrozenOrWebbedDebuff
						&& drawLayer != PlayerDrawLayers.EyebrellaCloud && drawLayer != PlayerDrawLayers.FinchNest && drawLayer != PlayerDrawLayers.IceBarrier
						&& drawLayer != PlayerDrawLayers.MountBack && drawLayer != PlayerDrawLayers.MountFront) && drawLayer.Mod == null || Player.dead)
						drawLayer.Hide();
				}
			}
		}
		public bool HasVisitedLocation(string place)
		{
			if (LocationsVisited.TryGetValue(place, out bool value))
				return value;

			LocationsVisited.TryAdd(place, false);
			return false;
		}
		public override void SaveData(TagCompound tag)
		{
			if (LocationsVisited?.Count > 0)
			{
				List<string> locationsVisited = [];
				foreach (KeyValuePair<string, bool> location in LocationsVisited)
				{
					if (location.Value)
						locationsVisited.Add(location.Key);
				}
				tag["visitedLocations"] = locationsVisited;
			}
			if (BaeTransformation)
				tag["LastTransformation"] = "Baelz";
			else if (KroniiTransformation)
				tag["LastTransformation"] = "Kronii";
			else if (OllieTransformation)
				tag["LastTransformation"] = "Ollie";
			else if (SoraTransformation)
				tag["LastTransformation"] = "Sora";
			else if (MintTransformation)
				tag["LastTransformation"] = "Mint";
			else
				tag["LastTransformation"] = "None";
			tag["MintWispMeter"] = MintWispSummonMeter;
		}
		public override void LoadData(TagCompound tag)
		{
			List<string> locationsVisited = [.. tag.GetList<string>("visitedLocations")];
			if (locationsVisited.Count <= 0)
				return;

			LocationsVisited = [];
			foreach (string location in locationsVisited)
			{
				LocationsVisited.Add(location, true);
			}
			LastTransformation = tag.GetString("LastTransformation");
			MintWispSummonMeter = tag.GetDouble("MintWispMeter");
		}
	}
}
