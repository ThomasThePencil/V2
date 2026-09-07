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
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using V2.Core;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Projectiles.Voraria.Weapons.Summon.ShroomFairy;

namespace V2.Items.Voraria.Weapons.Summon
{
	public class ShroomStaff : ModItem
	{
		public override LocalizedText DisplayName => Language.GetText("Mods.V2.ItemName.Voraria.Weapons.Summon.ShroomFairySummon");
		public override LocalizedText Tooltip => Language.GetText("Mods.V2.ItemTooltip.Voraria.Weapons.Summon.ShroomFairySummon.Short");
		public override void SetStaticDefaults()
		{
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1.5f;
			ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.SlimeStaff;
		}
		public override void SetDefaults()
		{
			Item.AsAnItem().ShouldSaveSummonWeights = true;

			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 46;
			Item.height = 46;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item44;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<ShroomFairyBuff>();
			Item.shoot = ModContent.ProjectileType<ShroomFairy>();
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			position = Main.MouseWorld;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, 0, 0, Main.myPlayer);
			projectile.originalDamage = 0;
			int FairyIndex = 1;
			foreach (bool flag in Item.AsAnItem().InUseSummonWeights)
			{
				if (!flag)
				{
					break;
				}
				FairyIndex++;
			}
			if (FairyIndex > Item.AsAnItem().SavedSummonWeights.Count)
			{
				Item.AsAnItem().SavedSummonWeights.Add(0.0);
				Item.AsAnItem().InUseSummonWeights.Add(false);
			}
			projectile.AsPred().ExtraWeight = Item.AsAnItem().SavedSummonWeights[FairyIndex - 1];
			Item.AsAnItem().InUseSummonWeights[FairyIndex - 1] = true;
			projectile.AsPred().TiedToSummonItem = Item;
			projectile.AsPred().TiedToSummonIndex = FairyIndex - 1;
			return false;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.ShroomiteBar, 2)
				.AddIngredient(ItemID.GlowingMushroom, 35)
				.AddIngredient(ItemID.PixieDust, 25)
				.AddTile(TileID.Autohammer)
				.Register();
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Weapons.Summon.ShroomFairySummon",
				new
				{

				}
			);
		}
	}
	public class SlimeStaff : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SlimeStaff;
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ShimmerTransformToItem[ItemID.SlimeStaff] = ModContent.ItemType<ShroomStaff>();
		}
	}
	public class ShroomStaffDrop : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (npc.type == NPCID.SporeBat || npc.type == NPCID.ZombieMushroom || npc.type == NPCID.ZombieMushroomHat || npc.type == NPCID.SporeSkeleton)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShroomStaff>(), 90));
			}
			else if(npc.type == NPCID.AnomuraFungus || npc.type == NPCID.MushiLadybug || npc.type == NPCID.FungoFish || npc.type == NPCID.FungiBulb || npc.type == NPCID.GiantFungiBulb)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShroomStaff>(), 50));
			}
		}
	}
}
