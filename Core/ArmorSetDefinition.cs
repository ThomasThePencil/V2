using Terraria;
using Terraria.ModLoader;

namespace V2.Core
{
	public abstract class ArmorSetDefinition : ModType
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;

		protected sealed override void Register()
		{
			ModTypeLookup<ArmorSetDefinition>.Register(this);

			ArmorSetHandler.RegisterArmorSet(this);
		}
		/// <summary>
		/// The equipment needed for the set bonus to take effect.
		/// </summary>
		public abstract (int? head, int? body, int? legs) RequiredEquipment { get; }

		/// <summary>
		/// The text displayed in tooltips to describe what this set bonus does.
		/// </summary>
		public abstract string SetBonusDescriptionKey { get; }

		/// <summary>
		/// The additional variables to be used for short- and long-form set bonus tooltips.
		/// </summary>
		public abstract object SetBonusDescriptionVariables { get; }

		/// <summary>
		/// The method you'll be using to actually apply set bonus effects.
		/// </summary>
		/// <param name="player">The player to apply the set bonus to.</param>
		public abstract void ApplySetBonus(Player player);

		public bool Active(Player player)
		{
			int? head = RequiredEquipment.head;
			int? body = RequiredEquipment.body;
			int? legs = RequiredEquipment.legs;
			if (head.HasValue)
			{
				if (player.armor[0].type != head.Value)
					return false;
			}
			if (body.HasValue)
			{
				if (player.armor[1].type != body.Value)
					return false;
			}
			if (legs.HasValue)
			{
				if (player.armor[2].type != legs.Value)
					return false;
			}
			return true;
		}
	}
}
