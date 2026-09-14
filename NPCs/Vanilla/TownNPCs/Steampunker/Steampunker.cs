using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace V2.NPCs.Vanilla.TownNPCs.Steampunker
{
	public static class SteampunkerStuff
	{
		public static Steampunker AsSteampunker(this NPC npc)
		{
			if (!npc.TryGetGlobalNPC(out Steampunker predSteampunker))
				throw new Exception("this instance of the Steampunker can't be pred or prey");

			return predSteampunker;
		}
		public static SteampunkerProfile PredSteampunkerProfile = new SteampunkerProfile();
	}

	public class SteampunkerProfile : ITownNPCProfile
	{
		private Asset<Texture2D> _defaultNoAlt;

		public SteampunkerProfile()
		{
			if (Main.dedServ) // #if SERVER
				return;

			string npcFileTitleFilePath = "V2/NPCs/Vanilla/TownNPCs/Steampunker/Steampunker_WeightBase_BellyBase";
			_defaultNoAlt = ModContent.Request<Texture2D>(npcFileTitleFilePath, AssetRequestMode.ImmediateLoad);
		}

		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return _defaultNoAlt;

			string exactTextureToUse = "V2/NPCs/Vanilla/TownNPCs/Steampunker/Steampunker";
			string weightString = "_WeightBase";
			exactTextureToUse += weightString;
			int bellySize = npc.AsPred().GetVisualBellySize.Invoke(npc);
			string bellyString = "_Belly" + (bellySize == 0 ? "Base" : bellySize);
			exactTextureToUse += bellyString;

			if (npc.altTexture == 1)
				exactTextureToUse += "_Party";

			return ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad);
		}

		public int GetHeadTextureIndex(NPC npc) => NPCHeadID.Steampunker;
	}

	public class Steampunker : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Steampunker;

		public override void SetDefaults(NPC npc)
		{
			npc.AsV2NPC().Gender = EntityGender.Female;

			npc.AsV2NPC().GetNewDialogue = GetSteampunkerChat;

			npc.AsFood().DefinedBaseSize = 1.06;
			npc.AsPred().MaxStomachCapacity = 50.0;
			npc.AsPred().BaseStomachacheMeterCapacity = 1250.0;

			npc.AsPred().SmallGulps = Gulps.Short;
			npc.AsPred().SmallGulpThreshold = 0.45;
			npc.AsPred().BigGulps = Gulps.Standard;
			npc.AsPred().CanBeForceFed = CanSteampunkerBeForceFed;
			npc.AsPred().OnForceFed = OnSteampunkerForceFed;

			npc.AsPred().DigestionType = EntityDigestionType.Acidic;
			npc.AsPred().GetDigestionTickRate = GetDigestionTickRate;
			npc.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;

			npc.AsPred().OnDigestionKill = null;
			npc.AsPred().MouthSoundRawOffset = npc.TrueCenter() + new Vector2(npc.direction * 8f, -14f);
			npc.AsPred().SmallBurps = Burps.Humanoid.Small;
			npc.AsPred().SmallBurpThreshold = 1.5;
			npc.AsPred().StandardBurps = Burps.Humanoid.Standard;
			npc.AsPred().GetAdditionalDigestedPlayerMessages = GetDigestedPlayerAdditionalDeathMessages;
			npc.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			npc.AsPred().GetVisualBellySize = GetVisualBellySize;
		}

		public override ITownNPCProfile ModifyTownNPCProfile(NPC npc) => SteampunkerStuff.PredSteampunkerProfile;

		// [TAG:DialogueParsing] TO-DO: transfer all dialogue to localization files
		public static List<string> GetSteampunkerChat(NPC npc, Player player)
		{
			List<NPC> nearbyResidentNPCs = npc.GetNearbyResidentNPCs(out int npcsWithinHouse, out int npcsWithinVillage);
			NPC hopelessRomantic = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.ArmsDealer);
			NPC bootlegChippy = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Clothier);
			NPC wireWoman = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Mechanic);
			NPC bestGirl = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Stylist);
			NPC succubus = nearbyResidentNPCs.FirstOrDefault(x => x.type == ModContent.NPCType<Lucinda>());

			List<string> steampunkerChatPool = new List<string>();
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
				if (Main.bloodMoon)
				{
					steampunkerChatPool.AddRange([
						"Bloody hell, QUIT your bellyaching and get to melting! You're already wasting my time in spades!",
						"Will you stop your squirming?! I already told you, you're never getting out! It's not like anyone'll see you kicking around in there...",
						"You know I've eaten blokes a dozen times bigger than you've ever been, right? You're a SNACK, at most. Settle down and melt; I've got work to be doing.",
					]);
				}
				else
				{
					bool noDigest = false;
					steampunkerChatPool.AddRange([
						"Don't worry about getting in the way; you don't even make a little baby bump from in there. That's the power of steam tech!",
						"Oi, give a good rub while you're in there, yeah? A nice little kick and a pat to stimulate the gut!",
					]);
					if (noDigest)
					{
						steampunkerChatPool.AddRange([
							"Do be careful not to kick about too much in there. If that machine keeping my stomach small gives out, I'll...hrm. I'm actually not sure. Probably digest you, though, so don't make a scene of it!",
						]);
					}
					else
					{
						steampunkerChatPool.AddRange([
							"[c/00BB00:*BUOARP!*]\n"
						  + "Oi, " + (player.Male ? "sir" : "ma'am") + ", you're REALLY working up some good steam in there! Keep it up, sure, but keep down the shouting!",
							"You? Getting out of my stomach? Preposterous! You've already been tucked away, my delicious dear, safe from the perception of the world.",
						]);
					}
				}
			}
			else
			{
				if (Main.bloodMoon)
				{
					steampunkerChatPool.AddRange([
						"You're getting in the way of my work. [c/FF0000:Quit it], or I'll just pack you away like the [c/FF0000:quaint little snack] you are.",
						"Keep it down, you bleeding ragamuffin. I don't CARE if you're in my tum or not; you're a [c/FF0000:disturbance] either way.",
						"Will you [c/FF0000:buzz off]? I'm already peevish and peckish enough as it is WITHOUT you snooping around!",
					]);
					if (wireWoman is not null)
					{
						steampunkerChatPool.AddRange([
							"If I have to tell off " + wireWoman.GivenName + " ONE MORE TIME, she'll be converted [c/FF0000:DIRECTLY into steam]! How DARE she try to upstage me...",
							"NO! I [c/FF0000:DO NOT WANT YOUR CRASS \"WIRING\" DOOHICKEYS!]...oh, I thought you were that daft wire girl. I'm only the NORMAL amount of [c/FF0000:peeved] with you.",
							"For the last time, tell " + wireWoman.GivenName + " that I DON'T WANT HER [c/FF0000:SCRAP METAL]! My steam, which I'll HAPPILY [c/FF0000:churn her into more of], is FAR superior!",
						]);
					}
					if (GetVisualBellySize(npc) > 0)
					{
						steampunkerChatPool.AddRange([
							"...bloody hell, you're a brave one. Alright, then: what do you want? Can't you see I'm busy?",
							"You know I'm not full, right? I can fit you in, too, and I will if you keep bothering me like this.",
							"No, I don't need your damnable stomach massages! What I NEED is a bit more to EAT so I can work in peace!",
						]);
					}
					if (Main.IsItRaining)
					{
						steampunkerChatPool.AddRange([
							"WHAT KIND OF BLUNDERSOME [c/FF0000:DUNCE] MAKES IT RAIN ON A NIGHT LIKE THIS!? I have WORK to be doing, and this rain is AWFUL!",
							"Horrible. Absolutely [c/FF0000:bloody] horrible...are YOU the one responsible for this!? Did you make it rain tonight!? You've [c/FF0000:ruined] ALL of the work I could've possibly done tonight!",
							"I am THIS CLOSE to making a machine specifically to reach up and rope down each of those rain clouds individually. I'm STARVED, and they'd make a better MEAL for me than [c/FF0000:anything they could be up there!]",
						]);
					}
					if (Main.IsItStorming)
					{
						steampunkerChatPool.AddRange([
							"Lightning? Electricity!? You [c/FF0000:DARE] to suggest I use it for my work?!? The ONLY use it will EVER HAVE will be as fuel for my engine, [c/FF0000:JUST LIKE THE CLOUDS IT COMES FROM!]",
							"The sounds of this BLOODY THUNDER are throwing off my data! Tonight's already HORRIBLE enough; make it quit being worse!!",
							"The storms above are NOTHING compared to the storms my stomach whips up on nights like these. ROPE DOWN THOSE DAMNABLE CLOUDS, AND I WON'T DARE WAIT TO DEMONSTRATE!",
						]);
					}
				}
				else
				{
					steampunkerChatPool.AddRange([
						"You know, be it what it would, a jetpack would work very nicely for blokes like you. Much easier to travel with a full furnace when you can fly around with one of these, you know.",
						"At one point, I considered becoming an air pirate. I never did, but it was always an interesting prospect. People carry a lot of food around in those convoys, you know...",
						"I like your gear. Does it come in brass? Bronze? Maybe a tasteful stainless steel?",
						"A license? For steam? Oh, you don't need a license to make superior machines! They're there for the betterment of all, oh hoho!",
						"Oi, have you seen some merfolk around? Been meaning to fix myself a good fish and chips to snack on while I work.",
					]);
					if (wireWoman is not null)
					{
						if (wireWoman.IsFoodFor(player))
						{
							if (player.AsPred().SafeStomach)
							{
								steampunkerChatPool.AddRange([
									"Ah, I see you've crammed that daft " + wireWoman.GivenName + " into you! You've certainly shown her what-for, and I'm sure now she'll think THRICE before calling my machines inferior again! HA!\n"
								  + "\n"
								  + "...erm...you ARE going to digest her, right?",
									"You know, I haven't seen that inferior \"mechanic\" in quite a hot moment...could that be her in there, permanently locked away inside your system? Oh ho ho, it IS! A good show, indeed, " + player.name + "!\n"
								  + "\n"
								  + "...well, that is, as long as you remember to melt her down at some point.",
								]);
							}
							else
							{
								steampunkerChatPool.AddRange([
									"Ah, I see you've helped yourself to " + wireWoman.GivenName + "! You've certainly shown her what-for, and I'm sure now she'll think THRICE before calling my machines inferior again! HA!",
									"You know, I haven't seen that inferior \"mechanic\" in quite a hot moment...could that be her in there, melting away into nothing inside your system? Oh ho ho, it IS! A good show, indeed, " + player.name + "!",
								]);
							}
						}
						else if (wireWoman.IsFoodFor(npc))
						{
							steampunkerChatPool.AddRange([
								"Hm? You say " + wireWoman.GivenName + " is missing? Oh, how pleasantly coincidental; I know where she is! That said, she's far too far up her own arse to worry over...I suppose you'll just have to accept my superior machinery instead.",
								"Ah, hello! Sorry if you hear any louder grumbles than normal; I just had a snack most scrummy indeed! She tasted, hmhm...shockingly delicious, if you will. Her hair went down just as well as the wire she uses for her inferior work, too.",
								"Oh? Well, I'm afraid " + wireWoman.GivenName + "'s met with some hard lines indeed! Devoured and stowed away within a greater machine worker...dreadful business, that. I'm sure you could find her, if you looked hard enough inside me...",
							]);
						}
						else
						{
							steampunkerChatPool.AddRange([
								"That overstiffened fool..." + wireWoman.GivenName + " insists those monstrous mechs were purely HER work. I'm not surprised; they're flawed, at best.",
								"The next time you see that bumbling \"mechanic\", tell her I'm NOT interested in her overengineered gizmos! I'm PLENTY fine with me and my superior steam!",
							]);
						}
					}
					if (Main.IsItAHappyWindyDay)
					{
						steampunkerChatPool.AddRange([
							"A harsh breeze? Ha! The only breeze I'm worried about is any that comes back up my throat, and I ALWAYS bring up healthy, hefty helpings of steam! I'll win out fine against a light wind like this!",
							"You know what goes quite lovely with all this delectable wind? A nice cup of tea. A small snack would be nice, too, of course, but tea is always the best.",
							"Well, this is certainly a phenomenon to consider...actually, hold on! This gives me an idea to improve my latest jetpack model!",
						]);
					}
					if (Main.IsItRaining)
					{
						steampunkerChatPool.AddRange([
							"The rain is never a welcome sight unless it can fill up my gut gadgets for a while...it always rusts my equipment! That said, it also makes for some very enticing steam...a double-edged sword, as it were.",
							"Hmph! A downpour at this hour? What a chore! I'll never get a lick of anything done in this weather!",
							"Think about it. A full-size raincloud, consumed and condensed as a meager meal for yours truly, and thusly, converted into its superior form: stomach-produced steam. Doesn't that sound perfect, " + player.name + "?",
						]);
					}
					if (Main.IsItStorming)
					{
						steampunkerChatPool.AddRange([
							"Electricity? " + (wireWoman is not null ? ("" + wireWoman.GivenName + "'s dullard ventures? ") : " ") + "Ha! Nothing beats the power and punctuality of steam!",
							"I'm almost starting to rethink making everything of mine out of metal. This dreadful weather could to strike one of my machines down at any moment!",
							"These storms are no match for my machines, external OR internal! Come at me, o' storm of the skies! I'll wolf you down like a complete colonial breakfast yet!",
						]);
					}
				}
			}
			return steampunkerChatPool;
		}
		public static bool CanSteampunkerBeForceFed(NPC npc) => true;

		public static void OnSteampunkerForceFed(NPC npc, Player player)
		{
			PredNPC.SetChatboxText(
				npc,
				player,
				Main.rand.NextFromCollection([
					"Huh? You want me to eat you? Alright, then, " + (player.Male ? "mister" : "missy") + ", you really want to stow away in my stomach?",
					"Oh, what's this? You're looking to fuel my engine? Well, that's bloody nice of you...here: let me introduce you to her!",
					"An offering for little ol' me? Well, that's awfully sweet of you. I'll just pack you away, and then get back to my theorycrafting.",
				])
			  + "\n[c/7F7F7F:<" + npc.GivenName + " smirks and pulls out a small device. With a single press of the button on top, you find yourself seamlessly transported directly into her stomach, which is MUCH roomier on the inside than your predator's appearance would suggest...at the moment, at least.>]\n"
			  + Main.rand.NextFromCollection([
					"There you go! Now, do keep it down as you digest. While my stomach is very eager to make introductions, I have to make haste with my work.",
					"...hmhm, a pleasant one. A shame I only barely ever taste them when they go in...ah, well. This is a good time to try out my new jetpack model!",
					"Ooh...a bit on the heavier side, aren't you? You'll throw off my density too much like this for me to work...I'll just let my stomach work you down to a more meager volume.",
					"And there you are! You'd best get to introducing yourself...my stomach's a lovely one, and she sounds more than ready to get to know a bloke like you better!",
				])
			);
		}

		public override void ModifyShop(NPCShop shop)
		{
			shop.Add(new NPCShop.Entry(ModContent.ItemType<SteampunkerGutFastTravel>()));
		}

		public static void GetDigestedPlayerAdditionalDeathMessages(NPC npc, Player player, List<string> deathReasonKeyList)
		{
			deathReasonKeyList.AddHumanoidPredMessages();
			deathReasonKeyList.AddRange(new List<string>
			{
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.1",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.2",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.3",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.4",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.5",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.6",
				"Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.7",
			});

			if (player.difficulty == PlayerDifficultyID.Hardcore)
			{
				deathReasonKeyList.Clear();
				deathReasonKeyList.Add("Mods.V2.Death.DigestedPlayer.SpecificNPC.Townsfolk.Steampunker.Hardcore");
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
			NPC wireWoman = nearbyResidentNPCs.FirstOrDefault(x => x.type == NPCID.Mechanic);
			bool proveToWireWomanThatSteamisBetter = false;
			RollForRandomGulp(ref proveToWireWomanThatSteamisBetter);
			RollForRandomGulp(ref proveToWireWomanThatSteamisBetter);
			RollForRandomGulp(ref proveToWireWomanThatSteamisBetter);
			RollForRandomGulp(ref proveToWireWomanThatSteamisBetter);
			RollForRandomGulp(ref proveToWireWomanThatSteamisBetter);
			if (wireWoman != null && wireWoman.Distance(npc.Center) <= npc.AsPred().MaxSwallowRange && proveToWireWomanThatSteamisBetter)
				PredNPC.Swallow(npc, wireWoman);
		}

		public static double GetDigestionTickRate(NPC npc, PreyData prey) => Main.bloodMoon ? 5.0 : 2.5;

		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => 20.0;

		public static double GetPreyAbsorptionRate(NPC npc)
		{
			double baseAbsorptionRate = 1.0 / (double)V2Utils.SensibleTime(
				minutes: 1,
				seconds: 0
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
				(int)npc.Center.X - 14,
				(int)npc.Center.Y - 25,
				28,
				50
			);
		}

		public static int GetVisualBellySize(NPC npc)
		{
			return Math.Min(
				(int)Math.Floor(0.5 * Math.Sqrt(PredNPC.GetCurrentBellyWeight(npc))),
				2
			);
		}
	}
}
