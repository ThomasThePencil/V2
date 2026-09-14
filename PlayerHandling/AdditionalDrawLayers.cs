using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Items.Voraria.Armor;

namespace V2.PlayerHandling
{
	public class HelmetGlowMask : PlayerDrawLayer
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			if (drawInfo.drawPlayer.dead)
			{
				return;
			}

			if (drawPlayer.armor[10].type == ModContent.ItemType<MushroomHairpin>() || (drawPlayer.armor[10].type == ItemID.None && drawPlayer.armor[0].type == ModContent.ItemType<MushroomHairpin>()))
			{
				Color color = drawPlayer.GetImmuneAlphaPure(Color.White, drawInfo.shadow);

				Texture2D texture = ModContent.Request<Texture2D>("V2/Items/Voraria/Armor/MushroomHairpin_Head_Glow").Value;
				Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - drawPlayer.bodyFrame.Width / 2, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.headPosition;
				Vector2 headVect = drawInfo.headVect;
				DrawData drawData = new DrawData(texture, drawPos.Floor() + headVect, drawPlayer.bodyFrame, color, drawPlayer.headRotation, headVect, 1f, drawInfo.playerEffect, 0)
				{
					shader = drawInfo.cHead
				};
				drawInfo.DrawDataCache.Add(drawData);
			}
			else if (drawPlayer.armor[10].type == ModContent.ItemType<ShroomiteHairpin>() || (drawPlayer.armor[10].type == ItemID.None && drawPlayer.armor[0].type == ModContent.ItemType<ShroomiteHairpin>()))
			{
				Color color = drawPlayer.GetImmuneAlphaPure(Color.White, drawInfo.shadow);

				Texture2D texture = ModContent.Request<Texture2D>("V2/Items/Voraria/Armor/ShroomiteHairpin_Head_Glow").Value;
				Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawPlayer.width / 2 - drawPlayer.bodyFrame.Width / 2, drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.headPosition;
				Vector2 headVect = drawInfo.headVect;
				DrawData drawData = new DrawData(texture, drawPos.Floor() + headVect, drawPlayer.bodyFrame, color, drawPlayer.headRotation, headVect, 1f, drawInfo.playerEffect, 0)
				{
					shader = drawInfo.cHead
				};
				drawInfo.DrawDataCache.Add(drawData);
			}
		}
	}
}
