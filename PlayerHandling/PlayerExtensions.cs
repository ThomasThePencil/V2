using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;
using V2.Projectiles;

namespace V2.PlayerHandling
{
	public static class PlayerExtensions
	{
		public static V2Player AsV2Player(this Player player) => player.GetModPlayer<V2Player>();
		public static PredPlayer AsPred(this Player player) => player.GetModPlayer<PredPlayer>();
		public static PreyPlayer AsFood(this Player player) => player.GetModPlayer<PreyPlayer>();

		public static bool IsFoodFor(this Player player, Entity entity, out bool pastTense)
		{
			pastTense = false;
			if (entity is NPC predNPC)
			{
				if (PredNPC.GetStomachTracker(predNPC) is null)
					return false;

				List<PreyData> playerAsPreyList = PredNPC.GetStomachTracker(predNPC).Prey.FindAll(x => x.Type == PreyType.Player && x.Instance.whoAmI == player.whoAmI);
				if (playerAsPreyList != null && playerAsPreyList.Count > 0)
				{
					if (playerAsPreyList.FirstOrDefault(x => !x.NoHealth) is null)
						pastTense = true;
					return true;
				}
			}
			else if (entity is Player predPlayer)
			{
				List<PreyData> playerAsPreyList = predPlayer.AsPred().StomachTracker?.Prey.FindAll(x => x.Type == PreyType.Player && x.Instance.whoAmI == player.whoAmI);
				if (playerAsPreyList != null && playerAsPreyList.Count > 0)
				{
					if (playerAsPreyList.FirstOrDefault(x => !x.NoHealth) is null)
						pastTense = true;
					return true;
				}
			}
			else if (entity is Projectile predProjectile)
			{
				if (PredProjectile.GetStomachTracker(predProjectile) is null)
					return false;

				List<PreyData> playerAsPreyList = PredProjectile.GetStomachTracker(predProjectile)?.Prey.FindAll(x => x.Type == PreyType.Player && x.Instance.whoAmI == player.whoAmI);
				if (playerAsPreyList != null && playerAsPreyList.Count > 0)
				{
					if (playerAsPreyList.FirstOrDefault(x => !x.NoHealth) is null)
						pastTense = true;
					return true;
				}
			}
			return false;
		}

		public static bool HasEaten(this Player player, string entity, out int howManyTimes)
		{
			howManyTimes = 0;
			if (!player.AsPred().mealCount.ContainsKey(entity))
				return false;
			if (player.AsPred().mealCount[entity] <= 0)
				return false;

			howManyTimes = player.AsPred().mealCount[entity];
			return true;
		}

		public static Vector2 TrueMountedCenter(this Player player)
			=> new Vector2(
				player.position.X + ((float)player.width / 2f),
				player.position.Y + 21f + player.HeightOffsetHitboxCenter
			);

		public static bool IsAirborne(this Player player)
		{
			if (player.mount.Active)
				return !MountID.Sets.Cart[player.mount.Type];

			if (player.velocity.Y == 0f)
				return false;

			if (player.CurrentCaptor() is not null)
				return false;

			return true;
		}


		/// <summary>
		/// Drops the given <see cref="Item"/> from the player at the given position.<br/>
		/// Only drops items that actually exist. Does not drop favorited items.<br/>
		/// </summary>
		/// <param name="player">The player from which to drop the given item.</param>
		/// <param name="source">The source of the item drop.</param>
		/// <param name="position">The position at which the item should be dropped.</param>
		/// <param name="item">The item itself, ready to be dropped into the world.</param>
		/// <param name="itemDrop">The item, now dropped into the world.</param>
		public static void ForceDropItem(this Player player, Vector2 position, ref Item item, out Item itemDrop)
		{
			itemDrop = null;
			if (item.IsAir)
				return;
			if (item.favorited)
				return;

			int itemDropId = Item.NewItem(player.GetSource_Misc("ThrowItem"), (int)position.X, (int)position.Y, player.width, player.height, item);
			itemDrop = Main.item[itemDropId];

			itemDrop.velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
			itemDrop.velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
			itemDrop.noGrabDelay = 100;
			itemDrop.newAndShiny = false;

			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemDropId);

			item.TurnToAir();
		}

		public static void DoPetHandlerBuff(this Player player, int buffIndex, ref bool petFlag, int petProjID)
		{
			player.buffTime[buffIndex] = 18000;
			petFlag = true;
			if (player.ownedProjectileCounts[petProjID] <= 0 && player.whoAmI == Main.myPlayer)
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, petProjID, 0, 0f, player.whoAmI);
		}
	}
}
