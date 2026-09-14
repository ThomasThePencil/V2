using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables.Potions;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.TownNPCs.Nurse
{
	public static class NurseStuff
	{
		public static class ItemTheftRules
		{
			public static DigestionLootRule ClothingHat => new DigestionLootRule(
				type: (npc, pred) => ItemID.NurseHat,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => 1.0
			);
			public static DigestionLootRule ClothingTop => new DigestionLootRule(
				type: (npc, pred) => ItemID.NurseShirt,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => 1.0
			);
			public static DigestionLootRule ClothingBottom => new DigestionLootRule(
				type: (npc, pred) => ItemID.NursePants,
				amount: (npc, pred) => 1,
				chance: (npc, pred) => 1.0
			);
		}
		public static Nurse AsNurse(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Nurse predNurse))
				throw new Exception("this instance of the Nurse can't be pred or prey");

			return predNurse;
		}
		public static NurseProfile PredNurseProfile = new NurseProfile();
	}

	public class NurseProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public NurseProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Nurse/Nurse_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Nurse/Nurse";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			if (npc.altTexture == 1)
				exactTextureToUse += "_Party";

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Nurse;
	}

	public partial class Nurse : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public bool randomGutHeal;
		public bool healTypeChoice;
		public int originalHealPrice;
		public int healOvertime;
		public bool digestScamPatient;
		public int healPlayerIndex;
		public int armsDealerHealTime = 0;
		public static int ArmsDealerMaxHealTime => V2Utils.SensibleTime(minutes: 6);

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Nurse;

		public override void SetDefaults(NPC npc)
		{
			npc.aiStyle = 7;

			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsV2NPC().GetNewDialogue = GetNurseChat;

			npc.AsV2NPC().NewAIMethod = V2NurseAI;

			npc.AsFood().DefinedBaseSize = 1.1625;
			npc.AsPred().WeightGainRatio = 0.56; //0.06
			npc.AsPred().MaxStomachCapacity = 1.8;
			npc.AsPred().BaseStomachacheMeterCapacity = 200.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.65;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanNurseBeForceFed;
			npc.AsPred().OnForceFed = OnNurseForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = null;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.65;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;

			npc.AsNurse().randomGutHeal = false;
			npc.AsNurse().healOvertime = 0;
			npc.AsNurse().originalHealPrice = 0;
			npc.AsNurse().digestScamPatient = false;
			npc.AsNurse().healPlayerIndex = -1;
			npc.AsNurse().armsDealerHealTime = 0;

			npc.AsFood().OnDigestedBy += OnKilledByDigestion_GrantCheapskateGoal;

			npc.AsFood().ItemTheftRules =
			[
				NurseStuff.ItemTheftRules.ClothingHat,
				NurseStuff.ItemTheftRules.ClothingTop,
				NurseStuff.ItemTheftRules.ClothingBottom,
			];
		}

		public override void ResetEffects(NPC npc)
		{
			if (PredNPC.GetStomachTracker(npc) is null || PredNPC.GetStomachTracker(npc).Prey.FindAll(x => x.Type == PreyType.Player).Count <= 0)
			{
				npc.AsNurse().healOvertime = 0;
				npc.AsNurse().digestScamPatient = false;
				npc.AsNurse().healPlayerIndex = -1;
				npc.AsNurse().armsDealerHealTime = 0;
			}
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => NurseStuff.PredNurseProfile;

		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public List<string> GetNurseChat(NPC npc, Player player)
		{
			npc.AsNurse().originalHealPrice = 0;
			npc.AsNurse().healTypeChoice = false;

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC guide = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Guide);
			NPC hopelessRomantic = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			NPC carefreeSwitch = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());

			List<string> nurseChatPool = new List<string>();
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
				bool noDigest = (npc.AsNurse().randomGutHeal || npc.AsNurse().originalHealPrice > 0) && !npc.AsNurse().digestScamPatient && npc.AsNurse().healPlayerIndex != -1 && npc.AsNurse().healPlayerIndex == player.whoAmI;
				if (Main.bloodMoon)
				{
					nurseChatPool.AddRange([
						"Just shut up and melt into ass fat already. Seriously, I eat dozens of dipshits like you without a single inch added onto me while my chest weighs half a hundred kilograms because of you assholes, and other girls get basketball-sized cheeks for free after ONE good meal?",
						"God, you weigh WAY too much. All that MEAT hanging around without adding to my cheeks. As such, I'm prescribing you a one-way trip onto my BUTT so you start weighing IT down instead of ME!",
					]);
				}
				else
				{
					nurseChatPool.AddRange([
						"...just running a neuro assessment on you while you're in there...yup, full strength in all limbs.",
						"You know, I really wish the people I eat would stop sending themselves straight to my chest...",
					]);
					if (hopelessRomantic != null)
					{
						nurseChatPool.AddRange([
							"...I wonder if " + hopelessRomantic.GivenName + " would find me even more attractive like this...or, better yet, if he would want to be in your place...",
						]);
					}
					if (noDigest)
					{
						nurseChatPool.AddRange([
							"Hope you're healing up alright in there...mainly because I'd like to make sure you pay your dues once you're done. Maybe you'll even let me add a little bit of you to my ass as a bonus...",
							"I'd hope you're enjoying your time. Largely because I expect you to pay me once you're all healed up. Healthcare isn't free, even when it's inside a stomach like mine.",
						]);
						if (npc.AsNurse().randomGutHeal)
						{
							nurseChatPool.AddRange([
								"Don't worry, I got an order for this restraint. Now quit resisting or I'll make it tighter.",
								"Stop squirming so much. You looked ill, so I decided to give you my special treatment. The more you move around in there, the longer this is going to take...not that I mind...",
								"It's for your safety. You were a high fall risk! Probably a high FAT risk, too, with all that adventuring you do!",
							]);
						}
					}
					else
					{
						nurseChatPool.AddRange([
							"Normally, I don't digest my patients, but since you were going to get yourself killed anyway, and the Hippocratic Oath says nothing about my stomach harming you, I'll make an exception.",
							"Still wanting to stay in there, or would you like to get out and have a lollipop for trying?...hah, I'm just kidding. You aren't getting out.",
							"As a word of warning, if eating you ends up making me catch some sort of illness, I'm including it in your next medical bill.",
							"Hmm...well, my stomach sounds perfectly healthy, and it feels perfectly healthy too. Seems you're just what the nurse ordered.",
							"You had better put all the fat you're about to give me onto my butt, instead of bumping me up a cup size like everyone else does. I am SO tired of people feigning sickness to fondle my breasts...",
						]);
						if (digestScamPatient)
						{
							nurseChatPool.AddRange([
								"Oh, what's that? You wanted to be healed, not hurt? Well, you shouldn't have tried to undercut me, then. Enjoy being a nutrient soup and my future ass fat...hopefully...",
								"Maybe next time, you'll pay me enough. If you couldn't pay your medical fees like everybody else, then you shouldn't have asked me to eat you.",
								"Scamming prick...you BETTER add a new layer of fat to my ass, you hear me? You don't, I'll FORCE you down my throat and KEEP racking up your debt to me until you do.",
							]);
						}
						else
						{
							nurseChatPool.AddRange([
								"Well, if it hurts so much, I can offer you some tramadol.",
								"The more you fight it, the quicker you'll end up as a new cellulite deposit. Hopefully, on my butt...",
							]);
						}
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					switch (player.statLife)
					{
						case int i when i > (int)((double)player.statLifeMax2 * 0.5):
							nurseChatPool.AddRange([
								"I don't think I like your tone. Better keep that tongue to yourself before I cram it into my ass with the rest of you.",
								"If you're about to ask me to heal you for [c/FF0000:basically nothing], I swear, you're gonna be headed [c/FF0000:straight to my ass]...hopefully.",
								"Don't tell [c/FF0000:your dumb ass went out and got a papercut] just to be able to come in here...I'm already booked solid enough as is.",
								"If you get blood on my floor, [c/FF0000:you'll be BEGGING me to give you treatment] by the time my stomach's done with you.",
								"If you're gonna [c/FF0000:die], do it either outside my house or [c/FF0000:inside my gut.] Got it?",
							]);
							break;
						case int i when i < (int)((double)player.statLifeMax2 * 0.5):
							if (PredNPC.CanSwallow(npc, player))
							{
								nurseChatPool =	[
									"You were a high fall risk. This way, you won't be a high FALL risk; only a high FAT risk, which I'm willing to take if it means you'll ACTUALLY ADD TO MY ASS.",
									"Next time, don't come in here half-digested if you don't want me finishing the job, dumbass. Better add some meat to the glutes while you're melting in there.",
									"THERE! Now STAY put so you melt faster. You better make sure you take up PERMANENT residence on the cheeks in back, or I swear, you'll be in there OVER AND OVER UNTIL YOU DO.",
									"You were already about to DIE as it is. Might as well put your pathetic ass to good use trying to pad out mine...",
								];
								PredNPC.SwallowWithTextIfApplicable(
									npc,
									player,
									"[c/7F7F7F:<As soon as you open your mouth to ask a question, " + npc.GivenName + " grabs you and forcefully thrusts you headfirst into her mouth. Within what feels like a few seconds, you're crammed into her stomach, which is already beginning to pulverize you.>]\n"
								  + nurseChatPool
								);
							}
							return null;
					}
				}
				else
				{
					nurseChatPool.AddRange([
						"You would think people would have a higher IQ than the pH of all the guts they keep jumping into. But considering what, and who, I've seen inside people's butts...well, after a while, you stop being surprised.",
						"Can I offer you a CHG bath? It's mint-flavo- I mean, mint-scented.",
						"Would you like a lollipop? Yeah? Too bad, you're getting an endoscope instead. Sit still long enough for me to examine your insides WITHOUT going into them, and you might get a reward.",
						"...I won't even bother reprimanding you. Yes, you can stare at them while I heal you, but don't touch. A woman as healthy as me has a VERY good metabolism.",
					]);
					switch (player.statLife)
					{
						case int i when i < player.statLifeMax2 && i > (int)((double)player.statLifeMax2 * 0.66):
							nurseChatPool.AddRange([
								"Staying healthy enough, I see. A few burns and scars, but nothing serious. They should heal up in a little while, as long as you don't do anything to worsen them.",
								"If you're about to ask me to heal you for basically nothing, I swear, you're gonna be headed right to my ass...hopefully.",
								"Please don't tell me you went out and got a papercut or two just to be able to come in here...I'm already booked solid enough as is.",
							]);
							break;
						case int i when i < (int)((double)player.statLifeMax2 * 0.66) && i > (int)((double)player.statLifeMax2 * 0.33):
							nurseChatPool.AddRange([
								succubus != null
									? "You look half-digested. Did you ask " + succubus.GivenName + " out to dinner again?"
									: "You look half-digested. Did you play around with a slime too long again?",
							]);
							break;
						case int i when i < (int)((double)player.statLifeMax2 * 0.33):
							nurseChatPool.AddRange([
								"Looks like you left your arm at the door. Lemme just get that for you...I'm sure you don't mind if I cram it down my throat for a cleaning while I examine the rest of you.",
								"Sheesh, what melted off half YOUR face? If you're coming to me like this without payment, I might as well just put you in \"quarantine\" and finish the job...",
							]);
							break;
					}
					if (player.AsPred().StomachTracker?.Prey.Count > 0)
					{
						if (player.AsPred().SafeStomach)
						{
							nurseChatPool.AddRange([
								"When I said I need you to sit still for the MRI, I meant both of you. Tell your contents to quit moving.",
							]);
						}
						else
						{
							nurseChatPool.AddRange([
								"Turn your head and cough...no, not burp, cough. No, I said- ugh, look, do you want my help or not?",
							]);
						}
					}
					else
					{
						nurseChatPool.AddRange([
							"Hunger pangs? Constant stomach growling? Desire to eat live creatures?...yes, those are all the signs of rising predatory needs. Perfectly normal. I recommend one or more healthy human-sized creatures per meal, no less frequently than 3 times a day.",
							"Hm. Your stomach hurts, you say? Well, there are two possibilities: either you're very hungry, or you have a stomachache. Do you feel hungry?",
						]);

					}
					if (guide != null)
					{
						if (guide.IsFoodFor(player))
						{
							if (player.AsPred().SafeStomach)
							{
								nurseChatPool.AddRange([
									"I can hear your gut wanting to digest him from here. You really shouldn't be trying to stave it off just to keep " + guide.GivenName + " alive in there, you know. It's not healthy.",
									"Alright, sit down...your \"patient\" might not be getting acid burns, but mine WILL be in a minute if you don't let me get your gut working on him again.",
								]);
							}
							else
							{
								nurseChatPool.AddRange([
									"I- ...oh, lemme guess. That's " + guide.GivenName + " in there.\n"
								  + "\n"
								  + "...well, at least that's one source of acid burns checked off the list...",
									"Alright, sit down...your notoriously heartburn-inducing \"patient\" should be well into your system by now, so I'm just going to conduct a quick examination to make sure he's digesting well.",
								]);
							}
						}
						else if (guide.IsFoodFor(npc))
						{
							nurseChatPool.AddRange([
								"Eh, keep it quiet. At least with " + guide.GivenName + " in my gut, I know EXACTLY what can hurt him and what can't, and can examine him accordingly.",
								"Don't worry about " + guide.GivenName + "; I've tackled heartburn before, and I'll fight it off again. He'll...HOPEFULLY be adding to my glutes soon enough.",
							]);
						}
						else
						{
							nurseChatPool.AddRange([
								"I really need to have a talk with " + guide.GivenName + ". Just how many times a week can one guy come in with lava burns, acid burns, or both?",
								guide.GivenName + " has been connected to so many heartburn cases around here, I've lost count. Just how much of a fight could he possibly be putting up...?",
							]);
						}
					}
					if (hopelessRomantic != null)
					{
						if (hopelessRomantic.IsFoodFor(player))
						{
							if (player.AsPred().SafeStomach)
							{
								nurseChatPool.AddRange([
									"...as much as I'd like to keep an eye on " + hopelessRomantic.GivenName + " by keeping him in MY gut instead of yours, I feel the need to remind you it's not healthy to prevent your stomach from digesting your meals.",
								]);
							}
							else
							{
								nurseChatPool.AddRange([
									"...at least let me have a taste again before you eat him next time...hmf.",
									"...alright, sit down. Let's hope you're not getting any metal poisoning from him. I know him to have quite the collection of bullets he keeps with him...",
								]);
							}
						}
						else if (hopelessRomantic.IsFoodFor(npc))
						{
							nurseChatPool.AddRange([
								"Mmmf...always nice to have him tucked away in there...o- oh. Hello there. Don't mind the stomach; just tending to a...particularly valued patient.",
								"Before you ask, no, my stomach is not available for healing. There's a very important patient in there right now.",
							]);
						}
						else
						{
							nurseChatPool.AddRange([
								"Hey, has " + hopelessRomantic.GivenName + " mentioned needing a check-up lately?...n- no reason, of course. Just curious.",
							]);
						}
					}
					if (salad != null)
					{
						if (salad.IsFoodFor(player))
						{
							if (player.AsPred().SafeStomach)
							{
								nurseChatPool.AddRange([
									"...really? You're not even going to digest her? That's hardly a healthy way to eat a dryad...",
									"Whatever you did to make your system not digest " + salad.GivenName + ", un-do it. You're actively hurting your body...\n\n"
								  + "[c/BFBFBF:(...that, and you're looking more and more appetizing. Might cram you into me, write it off as a snack break...)]",
								]);
							}
							else
							{
								nurseChatPool.AddRange([
									"...god damnit, you got to her first...well, whatever. She's better off being gut fodder, anyway. Shady plant magic can't be trusted over tried and true medical practices.",
									"[c/7F7F7F:<" + npc.GivenName + " catches a glance at your midsection, then looks the other way with her arms crossed. She seems...almost jealous?>]\n"
								  + "Well, that's a healthy meal for you, at least. Dryads generally are a pretty nutritious choice, though the pickier preds I've seen don't like them much.",
								]);
							}
						}
						else if (salad.IsFoodFor(npc))
						{
							nurseChatPool.AddRange([
								"Mmmm...now THAT'S a healthy lunch. Nice and active, too, so she should give my stomach a workout for the rest of the day. Now, do you need anything looked at?",
								"Huh? Oh, that dryad's currently busy being a meal for me. She's a healthy option, too. You should have her once she comes back, if you're looking for some healthy food that ISN'T me...",
							]);
						}
						else
						{
							nurseChatPool.AddRange([
								"...oh, yeah, I'll be with you in just a moment.\n"
							  + "[c/BFBFBF:(I swear, " + salad.GivenName + " flaunts around that ass of hers much more, and I'll have to take a lunch break to see if I can add it to mine...)]",
							]);
						}
					}
					if (carefreeSwitch != null)
					{
						if (carefreeSwitch.IsFoodFor(player))
						{
							if (player.AsPred().SafeStomach)
							{
								nurseChatPool.AddRange([
									"...I see " + carefreeSwitch.GivenName + " gave you a lollipop of sorts. You...don't seem to be digesting her, either. I should probably give you a quick check-up...",
									"Hmm...no, your system's not responding to her the way it should. Is that something you just randomly decided to do for a while, or a medical condition I should be looking into?",
								]);
							}
							else
							{
								nurseChatPool.AddRange([
									"...I see " + carefreeSwitch.GivenName + " gave you a lollipop of sorts. Must've felt like crashing in your gut. Be sure to check back in with me in a few hours. A girl that high in fat can clog up your system FAST if she doesn't digest well.",
									"Hmm...yeah, your system seems to be digesting that sweet tooth of hers just fine, at least for now. Still, keep an eye on your blood sugar and your fat levels, and give me a ring if you get any bad signs.",
								]);
							}
						}
						else if (carefreeSwitch.IsFoodFor(npc))
						{
							nurseChatPool.AddRange([
								"[c/7F7F7F:<You feel the compulsion to ask where" + carefreeSwitch.GivenName + " is. " + npc.GivenName + " tries to keep quiet, but her bloated, squirmy belly and the belch she stifles with her left hand say all you need to know.>]",
								"Huh? Where'd " + carefreeSwitch.GivenName + " get off to?\n"
							  + "[c/7F7F7F:<" + npc.GivenName + " tries to keep down a burp...but fails, and sighs defeatedly.>]\n"
							  + "Alright, so I had an unhealthy meal. Sue me. I was hungry, and she likes to feed me when she's done her check-ups...at least, when she's not eating me instead.",
							]);
						}
						else
						{
							nurseChatPool.AddRange([
								"Make sure " + carefreeSwitch.GivenName + " keeps an eye on her sugar and comes in again soon, alright? She'll probably need another dosage of healthy food before long...",
								"Hey, while you're here: remind " + carefreeSwitch.GivenName + " that her next check-up's tonight, and you make sure she's taking her prescribed one apple a day!",
							]);
						}
					}
					if (player.inventory[player.selectedItem].type == ItemID.Apple)
					{
						nurseChatPool.AddRange([
							"...that's an apple. What are you expecting? Apples are healthy, at least for younger people. For the older ones, you might need something a bit more filling...",
							"...what? It's an apple. What's the big holdup? Sit down and let me make sure you're alive, at least...",
						]);
					}
					if (player.AsPred().TotalMeals >= 50)
					{
						nurseChatPool.AddRange([
							"No, you can't fit someone in a PEG tube. Stop asking.",
						]);
					}
					if (player.AsFood().TotalTimesDigested >= 15 && player.AsFood().TotalTimesDigested <= 25)
					{
						nurseChatPool.AddRange([
							"...maybe you should be the one to ask me to say \"ahh\" for a change?",
							"Hmm. You've already been mulched a total of...lemme check your chart..." + player.AsFood().TotalTimesDigested + " times. Be a bit more careful.",
						]);
					}
					else if (player.AsFood().TotalTimesDigested > 50 && player.AsFood().TotalTimesDigested <= 500)
					{
						nurseChatPool.AddRange([
							"Shouldn't you be the one asking me to say \"ahh\"?",
							"The preds around this world have eaten you " + player.AsFood().TotalTimesDigested + " times already? Huh...wonder if any of them belched up a banner for you...",
						]);
					}
					else if (player.AsFood().TotalTimesDigested > 500)
					{
						nurseChatPool.AddRange([
							"Feels like you should be the one asking me to say \"ahh\". You've definitely gotten gurgled enough by now to make a gal believe you'd want that.",
							"Sheesh, you've been churned up " + player.AsFood().TotalTimesDigested + " times!? Ever consider a change of profession? Try some farming, perhaps? Maybe try moving in with someone who needs a dependable snack?",
						]);
					}
					if (Main.IsItAHappyWindyDay)
					{
						nurseChatPool.AddRange([
							"Haah, I've had to chase down my medical instruments all day. This gale is really keeping me fit, at least...",
							"The winds are fierce outside today. Beware of flying needles and hungry predators, and don't get too full of air.",
						]);
					}
					if (Main.IsItRaining)
					{
						nurseChatPool.AddRange([
							"Great. Now my uniform's all wet...and now you won't stop staring. See, this is why I need to get more weight onto my ass instead...",
							"If you stay out there too long, you'll catch a cold. I suggest having some nice, warm meals and keeping out of the rain.",
						]);
					}
					if (Main.IsItStorming)
					{
						nurseChatPool.AddRange([
							"I don't DO shock therapy. Go outside and sit under a tree if you need it that badly.",
							"I have had to treat SO many electrocuted patients today, it's not even funny. Just stay inside, for your safety and my sanity.",
						]);
					}
				}
			}
			return nurseChatPool;
		}

		public override void AI(NPC npc)
		{
			ModdedTownNPCAI.AI_007_TownEntities(npc);
		}

		public override void PostAI(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			if (PredNPC.GetStomachTracker(npc)?.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.ArmsDealer) is PreyData crushAsPrey)
			{
				NPC crush = crushAsPrey.Instance as NPC;
				npc.AsNurse().armsDealerHealTime += 1;
				if (npc.AsNurse().armsDealerHealTime >= ArmsDealerMaxHealTime)
				{
					npc.AsNurse().armsDealerHealTime = 0;
					crush.position = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
					crush.velocity = new Vector2(npc.direction * 12.5f, -2.5f);
					crush.AsFood().EatenSafetyFrames = 20;
					PredNPC.GetStomachTracker(npc).Prey.Remove(crushAsPrey);
					SoundEngine.PlaySound(
						npc.AsPred().StandardBurps,
						npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f)
					);
					return;
				}
				else if (npc.AsNurse().armsDealerHealTime % 15 == 0)
				{
					if (crush.life < crush.lifeMax)
						crush.life += 1;
				}
				return;
			}

			static void RollForRandomGulp(ref bool gulp) => gulp |= Main.rand.NextBool(7, 200);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC hopelessRomantic = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			bool gulpDownCrush = false;
			RollForRandomGulp(ref gulpDownCrush);
			RollForRandomGulp(ref gulpDownCrush);
			RollForRandomGulp(ref gulpDownCrush);
			if (hopelessRomantic != null && hopelessRomantic.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && PredNPC.GetStomachTracker(npc)?.Prey.Count == 0 && gulpDownCrush)
				PredNPC.Swallow(npc, hopelessRomantic);

			NPC guide = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Guide);
			bool placeGuideInQuarantine = false;
			RollForRandomGulp(ref placeGuideInQuarantine);
			if (guide != null && guide.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && placeGuideInQuarantine)
				PredNPC.Swallow(npc, guide);

			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			bool tryToStealSaladAss = false;
			RollForRandomGulp(ref tryToStealSaladAss);
			RollForRandomGulp(ref tryToStealSaladAss);
			RollForRandomGulp(ref tryToStealSaladAss);
			if (salad != null && salad.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && tryToStealSaladAss)
				PredNPC.Swallow(npc, salad);

			NPC partyGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);
			bool tryToConvertCakeLoverIntoCake = false;
			RollForRandomGulp(ref tryToConvertCakeLoverIntoCake);
			if (partyGirl != null && partyGirl.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && tryToConvertCakeLoverIntoCake)
				PredNPC.Swallow(npc, partyGirl);

			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			bool tryToStealBestGirlAss = false;
			RollForRandomGulp(ref tryToStealBestGirlAss);
			RollForRandomGulp(ref tryToStealBestGirlAss);
			if (bestGirl != null && bestGirl.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && tryToStealBestGirlAss)
				PredNPC.Swallow(npc, bestGirl);

			if (!ModContent.GetInstance<V2ServerConfig>().RandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().MaxSwallowRange || Main.CurrentPlayer.CurrentCaptor() is not null)
				return;

			bool shouldTryToAddPlayerToAss = false;
			RollForRandomGulp(ref shouldTryToAddPlayerToAss);

			if (Main.netMode != NetmodeID.Server && Main.CurrentPlayer.whoAmI == Main.myPlayer && Main.CurrentPlayer.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldTryToAddPlayerToAss)
			{
				if (Main.CurrentPlayer.statLife < (double)Main.CurrentPlayer.statLifeMax2 / 2.0)
				{
					npc.AsNurse().randomGutHeal = true;
					npc.AsNurse().healTypeChoice = false;
					npc.AsNurse().healOvertime = 0;
					npc.AsNurse().digestScamPatient = false;
					List<string> potentialRandomGulpLines = new List<string>
					{
						"Just know that since your insurance won't cover it, you'll have to pay me once you're healed up.",
						"You're a high fall risk, and I'd rather keep you safe in my gut than out there getting hurt.",
					};
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<" + npc.GivenName + "'s stomach growls softly; she proceeds to grab you and slowly guide you down her throat. As you settle in her middle, you find that there aren't any acids to be found, and the air is stunningly breathable.>]\n"
					  + "Don't worry, just putting you in quarantine before you ask. " + Main.rand.NextFromCollection(potentialRandomGulpLines)
					);
				}
				else
				{
					npc.AsNurse().randomGutHeal = false;
					npc.AsNurse().healTypeChoice = false;
					npc.AsNurse().healOvertime = 0;
					npc.AsNurse().digestScamPatient = false;
					List<string> potentialRandomGulpLines = new List<string>
					{
						"Let's hope your insurance will cover this...either that, or that you'll add a bit to my backside.",
						"I'm sure you don't mind a quick \"thorough examination\" of your body.",
						"After all, it’s easier to eat you than treat you, even if the two are pretty easily mixed up.",
					};
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<" + npc.GivenName + "'s stomach growls impatiently; she proceeds to grab you and cram you down her throazat, doing her best to hurry you down so that her belly can get to work.>]\n"
					  + "Don't worry, just taking a quick lunch break. " + Main.rand.NextFromCollection(potentialRandomGulpLines)
					);
				}
			}
		}

		public static bool CanNurseBeForceFed(NPC npc) => PredNPC.GetStomachTracker(npc)?.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.ArmsDealer) is null;

		public static void OnNurseForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				"[c/7F7F7F:<" + npc.GivenName + "'s stomach growls with glee as you cram yourself into her mouth and throat; shrugging, she just gulps you down without a care and pats her gut once you're settled in.>]\n"
			  + "Well, that's one way to give me a lunch break, I guess. Make sure to add a little bit to the back, alright? It's the least you can do, if you want me to eat you that badly..."
			);
			npc.AsNurse().healPlayerIndex = -1;
			npc.AsNurse().healOvertime = 0;
			npc.AsNurse().digestScamPatient = false;
		}


		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.6",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.7",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.8",
			});

			if (player.IsFoodFor(npc, out bool pastTense) && !pastTense && npc.AsNurse().healPlayerIndex != -1 && npc.AsNurse().healPlayerIndex == player.whoAmI && npc.AsNurse().digestScamPatient)
			{
				deathReasonKeyList.AddRange(new List<string>
				{
					"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.NoFundsForHeal.1",
					"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.NoFundsForHeal.2",
					"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.NoFundsForHeal.3",
					"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.NoFundsForHeal.4",
				});
			}

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Nurse.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey)
		{
			if (PredNPC.GetStomachTracker(npc)?.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.ArmsDealer) is PreyData crushAsPrey && !Main.bloodMoon)
				return 0.0;

			return Main.bloodMoon ? 2.3 : 1.15;
		}

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 12.5;

		public static void OnDigestionKill(NPC npc, PreyData digestedPrey)
		{
			if (npc.AsNurse().healPlayerIndex != -1 && digestedPrey.Type == PreyType.Player && digestedPrey.Instance.whoAmI == npc.AsNurse().healPlayerIndex)
				npc.AsNurse().healPlayerIndex = -1;
		}

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			//2, 30
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 0,
				seconds: 3
			);
			return baseAbsorptionRate;
		}

		public override void FindFrame(NPC npc, int frameHeight)
		{
			npc.frame.Width = 160;
		}

		public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
		{
			boundingBox = new Rectangle(
				(int)npc.Center.X - 18,
				(int)npc.Center.Y - 27,
				36,
				54
			);
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(5.0 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				5
			);
		}

		public static void OnKilledByDigestion_GrantCheapskateGoal(NPC npc, Entity pred)
		{
			if (pred is not Player predPlayer)
				return;

			int num4 = predPlayer.statLifeMax2 - predPlayer.statLife;
			for (int j = 0; j < Player.MaxBuffs; j++)
			{
				int num5 = predPlayer.buffType[j];
				if (Main.debuff[num5] && predPlayer.buffTime[j] > 60 && (num5 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num5]))
					num4 += 100;
			}

			int health = predPlayer.statLifeMax2 - predPlayer.statLife;
			bool removeDebuffs = true;
			if (NPC.downedGolemBoss)
				num4 *= 200;
			else if (NPC.downedPlantBoss)
				num4 *= 150;
			else if (NPC.downedMechBossAny)
				num4 *= 100;
			else if (Main.hardMode)
				num4 *= 60;
			else if (NPC.downedBoss3 || NPC.downedQueenBee)
				num4 *= 25;
			else if (NPC.downedBoss2)
				num4 *= 10;
			else if (NPC.downedBoss1)
				num4 *= 3;

			if (Main.expertMode)
				num4 *= 2;

			int copperCoins = (int)((double)num4 * 1.2);
			if (copperCoins > 0 && copperCoins < 1)
				copperCoins = 1;
			int originalHealPrice = copperCoins;

			if (originalHealPrice < 0)
				originalHealPrice = 0;

			PlayerLoader.ModifyNursePrice(predPlayer, npc, health, removeDebuffs, ref originalHealPrice);

			if (originalHealPrice >= 20000)
				ModContent.GetInstance<Cheapskate>().TrySetCompletion(predPlayer);
		}
	}
}
