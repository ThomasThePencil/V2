using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Accessories.Informational;
using V2.PlayerHandling;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Voraria.Accessories.Thingymajigs
{
	public class BiomeCrimsonThingy : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Thingymajigs.FullNameForExternalUse.Crimson");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.Thingymajigs.Crimson.Short");

		public static float DmgBuff = 0.06f;
		public static float DigestBuff = 0.25f;
		public static float AcheDebuff = -0.12f;
		public static float PermDigestBuff = 0.08f;

		private static Asset<Texture2D> MainTexture;
		private static Asset<Texture2D> OutlineTexture;

		public override void Load()
		{
			MainTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Accessories/Thingymajigs/BiomeCrimsonThingy");
			OutlineTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Accessories/Thingymajigs/BiomeCrimsonThingyOutline");
		}

		public override void Unload()
		{
			MainTexture = null;
			OutlineTexture = null;
		}

		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			Item.ResearchUnlockCount = 1;
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, new Vector3(255,255,255) * 0.005f);
		}
		public override void SetDefaults()
		{
			Item.accessory = true;

			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 1;

			Item.value = Item.sellPrice(0, 3);
			Item.rare = ItemRarityID.Lime;

			Item.AsFood().Size = 0.5;
			Item.AsFood().MaxHealth = 1500;

			Item.AsFood().EdibleOnUse = true;

			Item.AsFood().OnBreak += OnBreak;
		}
		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);
			if (pred is Player playerPred)
			{
				if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("Thingy_BiomeCrimson"))
					playerPred.AsPred().PermanentUpgradesGained.Add("Thingy_BiomeCrimson", false);

				if (!playerPred.AsPred().PermanentUpgradesGained["Thingy_BiomeCrimson"])
					playerPred.AsPred().PermanentUpgradesGained["Thingy_BiomeCrimson"] = true;
			}
			return true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) += DmgBuff;
			player.AsPred().DigestionTickDamageModifier += DigestBuff;
			player.AsPred().StomachacheMeterCapacityModifier += AcheDebuff;
		}
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			BlankThingy.DrawThingymajig(spriteBatch, position, scale, 0f, MainTexture, OutlineTexture, new Color(200, 0, 0), new Color(200, 135, 0), true);
			return false;
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (Item.CurrentCaptor() is not null)
				return false;
			BlankThingy.DrawThingymajig(spriteBatch, Item.Center - Main.screenPosition, scale, rotation, MainTexture, OutlineTexture, new Color(200, 0, 0), new Color(200, 135, 0));
			return false;
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (Main.HoverItem.type == Item.type || Main.guideItem == Item || new Rectangle((int)position.X - frame.Width / 2 - 8, (int)position.Y - frame.Height / 2 - 8, frame.Width + 16, frame.Height + 16).Contains(Main.MouseScreen.ToPoint()) || Item == Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem])
				return;
			Item.SetNameOverride(Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Thingymajigs.Name.Crimson")
				+ " " + Language.GetText("Mods.V2.ObjectNames." + Main.rand.Next(1, 36).ToString()).Value);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Voraria.Accessories.Thingymajigs.Crimson",
				new
				{
					Dmg = Math.Round(DmgBuff * 100),
					Dig = Math.Round(DigestBuff * 100),
					Ache = Math.Round(AcheDebuff * -100),
					PermDig = Math.Round(PermDigestBuff * 100)
				}
			);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<BlankThingy>()
				.AddIngredient(ItemID.SoulofNight, 10)
				.AddIngredient(ItemID.Ichor, 15)
				.AddIngredient(ItemID.CrimstoneBlock, 33)
				.Register();
		}
	}
}