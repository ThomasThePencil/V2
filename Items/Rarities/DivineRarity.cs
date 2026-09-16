using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace V2.Items.Rarities
{
	public class DivineRarity : ModRarity
	{
		public override bool IsLoadingEnabled(Mod mod) => true;
		public override Color RarityColor => new Color(255, 204, 0);
		public override int GetPrefixedRarity(int offset, float valueMult) => Type;
	}
}