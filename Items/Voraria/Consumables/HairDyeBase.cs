using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace V2.Items.Voraria.Consumables
{
	/// <summary>
	/// Handles automatically registering shader, common SetDefaults<br/>
	/// <br/>
	/// ripped from Clicker Class because I am the big stupid<br/>
	/// </summary>
	public abstract class HairDyeBase : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		/// <summary>
		/// If true, automatically registers the shader using <see cref="LegacyShaderMethod"/>.<br/>
		/// </summary>
		public virtual bool UsesLegacyShader => true;

		/// <summary>
		/// The LEGACY method to be used to determine how this hair dye should color the player's hair.<br/>
		/// </summary>
		/// <param name="player"></param>
		/// <param name="newColor"></param>
		/// <param name="lighting"></param>
		/// <returns></returns>
		public virtual Color LegacyShaderMethod(Player player, Color newColor, ref bool lighting)
		{
			return newColor;
		}

		public override void SetStaticDefaults()
		{
			if (!Main.dedServ && UsesLegacyShader)
			{
				GameShaders.Hair.BindShader(Type, new LegacyHairShaderData().UseLegacyMethod(LegacyShaderMethod));
			}
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;
			Item.UseSound = SoundID.Item3;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.consumable = true;

			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;
		}
	}
}