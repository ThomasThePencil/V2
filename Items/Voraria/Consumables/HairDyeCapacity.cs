using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.PlayerHandling;

namespace V2.Items.Voraria.Consumables
{
	public class HairDyeCapacity : HairDyeBase
	{
		public override bool UsesLegacyShader => true;

		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Consumables.HairDyeCapacity");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Consumables.HairDyeCapacity.Short");
		public override Color LegacyShaderMethod(Player player, Color newColor, ref bool lighting)
		{
			if (player.AsPred().Rose)
				return new Color(122, 0, 0);

			double fullnessRatio = player.AsPred().StomachFullness / player.AsPred().StomachCapacity;
			return Color.Lerp(
				new Color(114, 0, 0),
				new Color(157, 224, 97),
				(float)fullnessRatio
			);
		}

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			DrawAnimationVertical anim = new DrawAnimationVertical(8, 10);
			Main.RegisterItemAnimation(Type, anim);
			ItemID.Sets.AnimatesAsSoul[Type] = true;

			ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
				new Color(157, 224, 97),
				new Color(157, 224, 97),
				new Color(157, 224, 97)
			};
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Consumables.HairDyeCapacity",
				new
				{
					
				}
			);
		}
	}
}