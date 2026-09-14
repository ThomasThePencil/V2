using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.NPCs.Vanilla.TownNPCs.Painter
{
	public partial class Painter : GlobalNPC
	{
		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public static List<string> GetPainterChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC salad = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Dryad);
			NPC helloNurse = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Nurse);
			NPC electricityFan = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Mechanic);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			NPC suspiciouslyPersonShapedCake = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.PartyGirl);
			NPC steamEnjoyer = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Steampunker);
			NPC foxBimbo = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.BestiaryGirl);

			List<string> painterChatPool = [];
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
					painterChatPool.AddRange([
						"[c/FF0000:QUIET!] I can't focus on my latest work with you [c/FF0000:screaming for help] in there!",
						"Ughhh, I KNEW I should've eaten lighter tonight...[c/FF0000:or maybe even heavier]...shut up, SHUT UP, [c/FF0000:SHUT UP IN THERE!]",
					]);
				}
				else
				{
					painterChatPool.AddRange([
						"S- steady in there...if you move around too much, you'll mess up one of my brushstrokes!",
					]);
					if (noDigest)
					{
						painterChatPool.AddRange([]);
					}
					else
					{
						painterChatPool.AddRange([
							"Hey, stop kicking and- NO! Aww, GREAT! Now the best part of this piece is all messed up!",
							"Would you mind settling down and digesting? I really wanna paint this lovely gut, but all this...YOU is in the way like this...",
						]);
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					painterChatPool.AddRange([
						"Tonight gives me a lot of inspiration. Inspiration to make people [c/FF0000:my beautifully blood-red meals!]",
						"If you look out at the [c/FF0000:yearning, hungering] moon in the sky, you can see it glint red onto the rivers! It's a [c/FF0000:truly great] piece idea.",
						"These sorts of nights are great for finding [c/FF0000:vampires] to paint. They're all REALLY pretty...\n[c/FF0000:Especially all the tasty girls...]",
					]);
				}
				else
				{
					painterChatPool.AddRange([
						"Steady, steady...aaaaand...done!...oh! Hi! You're just in time for my break! I just got done painting a lovely piece!",
						"Huh? Titanium white?...I'm afraid I'm all out. Have been for years. It's not easy to get people to quit painting parts of their cars with that color...and it doesn't even look good!",
						"No, no, no! There are SO many different shades of gray! Who the hell told you there were only 50? I oughta give them a trip to my stomach...",
						"Hey! Just finishing up my latest masterpiece!...a- as long as I can get these last few brushstrokes right, of course.",
					]);
					if (!player.Male)
					{
						painterChatPool.AddRange([
							"Hm? Do I need a meal? Well...I think just painting you will do fine!...and it helps that you look so...so, SO good...a- as a subject, of course!",
						]);
					}
					else
					{
						painterChatPool.AddRange([
							"Hm? Do I need a meal? Well...i- if you have any tasty-lookin' girls you don't need, I can always put 'em to better use...and paint a pretty picture of 'em, too!",
						]);
					}
					if (salad != null)
					{
						if (salad.IsFoodFor(player))
						{
							painterChatPool.AddRange([
								"Hey!...oh, is that " + salad.GivenName + " in your stomach? Ugh, luckyyyyy...she's the prettiest mea- I MEAN model around!",
								"Oh, you must be havin' a GREAT time with a treat like that dryad in your belly! Do you mind if I paint you while you're still full of her?",
							]);
						}
						else if (salad.IsFoodFor(npc))
						{
							painterChatPool.AddRange([
								"...mmm...mine...tasty, beautiful " + salad.GivenName + ", best treat in town...all mine...",
								"...oh! J- just a minute...! Currently digesting a REALLY tasty salad...she's perfectly happy in my gut, too!\n"
							  + "[c/BFBFBF:<You hear muffled furious shouting in " + npc.GivenName + "'s stomach which strongly suggests the opposite.>]",
							]);
						}
						else
						{
							painterChatPool.AddRange([
								salad.GivenName + " looks REALLY pretty...ask her to come over and be my next art subject!...really soon!",
								"You know, doctors always tell me that one of the best options for staying in good shape to paint is to munch on salads. Can you nab the one livin' a couple doors down and get her here?",
							]);
						}
					}
					if (helloNurse != null)
					{
						if (helloNurse.IsFoodFor(player))
						{
							painterChatPool.AddRange([
								"...oh, is that " + helloNurse.GivenName + " in your stomach? She makes a great centerpiece for art...and a great snack...send her my way next time!",
								"Hm? Do I need a nurse to eat? No...well, not RIGHT NOW, at least...of course, it helps that she's currently bein' a meal for you instead!",
							]);
						}
						else if (helloNurse.IsFoodFor(npc))
						{
							painterChatPool.AddRange([
								"...oh, " + helloNurse.GivenName + "? W- well, I was just painting her, getting a good show of her good side...her r- REALLY good front side...and, well, I just couldn't help myself!",
								"Hiya! Oh, yeah, don't worry about this big ol' belly! Just a very HEALTHY gutful of girl settling into my stomach, haha!\n"
							  + "[c/BFBFBF:<A less-than-amused groan emanates from within " + npc.GivenName + "'s gut, the medical practitioner inside not appreciating the terrible joke.>]",
								"What? You gotta talk to " + helloNurse.GivenName + "? Well, if you talk loud enough to my belly, she miiight be able to hear you! Get it out fast, though...I don't think she has much longer, and I gotta paint a lovely picture of me and my meal before she churns up.",
							]);
						}
						else
						{
							painterChatPool.AddRange([
								helloNurse.GivenName + " looks really pretty...do you think she'd be willin' to come over and model for an art piece real quick? Askin' for a friend.",
								"Nurses are always super friendly to me! They help me deal with the stomachaches from the occasional gutful of watercolors or ink, and they taste great, too!",
							]);
						}
					}
					if (npc.position.Y < Main.worldSurface)
					{
						if (WorldGen.tEvil >= 5)
						{
							painterChatPool.AddRange([
								"There aren't many plants on corrupted surface soil, but when you CAN find them, they make for a great show of how life finds ways to flourish. Makes me wonder if life can find a way to flourish in my gut, too...I'd never have to eat again!\n\n"
							  + "...not that I wouldn't. Some gals are just too tasty to pass up.",
								"I've painted a landscape of the Corruption once or twice. It's beautiful, in a very dreary sort of way. Doesn't taste all that great, though, and I hear eatin' too much corrupted stuff gives you some kind of sickness.",
							]);
						}
						if (WorldGen.tBlood >= 5)
						{
							painterChatPool.AddRange([
								"The Crimson makes for a really macabre sort of display. It's kinda like if you took everything that ISN'T pretty about the human body and went \"okay, but what if it was huge?\", and turned the result into a landscape.",
								"People say you get really sick if you eat too bloody stuff from the giant fleshmasses I find engagin' landscapes...I wouldn't blame them. Most of the things there are only even remotely appetizing as art subjects, NOT as easy meals.",
							]);
						}
						if (WorldGen.tGood >= 5)
						{
							painterChatPool.AddRange([
								"Sometimes, when you're explorin' hallowed land...well, actually, a LOT of the time...you'll see rainbows in the sky. I hear they taste great, and I KNOW they're stunning when put to a fresh canvas.",
								"Unicorns are some of the Hallow's most majestic creatures...at least, when you can actually get them to sit still. I lost too many canvases and got a BAD stomachache the last time I tried paintin' one...but it digested well, at least!",
							]);
						}
						if (Main.IsItAHappyWindyDay)
						{
							painterChatPool.AddRange([
								"Gahhh, it's so WINDY today! How am I supposed to focus on my art with these gusts messing up my form!?",
								"Feh, I can't put ANYTHING from color to canvas like this! The wind keeps knocking over my things!",
							]);
						}
						if (Main.IsItRaining)
						{
							painterChatPool.AddRange([
								"I like listening to the rain pitter-patter against the windowsill. It makes for pleasant background noise while I paint, especially if you couple it with the calming gurgles of a full, still belly.",
								"Sometimes, while it's raining, you'll see special species of fish flying around. They're both really tasty and make for majestic art subjects...if you can keep them still and outside of your stomach, anyway.",
							]);
							if (Main.hardMode)
							{
								painterChatPool.AddRange([
									"I tried taking a nature walk recently in a light rain, and found myself being chased by a couple angry rain clouds that wanted me drenched! They were certainly an inspiration for a piece thereafter, and fairly filling, too...but I didn't know clouds could be so mean!",
								]);
							}
						}
						if (Main.IsItStorming)
						{
							painterChatPool.AddRange([
								"A lot of people get spooked by thunder, but honestly...? I find it a nice parallel to dashes of inspiration, broad brushstrokes of genius!",
								"Inspiration comes less often, but strikes as hard as lightning in weather like this! I can show you an example in my next painting, if you want!",
								"Hey, random question...if I ate a lightning bolt, or even a whole thundercloud, do you think I'd be able to paint at breakneck speeds while it digests? I could make so many pieces in a single dash of inspiration...!",
							]);
						}
					}
				}
			}
			return painterChatPool;
		}
	}
}
