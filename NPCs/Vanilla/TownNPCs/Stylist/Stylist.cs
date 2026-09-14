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
using V2.Items.Voraria.Consumables;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;
using V2.Sounds.Vore;

namespace V2.NPCs.Vanilla.TownNPCs.Stylist
{
	public static class StylistStuff
	{
		public static Stylist AsStylist(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Stylist predStylist))
				throw new Exception("this instance of the Stylist can't be pred or prey. best girl's best gut will have to wait, unfortunately");

			return predStylist;
		}

		public static StylistPredProfile StylistPredProfile => new StylistPredProfile();
	}

	public class StylistPredProfile : ITownNPCProfile
	{
		private readonly Asset<Texture2D> _defaultNoAlt;

		public StylistPredProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Stylist/Stylist_Default_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => "Amber";

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Stylist/Stylist";
			string outfitString = "_Default";
			if (npc.IsShimmerVariant)
				outfitString = "_Shimmer";
			exactTextureToUse += outfitString;
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			if (npc.altTexture == 1)
				exactTextureToUse += "_Party";

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Stylist;
	}

	public class Stylist : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Stylist;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsV2NPC().GetNewDialogue = GetStylistChat;

			npc.AsFood().DefinedBaseSize = 1.085;
			npc.AsPred().WeightGainRatio = 0.16;
			npc.AsPred().MaxStomachCapacity = 5.85;
			npc.AsPred().BaseStomachacheMeterCapacity = 175.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.3;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanStylistBeForceFed;
			npc.AsPred().OnForceFed = OnStylistForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = null;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 0.3;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => StylistStuff.StylistPredProfile;

		public static List<string> StylistFavoritismNames => [
			"Thomas",
			"ThomasThePencil",
			"the Sign Painter",
			"the pixelated Sign Painter",
		];

		public override void ModifyShop(NPCShop shop)
		{
			if (shop.NpcType != NPCID.Stylist)
				return;

			shop.InsertBefore(ItemID.WilsonBeardShort, ModContent.ItemType<HairDyeCapacity>(), V2ShopConditions.BeginnerStatPoints);
		}

		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public static List<string> GetStylistChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC armsDealer = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			NPC dyeTrader = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.DyeTrader);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());
			NPC partyGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);
			NPC tavernkeep = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.DD2Bartender);

			List<string> stylistChatPool = [];
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
					stylistChatPool.AddRange([
						"I told you to stay out of my hair tonight, hun. Now keep it down and digest!",
						"Ugh, just give up already! You keep kickin' around in there, and I'll just dump a few bottles of hair dye down my throat to DROWN you!",
						"You were JUST what I needed, hun: a quick Gut Cut to get the hunger pangs to shut up. No refunds, and no escape. Sorry not sorry, gut fodder.",
						"[c/BFBFBF:(...I swear, if you make me fat, I'll just \"shave\" off your arms and thighs once you come back. Bet you'll be swearin' on your soul to be a good client THEN, asshole...)]",
					]);
					if (PredNPC.GetStomachTracker(npc).Prey.Count > 1)
					{

					}
					else
					{

					}
				}
				else
				{
					stylistChatPool.AddRange([
						"Something about your- [c/00FF00:*hic!*] -scalp just makes it so unbelievably tasty. I gotta know, what kinda- [c/00FF00:*hic!*] -conditioner are you usin'? Hopefully, one that won't leave me with a- [c/00FF00:*hic!*] -stomachache...",
						"I really need to- [c/00FF00:*hic!*] -tell people about my special more...well, more than I already do. That, and I need to lower their- [c/00FF00:*hic!*] -prices a bit, I think...",
						"Between you and me, this is WAY better than just doing your hair with scissors and stuff. Way more fillin', too...",
						"Now, be careful not to- [c/00FF00:*hic!*] -rub against the walls too much. You wouldn't want your hair to get- [c/00FF00:*hic!*] -ruined before I get it styled.",
						"Ooh, are you usin' my favorite- [c/00FF00:*hic!*] -flavor of shampoo? I knew you couldn't resist it...and- [c/00FF00:*hic!*] -neither can I.~",
						"You really- [c/00FF00:*hic!*] -should try takin' better care of your hair, hun. All that- [c/00FF00:*hic!*] -blood from fightin' messes with your flavor...and your style- [c/00FF00:*hic!*] -too, of course.",
						"[c/00BB00:*BWOoooORP!*]\n"
					  + "Ooo, excuuuuse me! Gotta say, hun, you're makin' me- [c/00FF00:*hic!*] -a lot gassier than I thought you would...guess it's all that scrumptious hair on your- [c/00FF00:*hic!*] -scalp.",
						"Hope you're- [c/00FF00:*hic!*] -enjoyin' my signature Gut Cut service! Remember, no- [c/00FF00:*hic!*] -refunds!",
						"Y'know, I think you'd be- [c/00FF00:*hic!*] -a REALLY good treat for Kyoko! I know she always loves to eat meals like- [c/00FF00:*hic!*] -you...\n"
					  + "...wait, who's Kyoko? OH, right! She's a really good- [c/00FF00:*hic!*] -friend of mine! I'm sure you'll get to meet her someday!",
						"Careful not to- [c/00FF00:*hic!*] -soak your hair with juice too much. Unless you WANT to- [c/00FF00:*hic!*] -ruin it. I dunno.",
					]);
					if (player.Male)
					{
						stylistChatPool.AddRange([
							"You were delicious, sir! Unfortunately, the Gut Cut experience DOESN'T offer the option for added aftershave, on account of it tastin' awful and makin' me feel super bloated if I have any.",
							"You know, sir, I was originally- [c/00FF00:*hic!*] -gonna call this \"the Belly Barber special\". Glad I went with \"the- [c/00FF00:*hic!*] -Gut Cut experience\" instead. Rolls off the tongue, and gets clients rollin' onto- [c/00FF00:*hic!*] -mine, way better.",
						]);

						if (StylistFavoritismNames.Contains(player.name))
						{
							stylistChatPool.AddRange([
								"I bet I'm your- [c/00FF00:*hic!*] -favorite? Of course I am. I know I'm the best in the- [c/00FF00:*hic!*] -world. Now settle down in here and let my gut make you look- [c/00FF00:*hic!*] -perfect, hun.",
							]);
						}
					}
					else
					{
						stylistChatPool.AddRange([
							"You know, girl, I was originally- [c/00FF00:*hic!*] -gonna call this \"the Belly Barber special\". Glad I went with \"the- [c/00FF00:*hic!*] -Gut Cut experience\" instead. Rolls off the tongue, and gets clients rollin' onto- [c/00FF00:*hic!*] -mine, way better.",
							"Ahhh...one of the tastiest- [c/00FF00:*hic!*] -gal pals I've had yet! Of course, nothin' really beats Kyoko when it's- [c/00FF00:*hic!*] -time for my weekly practice...",
						]);
					}
					if (dyeTrader != null)
					{
						stylistChatPool.AddRange([
							"Hmm...I- [c/00FF00:*hic!*] -wonder...would " + dyeTrader.GivenName + " like how I look with you inside me? I- [c/00FF00:*hic!*] -remember him sayin' that a stuffed gut always looks nice in the right- [c/00FF00:*hic!*] -outfit...",
						]);
					}

					if (noDigest)
					{
						stylistChatPool.AddRange([
							"Now, you relax and- [c/00FF00:*hic!*] -marinate for a while...I'll check in and- [c/00FF00:*hic!*] -gulp down a drink for you in- [c/00FF00:*hic!*] -half an hour or so.",
							"You want me to send a- [c/00FF00:*hic!*] -magazine in there for you to read while you- [c/00FF00:*hic!*] -relax inside me? I'm sure you do...well, a shame I- [c/00FF00:*hic!*] -don't have one.",
							"I hope you- [c/00FF00:*hic!*] -know you're not gettin' an actual haircut. My belly doesn't- [c/00FF00:*hic!*] -do hair for free, hun.",
						]);
						if (player.Male)
						{
							stylistChatPool.AddRange([
								"Sir, please keep- [c/00FF00:*hic!*] -still. If I end up giving somebody a bad- [c/00FF00:*hic!*] -haircut because of you, I WON'T hesitate to make a- [c/00FF00:*hic!*] -meal outta you.",
							]);
						}
						else
						{
							stylistChatPool.AddRange([
								"Just- [c/00FF00:*hic!*] -relax and let my gut straighten out those knots, honey...we GOTTA- [c/00FF00:*hic!*] -get you up to speed on the local gossip.",
								"Giiiirl, you are my- [c/00FF00:*hic!*] -favorite belly filler! Now, what were we- [c/00FF00:*hic!*] -chattin' about?",
								"You know, I think you're the- [c/00FF00:*hic!*] -tastiest gal pal I've ever had. Just about the most filling, too...\n"
							  + "[c/00BB00:*urp!*]",
							]);
						}
					}
					else
					{
						stylistChatPool.AddRange([
							"Sorry, hun, no- [c/00FF00:*hic!*] -refunds! Just sit in there and marinate for a while...I'll- [c/00FF00:*hic!*] -check in to make sure you're digestin' well in- [c/00FF00:*hic!*] -half an hour or so.",
							"Well, while you- [c/00FF00:*hic!*] -wait to be digested, you've got an easy cut. Just stick your- [c/00FF00:*hic!*] -head in those acids for a bit, and your hair'll be nice and- [c/00FF00:*hic!*] -short!",
							"[c/BFBFBF:(...oh, don't make me fat, don't make me fat- ][c/00FF00:*hic!*][c/BFBFBF: -make one of the three B's too big if you want, but please, please, PLEEEEEASE don't make me fat- ][c/00FF00:*hic!*][c/BFBFBF: -I take too many cheat days as it is...)]",
							"[c/00BB00:*UUUURP!*]\n"
						  + "Mmmm...that's some grade-A- [c/00FF00:*hic!*] -flavor you've got there, hun! What products did ya use? Or are you just naturally this- [c/00FF00:*hic!*] -tasty?",
							"I guess I've just got the spider's mindset. I blame Kyoko for that...",
						]);
						if (player.Male)
						{
							stylistChatPool.AddRange([
								"Thank you for- [c/00FF00:*hic!*] -choosin' my services, Mr. " + player.name + "! I hope you enjoy your soon-to-be new look, because I run a strict no-refunds policy on gut cuts.",
								"Sir, I'm gonna need you to keep- [c/00FF00:*hic!*] -still. The digestive Gut Cut experience demands full cooperation from my- [c/00FF00:*hic!*] -clients, or else they end up digested AND unhappy with their- [c/00FF00:*hic!*] -hair."
							]);
						}
						else
						{
							stylistChatPool.AddRange([
								"Just relax and let your split ends- [c/00FF00:*hic!*] -melt away, honey...we GOTTA get you up to speed on the local- [c/00FF00:*hic!*] -gossip before the acids get to you.",
								"Giiiirl, you were the best meal- [c/00FF00:*hic!*] -EVER! Now, you take a nice, long acid soak in there, and I'll- [c/00FF00:*hic!*] -enjoy gettin' your hair all nice and cut down to size...",
								player.name + ", I gotta- [c/00FF00:*hic!*] -say, you made a great snack. Really hope your assets'll add onto- [c/00FF00:*hic!*] -mine...been feelin' JUST A LITTLE small lately...",
							]);
						}
						if (dyeTrader != null)
						{
							stylistChatPool.AddRange([
								"Try to- [c/00FF00:*hic!*] -plump up JUST the 3 B's a little for me, alright- [c/00FF00:*hic!*] -hun? I really wanna look my best for the next time " + dyeTrader.GivenName + "- [c/00FF00:*hic!*] -comes over...",
								"Now, all I need you to- [c/00FF00:*hic!*] -do is bulk my body up a bit! Got a- [c/00FF00:*hic!*] -hunch that " + dyeTrader.GivenName + "'s got a thing for gals with good- [c/00FF00:*hic!*] -assets...and I'M gonna win his heart, just you wait!",
							]);
						}
						bool bald = Main.CurrentPlayer.hair == 16 || Main.CurrentPlayer.head == ArmorIDs.Head.MonkBrows;
						bool stylish = Main.LocalPlayer.hairDye > 0;
						if (bald)
						{
							stylistChatPool.AddRange([
								"[c/00BB00:*urp.*]\n"
							  + "\"Acid-worn and bald are the- [c/00FF00:*hic!*] -same\", you say? Well, lemme prove you WRONG! My belly and I don't DO bald. Bald is- [c/00FF00:*hic!*] -NEVER a good look. Acid-worn, on the other hand...",
							]);
						}
						else if (stylish)
						{

							stylistChatPool.AddRange([
								"[c/00BB00:*BWOOOUURP!*]\n"
							  + "Mmmm-mmmm...mm-mm-MMM! That's the- [c/00FF00:*hic!*] -GOOD stuff I'm still feelin' on my tongue! I knew gettin' you to- [c/00FF00:*hic!*] -try out one of my delicious hair dyes was a great idea!",
							]);
						}
						else
						{
							stylistChatPool.AddRange([
								"[c/00BB00:*OOURP!*]\n"
							  + "Mmmm...that's some- [c/00FF00:*hic!*] -grade-A flavor you've got there, hun. What products did ya use? Or are you just- [c/00FF00:*hic!*] -naturally this good?",
							]);
						}
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					stylistChatPool.AddRange([
						"Tipping's optional, but remember: I've got teeth sharper than any razor, plenty of room in my belly, [c/FF0000:and access to your head.]",
						"Hun, you better stay outta my hair tonight...unless you [c/FF0000:wanna spend the night digestin'] to make more of it. Could really go for some good eats...",
						"I just sharpened my scissors, and your thighs look like they'd fatten up mine really nicely after a quick cut in my gut. [c/FF0000:Are you SURE you don't wanna pay extra?]",
						"The longer you sit there and keep waitin', the more likely I am to just cut to the chase and [c/FF0000:cram you down my throat.]",
						"Alright, how's your [c/FF0000:lousy scalp] gonna get styled tonight? Better pick something good, and pick it fast, [c/FF0000:before I pick it for you.]",
					]);
					if (PredNPC.GetStomachTracker(npc) is not null && PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.Dryad) is PreyData dryadAsPrey)
					{
						if (!dryadAsPrey.NoHealth)
						{
							stylistChatPool.AddRange([
								"Finally, got a good- [c/00FF00:*hic!*] -[c/FF0000:FUCKIN' MEAL] in this gut...a good salad like that does the bod- [c/00FF00:*hic!*] -wonders. I could go for some more MEAT, though...you wanna- [c/00FF00:*hic!*] -get a \"Gut Cut\" special for free, [c/FF0000:while I'm still willin' to give you the-] [c/00FF00:*hic!*] [c/FF0000:-choice?]",
								"Ahhh, that's the good stuff...- [c/00FF00:*hic!*] -...that's it, " + salad.GivenName + ", keep lettin' your body marinate in there. I'm sure you won't- [c/00FF00:*hic!*] -mind me digestin' your hair a little? The rest of you...well, a- [c/00FF00:*hic!*] -lot. [c/FF0000:You're not gettin' outta there, SALAD.]",
								"Oohhh...that was a good- [c/00FF00:*hic-][c/00BB00:BWOOAARP!*] -salad, hun. Now sit still and keep quiet; [c/FF0000:I'm in the mood to-] [c/00FF00:*hic!*] [c/FF0000:-cut something, and you look really slicable right-] [c/00FF00:*hic!*] [c/FF0000:-now.]",
							]);
						}
						else if (GetVisualBellySize(npc) >= 3)
						{
							stylistChatPool.AddRange([
								"Took that planty- [c/00FF00:*hic!*] -[c/FF0000:bitch] long enough to shut up. Maybe now she'll- [c/00FF00:*hic!*] -add to the three B's so I can get another meal for the night. What about- [c/00FF00:*hic!*] -you, hun? [c/FF0000:I'm STARVIN', hun, and you look like just the tip I-] [c/00FF00:*hic!*] [c/FF0000:-need...]",
								"Finally...- [c/00FF00:*hic!*] -...dumb vegan took FOREVER to [c/FF0000:quit her-] [c/00FF00:*hic!*] [c/FF0000:-bitchin' and moanin']. Come on, ya big sack of- [c/00FF00:*hic!*] -lettuce, just [c/FF0000:melt down a little more...] [c/00FF00:*hic-][c/00BB00:OURP!*]",
							]);
						}
					}
					else if (salad is not null)
					{
						stylistChatPool.AddRange([
							"God, I could really go for a [c/FF0000:salad] right about now...hun, go tell " + salad.GivenName + " to get over here and fill me up. [c/FF0000:NOW, or you'll go right in with her.]",
						]);
					}
				}
				else
				{
					if (PredNPC.GetStomachTracker(npc) is not null)
					{
						switch (GetVisualBellySize(npc))
						{
							default:
								break;
							case 1:
								stylistChatPool.AddRange([
									"Mmm...always a bit weird to have a potbelly like this hangin' off me. Not like it matters much...I'll be back to my slim self in no time! Now, how can I get your cut done, hun?",
									"Sit down for a sec, and I'll have you steppin' razor. Maybe a salad steppin' into my belly, too...need something healthy to get this midriff flat again.",
									"Oh, this? That's not a baby! Well...not a real one. Just a food baby!...yeah, you can poke it, but not a whole lot! Just a little.",
									"Now, now, hun. A little bloating never hurt anybody. A few minutes, and this gut'll be back to its hottest shape. Thin is in right now...I think.",
									"Hm? Distracted by something? Maybe you want to fill out this gut a bit more than it is right now...?~\n"
								  + "\n"
								  + "...aw, don't be such a downer, sweetheart. I'm just teasing! Besides, I'm on a diet, anyway! Gotta keep this bod slim for the summer shores! Now, let's see about getting you the right cut...",
									"Hey! Don't mind the stuffed stomach, just had a bit of good food! How can I work my magic on your scalp today?",
									"You want some tea? Some orange juice?...maybe just some food? I've got a bit of food in me, too, so I wouldn't blame you for askin'.",
								]);
								break;
							case 2:
								stylistChatPool.AddRange([
									"Mmm...always a bit weird to have a potbelly like this- [c/00FF00:*hic!*] -hangin' off me. Not like it matters much...I'll be back to my slim self in no time! Now, how can I get your cut done, hun?",
									"Gonna need a few salads steppin' into my stomach at this rate...look at all this gut! Of course...a cheat day every once in a while's not bad, right?",
									"Oh my God, I look pregnant! Late into it, too...did I really eat that much? Then again, I think I know a hairstyle that'd look great with this gut...",
									"Oh, this? That's not a baby! Well...not a real one. Just a really big food baby! Be careful not to- [c/00FF00:*hic!*] -poke it, though! Wouldn't want whatever's built up in there knockin' you out...",
									"Does this belly make my hair look bad? Gimme an honest answer, and I promise I won't get...TOO mad. Gimme a lie, though...",
									"My belly looks like a bald head...I'm not sure if I- [c/00FF00:*hic!*] -like that. Then again, you could say the apron counts as a nice hairstyle for it...it should be fine.",
									"Oh, hey! Sorry about the gut; I'm just working on a nice meal right now. It should be gone soon enough...in the meantime, what kinda cut do you want?",
								]);
								break;
							case 3:
								stylistChatPool.AddRange([
									"O- ooh...I look- [c/00FF00:*hic!*] -pregnant...overdue with twins, probably. I sure FEEL pregnant with twins, what with how heavy my belly is...",
									"I'm debating if this big belly looks good on me or not...what do you think, " + player.name + "? Could I make it work?...ah, who am I- [c/00FF00:*hic!*] -kiddin'? Of course I could.",
									"Please don't rest your head on my gut while I'm cuttin' your- [c/00FF00:*hic!*] -hair. It'll make the final product uneven.",
									"Hey, before I- [c/00FF00:*hic!*] -give you your cut...are my hands covered in saliva? Wouldn't want it messin' up your hair...",
								]);
								if (PredNPC.AnyPreyStillAlive(npc))
								{
									stylistChatPool.AddRange([
										"Oh, you want a haircut? Just sit in the- [c/00FF00:*hic!*] -chair, I'll do yours while my belly works on the one I have it doin'...",
										"Alright, just a minute, I'll be with you once my current client's done... what kinda cut do you want? The Gut Cut experience's booked right- [c/00FF00:*hic!*] -now, so if you want one, you'll have to wait.",
									]);
								}
								break;
							case 4:
								stylistChatPool.AddRange([
									"Oooh...I really ate a good bit, huh...well, I'm- [c/00FF00:*hic!*] -sure I can still cut your hair even with all this- [c/00FF00:*hic!*] -gut meat in the way, hun.",
									"...hey, could you- [c/00FF00:*hic!*] -pass me the orange juice? I think I could really- [c/00FF00:*hic-][c/00BB00:OURP!*] -use it right now...I always get bad- [c/00FF00:*hic!*] -hiccups after a good meal.",
									"Mmmf...if I have to carry this- [c/00FF00:*hic!*] -gut around much longer, I'm gonna have to- [c/00FF00:*hic!*] -cancel the rest of my clients for the day. I can't cut- [c/00FF00:*hic!*] -hair well with a belly this big!",
									"God, I feel so- [c/00FF00:*hic!*] -bloated...I'm sure there are people that are into this stuff, though. Maybe the right- [c/00FF00:*hic!*] -hairstyle can make it look better...",
									"Please don't- [c/00FF00:*hic!*] -rest your head on my gut when I'm cutting your- [c/00FF00:*hic!*] -hair. It'll make the final product uneven.",
									"Hey, before- [c/00FF00:*hic!*] -okay, I just wanted to check if my hands are- [c/00FF00:*hic!*] -...sticky with saliva...- [c/00FF00:*hic!*] -s- sorry about this, hun. I- [c/00FF00:*hic!*] -I get REAL bad hiccups when I'm this full.",
									"...eugh, all this food's gonna get me SO fat...if I eat, like, ANY more, I'm gonna need to ask Kyoko to shave off the weight later...",
								]);
								if (PredNPC.AnyPreyStillAlive(npc))
								{
									stylistChatPool.AddRange([
										"O- oh, you want a- [c/00FF00:*hic!*] -haircut? A- alright, just sit in the- [c/00FF00:*hic!*] -chair, I'll do yo-[c/00BB00:*ooOOUURP!*] -...y- yours, alongside the cut my- [c/00FF00:*hic!*] -belly's working on...",
										"Alright, I- [c/00FF00:*hic!*] -I'll be with you in just a- [c/00FF00:*hic-][c/00BB00:OOORRP!*] -minute... what kinda cut do you want? The- [c/00FF00:*hic!*] -Gut Cut experience's booked right now- [c/00FF00:*hic!*] -so if you want one, you'll- [c/00FF00:*hic!*] -probably have to wait...",
									]);
								}
								break;
						}
						if (PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.Dryad) is PreyData dryadAsPrey)
						{
							if (!dryadAsPrey.NoHealth)
							{
								stylistChatPool.AddRange([
									"What? My- [c/00FF00:*hic!*] -gut? Sorry about that, just had a big, meaty- [c/00FF00:*hic!*] -salad as a healthy snack...she's still- [c/00FF00:*hic!*] -kickin' around a bit in there, too.",
									"Hey, can you- [c/00FF00:*hic!*] -help me calm down the plant gal in my gut? She's REALLY not- [c/00FF00:*hic!*] -agreeing with me...if you do, your next cut's- [c/00FF00:*hic-][c/00BB00:URP!*] -...50% off.",
									"Huh? Where's " + salad.GivenName + "? She's right- [c/00FF00:*hic!*] -here! If you want, you can always book a- [c/00FF00:*hic!*] -Gut Cut experience, too, though you- [c/00FF00:*hic!*] -might have to wait a little.",
									"A- [c/00FF00:*hic!*] -really good friend of mine says dryads are- [c/00FF00:*hic!*] -the greatest meals around! Proteins and- [c/00FF00:*hic!*] -plant fibers together, all in one- [c/00FF00:*hic-][c/00BB00:URP!*] -delicious meal! I just had to try one!",
								]);
							}
							else if (GetVisualBellySize(npc) >= 3)
							{
								stylistChatPool.AddRange([
									"Mmm...I love it when a good- [c/00FF00:*hic-][c/00BB00:URP!*] -salad finally calms down. You wanna give this- [c/00FF00:*hic!*] -gut a little rub to help it break down that- [c/00FF00:*hic!*] -prissy plant gal?",
									"Hey, can you- [c/00FF00:*hic!*] -help me break down the plant gal in my gut? She's all calmed- [c/00FF00:*hic!*] -down now, but it'll take me way too- [c/00FF00:*hic!*] -long to churn her into nutrients...",
									"Mmmf...- [c/00FF00:*hic!*] -...I knew Kyoko's advice about dryads was- [c/00FF00:*hic!*] -good to follow. A few more of her type, and I'll- [c/00FF00:*hic!*] -be the hottest gal in the world in no time!",
								]);
							}
						}
						if (PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.PartyGirl) is PreyData partyGirlAsPrey)
						{
							if (!partyGirlAsPrey.NoHealth)
							{
								stylistChatPool.AddRange([
									"What? My- [c/00FF00:*hic!*] -gut? Sorry about that...lil' ol' " + partyGirl.GivenName + "- [c/00FF00:*hic!*] -INSISTED on spendin' some- [c/00FF00:*hic!*] -quality time in my gut as th- [c/00FF00:*hic!*] -thanks for doin' her hair so well.",
									"Huh? Where's " + partyGirl.GivenName + "? She's right- [c/00FF00:*hic!*] -here! She REALLY wanted a Gut Cut ex- [c/00FF00:*hic!*] -...experience, and I couldn't just say- [c/00FF00:*hic!*] -no! I- I'll...call it a cheat day.",
									"What? Me? Eatin' that cake-flavored- [c/00FF00:*hic!*] -cutie for a snack? No, nono- [c/00FF00:*hic!*] -no! I'd never! I can't eat any- [c/00FF00:*hic!*] -junk food like that! I'm on a SUPER strict diet!\n"
								  + "\n"
								  + "[c/7F7F7F:...th- that I- ][c/00FF00:*hic-][c/00BB00:OOOURP!*][c/7F7F7F: -...sometimes cheat on...not my fault she's so delicious...]",
								]);
							}
							else if (GetVisualBellySize(npc) >= 3)
							{
								stylistChatPool.AddRange([
									"Ooof...I know that gal wanted me to- [c/00FF00:*hic!*] -eat her, but I gotta say, hun...I'm kinda- [c/00FF00:*hic!*] -worried about how fat she's gonna make me. Don't wanna- [c/00FF00:*hic!*] -bulk up too much in the wrong places, y'know...?",
									"H- hun, I've got an honest- [c/00FF00:*hic!*] -question for you...does the cake-flavored gal currently- [c/00FF00:*hic!*] -churnin' away inside me make me look- [c/00FF00:*hic!*] -too fat? I...really hope not...",
									"Sheesh, she's already startin' to- [c/00FF00:*hic!*] -settle in...I can feel her goin' to the wrong- [c/00FF00:*hic!*] -spots already! Damnit, why does all the junk food have to- [c/00FF00:*hic!*] -taste the best!?",
								]);
							}
						}
						if (PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.DD2Bartender) is PreyData tavernkeepAsPrey)
						{
							if (!tavernkeepAsPrey.NoHealth)
							{
								stylistChatPool.AddRange([
									"WHAT!? Ohh, you wanna- [c/00FF00:*hic!*] -see " + tavernkeep.GivenName + "? Well, maybe if you talk loud enough over me and my- [c/00FF00:*hic!*] -GUT, you can yell at him! Better make it quick, though...he's gonna SHUT THE FUCK- [c/00FF00:*hic!*] -UP in a few minutes...",
									"Hun, lemme give ya a piece of- [c/00FF00:*hic!*] -advice. Never, ever, EVER take any sass from a baldie...or advice. Or anything other than their privilege to exist outside your gut, actually. They're all the- [c/00FF00:*hic!*] -same: no hair, no flair, so they're just a meal.",
								]);
							}
							else if (GetVisualBellySize(npc) >= 3)
							{
								stylistChatPool.AddRange([
									"Don't even get me STARTED on that dumb- [c/00FF00:*hic!*] -barkeep! He was practically BEGGING to- [c/00FF00:*hic!*] -get mulched, walkin' into MY bubble with that hairless DOME he- [c/00FF00:*hic!*] -called his head!",
									"[c/00BB00:OOOURP!*]\n"
								  + "Ugggh, why'd that stupid- [c/00FF00:*hic!*] -barkeep hafta be such a fuckin'- [c/00FF00:*hic!*] -BEEFCAKE!? He's gonna take me for- [c/00FF00:*hic!*] -EVER to melt down, never mind how hard I'll hafta work to- [c/00FF00:*hic!*] -burn him off...",
								]);
							}
						}
					}
					else
					{
						stylistChatPool.AddRange([
							"Tea? Coffee? Or do you just want some orange juice again? A lot of my clients just gulp down all three...they'd gulp me down, too, but I wasn't born yesterday.",
							"Sit down for a sec, and I'll have you steppin' razor. I guarantee, I'll give you the hottest cut around, or your next one's free.",
							"I'm tellin' you, hun, my hands are NOT coated with saliva right now. Yours, on the other hand...well, they might be, if you want my signature special cut!",
							"Hmm...yeah, definitely something low-maintenance for you. You look like the type who'd get " + (player.Male ? "him" : "her") + "self eaten just because a cute girl or guy asked.\n"
						  + "\n"
						  + "...you wouldn't mind ME askin', right? Just curious...",
							"Want me to take a little off the top? Need some scrubbed off the sides? Scissors, trimmers, razor-sharp teeth...you pick how you want what done!",
							"Welcome back! Want a quick, traditional haircut, or maybe you're here to book something more unique?\n"
						  + "\n"
						  + "...or are you just here to say hi?",
							"Either you have style, or you get styled. Gotta pick one or the other, hun."
						]);
						if (player.Male)
						{
							stylistChatPool.AddRange([
								"Hey, " + player.name + "! I'm " + npc.GivenName + ", and I'll be your barber today. What can I do for you?",
								"Which aftershave can I getcha today, mister? I'm sure there's SOMETHING here that suits your style...and your flavor!",
								"So, what kinda cut are we talking? Buzz cut, pompadour, mohawk? Maybe a bit of acid-worn to go with them?",
							]);

							if (StylistFavoritismNames.Contains(player.name))
							{
								stylistChatPool.AddRange([
									"I bet I'm your favorite? Of course I am. I know I'm the best in the world. Now take a seat and let me make you look perfect, hun.",
								]);
							}
						}
						else
						{
							stylistChatPool.AddRange([
								"Oh, you poor thing...shhh, it's alright, just lean back in the chair, and lemme solve all those tangled-up knots...",
								"Giiiiiirl, we GOTTA get you some dye for that hair of yours. Would make it look even better!...and a bit tastier, but you don't have to be the tip.",
								"A pixie cut? Sounds good! Wanna keep some lady burns, or should I trim those, too?",
								"So, what kinda cut are we talking? A long ponytail, maybe some curls, a cute little bun at the back? Maybe a Gut Cut to add some acid-worn flair?",
							]);
						}
						if (player.HasEaten("Terraria: Stylist", out int ateBestGirlHowMuch) && ateBestGirlHowMuch >= 3)
						{
							stylistChatPool.AddRange([
								"...hun, you've got a REAL hungry look on your face. You sure you wanna try and keep this feisty feast down again? Gonna need to cancel my next few clients if I end up gut fodder for a bit...",
								"Aww, what's the matter. Gettin' all hot in the oven over lil' ol' me again? Well, if you sit still for your cut, and you tip me good, I'll letcha have me for a little while, alright?",
								"Look, all I'm sayin' is that you BETTER not mess up my hair if-...no, WHEN you eat me. If my hair gets ruined because of you, you'll be a new, thick set of hips and a flashy new muffin top for me by sunrise tomorrow.",
							]);
						}
						if (BirthdayParty.PartyIsUp)
						{
							stylistChatPool.AddRange([
								"Sure, I did my hair up just for today, but...honestly? I just wanna pop balloons with my scissors and have some cupcakes.",
								"If that birthday cake gets in ANYBODY'S hair, I think I'll just have 'em for lunch and call it a day. Every gal's gotta have a cheat day once in a while, and a party like this is the perfect excuse!",
							]);
						}
						if (dyeTrader != null)
						{
							stylistChatPool.AddRange([
								"I tried one of " + dyeTrader.GivenName + "'s super-stylin' dyes once. Here's a tip, hun: the guy's a total hottie, but his wares are another story. That dye gave me indigestion for 4 hours straight, had me feeling like the fattest girl alive with how bad it bloated me, and went straight to my sides when it was finally over. Total disaster. Never again.",
								dyeTrader.GivenName + " always looks so stylish, no matter what he's got on...one of these days, I think I'll ask him out. Maybe show him my signature Gut Cut experience...I'm sure he'd be just the HOTTEST guy around with a nice, acid-worn scalp!~",
							]);
						}
						if (partyGirl != null)
						{
							stylistChatPool.AddRange([
								"Hope you like what I did to " + partyGirl.GivenName + "'s hair! She WAS gonna give me a bunch of cupcakes as a tip, but she ate 'em all while I was doing her hair. She offered to let HERSELF be my tip instead, but...I'm on a diet.",
							]);
						}
					}
				}
			}
			return stylistChatPool;
		}

		public static bool CanStylistBeForceFed(NPC npc) => true;

		public static void OnStylistForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				"[c/7F7F7F:<Squeaking in surprise as you start to cram yourself down her throat, " + npc.GivenName + " soon shrugs and helps you along your journey into her waiting stomach.>]\n"
			  + Main.rand.NextFromCollection(new List<string>
				{
					"Mmm...you REALLY wanted me to taste-test your scalp, didn't you- [c/00FF00:*hic!*] -hun? Lucky for you, I think it's- [c/00FF00:*hic!*] -PERFECT for a good little treat like you. Now- [c/00FF00:*hic!*] -don't move around too much...I've- [c/00FF00:*hic!*] -still got more haircuts to do, whether I'm- [c/00FF00:*hic!*] -stuffed with you or not!",
					"W- wow, you wanted one of my signature acid-worn hairdos REALLY bad, didn'tcha? Well, I'm more than happy to provide, since you don't seem to mind being my meal! Why don't you just marinate in there for a while and lemme know when you're done?",
					"Mmmf...something about how eager you are tells me you're NOT here for a haircut. If that's the case, I'm more than happy to have you served to me, " + (player.Male ? "sir" : "ma'am") + "! Enjoy your time as a meal, and remember: no refunds!",
				})
			  + "\n[c/7F7F7F:<After giving her now-full gut a comfy pat, she mutters something else about her diet under her breath. Guess it's a cheat day now.>]"
			);
		}

		public override void PostAI(NPC npc)
		{
			if (npc.CurrentCaptor() is not null)
				return;

			if (PredNPC.GetStomachTracker(npc)?.Prey.Count > 0)
				return;

			if (Main.GameUpdateCount % 60 != 0)
				return;

			static void RollForRandomGulp(ref bool gutCut) => gutCut |= Main.rand.NextBool(5, 100);

			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			bool shouldSnackOnSalad = false;
			RollForRandomGulp(ref shouldSnackOnSalad);
			RollForRandomGulp(ref shouldSnackOnSalad);
			if (salad != null && salad.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && shouldSnackOnSalad)
				PredNPC.Swallow(npc, salad);

			if (!ModContent.GetInstance<V2ServerConfig>().RandomGulpsAgainstPlayers)
				return;

			if (!Main.CurrentPlayer.active || Main.CurrentPlayer.dead || Main.CurrentPlayer.Distance(npc.Center) > npc.AsPred().MaxSwallowRange || Main.CurrentPlayer.CurrentCaptor() is not null)
				return;

			bool bald = Main.CurrentPlayer.hair == 16 || Main.CurrentPlayer.head == ArmorIDs.Head.MonkBrows;
			bool hairDye = Main.CurrentPlayer.hairDye != 0;

			bool shouldGiveGutCut = false;
			RollForRandomGulp(ref shouldGiveGutCut);
			if (bald)
			{
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
			}
			else if (hairDye)
			{
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
				RollForRandomGulp(ref shouldGiveGutCut);
			}

			if (shouldGiveGutCut)
			{
				if (bald)
				{
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<With little warning, " + npc.GivenName + " stuffs you down her throat, headfirst. She gives a disgruntled groan as you settle into her stomach.>]\n"
					  + "Eugh, your head tasted HORRIBLE. So bland, no REAL taste to it...at least you have a decent base flavor. Serves you right for lettin' that bald BULB you call your head STRUT AROUND outside my gut! Maybe a quick \"marination\" and some time as a new set of hips for me'll make you think twice about not havin' any hair..."
					);
				}
				else if (hairDye)
				{
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<With little warning, " + npc.GivenName + " stuffs you down her throat, headfirst. She gives a particularly gleeful hum as you settle into her stomach.>]\n"
					  + "Oooh...now THAT'S a great meal! Hate to drag you away, hun...I know I'm on a diet, and you're probably busy, but I just couldn't help myself! Your scalp looked WAY too tasty with that DELICIOUS hair dye I gave you...I just HAD to have a bite! Surely, you don't mind bein' a meal worth a cheat day for me, right?"
					);
				}
				else
				{
					PredNPC.SwallowWithTextIfApplicable(
						npc,
						Main.CurrentPlayer,
						"[c/7F7F7F:<With little warning, " + npc.GivenName + " stuffs you down her throat, headfirst. She gives a pleasant hum as you settle into her stomach.>]\n"
					  + "Oooh...now THAT'S a good meal! Sorry, hun...I know I'm supposed to be on a diet, but your scalp looked really tasty...! I bet a quick shave with the help of my belly would make you look even better, too! That, and I'm honestly pretty hungry...I'm sure ONE more cheat day can't hurt, yeah?"
					);
				}
			}
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange([
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.6",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.7",
			]);

			if (Main.bloodMoon)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.BloodMoonHaircut");
			}

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Stylist.Hardcore");
			}
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => Main.bloodMoon ? 2.5 : 1.25;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 20;

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 45
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
	}
}
