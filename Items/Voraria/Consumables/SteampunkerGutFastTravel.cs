using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Voraria.Consumables
{
	public class SteampunkerGutFastTravel : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.SteampunkerGutFastTravel");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.SteampunkerGutFastTravel.Short");
		public override string Texture => "V2/Items/UnspritedItem";
		public override void SetStaticDefaults()
		{
			DrawAnimationVertical anim = new DrawAnimationVertical(6, 12);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.AsFood().Size = 0.15;
			Item.AsFood().MaxHealth = 4000;
			Item.AsFood().AcidResistTier = 2;

			Item.useAnimation = 6;
			Item.useTime = 6;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noMelee = true;

			Item.AsFood().CanUseInStomach = CanUseInStomach;
			Item.AsFood().UseInStomach = UseInStomach;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(
				gold: 12,
				silver: 50
			);
		}

		public override bool CanUseItem(Player player) => NPC.AnyNPCs(NPCID.Steampunker);

		public override void UseAnimation(Player player)
		{
			NPC steamLass = Main.npc.FirstOrDefault(x => x.active && x.life > 0 && x.type == NPCID.Steampunker && PredNPC.CanSwallow(x, player));
			if (steamLass is not null)
				SendDirectlyToSteamGalGut(steamLass, player);
		}

		public static bool CanUseInStomach(Item item, Player player, Entity pred) => true;
		public static void UseInStomach(Item item, Player player, Entity pred)
		{
			NPC steamLass = Main.npc.FirstOrDefault(x => x.active && x.life > 0 && x.type == NPCID.Steampunker && PredNPC.CanSwallow(x, player, true));
			if (steamLass is not null)
			{
				player.CurrentCaptor().Prey.RemoveAll(x => !x.NoHealth && x.Instance is Player preyPlayer && preyPlayer.whoAmI == player.whoAmI);
				SendDirectlyToSteamGalGut(steamLass, player);
			}
		}

		public static void SendDirectlyToSteamGalGut(NPC steamLass, Player food)
		{
			food.position = steamLass.position;
			PredNPC.Swallow(steamLass, food, playSound: false);
			SoundEngine.PlaySound(MuffledItemSounds.MagicMirrorTeleport, food.position);
			SoundEngine.PlaySound(Burps.Humanoid.Small, food.position);
			SoundEngine.PlaySound(StomachNoises.Muffled with { Pitch = 0.3f }, food.position);
			SoundEngine.PlaySound(StomachNoises.Muffled, food.position);
			SoundEngine.PlaySound(StomachNoises.Muffled with { Pitch = -0.3f }, food.position);
			PredNPC.SetChatboxText(
				steamLass,
				food,
				"[c/7F7F7F:<As you press the Translator's button, you find yourself packed away in a gut. Someone didn't read the fine print, it would seem...or maybe you did, and wanted this outcome. Either way, " + steamLass.GivenName + " finds a light belch forced out of her as you settle inside her tummy.>]\nAww, needed a quick reprieve from all those gluttonous gannets of the world? Well, you'll be quite comfortable in there! Just don't be surprised if I start melting you down instead, oho! A hard-working inventor such as myself needs herself nutrients aplenty, you know!"
			);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.SteampunkerGutFastTravel",
				new
				{
					
				}
			);
		}
	}
}
