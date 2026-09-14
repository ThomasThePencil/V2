using BetterDialogue;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using V2.Core;
using V2.Items.Voraria.Charms;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using V2.Items.Voraria.Consumables.Potions;
using V2.Items.Voraria.Weapons.Ranged.Throwables;
using V2.NPCs.Voraria.TownNPCs.Succubus.ChatButtons;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Voraria.TownNPCs.Succubus
{
	public static class SuccubusStuff
	{
		public static SuccubusProfile SuccubusProfile = new SuccubusProfile();
	}

	public class SuccubusProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public SuccubusProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Voraria/TownNPCs/Succubus/Lucinda_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => "Lucinda";

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Voraria/TownNPCs/Succubus/Lucinda";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = 0;
			if (!V2.GetFooled)
				bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("V2/NPCs/Voraria/TownNPCs/Succubus/Lucinda_Head");
	}

	[AutoloadHead]
	public class Lucinda : ModNPC
	{
		const int BaseTownNPC = NPCID.Dryad;

		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/NPCs/Voraria/TownNPCs/Succubus/Lucinda_WeightBase_BellyBase";
		public override string HeadTexture => "V2/NPCs/Voraria/TownNPCs/Succubus/Lucinda_Head";

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[BaseTownNPC];
			NPCID.Sets.ExtraFramesCount[NPC.type] = NPCID.Sets.ExtraFramesCount[BaseTownNPC];
			NPCID.Sets.AttackFrameCount[NPC.type] = NPCID.Sets.AttackFrameCount[BaseTownNPC];
			NPCID.Sets.DangerDetectRange[NPC.type] = NPCID.Sets.DangerDetectRange[BaseTownNPC];
			NPCID.Sets.AttackType[NPC.type] = NPCID.Sets.AttackType[BaseTownNPC];
			NPCID.Sets.AttackTime[NPC.type] = 60;
			NPCID.Sets.AttackAverageChance[NPC.type] = NPCID.Sets.AttackAverageChance[BaseTownNPC];
			NPCID.Sets.HatOffsetY[NPC.type] = NPCID.Sets.HatOffsetY[BaseTownNPC];

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = -1
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			NPC.Happiness
				.SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Love)
				.SetNPCAffection(NPCID.Nurse, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Guide, AffectionLevel.Like)
				.SetNPCAffection(NPCID.Stylist, AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Princess, AffectionLevel.Dislike)
				.SetNPCAffection(NPCID.Angler, AffectionLevel.Hate)
				.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate)
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Hate)
				.SetBiomeAffection<OceanBiome>(AffectionLevel.Love)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Like)
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
				.SetBiomeAffection<CorruptionBiome>(AffectionLevel.Hate)
				.SetBiomeAffection<CrimsonBiome>(AffectionLevel.Hate)
				.SetBiomeAffection<DungeonBiome>(AffectionLevel.Hate);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.TownNPCs.Succubus"),
			});
		}

		public override void SetDefaults()
		{
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.lifeMax = 700;
			NPC.damage = 35;
			NPC.defense = 22;
			NPC.knockBackResist = 0.5f;
			NPC.HitSound = SoundID.NPCHit1;
			AnimationType = BaseTownNPC;

			NPC.AsV2NPC().GetNewDialogue = GetSuccubusChat;

			NPC.AsFood().DefinedBaseSize = 1.15;
			NPC.AsPred().WeightGainRatio = 0.125;
			NPC.AsPred().MaxStomachCapacity = 2.2;
			NPC.AsPred().BaseStomachacheMeterCapacity = 155.0;

			NPC.AsPred().SmallGulps = Gulps.Short;
			NPC.AsPred().SmallGulpThreshold = 0.45;
			NPC.AsPred().BigGulps = Gulps.Standard;
			NPC.AsPred().CanBeForceFed = CanSuccubusBeForceFed;
			NPC.AsPred().OnForceFed = OnSuccubusForceFed;

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;
			NPC.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			NPC.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			NPC.AsPred().OnDigestionKill = null;
			NPC.AsPred().MouthSoundRawOffset = NPC.TrueCenter() + new Vector2(NPC.direction * 8f, -14f);
			NPC.AsPred().SmallBurps = Burps.Humanoid.Small;
			NPC.AsPred().SmallBurpThreshold = 0.75;
			NPC.AsPred().StandardBurps = Burps.Humanoid.Standard;
			NPC.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;

			NPC.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			NPC.AsPred().GetVisualBellySize = GetVisualBellySize;

			NPC.AsFood().OnDigestedBy = PreyNPC.OnKilledByDigestion_GrantLivePreyGoal;

			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.OnFire3] = true;
			NPC.buffImmune[BuffID.Burning] = true;
			NPC.buffImmune[BuffID.ShadowFlame] = true;
			NPC.lavaImmune = true;
		}

		public override void ModifyTypeName(ref string typeName) => typeName = "Succubus";

		public override bool CanTownNPCSpawn(int numTownNPCs) => ModContent.GetInstance<V2MasterSystem>().freedSucc;

		public override ITownNPCProfile TownNPCProfile() => SuccubusStuff.SuccubusProfile;

		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public static List<string> GetSuccubusChat(NPC npc, Player player)
		{
			LucindaHelpButton.HelpIndex = 0;

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);

			List<string> succubusChatPool = [];
			V2Utils.FigureOutWhatTimeItIs(
				out bool pastMorning,
				out int hour,
				out int minute,
				out int second,
				out MealTime mealTime
			);
			double totalBellyWeight = PredNPC.GetCurrentBellyWeight(npc);
			bool playerIsFood = player.IsFoodFor(npc, out bool playerWasAlreadyDigested);
			bool isFoodForPlayer = npc.IsFoodFor(player);
			if (playerIsFood && !playerWasAlreadyDigested)
			{
				if (Main.bloodMoon)
				{
					succubusChatPool.AddRange([
						"I warned you not to get in my way, didn't I? Now quiet down like the helpless gut fodder you are.",
					]);
				}
				else
				{
					succubusChatPool.AddRange([
						"Finally got you right where I want you: ready to plump up my demonic derrière even more.",
						"Getting cozy in there? I know I'm loving having you in there; hope you don't mind being demon fat.",
						"I wonder how much you'll bulk up my breasts...maybe they'll finally burst this old shirt. I know a few people who'd love that.",
						"I can't wait to see how much better my thighs look with a few dozen pounds of you draped over them, gutmeat.",
						"Comfy in there, lunch? I'd hope so...'cause the only place you're goin' from where you are right now is right downstairs to my ass and my thighs. Maybe a little for the rack, too...",
					]);
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					succubusChatPool.AddRange([
						"Get outta my face, before I stuff you right into it. I'm REALLY hangry right now.",
						"These blood moons always get me so FUCKIN' MAD...leave me alone, or I'll digest you!",
						"Just- could you just...agh, get in my belly or get lost!",
						"I need to scream! After that, I'm gonna eat anything in my sight, you included!",
						"Whaddaya want, gut-meat-to-be? Can't you see I'm pissed!?",
						"I'm in the worst mood possible tonight, meat. Stay outta my way, or you won't stay outta my stomach!",
					]);
					if (PredNPC.GetStomachTracker(npc) is not null)
					{
						succubusChatPool.AddRange([
							"You think I'm full? You and those CHUMPS back home don't know SHIT about bein' full! Get me more or get in my gut, meatsack!",
							"Me? Content? I'll have eaten the rest of the WORLD before I'm satisfied, startin' with you if you don't scram!",
						]);
						if (playerIsFood && playerWasAlreadyDigested)
						{
							succubusChatPool.AddRange([
								"How are you already-...y'know what, WHATEVER! Just get in my gut already!...again!",
							]);
						}
					}
				}
				else
				{
					if (PredNPC.GetStomachTracker(npc) is not null)
					{
						switch (GetVisualBellySize(npc))
						{
							default:
								succubusChatPool.AddRange([
									"I can't figure out if my gut is or isn't empty. How about you give me a little help solvin' that problem?",
									"The worst kind of food is the kind that doesn't bloat up your gut. Howzabout you be a better brunch than whatever's currently in or out of mine?",
								]);
								break;
							case 1:
								succubusChatPool.AddRange([
									"Hey. Don't mind the- [c/00BB00:*urp!*] -rumbles from my waist. Just ate a little appetizer.",
									"Hm? Oh, this? Well, a day's always better with a good snack! Now, why don't you help turn that into a decent meal?",
									"Oh, this? Just had a light snack, that's all. Think you'll wanna add to that?",
									"Hey. Just had a- [c/00FF00:*hic!*] -little treat for myself. Don't mind me.",
								]);
								break;
							case 2:
								succubusChatPool.AddRange([
									"[c/00BB00:*belch!*]\nAhhh, that's the good stuff. Nothing like a bloated little belly to make a better day. Definitely want some more, though.",
									"Just finished up a good appetizer here. Could go for a lot more, though...you offerin'?",
									"Huh? Oh, this. Just a nice little- [c/00BB00:*burp!*] -snack to tide me over until lunch. Don't worry about it, lunch.",
									"Finally got the starts of a good meal goin' here. Mind if I make it a little better with you as the next course?",
									"Hey there, f-[c/00BB00:*oourp.*]- ...food. Care to help me fix up my post-snack munchies a little better?",
								]);
								break;
							case 3:
								succubusChatPool.AddRange([
									"[c/00BB00:*BWORP!*]\nTHERE we go...that's some good eats right in there, though I'll go for a little more, I think. Got any recommendations, such as yourself?",
									"Nice day, yeah? Even better now that I've got a good-sized meal in my gut. Then again...could always use a second course.",
									"Huh? Yeah, I could go for more. I've still got a TON of room in this gut for morsels like you, just you wait!",
								]);
								break;
							case 4:
								succubusChatPool.AddRange([
									"[c/00BB00:*BWOOOURRP!*]\nNow THAT's a good meal. Almost makes me not wanna eatcha...almost. You free to be food?",
									"Huh? Yeah, I could go for more! Just...takin' a minute before I do. Don't wanna eat too fast and get hiccups.",
									"What's the matter, hotshot? Jealous of my big, food-filled belly? You should be, because I'm just- [c/00FF00:*hic!*] -...er...j- just gettin' started.",
								]);
								break;
						}
					}
					else
					{
						succubusChatPool.AddRange([
							"Hey. Looking to spend some time on the waistline of an incredible pred like me?",
							"Huh? Am I hungry? I'm ALWAYS hungry, morsel. Hungry for SNACKS like you! YEAH!",
							"Not lookin' to head back home, at least for the moment, so maybe keep some good food around, yeah?",
							"Got any rowdy townsfolk you need taken care of? I'll be sure to put 'em to REAL good- ...what do you mean, you don't?",
							"Yeah, I'm an apex pred. I eat chumps like YOU for breakfast, lunch, AND dinner, EVERY DAY. Got a problem with that?",
						]);

						if (Main.dayTime)
						{
							succubusChatPool.AddRange([
								"All those slimes goin' around are...honestly, exceptionally mediocre prey. I prefer my meals with some MEAT to them, if you know what I mean.",
								"Every so often, a small, pink slime'll show up in the forests. Might wanna munch on it when you see it...I hear it tastes REALLY good.",
								"Tried to grill a slime earlier...the damn thing just burned right up, right in front of me! So much for grilled gel...those things wouldn't last 5 seconds back home.",
							]);

							if (Main.IsItAHappyWindyDay)
							{
								succubusChatPool.AddRange([
									"Damn, just take a look at this wind! Bet you could calm some rowdy prey down REAL quick with gusts like this knockin' 'em around in ya!",
									"These sorts of days are always great. Plenty of opportunities for some good prey to blow right on into my mouth.",
								]);
							}
						}
						else
						{
							succubusChatPool.AddRange([
								"Those zombies that always shamble around at these hours are really annoyin'. They're not even good food...you'll just get food poisonin' if you try.",
								"I feel like those eyes flyin' around all the time at night could really be good for my eyesight. Maybe yours, too, if you're hungry enough.",
								"Might go out and gulp down a few dozen of those little fairies I sometimes see when the moon's at its peak. Hear they're real good at gettin' preds into their prime.",
							]);
						}

						if (Main.IsItRaining)
						{
							succubusChatPool.AddRange([
								"...well, this sure never happened back home. I'm not gonna, like...melt into a puddle or anything else dumb if I touch the rain, right?",
								"The sounds this rain makes against the roof...they kinda remind me of fingers, happily drummin' on a calm, full gut. It's...weirdly relaxin'. Could listen to it for a while.",
							]);
						}
						if (Main.IsItStorming)
						{
							succubusChatPool.AddRange([
								"HAH! LOOK at all that lightning! HEAR all that thunder! That's the heavens above, SCARED of me and my gut! I'll eat every last angel up there one day, y'hear!?",
								"One of these days, I'M gonna eat one of those HUGE stormclouds, and I'm gonna melt it RIGHT down into fat for my breasts and my backside, to show that I'm the BEST pred there is. Just you wait, morsel.",
							]);
						}
					}
				}
			}
			return succubusChatPool;
		}

		public override void AddShops()
		{
			NPCShop succubusShop = new NPCShop(NPC.type, "Shop");
			succubusShop.Add<CharmBetterDigestion>();
			succubusShop.Add<CharmRegenFromAbsorption>();
			succubusShop.Add<ThrowableHotSauceBottle>();
			succubusShop.Register();

			NPCShop nurseShop = new NPCShop(NPCID.Nurse, "Shop");
			nurseShop.Add(ItemID.LesserHealingPotion, [Condition.NotDownedEowOrBoc]);
			nurseShop.Add(ItemID.HealingPotion, [Condition.DownedEowOrBoc, Condition.NotDownedMechBossAny]);
			nurseShop.Add(ItemID.GreaterHealingPotion, [Condition.DownedMechBossAny, Condition.NotDownedMoonLord]);
			nurseShop.Add(ItemID.SuperHealingPotion, [Condition.DownedMoonLord]);
			nurseShop.Add(ItemID.AdhesiveBandage);
			nurseShop.Add<FastDigestionPotion>();
			nurseShop.Add<StomachacheMeterCapacityPotion>([Condition.DownedEowOrBoc]);
			nurseShop.Add(new Item(ItemID.LifeCrystal) { shopCustomPrice = Item.buyPrice(gold: 20) }, [Condition.DownedSkeletron]);
			nurseShop.Add<PureSwallowBoost1>();
			nurseShop.Register();
		}


		public override void PostAI()
		{
			if (NPC.CurrentCaptor() is not null)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			static void RollForRandomGulp(ref bool gulp) => gulp |= Main.rand.NextBool(8, 100);

			List<NPC> nearbyResidentNPCs = NPC.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC guide = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Guide);
			bool shouldSnackOnGuide = false;
			RollForRandomGulp(ref shouldSnackOnGuide);
			RollForRandomGulp(ref shouldSnackOnGuide);
			if (guide != null && guide.Distance(NPC.Center) <= NPC.AsPred().MaxSwallowRange && shouldSnackOnGuide)
				PredNPC.Swallow(NPC, guide);

			NPC foxBimbo = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.BestiaryGirl);
			bool shouldSnackOnFoxBimbo = false;
			RollForRandomGulp(ref shouldSnackOnFoxBimbo);
			RollForRandomGulp(ref shouldSnackOnFoxBimbo);
			if (foxBimbo != null && foxBimbo.Distance(NPC.Center) <= NPC.AsPred().MaxSwallowRange && shouldSnackOnFoxBimbo)
				PredNPC.Swallow(NPC, foxBimbo);

			NPC nurse = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Nurse);
			bool shouldSnackOnNurse = false;
			RollForRandomGulp(ref shouldSnackOnNurse);
			RollForRandomGulp(ref shouldSnackOnNurse);
			if (nurse != null && nurse.Distance(NPC.Center) <= NPC.AsPred().MaxSwallowRange && shouldSnackOnNurse)
				PredNPC.Swallow(NPC, nurse);

			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			bool shouldSnackOnSalad = false;
			RollForRandomGulp(ref shouldSnackOnSalad);
			RollForRandomGulp(ref shouldSnackOnSalad);
			RollForRandomGulp(ref shouldSnackOnSalad);
			if (salad != null && salad.Distance(NPC.Center) <= NPC.AsPred().MaxSwallowRange && shouldSnackOnSalad)
				PredNPC.Swallow(NPC, salad);

			NPC scrooge = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.TaxCollector);
			bool shouldSnackOnScrooge = false;
			RollForRandomGulp(ref shouldSnackOnScrooge);
			RollForRandomGulp(ref shouldSnackOnScrooge);
			RollForRandomGulp(ref shouldSnackOnScrooge);
			if (scrooge != null && scrooge.Distance(NPC.Center) <= NPC.AsPred().MaxSwallowRange && shouldSnackOnScrooge)
				PredNPC.Swallow(NPC, scrooge);

			if (!ModContent.GetInstance<V2ServerConfig>().RandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(NPC.Center) > NPC.AsPred().MaxSwallowRange || Main.CurrentPlayer.CurrentCaptor() is not null)
				return;

			bool shouldSnackOnPlayer = false;
			RollForRandomGulp(ref shouldSnackOnPlayer);

			if (shouldSnackOnPlayer)
			{
				PredNPC.SwallowWithTextIfApplicable(
					NPC,
					Main.CurrentPlayer,
					"[c/7F7F7F:<Suddenly, with a sly glint in her eyes, " + NPC.GivenName + " starts stuffing you down her throat. By the time you can process what's happened, she's already packed you away in her gut>]\n"
				  + "Ahhh...always feels good to catch my prey unaware. Better get comfy in there, meat, 'cause you're on a one-way trip STRAIGHT to my waistline."
				);
			}
		}

		public static bool CanSuccubusBeForceFed(NPC npc) => true;

		public static void OnSuccubusForceFed(NPC npc, Player player)
		{
			PredNPC.SwallowWithTextIfApplicable(
				npc,
				player,
				"[c/7F7F7F:<Catching you as you start to cram yourself down her throat, " + npc.GivenName + " guides you to your destination: one very eager demon tum, belonging to one very eager demon.>]\n"
			  + Main.rand.NextFromCollection(new List<string>
				{
					"Ahhh...that's the good stuff. Always love a meal that knows where they belong: meltin' in my gut.",
					"There. That gets you all tucked away, gutmeat. Now, don't you start tryin' to get out...but, by all means, thrash around as much as you like.",
					"You know, I'm not usually given willing prey on a platter, so to speak...but you really just look too tasty to say no to.",
				})
			);
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Succubus.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Succubus.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Succubus.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Succubus.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Succubus.5",
			});
			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Succubus.Hardcore");
			}
		}

		public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackMagic(ref float auraLightMultiplier)
		{
			auraLightMultiplier = 1f;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.DemonScythe;
			attackDelay = 40;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 12f;
			randomOffset = 0f;
		}

		public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
		{
			itemWidth = 40;
			itemHeight = 40;
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => Main.bloodMoon ? 3.6 : 1.8;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 20;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 5
			);
			return baseAbsorptionRate;
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				4
			);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Width = 96;
		}

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle(
				(int)NPC.Center.X - 16,
				(int)NPC.Center.Y - 18,
				32,
				44
			);
		}
	}
}
