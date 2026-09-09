using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles;

namespace V2.UI.VoreBestiary
{
	public static class DISwitchHandling
	{
		public static List<DISwitch> Switches => [
			DISwitch.Male,
			DISwitch.Female,
			DISwitch.Enby,
		];
	}
	public abstract class DISwitch
	{
		public static DISwitchMale Male => new DISwitchMale();
		public static DISwitchFemale Female => new DISwitchFemale();
		public static DISwitchEnby Enby => new DISwitchEnby();
		public static DISwitchBat Bat => new DISwitchBat();
		public static DISwitchBird Bird => new DISwitchBird();
		public static DISwitchBug Bug => new DISwitchBug();
		public static DISwitchCanine Canine => new DISwitchCanine();
		public static DISwitchFish Fish => new DISwitchFish();
		public static DISwitchObject Object => new DISwitchObject();
		public static DISwitchPlant Plant => new DISwitchPlant();
		public static DISwitchSlime Slime => new DISwitchSlime();
		public static DISwitchSpider Spider => new DISwitchSpider();
		public static DISwitchTownsfolk Townsfolk => new DISwitchTownsfolk();
		public static DISwitchUndead Undead => new DISwitchUndead();
		public enum SwitchCategory
		{
			Gender,
			Type,
		}

		public abstract SwitchCategory Category { get; }

		/// <summary>
		/// The localization key which this Divine Intergestion toggle uses to fetch its title and commentary.
		/// </summary>
		public abstract string LocalizeKey { get; }
		/// <summary>
		/// Gets the intended title and St. Promethia commentary for this Divine Intergestion toggle.
		/// </summary>
		public void GetTitleAndDescription(out LocalizedText title, out LocalizedText stPrommentary)
		{
			string intendedKey = "Mods.V2.AEM.DivineIntervention." + (Category switch { SwitchCategory.Gender => "GenderRelated", _ => "TypeRelated" }) + "." + LocalizeKey;
			title = Language.GetText(intendedKey + ".Title");
			stPrommentary = Language.GetText(intendedKey + ".Description");
		}
		/// <summary>
		/// The key which this Divine Intergestion toggle uses to know if its pred type should or shouldn't be enabled.
		/// </summary>
		public abstract string ToggleStateKey { get; }
		/// <summary>
		/// Gets the current enabled-or-disabled state of this category of pred as per Divine Intergestion.
		/// </summary>
		/// <returns></returns>
		public bool GetToggleState()
		{
			PreyPlayer localFood = Main.LocalPlayer.AsFood();
			localFood.CanBeEatenBy.TryAdd(ToggleStateKey, true);
			return localFood.CanBeEatenBy[ToggleStateKey];
		}
		/// <summary>
		/// Sets the enabled-or-disabled state of this category of pred as per Divine Intergestion to the provided new value.
		/// </summary>
		/// <param name="newState"></param>
		public void SetToggleState(bool newState)
		{
			PreyPlayer localFood = Main.LocalPlayer.AsFood();
			localFood.CanBeEatenBy.TryAdd(ToggleStateKey, newState);
			localFood.CanBeEatenBy[ToggleStateKey] = newState;
		}
		/// <summary>
		/// Saves the current enabled-or-disabled state of this category of pred as per Divine Intergestion to save data.
		/// </summary>
		/// <returns></returns>
		public void SaveToggleData(ref TagCompound tag) => tag["ediblefor" + ToggleStateKey] = GetToggleState();
		/// <summary>
		/// Loads the saved enabled-or-disabled state of this category of pred as per Divine Intergestion from save data.
		/// </summary>
		/// <param name="newState"></param>
		public void LoadToggleData(TagCompound tag) => SetToggleState(tag.GetBool("ediblefor" + ToggleStateKey));
		public abstract bool ComparisonRule(Player player, Entity entity);
		/// <summary>
		/// Determines, using the toggle state key and provided logic, whether or not the given entity can eat the player as per Divine Intergestion toggles.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool CompareCanBeEatenBy(Player player, Entity entity)
		{
			if (ComparisonRule(player, entity))
				return GetToggleState();

			return true;
		}
	}

	public class DISwitchMale : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Gender;
		public override string LocalizeKey => "Male";
		public override string ToggleStateKey => "blacksapphirecookie";
		public override bool ComparisonRule(Player player, Entity entity)
		{
			if (entity is Player predPlayer)
				return predPlayer.Male;
			else if (entity is NPC predNPC)
				return predNPC.AsV2NPC().Gender == EntityGender.Male;
			else if (entity is Projectile predProjectile)
				return predProjectile.AsV2Proj().Gender == EntityGender.Male;

			return false;
		}
	}

	public class DISwitchFemale : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Gender;
		public override string LocalizeKey => "Female";
		public override string ToggleStateKey => "povidoneiodinecookie";
		public override bool ComparisonRule(Player player, Entity entity)
		{
			if (entity is Player predPlayer)
				return !predPlayer.Male;
			else if (entity is NPC predNPC)
				return predNPC.AsV2NPC().Gender == EntityGender.Female;
			else if (entity is Projectile predProjectile)
				return predProjectile.AsV2Proj().Gender == EntityGender.Female;

			return false;
		}
	}

	public class DISwitchEnby : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Gender;
		public override string LocalizeKey => "NonBinary";
		public override string ToggleStateKey => "rinpenroseifshewasatheysloshthem";
		public override bool ComparisonRule(Player player, Entity entity)
		{
			if (entity is Player)
				return false;
			else if (entity is NPC predNPC)
				return predNPC.AsV2NPC().Gender == EntityGender.Other;
			else if (entity is Projectile predProjectile)
				return predProjectile.AsV2Proj().Gender == EntityGender.Other;

			return false;
		}
	}

	public class DISwitchBat : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Bat";
		public override string ToggleStateKey => "thatoneshinynoivernIcaughtwhilegettingtheshinycharm";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchBird : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Bird";
		public override string ToggleStateKey => "birdup";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchBug : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Bat";
		public override string ToggleStateKey => "10kfireflies";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchCanine : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Canine";
		public override string ToggleStateKey => "dogememes";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchFish : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Fish";
		public override string ToggleStateKey => "fih";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchObject : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Object";
		public override string ToggleStateKey => "theentirecastofii";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchPlant : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Plant";
		public override string ToggleStateKey => "jarona";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchSlime : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Slime";
		public override string ToggleStateKey => "beatrixlebeau";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchSpider : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Spider";
		public override string ToggleStateKey => "gwenstacy";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchTownsfolk : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Townsfolk";
		public override string ToggleStateKey => "4town";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}

	public class DISwitchUndead : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Undead";
		public override string ToggleStateKey => "thezombiesonyourlawn";
		public override bool ComparisonRule(Player player, Entity entity) => false;
	}
}
