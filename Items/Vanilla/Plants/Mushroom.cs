using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Vanilla.Plants
{
	public class Mushroom : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int HealAmount => 15;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.Mushroom;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.02;
			item.AsFood().WellFedPower = 0.5;

			item.AsFood().UpdateInStomach += UpdateInStomach;
			item.AsFood().OnBreak += OnBreak;

			item.AsFood().EdibleOnUse = true;
			item.AsFood().AlwaysEatenByUse = true;
		}

		public static void UpdateInStomach(Entity prey, Entity pred, bool dead)
		{
			if (dead)
				pred.AddStatus(BuffID.PotionSickness, V2Utils.SensibleTime(seconds: 30), true);
		}

		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			if (pred is Player playerPred && !playerPred.HasBuff(BuffID.PotionSickness))
				playerPred.Heal(HealAmount);
			return true;
		}
	}
	public class EvilMushroom : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static int HealAmount => 15;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type is ItemID.VileMushroom or ItemID.ViciousMushroom;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 12;
			item.AsFood().Size = 0.02;
			item.AsFood().WellFedPower = -1.5;
		}
	}
}
