using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;
using V2.StatusEffects.Vanilla.Buffs;

namespace V2.Items.Vanilla.Consumables.Potions;

public abstract class PotionTemplate : GlobalItem
{
	public override bool InstancePerEntity => true;

	public abstract string TooltipTranslationKey { get; }
	
	/// <summary>
	/// Apply this effect when digested. Use <see cref="BuffID"/>
	/// </summary>
	public abstract int DigestedPotionEffectID { get; }
	/// <summary>
	/// Apply this effect for <see cref="DigestedPotionEffectDuration"/> duration. Use <see cref="V2Utils.SensibleTime"/>
	/// </summary>
	public abstract int DigestedPotionEffectDuration { get; }
	/// <summary>
	/// Apply to this potion-item. Use <see cref="ItemID"/>
	/// </summary>
	public abstract int AppliesToPotionItem { get; }

	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return entity.type == this.AppliesToPotionItem;
	}

	/// <summary>
	/// Durability of item, defaults to 400. Override to change
	/// </summary>
	public virtual int PotionDurability => 250;
	
	/// <summary>
	/// Size of item (consumption-wise). Defaults to 0.2. Override to change
	/// </summary>
	public virtual double PotionSize => 0.15;

	public override void SetDefaults(Item item)
	{
		item.AsFood().MaxHealth = this.PotionDurability;
		item.AsFood().Size = this.PotionSize;

		item.AsFood().UpdateInStomach += UpdateInStomach;
		item.AsFood().OnBreak += OnBreak;

		item.AsFood().EdibleOnUse = true;
		item.AsFood().AlwaysEatenByUse = true;
	}

	private bool OnBreak(Item item, Entity pred, bool direct)
	{
		SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
		SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
		return true;
	}

	private void UpdateInStomach(Entity prey, Entity pred, bool dead)
	{
		if (dead)
			pred.AddStatus(this.DigestedPotionEffectID, this.DigestedPotionEffectDuration, true);
	}

	public abstract dynamic TooltipVariables();
	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		tooltips.AddVorariaItemTooltip(this.TooltipTranslationKey, (object)this.TooltipVariables());
		tooltips.FirstOrDefault(x => x.Name == "BuffTime").Hide();
	}
}