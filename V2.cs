using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using V2.Core.StruggleSystem;
using V2.NPCs.Voraria.TownNPCs.Succubus;
using V2.NPCs.Voraria.TownNPCs.Enigma;
using V2.Items.Voraria.Consumables.PermanentUpgrades;
using System;
using V2.PlayerHandling;
using V2.Compat;

namespace V2
{
	public partial class V2 : Mod
	{
		internal static V2 Instance;

		public static ModKeybind CheckAEMHotkey { get; set; }
		public static ModKeybind SwallowHotkey { get; set; }
		public static ModKeybind RegurgitateHotkey { get; set; }
		public static ModKeybind FeedHotkey { get; set; }
		public static ModKeybind ItemGulpHotkey { get; set; }
		public static ModKeybind StruggleUpHotkey { get; set; }
		public static ModKeybind StruggleLeftHotkey { get; set; }
		public static ModKeybind StruggleRightHotkey { get; set; }
		public static ModKeybind StruggleDownHotkey { get; set; }
		public static ModKeybind StruggleSpecialHotkey { get; set; }

		public static ModKeybind RespawnAfterDigestionHotkey { get; set; }

		public static ModKeybind OllieDashHotkey { get; set; }
		public static ModKeybind MintWispHotkey { get; set; }

		/// <summary>
		/// A special flag which decides whether or not the vore blacklists are actually filled.<br/>
		/// Defaults to <see langword="true"/>. If set to <see langword="false"/> instead, the blacklists remain empty.<br/>
		/// This allows several entities which otherwise would not be included in vore mechanics, namely as predators, to instead be given full reign.<br/>
		/// Should not be manually set to <see langword="false""/> outside of specific testing cases.
		/// </summary>
		public static bool BlacklistsActive => !GetFooled;
		public static List<int> VoreNPCBlacklist { get; set; }
		public static List<int> VoreProjectileBlacklist { get; set; }

		/// <summary>
		/// A special flag which causes everything not immediately relevant to the mod's core vore functionality to fail to load; this builds a "Fundamentals" version of the mod.<br/>
		/// All new content, most non-NPC reworks, and many player pred goals associated with new content will not load while this flag is set to <see langword="true"/>.<br/>
		/// <b>Controlled by the "Design-Mode [Fundamentals]" configuration option.</b><br/>
		/// </summary>
		public static bool BasicMode => ModContent.GetInstance<V2ServerConfig>().BasicMode || GetFooled;

		/// <summary>
		/// A special flag which decides whether or not the April Fool's branch is active.<br/>
		/// Defaults to <see langword="false"/>. If it is April Fool's Day, thus making this return <see langword="true"/>, the following things become true:<br/>
		/// - <see cref="BlacklistsActive"/> is overridden to <see langword="false"/>. Nothing shall escape the fury of the Great Fool of April.<br/>
		/// - <see cref="BasicMode"/> is overridden to <see langword="true"/>. Who needs new content when you've got all the old, shitty content you could ever want?<br/>
		/// - All normal <see cref="GlobalNPC"/>s used for specific NPCs are inactive; instead, the universal AprilFoolsPredNPC is used.<br/>
		/// - All of the nice, well-made belly sprites are replaced with a unified circle tool that's only from Paint.NET because I can't be bothered to remember if I still have MS Paint on here.<br/>
		/// - Only one tum gurgly sound is used. This sound is never used in the normal game.<br/>
		/// - Only one burp sound is used. This sound is also never used in the normal game.<br/>
		/// - Both of the above are intentionally made to sound incredibly bad.<br/>
		/// - VSC's dialogue changes are completely undone.<br/>
		/// - NPCs slowly increase in size as they digest more food. All digestion stats are based on their scale.<br/>
		/// More may be added if time permits.<br/>
		/// <br/>
		/// overall, this was made by yours truly stepping backwards about 13-14 years in time mentally and channeling that energy into assessment of mod quality<br/>
		/// now get the fuck out of my house<br/>
		/// </summary>
		public static bool GetFooled => DateTime.Today.Month == 4 && DateTime.Today.Day == 1;

