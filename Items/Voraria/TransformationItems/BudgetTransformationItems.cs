using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;
using V2.Core;
using V2.Items.Vanilla.Accessories;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.StatusEffects.Voraria.Buffs;

namespace V2.Items.Voraria.TransformationItems
{
	public class BudgetTransformationItems
	{
		public class KroniiTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.KroniiTransformationItem.ActiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.KroniiTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDie";
			public override void SetStaticDefaults()
			{
				DrawAnimationVertical anim = new DrawAnimationVertical(8, 4);
				Main.RegisterItemAnimation(Type, anim);
				ItemID.Sets.AnimatesAsSoul[Type] = true;
			}

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override void PostUpdate()
			{
				Lighting.AddLight(Item.Center, new Vector3(95, 255, 255) * 0.005f);
			}
			public override void UpdateInventory(Player player)
			{
				if (!player.AsV2Player().HasTransformation)
				{
					player.AsV2Player().HasTransformation = true;
					player.AsV2Player().KroniiTransformation = true;
					player.AddBuff(ModContent.BuffType<BaelzTransformation>(), V2Utils.SensibleTime(frames: 4));
				}
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<InactiveKroniiTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.KroniiTransformationItem",
					new
					{
					}
				);
			}
		}
		public class InactiveKroniiTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.KroniiTransformationItem.InactiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.KroniiTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDieInactive";

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<KroniiTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.KroniiTransformationItem",
					new
					{

					}
				);
			}
		}

		public class OllieTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.OllieTransformationItem.ActiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.OllieTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDie";
			public override void SetStaticDefaults()
			{
				DrawAnimationVertical anim = new DrawAnimationVertical(8, 4);
				Main.RegisterItemAnimation(Type, anim);
				ItemID.Sets.AnimatesAsSoul[Type] = true;
			}

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override void PostUpdate()
			{
				Lighting.AddLight(Item.Center, new Vector3(95, 255, 255) * 0.005f);
			}
			public override void UpdateInventory(Player player)
			{
				if (!player.AsV2Player().HasTransformation)
				{
					player.AsV2Player().HasTransformation = true;
					player.AsV2Player().OllieTransformation = true;
					player.AddBuff(ModContent.BuffType<BaelzTransformation>(), V2Utils.SensibleTime(frames: 4));
				}
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<InactiveOllieTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.OllieTransformationItem",
					new
					{
					}
				);
			}
		}
		public class InactiveOllieTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.OllieTransformationItem.InactiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.OllieTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDieInactive";

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<OllieTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.OllieTransformationItem",
					new
					{

					}
				);
			}
		}

		public class SoraTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.SoraTransformationItem.ActiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.SoraTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDie";
			public override void SetStaticDefaults()
			{
				DrawAnimationVertical anim = new DrawAnimationVertical(8, 4);
				Main.RegisterItemAnimation(Type, anim);
				ItemID.Sets.AnimatesAsSoul[Type] = true;
			}

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override void PostUpdate()
			{
				Lighting.AddLight(Item.Center, new Vector3(95, 255, 255) * 0.005f);
			}
			public override void UpdateInventory(Player player)
			{
				if (!player.AsV2Player().HasTransformation)
				{
					player.AsV2Player().HasTransformation = true;
					player.AsV2Player().SoraTransformation = true;
					player.AddBuff(ModContent.BuffType<BaelzTransformation>(), V2Utils.SensibleTime(frames: 4));
				}
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<InactiveSoraTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.SoraTransformationItem",
					new
					{
					}
				);
			}
		}
		public class InactiveSoraTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.SoraTransformationItem.InactiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.SoraTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDieInactive";

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<SoraTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.SoraTransformationItem",
					new
					{

					}
				);
			}
		}

		public class MintTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.MintTransformationItem.ActiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.MintTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDie";
			public override void SetStaticDefaults()
			{
				DrawAnimationVertical anim = new DrawAnimationVertical(8, 4);
				Main.RegisterItemAnimation(Type, anim);
				ItemID.Sets.AnimatesAsSoul[Type] = true;
			}

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override void PostUpdate()
			{
				Lighting.AddLight(Item.Center, new Vector3(95, 255, 255) * 0.005f);
			}
			public override void UpdateInventory(Player player)
			{
				if (!player.AsV2Player().HasTransformation)
				{
					player.AsV2Player().HasTransformation = true;
					player.AsV2Player().MintTransformation = true;
					player.AddBuff(ModContent.BuffType<BaelzTransformation>(), V2Utils.SensibleTime(frames: 4));
				}
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<InactiveMintTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.MintTransformationItem",
					new
					{
					}
				);
			}
		}
		public class InactiveMintTF : ModItem
		{
			public override bool IsLoadingEnabled(Mod mod) => !V2.GetFooled;
			public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.TransformationItems.MintTransformationItem.InactiveName");
			public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.TransformationItems.MintTransformationItem.Short");
			public override string Texture => "V2/Items/Voraria/TransformationItems/Baelz/LoadedDieInactive";

			public override void SetDefaults()
			{
				Item.width = 34;
				Item.height = 34;
				Item.rare = ItemRarityID.Red;
				Item.value = Item.sellPrice(
					gold: 3,
					silver: 15
				);
			}
			public override bool CanRightClick() => true;
			public override void RightClick(Player player)
			{
				bool Favourited = Item.favorited;
				Item.SetDefaults(ModContent.ItemType<MintTF>());
				Item.favorited = Favourited;
				Item.stack++;
				SoundEngine.PlaySound(SoundID.Unlock);
			}
			public override void ModifyTooltips(List<TooltipLine> tooltips)
			{
				tooltips.AddVorariaItemTooltip(
					"Voraria.TransformationItems.MintTransformationItem",
					new
					{

					}
				);
			}
		}
	}
}
