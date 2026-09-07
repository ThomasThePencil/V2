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
	public class BiomeSkyThingy : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Thingymajigs.FullNameForExternalUse.Sky");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Accessories.Thingymajigs.Sky.Short");

		public static float StatBuff = -0.15f;
		public static float PermBuff = -0.08f;
		public static float WingBuff = 0.2f;

		private static Asset<Texture2D> MainTexture;
		private static Asset<Texture2D> OutlineTexture;

		public override void Load()
		{
			MainTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Accessories/Thingymajigs/BiomeSkyThingy");
			OutlineTexture = ModContent.Request<Texture2D>("V2/Items/Voraria/Accessories/Thingymajigs/BiomeSkyThingyOutline");
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
				if (!playerPred.AsPred().PermanentUpgradesGained.ContainsKey("Thingy_BiomeSky"))
					playerPred.AsPred().PermanentUpgradesGained.Add("Thingy_BiomeSky", false);

				if (!playerPred.AsPred().PermanentUpgradesGained["Thingy_BiomeSky"])
					playerPred.AsPred().PermanentUpgradesGained["Thingy_BiomeSky"] = true;
			}
			return true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = (int)Math.Round(player.wingTimeMax * (1f + WingBuff));
			player.AsPred().StomachWeightModifier += StatBuff;
			player.AsPred().BodyWeightModifier += StatBuff;
		}

		public void DrawItem(SpriteBatch spriteBatch, Vector2 position, float scale, bool inInventory = false)
		{
			Main.spriteBatch.End();
			if (inInventory)
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
			else
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			Vector2 Center = new Vector2(MainTexture.Value.Bounds.Width / 2,
			MainTexture.Value.Bounds.Height / 2);
			float angle = (Main.GlobalTimeWrappedHourly * 2) % 360;
			float ColorSequence = (Main.GlobalTimeWrappedHourly % 8) / 4f;
			if (ColorSequence > 1)
				ColorSequence = 2 - ColorSequence;

			Vector2 Offset = new Vector2((float)(1 * Math.Cos(angle) - 1 * Math.Sin(angle)), (float)(1 * Math.Cos(angle) + 1 * Math.Sin(angle)));

			Color colorA = new Color(255,200,0);
			Color colorB = new Color(125,0,100);

			Color actualColor = new Color(
				(int)Math.Round(colorA.R * ColorSequence + colorB.R * (1 - ColorSequence))
				, (int)Math.Round(colorA.G * ColorSequence + colorB.G * (1 - ColorSequence))
				, (int)Math.Round(colorA.B * ColorSequence + colorB.B * (1 - ColorSequence)));

			Color BGColor = new Color(actualColor.R, actualColor.G, actualColor.B, 100);

			if (inInventory)
			{
				Offset /= 1.5f;
				BGColor = new Color(BGColor.R, BGColor.G, BGColor.B, 50);
			}

			spriteBatch.Draw(OutlineTexture.Value
				, position + Offset * 3, OutlineTexture.Value.Bounds, BGColor, 0f, Center, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(OutlineTexture.Value
				, position + Offset * -3, OutlineTexture.Value.Bounds, BGColor, 0f, Center, scale, SpriteEffects.None, 0f);

			spriteBatch.Draw(OutlineTexture.Value
				, position, OutlineTexture.Value.Bounds, actualColor, 0f, Center, scale, SpriteEffects.None, 0f);

			spriteBatch.Draw(MainTexture.Value
				, position, MainTexture.Value.Bounds, Color.White, 0f, Center, scale, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			if (inInventory)
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
			else
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
		}
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			DrawItem(spriteBatch, position, scale, true);
			return false;
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (Item.CurrentCaptor() is not null)
				return false;
			DrawItem(spriteBatch, Item.Center - Main.screenPosition, scale);
			return false;
		}
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (Main.HoverItem.type == Item.type || Main.guideItem == Item || new Rectangle((int)position.X - frame.Width / 2 - 8, (int)position.Y - frame.Height / 2 - 8, frame.Width + 16, frame.Height + 16).Contains(Main.MouseScreen.ToPoint()) || Item == Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem])
				return;
			Item.SetNameOverride(Language.GetText("Mods.V2.ItemName.Voraria.Accessories.Thingymajigs.Name.Sky")
				+ " " + Language.GetText("Mods.V2.ObjectNames." + Main.rand.Next(1, 36).ToString()).Value);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			tooltips.AddVorariaItemTooltip(
				"Voraria.Accessories.Thingymajigs.Sky",
				new
				{
					Stat = Math.Round(StatBuff * -100),
					Perm = Math.Round(PermBuff * -100),
					Wing = Math.Round(WingBuff * 100)
				}
			);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<BlankThingy>()
				.AddIngredient(ItemID.MeteoriteBar, 10)
				.AddIngredient(ItemID.SoulofFlight, 20)
				.AddIngredient(ItemID.Cloud, 15)
				.AddIngredient(ItemID.RainCloud, 15)
				.AddIngredient(ItemID.SnowCloudBlock, 15)
				.Register();
		}
	}
}