		public static List<ResourcePack> EnabledResourcePacks => [.. Main.AssetSourceController.ActiveResourcePackList.EnabledPacks];

		public static Dictionary<int, GlobalBuff> ModifiedStatusEffects { get; set; }

		public V2()
		{
			Instance = this;
			ModifiedStatusEffects = [];
		}

		public override void Load()
		{
			CheckAEMHotkey = KeybindLoader.RegisterKeybind(this, "CheckAEM", "Q");

			SwallowHotkey = KeybindLoader.RegisterKeybind(this, "Swallow", "V");
			RegurgitateHotkey = KeybindLoader.RegisterKeybind(this, "Regurgitate", "X");
			FeedHotkey = KeybindLoader.RegisterKeybind(this, "Feed", "G");
			ItemGulpHotkey = KeybindLoader.RegisterKeybind(this, "EatItems", "RightShift");

			StruggleUpHotkey = KeybindLoader.RegisterKeybind(this, "StruggleUp", "Up");
			StruggleLeftHotkey = KeybindLoader.RegisterKeybind(this, "StruggleLeft", "Left");
			StruggleRightHotkey = KeybindLoader.RegisterKeybind(this, "StruggleRight", "Right");
			StruggleDownHotkey = KeybindLoader.RegisterKeybind(this, "StruggleDown", "Down");
			StruggleSpecialHotkey = KeybindLoader.RegisterKeybind(this, "StruggleSpecial", "Space");
			RespawnAfterDigestionHotkey = KeybindLoader.RegisterKeybind(this, "RespawnAfterDigestion", "LeftShift");

			OllieDashHotkey = KeybindLoader.RegisterKeybind(this, "OllieDash", "Q");
			MintWispHotkey = KeybindLoader.RegisterKeybind(this, "MintSummonWisp", "Q");

			BetterDialogue.BetterDialogue.SupportedNPCs.Add(ModContent.NPCType<Lucinda>());
			BetterDialogue.BetterDialogue.SupportedNPCs.Add(ModContent.NPCType<LucindaBound>());

			BetterDialogue.BetterDialogue.SupportedNPCs.Add(ModContent.NPCType<Clover>());
			BetterDialogue.BetterDialogue.SupportedNPCs.Add(ModContent.NPCType<CloverBound>());

			BetterDialogue.BetterDialogue.RegisterShoppableNPC(NPCID.Nurse);
			BetterDialogue.BetterDialogue.RegisterShoppableNPC(ModContent.NPCType<Lucinda>());
			BetterDialogue.BetterDialogue.RegisterShoppableNPC(ModContent.NPCType<Clover>());

			StruggleChartLoader.Load();

			EngageVoraciousGameFuckery();
		}

		public override void PostSetupContent()
		{
			VoreNPCBlacklist = [
				NPCID.Angler,
				NPCID.SleepingAngler,
				NPCID.Princess,
			];

			VoreProjectileBlacklist = [
			];

			if (!BlacklistsActive)
			{
				VoreNPCBlacklist.Clear();
				VoreProjectileBlacklist.Clear();
			}

			// Munchies handling
			if(ModLoader.TryGetMod("munchies", out Mod munchies))
			{
				V2MunchiesCompat MunchiesCompat = new V2MunchiesCompat(munchies);
				MunchiesCompat.ApplyCompatibility();

			}

			if(ModLoader.TryGetMod("WeaponDisplay", out Mod armamentdisplay))
			{
				V2WeaponDisplay ArmamentDisplayCompat = new V2WeaponDisplay(armamentdisplay);
				ArmamentDisplayCompat.ApplyCompatibility();
			}
		}

		public override void Unload()
		{
			VoreNPCBlacklist = null;
			VoreProjectileBlacklist = null;

			StruggleChartLoader.Unload();

			DisengageVoraciousGameFuckery();

			for (int i = 0; i < NPCID.Count; i++)
			{
				TextureAssets.Npc[i] = ModContent.Request<Texture2D>("Terraria/Images/NPC_" + i);
			}
			for (int i = 0; i < ProjectileID.Count; i++)
			{
				TextureAssets.Projectile[i] = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + i);
			}
		}
	}
}