using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Accessories.Informational;
using V2.Items.Voraria.Charms;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.TownNPCs.Mechanic
{
	public static class MechanicStuff
	{
		public static class ItemTheftRules
		{
			public static DigestionLootRule CombatWrench => new DigestionLootRule(
				type: (npc, pred) => ItemID.CombatWrench,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 2.0 / 3.0,
						GameModeID.Expert => 1.0 / 2.0,
						_ => 1.0 / 3.0,
					};
				}
			);
			public static DigestionLootRule MealSizeScanner => new DigestionLootRule(
				type: (npc, pred) => ModContent.ItemType<MealSizeScanner>(),
				amount: (npc, pred) => 1,
				chance: (npc, pred) => {
					return Main.GameMode switch
					{
						GameModeID.Master => 0.175,
						GameModeID.Expert => 0.15,
						_ => 0.10,
					};
				}
			);
		}
		public static Mechanic AsMechanic(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Mechanic predMechanic))
				throw new Exception("this instance of the Mechanic can't be pred or prey");

			return predMechanic;
		}
		public static MechanicProfile PredMechanicProfile = new MechanicProfile();
	}

	public class MechanicProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public MechanicProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Mechanic/Mechanic_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Mechanic/Mechanic";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			if (npc.altTexture == 1)
				exactTextureToUse += "_Party";

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Mechanic;
	}

	public class Mechanic : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Mechanic;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsV2NPC().GetNewDialogue = GetMechanicChat;
			
			npc.AsFood().DefinedBaseSize = 0.96;
			npc.AsPred().WeightGainRatio = 0.05;
			npc.AsPred().MaxStomachCapacity = 1.75;
			npc.AsPred().BaseStomachacheMeterCapacity = 300.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.5;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanMechanicBeForceFed;
			npc.AsPred().OnForceFed = OnMechanicForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = null;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.5;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;

			npc.AsFood().ItemTheftRules = [
				MechanicStuff.ItemTheftRules.CombatWrench,
				MechanicStuff.ItemTheftRules.MealSizeScanner,
			];
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => MechanicStuff.PredMechanicProfile;

		public override void ModifyShop(NPCShop shop)
		{
			if (shop.NpcType != NPCID.Mechanic)
				return;

			shop.Add(
				ModContent.ItemType<CharmLessStomachWeight>(),
				V2ShopConditions.ShopOwnerHasEatenWellRecently
			);
		}
		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public static List<string> GetMechanicChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC hopelessRomantic = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			NPC bootlegChippy = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Clothier);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			NPC steamLass = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());

			List<string> mechanicChatPool = new List<string>();
			V2Utils.FigureOutWhatTimeItIs(
				out bool pastMorning,
				out int hour,
				out int minute,
				out int second,
				out MealTime mealTime
			);
			double totalBellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			bool playerIsFood = player.IsFoodFor(npc, out bool playerWasAlreadyDigested);
			if (playerIsFood && !playerWasAlreadyDigested)
			{
				bool noDigest = false;
				if (Main.bloodMoon)
				{
					mechanicChatPool.AddRange([
						"[c/FF0000:Shut your mouth. Do that and melt.] I'm trying to work.",
						"Exactly as calculated; you're [c/FF0000:digesting] just fine. [c/FF0000:Continue, quietly.]",
					]);
				}
				else
				{
					mechanicChatPool.AddRange([
						"...you shouldn't bother worrying about getting in the way. I stitched these overalls myself specifically to resolve that issue.",
						"You...DO know that moving around too much in there is going to hurt both of us, right? System damage goes both ways...",
					]);
					if (noDigest)
					{
						mechanicChatPool.AddRange([
							"There should be some spare wire inside there. Push some up. I need some.",
							"...thank you for staying calm inside me. This helps me focus.",
							"This gives me valuable additional data on how the human stomach handles...live inhabitants without digesting. Continue.",
						]);
					}
					else
					{
						mechanicChatPool.AddRange([
							"[c/00BB00:*BUuUWwoOoOOAaaAAArRRp!!*]\n"
						  + "...my focus is coming back after that. Good. I need to work.",
							"You're digesting well, or at least it sounds like you are...can't help but wonder if I could optimize my stomach just a bit more...",
							"...nothing like some decent brain food while you work. Helps the mind stay on track.",
							"...better not add too much cellulite to me. Gets hard to work if I'm heavy past a certain breakpoint.",
						]);
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					mechanicChatPool.AddRange([
						"...don't bother me. [c/FF0000:For your own safety.]",
						"My workflow is interrupted. Leave or become a [c/FF0000:part of it].",
						"I've eaten [c/FF0000:skeletons] less bothersome than you. What do you want?",
					]);
					if (GetVisualBellySize(npc) >= 3)
					{
						mechanicChatPool.AddRange([
							"[c/00BB00:*BUOARP!*]\n"
						  + "...what? Never witnessed gaseous expulsion before?",
							"My workflow is interrupted. Leave or become another part of it.",
							"Count yourself lucky I'm on decent fueling. Spit it out, then: what do you want?",
						]);
					}
					if (Main.IsItRaining)
					{
						mechanicChatPool.AddRange([
							"I like rain only when I can control it. This is not controllable to the degree I'd like. [c/FFFF00:Feed me the clouds to fix that.]",
							"I will whip you with a frayed extension cord if you bother me tonight. [c/FFFF00:The rain will make sure you're fried to a fine crisp as a result.]",
						]);
					}
					if (Main.IsItStorming)
					{
						mechanicChatPool.AddRange([
							"If my wires get shorted [c/FF0000:one more fucking time], I'm going to find a way to [c/FF0000:swallow the sky.]",
							"Storms like this are only [c/FF0000:EVER] good as batteries. The only problem is ingesting them.",
						]);
					}
					if (steamLass != null)
					{
						if (steamLass.IsFoodFor(player))
						{
							mechanicChatPool.AddRange([
								"Made " + steamLass.GivenName + " into a battery? Good. [c/FF0000:It's the only thing she's EVER been good for.]",
								"Steam is worse. Every time. Good to know we can agree on that. If there's ANYTHING of hers left undigested by morning, you're [c/FF0000:breakfast.]",
							]);
						}
						else if (steamLass.IsFoodFor(npc))
						{
							mechanicChatPool.AddRange([
								steamLass.GivenName + " is busy digesting into ass fat. If I find you back in the morning before I've had the chance to calm down, I'll [c/FF0000:smother you with whatever she adds.]",
								"Steam is worse than electricity. Doesn't matter where, when, or why. It always is. Get the [c/FF0000:dumbfuck] inside me to quit screaming that it isn't.",
							]);
						}
						else
						{
							mechanicChatPool.AddRange([
								"You. " + steamLass.GivenName + "'s engines are broken. [c/FF0000:Feed her to me.]",
								"Steam is [c/FF0000:worthless] for anything except a marginally more attractive belch. My [c/FF0000:stuck-up, idiotic] \"rival\" should know this by now.",
								"I'm [c/FF0000:sick and tired] of that steam [c/FF0000:bitch's] yelling. She'll make a perfect midnight snack.",
							]);
						}
					}
				}
				else
				{
					mechanicChatPool.AddRange([
						"...if you don't buy enough wire this time, I'm gonna start charging extra.",
						"...I should install more lights here. Inside me, too. My stomach's craving a bit of electrical light right now.",
						"...my stomach's really well-optimized. Most of my other machines are, too. If you're not busy, I can demonstrate them.",
						"...did you make sure your device was plugged in? To an actual power outlet, and NOT your navel?",
						"...why do I eat wire so much? It's easy to eat, and I wasn't asked questions about it much.",
					]);
					if (GetVisualBellySize(npc) >= 3)
					{
						mechanicChatPool.AddRange([
							"...a full stomach like this makes a lot of useful ideas. I can make one for you real quick, in exchange for a quick dessert. Coins are easy to swallow and taste nice.",
							"...the sounds of my stomach digesting a large meal often help calm my mind. Does it have the same benefit for you, I wonder...?",
							"...it's always nice to have a flavorful battery inside your stomach, isn't it? I think I can make something to make it easier to carry that battery around until it's digested, too.",
						]);
					}
					if (bootlegChippy != null)
					{
						if (bootlegChippy.IsFoodFor(player))
						{
							mechanicChatPool.AddRange([
								"...seeing him gradually slow to a grinding halt in your stomach as it digests him is...satisfying. Continue keeping him inside you; don't let him go. Ever.",
								"..." + bootlegChippy.GivenName + " has already done enough damage as it is. It's...pleasant to see that you've made him your power source. Finally gets what's coming to him...",
							]);
						}
						else if (bootlegChippy.IsFoodFor(npc))
						{
							mechanicChatPool.AddRange(
							[
								"...what? I have no patience for people that violate my safety policy for years on end. I'm sure you have something more important to address.",
								"..." + bootlegChippy.GivenName + " has already done enough damage as it is. It's...pleasant to feel him inside me, and to have him beg for mercy. To finally be paid his debt.",
							]);
						}
						else
						{
							mechanicChatPool.AddRange(
							[
								"...tell " + bootlegChippy.GivenName + " that he's behind on electrical. He should visit me as soon as possible so I can...collect his dues, for lack of a better phrase.",
								"..." + bootlegChippy.GivenName + " won't stop bothering me. I don't know WHY he keeps pretending he didn't do what he did, but if you don't resolve his continued presence soon, I'll do so for you.",
							]);
						}
					}
					if (steamLass != null)
					{
						if (steamLass.IsFoodFor(player))
						{
							mechanicChatPool.AddRange([
								"...finally turned " + steamLass.GivenName + " into the battery she ought to be, did you? Good. She's better off that way.",
								"...it's just like I told you. Electricity is factually better than steam. The current contents of your stomach prove that.",
							]);
						}
						else if (steamLass.IsFoodFor(npc))
						{
							mechanicChatPool.AddRange([
								"...huh? You need to talk to " + steamLass.GivenName + "?\n"
							  + "...give me up to 8 hours to process her. Shouldn't take any longer than that. She'll return afterwards, if you need her THAT badly.",
								"...I've always told people that steam is strictly inferior to electricity, and that's because I'm RIGHT. Unfortunately..." + steamLass.GivenName + ", who's currently melting in my stomach, doesn't seem to believe me.",
							]);
						}
						else
						{
							mechanicChatPool.AddRange([
								"...got a moment? Good. Tell " + steamLass.GivenName + " that her engines, both internal and external, are outdated. Send her here for a...tune-up.",
								"...the only advantage I can see to steam, and the only reason I can see that " + steamLass.GivenName + " would love it so much, is that you can belch it out after a healthy meal.",
								"...teleportation? Via steam power? Eugh...that sort of weirder magic bothers me, and I hate steam. Unpredictable and unreliable...nothing like the circuits I'm used to.",
							]);
						}
					}
					if (Main.IsItAHappyWindyDay)
					{
						mechanicChatPool.AddRange([
							"...I need help testing something. Wires don't work in this weather. Do you think my stomach could work for power generation and transfer instead?",
							"...frustation levels rising...why would storing wire in my stomach genuinely be easier than dealing with all its tangling in this wind?",
						]);
					}
					if (Main.IsItRaining)
					{
						mechanicChatPool.AddRange([
							"...rain makes for a wonderful electrical conductor. I'm wondering whether or not filling myself with it would let me effectively \"digest\" electrical currents.",
							"...exercise caution around my machines. Rain makes a great conductor...for better or worse, and I particularly like food fried by electrical shock.",
						]);
					}
					if (Main.IsItStorming)
					{
						mechanicChatPool.AddRange([
							"...do you think it's possible to swallow and digest lightning? I'm tired of it overloading my devices. So much time, having to be wasted on increasingly-annoying repairs...",
							"...would you mind helping me out? I need to eat one of those stormclouds. The many books and manuals I've read in my time suggest it can turn my stomach into a portable battery.",
							"...so many different things to bolt down and cover in this weather. I have half a mind to start storing pieces of the power grid in my stomach...even if I know by now that it never ends well.",
						]);
					}
					if (LanternNight.LanternsUp)
					{
						mechanicChatPool.AddRange([
							"...I wonder if my stomach, if provided good fuel, could serve as one of these lanterns with a bit of technical work. The light that comes from them seems enticing...appetizing, almost.",
							"...does this activity actually influence \"luck\" in any way? All it seems like to me is a way to celebrate an important night. Doesn't help that \"luck\" isn't usually quantifiable...",
						]);
					}
					if (player.ZoneSnow)
					{
						if (player.ZoneOverworldHeight)
						{
							mechanicChatPool.AddRange(
							[
								"...this place is nice. Nice and cold, and in a way I can enjoy, too. My machines also quite like it; it helps them run better.",
								"...electricity flows better in low-heat environments like this. I like it here.",
							]);
						}
						else if (player.ZoneDirtLayerHeight)
						{
							mechanicChatPool.AddRange(
							[
								"...the protection of a roof without the bothers of maintenance, and it's still cold enough to keep my machines running well. This, I think, is the ideal workspace.",
								"...being a light distance underground helps to keep the weather from bothering me. Good.",
							]);
						}
						else if (player.ZoneRockLayerHeight)
						{
							mechanicChatPool.AddRange(
							[
								"...the cold here makes my machines work well, but I don't like being this far underground.",
								"...I have mixed feelings about this place. Useful for my work. Not so much for me.",
							]);
						}
					}
					if (Main.hardMode)
					{
						if (!NPC.downedMechBossAny)
						{
							mechanicChatPool.AddRange(
							[
								"...when the sun sets, if you hear mechanical rumbling coming closer, be ready to fight. Those robots won't go easy on you.",
								"...if you get the chance...could you try and \"fix\" one of those...THINGS I was forced to make? I'm not very proud of them.",
							]);
						}
						else
						{
							if (NPC.downedMechBoss1 && !NPC.downedMechBoss2 && !NPC.downedMechBoss3)
							{
								mechanicChatPool.AddRange([
									"...you dismantled the Spine. That's good to hear. She always tended to be the most...destructive of the parts I was allowed to finish, by no small margin.",
									"...one down. The Eyes and the Hand are still at large. Prepare for further encounters while you can.",
								]);
							}
							else if (!NPC.downedMechBoss1 && NPC.downedMechBoss2 && !NPC.downedMechBoss3)
							{
								mechanicChatPool.AddRange([
									"...you dismantled the Eyes. This is good. They were always too perceptive for their own good...only held back by their fights with one another. I count myself lucky as having been allowed to make them siblings...to a fault.",
									"...one down. The Spine and the Hand are still at large. Prepare for further encounters while you can.",
								]);
							}
							else if (!NPC.downedMechBoss1 && !NPC.downedMechBoss2 && NPC.downedMechBoss3)
							{
								mechanicChatPool.AddRange(
								[
									"...you dismantled the Hand. Good. He was always too ambitious, yet too blinded by his lack of intellect to know he was never truly the one pulling the strings. Not himself, nor the mechanical hand that held him under its thumb.",
									"...one down. The Spine and the Eyes are still at large. Prepare for further encounters while you can.",
								]);
							}
							else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && !NPC.downedMechBoss3)
							{
								mechanicChatPool.AddRange([
									"...you dismantled the Spine and the Eyes. Very good. All that's left is the Hand.",
									"...two parts gone, but the Hand still lays dormant back in those haunted halls. He always seemed to get \"hungrier\" with certain paintings around; I could never figure out why.",
								]);
							}
							else if (NPC.downedMechBoss1 && !NPC.downedMechBoss2 && NPC.downedMechBoss3)
							{
								mechanicChatPool.AddRange([
									"...you dismantled the Spine and the Hand. Very good. All that's left are the Eyes.",
									"...two parts gone, but the Eyes are still in the low atmosphere. If it helps, she likes to prey on particularly rare birds; he, on particularly heavy birds. These two preferences can overlap.",
								]);
							}
							else if (!NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
							{
								mechanicChatPool.AddRange([
									"...you dismantled the Eyes and the Hand. Very good. All that's left is the Spine.",
									"...two parts gone, but the Spine still roams free underground. She most commonly hunts and gorges herself on gemstone constructs, if that helps you figure out how to get her attention.",
								]);
							}
							else if (!NPC.downedPlantBoss)
							{
								mechanicChatPool.AddRange([
									"...why were the robots a threat? Well...I made those mechanical monstrosities under orders from the High Priest of the Fallen Star. They seek to make augmented, self-sustaining bodily extensions for...[c/7F5FBF:them].",
									"...you dismantled all three of the mechs to be used for the cult's goal? Hm...\n"
								  + "\n"
								  + "...thank you. Maybe now I can return to my normal work.",
								]);
							}
						}
					}
				}
			}
			return mechanicChatPool;
		}

		public static bool CanMechanicBeForceFed(NPC npc) => true;

		public static void OnMechanicForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				"[c/7F7F7F:<" + npc.GivenName + " is completely silent as you feed yourself into her throat, simply helping the process along with steady, calculated swallows, until her system has completely accepted its spontaneous input.>]\n"
			  + Main.rand.NextFromCollection(new List<string>
				{
					"...intake works fine. My stomach seems eager to get to know you, too. It's not often I have someone give themselves to me as brain food.",
					"...and that, " + player.name + ", is a hands-on demonstration of the human body's capacity for ingestion. Naturally, it follows that I'll demonstrate digestion now.",
					"...your flavors interlock nicely. I'll study your tastes more in-depth later; for now, I have things I'd like to continue blueprinting. Stay quiet and don't move...much.",
				})
			);
		}


		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.6",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.7",
			});

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Mechanic.Hardcore");
			}
		}

		public override void PostAI(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			static void RollForRandomGulp(ref bool gulp) => gulp |= Main.rand.NextBool(3, 100);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC hopelessRomantic = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			bool resolveHopelessRomantic = false;
			RollForRandomGulp(ref resolveHopelessRomantic);
			RollForRandomGulp(ref resolveHopelessRomantic);
			if (hopelessRomantic != null && hopelessRomantic.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && resolveHopelessRomantic)
				PredNPC.Swallow(npc, hopelessRomantic);

			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			bool resolveBestGirl = false;
			RollForRandomGulp(ref resolveBestGirl);
			RollForRandomGulp(ref resolveBestGirl);
			if (bestGirl != null && bestGirl.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && resolveBestGirl)
				PredNPC.Swallow(npc, bestGirl);

			NPC bootlegChippy = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Clothier);
			bool resolveBootlegChippy = false;
			RollForRandomGulp(ref resolveBootlegChippy);
			RollForRandomGulp(ref resolveBootlegChippy);
			RollForRandomGulp(ref resolveBootlegChippy);
			RollForRandomGulp(ref resolveBootlegChippy);
			if (bootlegChippy != null && bootlegChippy.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && resolveBootlegChippy)
				PredNPC.Swallow(npc, bootlegChippy);

			NPC steamLass = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			bool resolveSteamLass = false;
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			RollForRandomGulp(ref resolveSteamLass);
			if (steamLass != null && steamLass.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && resolveSteamLass)
				PredNPC.Swallow(npc, steamLass);

			if (!ModContent.GetInstance<V2ServerConfig>().RandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().MaxSwallowRange || Main.CurrentPlayer.CurrentCaptor() is not null)
				return;

			bool shouldHaveBrainFood = false;
			RollForRandomGulp(ref shouldHaveBrainFood);

			if (Main.netMode != NetmodeID.Server && Main.CurrentPlayer.whoAmI == Main.myPlayer && Main.CurrentPlayer.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldHaveBrainFood)
			{
				List<string> potentialRandomGulpLines = [
					"...I need some brain food. You'll have to do.",
					"...sorry, but I need fuel, and you were right there.",
				];
				PredNPC.SwallowWithTextIfApplicable(
					npc,
					Main.CurrentPlayer,
					"[c/7F7F7F:<Without warning, " + npc.GivenName + " stuffs you down her throat, headfirst. With your body being compacted into a rather tight state due to her space-efficient outfit, " + npc.GivenName + " pats her belly exactly once before returning to her work.>]\n"
				  + Main.rand.NextFromCollection(potentialRandomGulpLines)
				);
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => Main.bloodMoon ? 6.5 : 3.25;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 38.85;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 3,
				seconds: 0
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(4.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				3
			);
		}
	}
}
