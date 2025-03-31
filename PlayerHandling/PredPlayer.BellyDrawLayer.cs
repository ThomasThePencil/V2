using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;

namespace V2.PlayerHandling;
public class BellyDrawLayer : PlayerDrawLayer
{
    // Internal cache
    private static readonly IDictionary<string, Texture2D> TumSpritesCache = new Dictionary<string, Texture2D>();

    private static Texture2D RequestTexture(string path)
    {
        if (TumSpritesCache.TryGetValue(path, out Texture2D result))
            return result;
        
        result = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
        TumSpritesCache[path] = result;
        return result;
    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);
    public static int GetKingSlimeDigestionStage(Player plr)
    {
        if (plr.AsPred().StomachTracker?.Prey?.FirstOrDefault(x => x.Type == PreyType.NPC && x.ExactType == NPCID.KingSlime) is not PreyData giantJelloDessert)
            return 0;

        if (!giantJelloDessert.NoHealth)
            return 1;
        else
        {
            if (giantJelloDessert.WeightLeftToDigest > 20.0)
                return 1;
            /*else if (giantJelloDessert.WeightLeftToDigest > 50.0)
                return 2;
            else if (giantJelloDessert.WeightLeftToDigest > 40.0)
                return 3;*/
            else
                return 0;
        }
    }

    private int getFrameForBelly(Player player)
    {
        var Frame = 0;
        switch (player.legFrame.Y / 56)
        {
            case 0:
                Frame = 0;
                break;
            case 5:
                Frame = 1;
                break;
            case 7 or 14:
                Frame = 3;
                break;
            case 8 or 9 or 15 or 16:
                Frame = 5;
                break;
            case 10 or 17:
                Frame = 4;
                break;
        }

        if ((player.ItemAnimationActive ||
             player.inventory[player.selectedItem].holdStyle != ItemHoldStyleID.None) && Frame != 1)
            Frame = 0;

        if (player.sitting.isSitting) Frame = 2;
        else if (player.sleeping.isSleeping) Frame = 1;

        return Frame;
    }

    public static readonly List<BellyDrawer> BellyDrawers =
    [
        new RegularBelly(),
        new TorsoClothedBelly()
    ];

    private static void DrawPlayerBelly(ref PlayerDrawSet drawInfo, int size, int frame)
    {
        foreach (BellyDrawer bellyDrawer in BellyDrawers)
        {
            bellyDrawer.PreparePlayer(ref drawInfo, size, frame);
            if (bellyDrawer.ShouldDraw(ref drawInfo, size, frame))
            {
                DrawData draw = bellyDrawer.BuildDrawData(ref drawInfo, size, frame);
                if (draw.texture is null) continue;
                drawInfo.DrawDataCache.Add(draw);
            }
        }
    }


    protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		if (drawInfo.drawPlayer.isDisplayDollOrInanimate)
			return;

