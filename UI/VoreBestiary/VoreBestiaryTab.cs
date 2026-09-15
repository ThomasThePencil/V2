using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles;

namespace V2.UI.VoreBestiary
{
	public static class VoreBestiaryTabHandling
	{
		public static List<DISwitch> Switches => [
			DISwitch.Male,
			DISwitch.Female,
			DISwitch.Enby,
		];
	}
	public abstract class VoreBestiaryTab
	{
		public static DivineIntervention DivineIntervention { get; private set; } = new DivineIntervention();

		/// <summary>
		/// 
		/// </summary>
		public abstract int Priority { get; }

		/// <summary>
		/// The localization key which this Divine Intergestion toggle uses to fetch its title and commentary.
		/// </summary>
		public abstract Texture2D Texture { get; }

		/// <summary>
		/// The localization key which this Divine Intergestion toggle uses to fetch its title and description.
		/// </summary>
		public abstract string LocalizeKey { get; }

		public abstract AEMTab Signifier { get; }
		public bool IsActiveTab => VoreBestiaryUI.SelectedTab == Signifier;
	}

	public class DivineIntervention : VoreBestiaryTab
	{
		public static readonly Asset<Texture2D> InactiveTex = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_GenericTabInactive");
		public static readonly Asset<Texture2D> ActiveTex = ModContent.Request<Texture2D>("V2/UI/VoreBestiary/VoreBestiary_GenericTabActive");
		public override int Priority => 5;
		public override Texture2D Texture => (IsActiveTab ? ActiveTex : InactiveTex).Value;
		public override string LocalizeKey => "DivineIntervention";
		public override AEMTab Signifier => AEMTab.DivineIntervention;
	}
}
