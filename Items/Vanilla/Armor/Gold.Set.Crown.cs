using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Projectiles.Voraria.Armor.Familiars;

namespace V2.Items.Vanilla.Armor
{
	public class GoldSetWithCrown : ArmorSetDefinition
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static double PrinceMidasChance => 0.15;
		public static double PrinceMagicDamageUp => 0.15;
		public static double PrinceDowntime => 900;
		public override (int? head, int? body, int? legs) RequiredEquipment => (
			head: ItemID.GoldCrown,
			body: ItemID.GoldChainmail,
			legs: ItemID.GoldGreaves
		);

		public override string SetBonusDescriptionKey => "Vanilla.Armor.Gold.SetBonus";

		public override object SetBonusDescriptionVariables => new
		{
			GoldPrinceMidasChance = PrinceMidasChance.ToPercentage(),
			GoldPrinceMagDMGUp = PrinceMagicDamageUp.ToPercentage(),
			GoldPrinceRecoverTime = (PrinceDowntime / 60.0).CastToDecimalPlaces(2),
			GoldPrinceSwallowCapacity = GoldSetPrinceStuff.SwallowCapacity,
			GoldPrinceStomachCapacity = GoldSetPrinceStuff.MaxStomachCapacity,
			GoldPrinceDigestDamage = GoldSetPrinceStuff.DigestDamage,
			GoldPrinceDigestRate = GoldSetPrinceStuff.DigestRate,
			GoldPrinceAbsorbRate = GoldSetPrinceStuff.AbsorbRate,
			GoldPrinceDigestingRegen = GoldSetPrinceStuff.DigestingRegen,
			GoldPrinceDigestingDefense = GoldSetPrinceStuff.DigestingDefense,
			GoldPrinceSize = GoldSetPrinceStuff.Size,
			GoldPrinceMaxHealth = GoldSetPrinceStuff.MaxHealth,
		};

		public override void ApplySetBonus(Player player)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldSetPrince>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Accessory(player.armor[0]), player.TrueCenter(), Vector2.Zero, ModContent.ProjectileType<GoldSetPrince>(), 0, 0f);
			}
		}
	}
}
