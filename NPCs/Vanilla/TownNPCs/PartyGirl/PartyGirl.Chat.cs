using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.PlayerHandling;

namespace V2.NPCs.Vanilla.TownNPCs.PartyGirl
{
	public partial class PartyGirl : GlobalNPC
	{
		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public static List<string> GetPartyGirlChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);

			List<string> partyGirlChatPool = new List<string>();
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
				partyGirlChatPool.AddRange(new List<string>
				{
					"Mmmf, you filled me up so well, " + player.name + "! I feel like a big ol' water balloon...and I'm ready to party the night away!",
					"Hmmm...  [c/00BB00:*urp!*]\n"
				  + "Y'know, you made a really nice " + (BirthdayParty.PartyIsUp ? "mid-party" : "pre-party") + " meal! Even tastier than all those cakes I eat...",
					"Yeesh, you really filled me up like a birthday balloon...better hope I don't pop, hehee!\n"
				  + "...aww, don't get upset. I'm just jokin'! It'll take a lot more than one snack to pop open THIS piñata!",
				});

				if (noDigest)
				{
					partyGirlChatPool.AddRange(new List<string>
					{
						"Ooo, I feel like a freshly-filled piñata...filled to the brim with food! You don't even hafta worry about bein' digested! Just gotta wait 'til somebody comes to getcha out!",
						"Don'tcha worry about the juices in there. As long as I'm not hungry enough to gurgle ya, you can stay in there rent-free! Consider it a belly party JUST for you!",
						"So, you feelin' excited in there? Want me to gulp down some party favors to help ya get in the party mood? Gotta have you pumped up for my next party, after all, whether you're in me or not!",
					});
				}
				else
				{
					partyGirlChatPool.AddRange(new List<string>
					{
						"Ooo, I feel like a freshly-filled piñata...filled to the brim with food! Of course, my belly can digest much more than a piñata can...",
						"Yeah, that's right, party it up in there! Don't worry about the cleanup afterwards, either! My belly always makes sure none of the leftovers go to waste...",
						"Oof, you're really havin' fun in there, aren'tcha? Thanks again for the gift; you made a great appetizer! Of course, the party cake'll be the main course...",
					});
				}
			}
			else
			{
				if (PredNPC.GetStomachTracker(npc) is not null)
				{
					if (PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.HallowBoss) is PreyData sprinkles && GetEmpressDigestionStage(npc) > 0)
					{
						partyGirlChatPool.AddRange(new List<string>
						{
							"Huh? What happened to the candy fairy's crown? I'm savin' it for later! Duh! After all, I don't really have room for dessert with all this dinner in me...ah, just kiddin'! I'm just enjoyin' the candy fairy right now.",
							"Hey, you know what'd go REALLY well with this massive mountain of sprinkles? Some ice cream. Shouldn't have sprinkles without some ice cream to put 'em on, after all!",
							"[c/7F7F7F:<" + npc.GivenName + " gleefully and absentmindedly hums, her feet kicking behind her, as her gut continues to melt the Empress. She seems to be in a state of bliss.>]",
							"You know, while I'm stuck here, meltin' down this gutful of sprinkles, I wanted to ask you something.\n"
						  + "Do you ever wonder if people, out there somewhere, would wanna be in the spot that this so-called \"Empress\" is in right now?\n"
						  + "\n"
						  + "I don't, because I already know they'd love it! I'm the best belly party hostess around, after all!",
						});
						if (!sprinkles.NoHealth)
						{
							partyGirlChatPool.AddRange(new List<string>
							{
								"Mmmmm~mmm! That fairy was- [c/00BB00:OURP!*] -JUST what I needed! Thanks for the meal, " + player.name + "!",
								"Y'know, I never knew sprinkles could be so fat...then again-\n"
							  + "[c/7F7F7F:<As if on cue, an enraged screech can be heard from within " + (npc.GivenName.Last().ToString() == "s" ? npc.GivenName + "'" : npc.GivenName + "'s") + " gut.>]\n"
							  + "Aw, COME ON! You know those thunder thighs of yours were too juicy and jiggly to NOT chow down on! Ugh...I swear, some candy is just so RUDE...anyway, what were you sayin'?",
								"Eep! Easy there...! Sorry, " + player.name + ", that candy fairy's REALLY not in the mood to be food...but I WON'T- [c/00FF00:*hic-][c/00BB00:OURP!*] -GIVE UP! She's MY candy, and ONLY mine!",
							});
						}
						else
						{
							switch (GetEmpressDigestionStage(npc))
							{
								case 1:
									partyGirlChatPool.AddRange(new List<string>
									{
										"Ahh...feels great to finally have that candy fairy all calmed down! Proooobably gonna take me a while to digest her, but hey! I got what I wanted! :D",
										"I've gotta hand it to ya, " + player.name + ": you REALLY know how to make a gal the life of the party! Nobody'll EVER keep outta the celebration ever again while I've got a gut like this!",
										"[c/00BB00:BWWOOOOOUUUUURRRRRP!*]\n"
									  + "...what? You KNOW you're supposed to say \"bless you!\" when somebody lets a good burp rip like that, right? It's a sign they liked their meal, and I sure loved mine!",
									});
									break;
								case 2:
								case 3:
									partyGirlChatPool.AddRange(new List<string>
									{
										"Hmmhmm...oh, hey! Still just workin' hard on that big ol' bag of sprinkles...finally got her to start softenin' up. Guess she's settlin' in fine after all!",
										"Mmmm...I always love candy like this. What good is candy if it's NOT the size of your livin' room?",
									});
									break;
								case 4:
								case 5:
									partyGirlChatPool.AddRange(new List<string>
									{
										"Seems that candy fairy's finally startin' to REALLY digest...can't wait for her to make my belly all big and soft!",
										"Hey, can you pass me some soda? I think there's still something or other from that sprinkle cake that's not sittin' all that well...a quick burp or two oughta get it loose.",
									});
									break;
							}
						}
					}
					else
					{
						switch (GetVisualBellySize(npc))
						{
							default:
								partyGirlChatPool.AddRange(new List<string>
								{
									"H- hey, don't poke my tum! I don't wanna let out all the burpiness before the burpin' contest I'm havin' with myself later!",
									"What? Think I'm gonna get all antsy over some good food? Nah...I just really like to eat!",
								});
								break;
							case 1:
								partyGirlChatPool.AddRange(new List<string>
								{
									"Huh? My belly? Aw, I'm just havin' a pre-party snack! Can't set up a party on an empty belly, now, can ya?",
									"Yeah...there are a few cupcakes or so in there, I think. Can't really remember...but hey, it keeps the tum-tum quiet!",
								});
								goto default;
							case 2:
								partyGirlChatPool.AddRange(new List<string>
								{
									"Yeah, just had some good treats to tide me over 'til the next party. Shouldn't be too hard to make this last!",
									"This? Ah, this is nothin'! Catch me the next time I gulp down a bunch of party favors, THEN I'll show ya a good gut!",
								});
								break;
							case 3:
								partyGirlChatPool.AddRange(new List<string>
								{
									"Hey! Just workin' on a nice buncha treats for myself! You can rub it, if you want! Just don't get me all burpy for no reason. Always hate that.",
									"[c/7F7F7F:<" + npc.GivenName + " gleefully swings from side to side, watching her bloated belly rock back and forth. She seems to be having fun.>]\n"
								  + "\n"
								  + "...o- oh! Hey! Whatcha up to?",
								});
								break;
							case 4:
								partyGirlChatPool.AddRange(new List<string>
								{
									"W- wow, I REALLY ate a lot...NAH, just kiddin'! This is just a TASTE of what I can pack away!",
									"Impressed that I can eat this much without breakin' a sweat? Most people are! Then again, they've got NO idea what I'm REALLY capable of, hehee!",
								});
								break;
							case 5:
								partyGirlChatPool.AddRange(new List<string>
								{
									"Mmm...now THAT's a good snack. Plenty of food to fill out the tum-tum and keep 'er quiet while I figure out what the next big thing on my pre-party prep list is.",
									"Hey, pass the soda, will you? Need something to chase all this down, and get ready for the contest I'm havin' with myself later...",
								});
								break;
						}
						if (PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.TaxCollector && !x.NoHealth) is PreyData scroogeAsPrey)
						{
							NPC scrooge = scroogeAsPrey.Instance as NPC;
							partyGirlChatPool.AddRange(new List<string>
							{
								"What? You know as well as I do he never liked my parties! It's only fair that I FORCE him to be part of 'em, by bein' part of me! I do NOT throw \"frivolous\" or \"childish\" parties, and I'm PERFECTLY responsible! HMPH! >:(",
								"Huh? What ABOUT grumpy old " + scrooge.GivenName + "!? He hates parties, hates fun, hates colors, and hates me! He's WAY better off as my " + (BirthdayParty.PartyIsUp ? "mid-party" : "pre-party") + " lunch than he is skulking in the corner all the time!",
							});
						}
						if (PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.Wizard && !x.NoHealth) is PreyData harryWizardAsPrey)
						{
							NPC harryWizard = harryWizardAsPrey.Instance as NPC;
							partyGirlChatPool.AddRange(new List<string>
							{
								"Huh? Why'd I eat that wizard guy, " + harryWizard.GivenName + "? Well, it's honestly really simple. He makes really cool sparkly effects at my parties, so I figured I might be able to get those for myself sometime soon if he's in there long enough!",
								"Don't worry about the magic guy! He's havin' a grand old time in my belly, and I'll make sure he keeps havin' a good time!",
							});
						}
						if (PredNPC.GetStomachTracker(npc).Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.BestiaryGirl && !x.NoHealth) is PreyData furryAsPrey)
						{
							NPC furry = furryAsPrey.Instance as NPC;
							partyGirlChatPool.AddRange(new List<string>
							{
								"Mmmm...y'know, " + furry.GivenName + " isn't just great at parties. She's also a great belly filler! Of course, she knows I don't mean anything bad by it...I was just in the mood for a snack, and she didn't mind!",
								"The foxgal in my gut once asked me if I've ever eaten any cute little things...\n"
							  + "\n"
							  + "...don't tell her I said this, but I've eaten a LOT of different animals before. I think the \"cute little things\" she's talking about fit into that, pretty extensively.",
							});
						}
					}
				}
				else
				{
					if (NPC.AnyNPCs(NPCID.HallowBoss))
					{
						partyGirlChatPool.AddRange(new List<string>
						{
							"...I NEED her...I REALLY NEED HER. Give me that big candy fairy, NOW.",
							"My belly's screaming out for that big bag of sprinkles, and I don't blame it. Gimme her.",
							"Just LOOK at that big fairy food cake, flying around outside me...I NEED it, yesterday.",
							"I have NEVER wanted sprinkles so badly in my life as I do that fairy that's basically MADE of 'em. Pass her here...please?",
						});
					}
					else
					{
						partyGirlChatPool.AddRange(new List<string>
						{
							"I can't decide what I like more: normal parties, afterparties, or belly parties. You got any input on that?",
							"I once went to this really cool kingdom. It had lots of cake, and not just the sweet kind, either. They partied REALLY good there; why aren't you like that?",
							"We hafta talk. It's...it's about parties. Moreso, it's about the fact that I haven't hosted a belly party in a while.",
							"I should set up an eatin' contest as a party. Would be a great way to get all the preds in town up and at 'em for some good snacks! Winner gets to eat ME, obviously!",
							"Yeah, I'm a bit of a master at plannin' parties. Just ask all the people that had one in my belly! None of 'em ever complained!",
						});
						if (Main.IsItRaining)
						{
							partyGirlChatPool.AddRange(new List<string>
							{
								"I panicked 'cause I thought there was a crash of thunder at the disco for a sec!...and then I realized it was just the ol' tum-tum.",
								"Come on, let's dance! We'll eat our fill, and shake our full bellies to the beat of the sky!",
								"Sometimes, I just wanna go gulp down a lightning bolt. Always gives me a bunch more energy, and you GOTTA have a bunch of energy to host a party!",
							});
						}
						if (Main.IsItStorming)
						{
							partyGirlChatPool.AddRange(new List<string>
							{
								"I panicked 'cause I thought there was a crash of thunder at the disco for a sec!...and then, I realized it was just the ol' tum-tum.",
								"Come on, let's dance! We'll eat our fill, and shake our full bellies to the beat of the sky!",
								"Sometimes, I just wanna go gulp down a lightning bolt. Always gives me a bunch more energy, and you GOTTA have a bunch of energy to host a party!",
							});
						} 
						if (Main.IsItAHappyWindyDay)
						{
							partyGirlChatPool.AddRange(new List<string>
							{
								"Wow, it's super windy today! I can blow my belly- [c/00BB00:*urp!*] -like a balloon with this much hot air going around!",
								"Oh, just LOOK at that- [c/00BB00:*belch!*] -breeze outside! This is the perfect day to go sip some good wind! It feels REAL nice goin' down- [c/00BB00:*burp!*] -and it feels nice in your gut, too...",
								"These kinds of days always get me really- [c/00BB00:bworp!*] -burpy...probably 'cause I keep gobblin' up all the strong gusts that blow my way, hehee! Not MY fault they wanna end up comin' back- [c/00BB00:*urp!*]",
							});
						}
						if (player.ZoneGraveyard)
						{
							partyGirlChatPool.AddRange(new List<string>
							{
								"Woo, let's-...hey, why's nobody movin'? There's a party just WAITIN' to be held here!",
								"Ever wanted to know how to party loud enough to LITERALLY wake the dead? Here, I'll teach ya!",
							});
						}
						if (DD2Event.DownedInvasionAnyDifficulty)
						{
							partyGirlChatPool.AddRange(new List<string>
							{
								"Have you seen an ogre yet? I wanna ride on the back of one!...maybe not in its gut, though. They don't seem all that fun to be in...",
							});
						}
						if (BirthdayParty.PartyIsUp)
						{
							if (NPC.freeCake)
							{
								partyGirlChatPool.AddRange(new List<string>
								{
									"Where've you been? There's a total party goin' on! Here, take a slice of cake! I'll letcha in on a little secret...I already ate the rest, and it's totally awesome!",
									"Shhh! Here! It's a piece of the cake that was made for today! Don't tell ANYONE I gave you it! It's a party surprise!...alongside the fact that I'm gonna gorge myself on the rest later.",
									"Party up! You're TOTALLY in charge of the cake, " + player.name + "! Just...if anyone asks, I did NOT eat half the sweets baked for today.",
								});
							}
							else
							{
								partyGirlChatPool.AddRange(new List<string>
								{
									"MY TIME HAS COME!...and my belly's, too! What, you think I'm NOT gonna stuff myself silly with party food?",
									"Huh? Oh, nothing special today...HA, just kiddin'! It's party time, afterparty time, and belly party time, all at once! YEAH!",
								});
							}
						}
						if (player.AsPred().StomachTracker?.Prey.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.TaxCollector && !x.NoHealth) is PreyData scroogeAsPrey)
						{
							partyGirlChatPool.AddRange(new List<string>
							{
								"FINALLY! Can't believe that guy went this long without gettin' ate. He was always rainin' on my party parade. Happy to see you've got him in the gut rave groove!",
								"...well? How was he? Hope you really give him a piece of your mind. That'll teach him to grump about at MY parties...",
							});
						}
					}
				}
			}
			return partyGirlChatPool;
		}

		public static bool CanPartyGirlBeForceFed(NPC npc) => true;
		public static void OnPartyGirlForceFed(NPC npc, Player player)
		{
			if (GetEmpressDigestionStage(npc) > 0)
			{
				PredNPC.SetChatboxText(
					npc,
					player,
					Main.rand.NextFromCollection(new List<string>
					{
						"Huh? Well, that bag of sprinkles DID fill me up pretty good...but I can never say no to a little dessert!",
						"Do I want a second course? Well, not really a second course, but a quick post-dinner snack is fine! Come on in!",
						"You wanna join the fairy? Well, I guess I can squeeze you in! Just hold still and lemme get you settled in!",
					}) + "\n"
				  + "[c/7F7F7F:<Preparing herself briefly before reaching over her gut and picking you up, " + npc.GivenName + " nonchalantly tosses you down her suddenly-cavernous throat all at once, humming with glee as you end up joining the Empress inside her titanic tum.>]"
				);
			}
			else
			{
				PredNPC.SetChatboxText(
					npc,
					player,
					Main.rand.NextFromCollection(new List<string>
					{
						"You want me to eatcha?...eh, probably a good change of pace from all the balloons and party favors I eat. Come on in!",
						"You wanna be a cake for me? Aww...how can I say no to that? Get over here and get in my belly, hehee!",
						"A pre-party snack? Hmm...well, I guess we'll be the life of the party with you inside me! Besides, I can't host a party on an empty tum!",
					}) + "\n"
				  + "[c/7F7F7F:<Before you can even BEGIN to force yourself into her, " + npc.GivenName + " happily crams your entire body into her mouth at once, gulping down your form in full to fill out her gut in a single, smooth swallow. She happily pats her newly-filled gut with a hum of gratitude.>]"
				);
			}
		}

		public static void PlayerPreyChat(NPC npc, Player player, ref List<string> chatPool)
		{

		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.PartyGirl.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.PartyGirl.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.PartyGirl.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.PartyGirl.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.PartyGirl.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.PartyGirl.6",
			});

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.PartyGirl.Hardcore");
			}
		}
	}
}
