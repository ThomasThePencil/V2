using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using V2.PlayerHandling;

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
	}

	public class DISwitchMale : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Gender;
		public override string LocalizeKey => "Male";
		public override string ToggleStateKey => "blacksapphirecookie";
	}

	public class DISwitchFemale : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Gender;
		public override string LocalizeKey => "Female";
		public override string ToggleStateKey => "povidoneiodinecookie";
	}

	public class DISwitchEnby : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Gender;
		public override string LocalizeKey => "NonBinary";
		public override string ToggleStateKey => "rinpenroseifshewasatheysloshthem";
	}

	public class DISwitchBat : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Bat";
		public override string ToggleStateKey => "batswithbatsthatbatbats";
	}

	public class DISwitchBird : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Bird";
		public override string ToggleStateKey => "birdup";
	}

	public class DISwitchBug : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Bat";
		public override string ToggleStateKey => "nongamebreakingbugs";
	}

	public class DISwitchCanine : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Canine";
		public override string ToggleStateKey => "dogememes";
	}

	public class DISwitchFish : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Fish";
		public override string ToggleStateKey => "fih";
	}

	public class DISwitchObject : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Object";
		public override string ToggleStateKey => "theentirecastofii";
	}

	public class DISwitchPlant : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Plant";
		public override string ToggleStateKey => "jarona";
	}

	public class DISwitchSlime : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Slime";
		public override string ToggleStateKey => "beatrixlebeau";
	}

	public class DISwitchSpider : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Spider";
		public override string ToggleStateKey => "gwenstacy";
	}

	public class DISwitchTownsfolk : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Townsfolk";
		public override string ToggleStateKey => "taunieandurbain";
	}

	public class DISwitchUndead : DISwitch
	{
		public override SwitchCategory Category => SwitchCategory.Type;
		public override string LocalizeKey => "Undead";
		public override string ToggleStateKey => "thezombiesonyourlawn";
	}
}
