using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Consumables
{
	public class DigestionRepairScroll : ModItem
	{
		public static double CooldownMax => V2Utils.SensibleTime(minutes: 24);
		public double Cooldown { get; set; }

		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.DigestionRepairScroll");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.DigestionRepairScroll.Short");
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.consumable = false;
			Item.maxStack = 1;

			Item.AsFood().Size = 0.225;
			Item.AsFood().MaxHealth = 100;
			Item.AsFood().AcidResistTier = 99;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(
				gold: 10
			);
		}

		public override bool CanUseItem(Player player) => Cooldown <= 0.0;

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			Cooldown -= 1.0;
			if (Cooldown <= 0.0)
				Cooldown = 0.0;
		}

		public override void UpdateInventory(Player player)
		{
			Cooldown -= 1.0;
			if (Cooldown <= 0.0)
				Cooldown = 0.0;
		}

		public override void UseAnimation(Player player)
		{
			Cooldown = CooldownMax;
			for (int i = 0; i < 58; i++)
			{
				Item repairableItem = player.inventory[i];
				if (repairableItem.AsFood().MaxHealth == -1)
					continue;

				repairableItem.AsFood().Health = repairableItem.AsFood().MaxHealth;
			}
			player.AsFood().SoftenedDigestionDamageTaken = 0;
			SoundEngine.PlaySound(SoundID.Item35 with { Pitch = -0.5f, PitchVariance = 0.0f }, player.TrueCenter());
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			int minutes = (int)Math.Floor(Cooldown / 60.0);
			int seconds = (int)Math.Ceiling(Cooldown % 60.0);
			string remainingCooldownText = "[c/00FF00:Ready to use]";
			if (minutes > 0)
			{
				if (minutes >= 12)
					remainingCooldownText = "[c/FF0000:On cooldown for " + minutes + "m" + (seconds > 0 ? (seconds + "s") : "") + "]";
				else
					remainingCooldownText = "[c/FF7F00:On cooldown for " + minutes + "m" + (seconds > 0 ? (seconds + "s") : "") + "]";
			}
			else if (seconds > 0)
			{
				remainingCooldownText = "[c/FFFF00:On cooldown for " + seconds + "s]";
			}
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.DigestionRepairScroll",
				new
				{
					RemainingCooldown = remainingCooldownText,
				}
			);
		}
	}
}
