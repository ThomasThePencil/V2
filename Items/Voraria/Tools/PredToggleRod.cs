using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using V2.Core;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Tiles.Vanilla.Paintings;

namespace V2.Items.Voraria.Tools
{

	public class PredToggleRod : ModItem
	{
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.width = 44;
			Item.height = 44;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(silver: 40);
		}
		public override void HoldItem(Player player)
		{
			player.AsV2Player().HoldingPredToggleRod = true;
		}
		public static void OnUse(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				Vector2 position = Main.MouseWorld;
				Point TilePos = position.ToTileCoordinates();
				Rectangle ClickPos = new Rectangle((int)position.X, (int)position.Y, 1, 1);

				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					//later
				}
				else
				{
					foreach (var npc in Main.ActiveProjectiles)
					{
						if (npc.active && ClickPos.Intersects(npc.Hitbox) && npc.AsPred().IsPredTileEntity)
						{
							if (npc.ai[2] == 0)
							{
								npc.ai[2] = 1;
							}
							else
							{
								npc.ai[2] = 0;
							}
						}
					}
					/*if (TileEntity.ByPosition.TryGetValue(new Point16(TilePos.X, TilePos.Y), out TileEntity tileEntity))
					{
						if (tileEntity is Dryadisque_TileEntity)
						{
							foreach (var npc in Main.ActiveProjectiles)
							{
								if (npc.active && (npc.position / 16).Distance(tileEntity.Position.ToVector2()) < 2f && npc.type == ModContent.ProjectileType<Dryadisque_ProjectileEntity>())
								{
									if (npc.ai[2] == 0)
										npc.ai[2] = 1;
									else npc.ai[2] = 0;
								}
							}
						}
					}*/
				}
			}
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.AddVorariaItemTooltip(
				"Voraria.Tools.PredToggleRod",
				new
				{

				}
			);
		}
		public override bool? UseItem(Player player)
		{
			OnUse(player);
			return true;
		}
	}
}
