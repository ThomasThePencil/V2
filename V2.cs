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

		/// <summary>
		/// A special flag which decides whether or not the vore blacklists are actually filled.<br/>
		/// Defaults to <see langword="true"/>. If set to <see langword="false"/> instead, the blacklists remain empty.<br/>
		/// This allows several entities which otherwise would not be included in vore mechanics, namely as predators, to instead be given full reign.<br/>
		/// </summary>
		public static bool BlacklistsActive { get; set; }
		public static List<int> VoreNPCBlacklist { get; set; }
		public static List<int> VoreProjectileBlacklist { get; set; }

		/// <summary>
		/// A special flag which decides whether or not the April Fool's branch is active.<br/>
		/// Defaults to <see langword="false"/>. If set to <see langword="true"/> instead, the following things become true:<br/>
		/// - <see cref="BlacklistsActive"/> is overridden to <see langword="false"/>. Nothing shall escape the fury of the Fool of April.<br/>
		/// - All normal <see cref="GlobalNPC"/>s used for specific NPCs are inactive; instead, the universal AprilFoolsPredNPC is used.<br/>
		/// - All of the nice, well-made belly sprites are replaced with a unified circle tool from Paint.NET because I can't be bothered to rember if I still have MS Paint on here.<br/>
		/// - Only one tum gurgly sound is used. This sound is never used in the normal game.<br/>
		/// - Only one burp sound is used. This sound is also never used in the normal game.<br/>
		/// - Both of the above are intentionally made to sound incredibly bad.<br/>
		/// - VSC's dialogue changes are completely undone.<br/>
		/// - NPCs slowly increase in size as they digest more food. All digestion stats are based on their scale.<br/>
		/// - None of VSC's unique content is loaded.<br/>
		/// More may be added if time permits.<br/>
		/// <br/>
		/// overall, this was made by yours truly stepping backwards about a decade in time mentally and channeling that energy into assessment of mod quality<br/>
		/// now get the fuck out of my house<br/>
		/// </summary>
		public static bool GetFooled { get; set; }

		public static List<ResourcePack> EnabledResourcePacks => Main.AssetSourceController.ActiveResourcePackList.EnabledPacks.ToList();

		public static Dictionary<int, GlobalBuff> ModifiedStatusEffects { get; set; }

		public V2()
		{
			Instance = this;
			BlacklistsActive = true;
			GetFooled = false;
			ModifiedStatusEffects = [];
		}

		public override void Load()
		{
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
			VoreNPCBlacklist = new List<int>
			{
				NPCID.Angler,
				NPCID.SleepingAngler,
				NPCID.Princess,
			};
			if (ModContent.TryFind("Fargowiltas", "Deviantt", out ModNPC Deviantt))
				VoreNPCBlacklist.Add(Deviantt.Type);

			VoreProjectileBlacklist = new List<int>
			{

			};

			if (!BlacklistsActive || GetFooled)
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