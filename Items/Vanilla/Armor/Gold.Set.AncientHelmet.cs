using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Projectiles.Voraria.Armor.Familiars;

namespace V2.Items.Vanilla.Armor
{
	public class GoldSetWithOldHat : ArmorSetDefinition
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static double PrinceMidasChance => 0.15;
		public static double PrinceMagicDamageUp => 0.15;
		public static double PrinceDowntime => 900;
		public override (int? head, int? body, int? legs) RequiredEquipment => (
			head: ItemID.GoldHelmet,
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
			player.AddHealthRegenEffect(
				healthPerSecond: 0.0,
				modifyTotalHealthRegenMethod: ModifyTotalHealthRegen
			);
		}

		public static void ModifyTotalHealthRegen(Player player, ref double naturalRegenAdditive, ref double naturalRegenMultiplicative, ref double artificialRegenAdditive, ref double artificialRegenMultiplicative)
		{
			naturalRegenAdditive += 0.05f;	
			if (player.position.Y < Main.worldSurface && player.behindBackWall && Main.dayTime)
				naturalRegenAdditive += 0.05f;
		}
	}
}