		var player = drawInfo.drawPlayer;
        var tumSize = player.AsPred().StomachSize;
        // int Frame = getFrameForBelly(drawInfo.drawPlayer);
        if (V2.GetFooled)
        {
            var spriteEffects = player.direction switch
            {
                -1 => SpriteEffects.FlipHorizontally,
                _ => SpriteEffects.None
            };
            var exactTextureToUse = "V2/AprilFools/BellyColorless";
            var bellySize = player.AsPred().StomachFullness;
            bellySize /= PreyData.NewData(player).InitialSize;

            var texture = ModContent.Request<Texture2D>(exactTextureToUse, AssetRequestMode.ImmediateLoad)
                .Value;
            var tumDraw = new DrawData
            (
                texture,
                player.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY) +
                new Vector2(player.direction == 1 ? 6f : -26f, 2f) * (float)bellySize + new Vector2(0f, 8f),
                texture.Bounds,
                drawInfo.colorBodySkin,
                player.bodyRotation,
                new Vector2(32f, 32f),
                (float)bellySize * 0.33f,
                spriteEffects
            );
            drawInfo.DrawDataCache.Add(tumDraw);
        }
        else
        {
            if (GetKingSlimeDigestionStage(player) > 0)
            {

            }
            else DrawPlayerBelly(ref drawInfo, tumSize, getFrameForBelly(player));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class BellyDrawer
    {
        public const string V2TumSpritesRoot = "V2/PlayerHandling/TumSprites/";

        /// <summary>
        /// Builds and returns <see cref="DrawData"/> for use when rendering belly (or anything).
        /// </summary>
        /// <param name="drawInfo">DrawInfo of player</param>
        /// <param name="size">Belly size</param>
        /// <param name="frame">Selected frame, determined by leg position (for walking animation)</param>
        /// <returns>DrawData used for drawing. Or <code>default(DrawData)</code> for no drawing at all.</returns>
        public abstract DrawData BuildDrawData(ref PlayerDrawSet drawInfo, int size, int frame);

        /// <summary>
        /// Determines if this drawer should act or not.
        /// </summary>
        /// <param name="drawInfo">DrawInfo of player</param>
        /// <param name="size">Belly size</param>
        /// <param name="frame">Selected frame, determined by leg position (for walking animation)</param>
        /// <returns>True - Layer will be drawn; False - Layer will not be drawn.</returns>
        public abstract bool ShouldDraw(ref PlayerDrawSet drawInfo, int size, int frame);

        /// <summary>
        /// Allows you to prepare the player for this layer.
        /// </summary>
        /// <param name="drawInfo"></param>
        /// <param name="size"></param>
        /// <param name="frame"></param>
        public virtual void PreparePlayer(ref PlayerDrawSet drawInfo, int size, int frame)
        {
        }
    }

    public class RegularBelly : BellyDrawer
    {
        public static Vector2 BellyPosition;
        public static Rectangle SourceBare;
        
        // FRAME 4, 6 NEEDS TO BE OFFSET BY A SINGLE PIXEL BECAUSE THE PLAYER BOUNCES UP GENTLY
        // FRAME 3 NEEDS TO BE OFFSET BY 2 PIXELS

        public static Vector2 getPositionAtFeetOfPlayer(ref PlayerDrawSet drawInfo)
        {
            Vector2 p = drawInfo.Center + Vector2.UnitY * drawInfo.drawPlayer.height / 2f;
            return p.Floor() - Main.screenPosition.Floor();
        }

        public static Vector2 getPositionForTumRender(Vector2 feetPosition, ref PlayerDrawSet drawInfo,
            float offsetX, float offsetY, Texture2D sprite)
        {
            // Offset belly accordingly
            feetPosition.X += offsetX * drawInfo.drawPlayer.direction;
            feetPosition.Y -= offsetY - 2;

            feetPosition.X -= sprite.Width * (drawInfo.drawPlayer.direction == -1 ? 1 : 0);

            return feetPosition;
        }

        public static readonly IReadOnlyList<TextureOffset> StandardBellies =
        [
            new (-1, 27),  // 1
            new (-1, 27),  // 2
            new (-1, 27),  // 3
            new (-1, 37),  // 4
            new (-1, 37),  // 5
            new (0,  37),   // 6
            new (0,  37),   // 7
            new (0,  37),   // 8
            new (0,  37),   // 9
            new (0,  37),   // 10
            new (-1, 37),  // 11
            new (-1, 37),  // 12
            new (-1, 35),  // 13
            new (-1, 35),  // 14
            new (-1, 35),  // 15
            new (-1, 35),  // 16
            new (-1, 35),  // 17
            new (-1, 25),  // 18
            new (-1, 25),  // 19
            new (-1, 25),  // 20
        ];


        public static readonly IReadOnlyList<TextureOffset> BossBellies =
        [
            new (-1, 63),  // KingSlime
        ];

        private const int MAX_FRAMES = 6;

        public override bool ShouldDraw(ref PlayerDrawSet drawInfo, int size, int frame)
        {
            return size > 0 && size <= StandardBellies.Count;
        }

        public override DrawData BuildDrawData(ref PlayerDrawSet drawInfo, int size, int frame)
        {
            var player = drawInfo.drawPlayer;

            if (player.AsPred().IsLayingOnTum) return default;
            
            if (size >= 1 && size <= StandardBellies.Count)
            {
                TextureOffset tum = StandardBellies[size - 1];
                //TextureOffset tum = BossBellies[0];

                var offsetX = tum.xOffset * 2;
                var offsetY = tum.yOffset * 2;

                Vector2 tumLocation = getPositionAtFeetOfPlayer(ref drawInfo);

                string bellySpritePath = V2TumSpritesRoot + $"Tum{size}/Bare";
                //string bellySpritePath = V2TumSpritesRoot + $"Boss_KingSlime/Bare";
                Texture2D bareTum = RequestTexture(bellySpritePath);

                tumLocation = getPositionForTumRender(tumLocation, ref drawInfo, offsetX, offsetY, bareTum);

                if (frame == 3 || frame == 5) tumLocation.Y -= 2;
                if (frame == 2) tumLocation.Y -= player.sitting.offsetForSeat.Y - 4;
                if (player.mount.Active)
                {
                    tumLocation.Y += player.mount.HeightBoost;
                    frame = 1;
                }
                else if (player.portableStoolInfo.IsInUse)
                {
                    // Line below is no longer needed as drawInfo automatically incorporates the offset data.
                    // tumLocation.Y -= 8f;
                    frame = 1;
                }

                if (player.gravDir == -1) tumLocation.Y += 6;

                var sourceRectBare =
                    new Rectangle(0, frame * (bareTum.Height / MAX_FRAMES), bareTum.Width, bareTum.Height / MAX_FRAMES);
                var actualDrawBare = new DrawData(bareTum, tumLocation, sourceRectBare, drawInfo.colorBodySkin,
                    player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);

                BellyPosition = tumLocation;
                SourceBare = sourceRectBare;

                return actualDrawBare;
            }

            return default;
        }
    }

    public class TorsoClothedBelly : BellyDrawer
    {
        private static string GetTummyCoverFromEquips(Item item, int size)
        {
            var ValidArmor = "Bare";

            switch (item.type)
            {
                case ItemID.TheBrideDress:
                    ValidArmor = "WeddingDress";
                    break;
                case ItemID.FlinxFurCoat:
                    ValidArmor = "FlinxFurCoat";
                    if (size > 4) ValidArmor = "Bare";
                    break;
                case ItemID.PrinceUniform:
                    ValidArmor = "PrinceUniform";
                    if (size > 4) ValidArmor = "Bare";
                    break;
            }

            return ValidArmor;
        }

        public override DrawData BuildDrawData(ref PlayerDrawSet drawInfo, int size, int frame)
        {
            Player player = drawInfo.drawPlayer;

            string tumCover = null;

            // Get tum cover from torso slot (Both Regular Equipment Slot and Social Slot)
            if (!player.armor[11].IsAir && player.armor[11].type != ItemID.FamiliarShirt)
                tumCover = GetTummyCoverFromEquips(player.armor[11], size);
            else if (!player.armor[2].IsAir && player.armor[2].type != ItemID.FamiliarShirt)
                tumCover = GetTummyCoverFromEquips(player.armor[2], size);

            // if (string.IsNullOrWhiteSpace(tumCover) || tumCover == "Bare") return default;


            var filePath = V2TumSpritesRoot + $"Tum{size}/{tumCover}";
            if (ModContent.HasAsset(filePath))
            {
                var TumArmor = RequestTexture(filePath);

                var clothingDraw = new DrawData(TumArmor, RegularBelly.BellyPosition, RegularBelly.SourceBare,
                    drawInfo.colorArmorBody,
                    player.bodyRotation, Vector2.Zero, 1f, drawInfo.playerEffect);
                clothingDraw.shader = drawInfo.cBody;
                
                return clothingDraw;
            }

            return default;
        }

        public override bool ShouldDraw(ref PlayerDrawSet drawInfo, int size, int frame)
        {
            if (size <= 0) return false;
            if (drawInfo.drawPlayer.AsPred().IsLayingOnTum) return false;
            Player player = drawInfo.drawPlayer;

            string tumCover = "Bare";
            // Get tum cover from torso slot (Both Regular Equipment Slot and Social Slot)
            if (!player.armor[11].IsAir && player.armor[11].type != ItemID.FamiliarShirt)
                tumCover = GetTummyCoverFromEquips(player.armor[11], size);
            else if (!player.armor[2].IsAir && player.armor[2].type != ItemID.FamiliarShirt)
                tumCover = GetTummyCoverFromEquips(player.armor[2], size);

            return tumCover != "Bare";
        }
    }

    /// <summary>
    /// Defines textures (if supplied) and X, Y offset data.
    /// </summary>
    public struct TextureOffset
    {
        public string TexturePath { get; }
        public int xOffset { get; }
        public int yOffset { get; }

        public TextureOffset(int x, int y)
        {
            TexturePath = string.Empty;
            xOffset = x;
            yOffset = y;
        }

        public TextureOffset(string tum, int x, int y)
        {
            TexturePath = tum;
            xOffset = x;
            yOffset = y;
        }
    }
}