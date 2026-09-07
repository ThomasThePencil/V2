using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Projectiles.Vanilla.Summons.Pets;

namespace V2.Items.Vanilla.Accessories.Pets.Light
{
	public class CandyFairyMasterPetItem : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.FairyQueenPetItem;

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Vanilla.Accessories.Pets.Light.CandyFairyMasterPetItem",
				new
				{
					MiniCandyFairyMaxHealth = FairyPrincessStuff.MaxHealth,
					MiniCandyFairyStomachCapacity = FairyPrincessStuff.MaxStomachCapacity,
					MiniCandyFairyDigestDamage = FairyPrincessStuff.DigestDamage,
					MiniCandyFairyDigestRate = FairyPrincessStuff.DigestRate,
					MiniCandyFairyAbsorbRate = (FairyPrincessStuff.AbsorbRate * 60 * 60).CastToDecimalPlaces(2),
				}
			);
		}
	}
}
