using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using V2.Core;
using V2.Items.ItemGroupUtils;
using V2.NPCs;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Starter;
using V2.StatusEffects.Voraria.Buffs;
using V2.UI;

namespace V2.Items
{
	public static class PreyItemStuff
	{
		public static PreyItem AsFood(this Item item)
		{
			if (item.IsAir)
				return null;

			bool appliedAsPreyItem = item.TryGetGlobalItem(out PreyItem result);
			if (appliedAsPreyItem)
				return result;
			else
				return null;
		}

		/// <summary>
		/// Deals the given amount of digestion damage to the given item, respecting damage variation and luck.<br/>
		/// </summary>
		/// <param name="pred">The pred currently digesting this item.</param>
		/// <param name="digestionDamage">The total amount of digestion damage to be dealt, before damage variation calculations.</param>
		/// <returns>Whether or not the resulting digestion tick "kills" (depletes the durability of) the item.</returns>
		public static bool TakeDigestionDamage(this Item item, Entity pred, double digestionDamage, bool direct = true, int indirectWhoAmI = -1)
		{
			int trueDigestionDamage = Main.DamageVar((float)digestionDamage);
			if (ModContent.GetInstance<V2ServerConfig>().DefenseInDigestionCalcs)
				trueDigestionDamage -= item.defense / 2;
			if (trueDigestionDamage < 1)
				trueDigestionDamage = 1;

			//Baelz digestion crit (we are so fuckin good at making content)
			bool digestionCrit = false;
			Color DigestionTextColor = Color.DarkGreen;
			if (pred is Player)
			{
				Player predPlayer = pred as Player;
				int chance = Main.rand.Next(101);
				int critChance = BaelzTransformation.GetCritChanceForDigestionTicks(predPlayer);
				if (chance <= critChance)
				{
					digestionCrit = true;
					trueDigestionDamage *= 2;
					DigestionTextColor = Color.FromNonPremultiplied(125, 175, 0, 255);
				}
			}

			item.AsFood().Health -= trueDigestionDamage;
			if (item.type == ItemID.GuideVoodooDoll)
			{
				foreach (NPC npc in Main.ActiveNPCs)
				{
					if (npc.type == NPCID.Guide)
					{
						PreyNPC.TakeDigestionDamage(npc, pred, digestionDamage, voodoo: true);
						break;
					}
				}
			}
			if (item.type == ItemID.ClothierVoodooDoll)
			{
				foreach (NPC npc in Main.ActiveNPCs)
				{
					if (npc.type == NPCID.Clothier)
					{
						PreyNPC.TakeDigestionDamage(npc, pred, digestionDamage, voodoo: true);
						break;
					}
				}
			}

			if (Main.netMode == NetmodeID.SinglePlayer && ModContent.GetInstance<V2ClientConfig>().ShowChurnDamageNumbers)
			{
				CombatText digestionText = Main.combatText[CombatText.NewText(
					item.Hitbox,
					DigestionTextColor,
					trueDigestionDamage,
					digestionCrit,
					true
				)];
				digestionText.position.X = pred.Center.X;
				digestionText.position.X += pred.direction * 14;
				if (pred.direction == -1)
					digestionText.position.X -= ChatManager.GetStringSize(FontAssets.CombatText[0].Value, digestionText.text, new Vector2(digestionText.scale)).X;
				digestionText.position.Y = item.Center.Y;
				digestionText.position.Y += item.height / 5f;
				digestionText.velocity.X = pred.direction * 2.5f;
				digestionText.velocity.Y = -4f;
			}

			if (item.AsFood().Health <= 0)
			{
				item.AsFood().Health = 0;
				bool? churnable = null;
				for (int i = 0; i < item.stack; i++)
					churnable = item.AsFood().OnBreak?.Invoke(item, pred, direct);
				bool indirectValid = !direct && indirectWhoAmI != -1;
				if (indirectValid)
				{
					Player player = Main.player[indirectWhoAmI];
					if (player.difficulty is PlayerDifficultyID.MediumCore or PlayerDifficultyID.Hardcore || ModContent.GetInstance<V2ServerConfig>().PermaChurnableEquipment)
					{
						player.CurrentCaptor().QueueNewPrey(PreyData.NewData(PreyType.Item, item.type, item.AffixName(), item.CalculateSnackSize()));
						return true;
					}

					return false;
				}

				if (churnable.HasValue && !churnable.Value)
					return false;

				return true;
			}
				
			return false;
		}

		public static double CalculateSnackSize(this Item item) => item.AsFood().Size * item.stack;
	}

	public class PreyItem : GlobalItem
	{
		public int MaxHealth { get; set; } = -1;
		private int _health = -1;
		public int Health
		{
			get => _health;
			set => _health = Math.Min(value, MaxHealth);
		}
		/// <summary>
		/// Prevents this possibly tasty looking item from being eaten, ever.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool CannotBeEatenDueToShenanigans { get; set; }
		/// <summary>
		/// Prevents this item from being regurgitated.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool CannotBeRegurgitated { get; set; }
		public double Size { get; set; } = 0.0;
		public int VanillaWellFedDuration { get; set; } = 0;
		/// <summary>
		/// Multiplies the weight that preds gain from fully digesting this item.<br/>
		/// Defaults to 1.<br/>
		/// </summary>
		public double CalorieMultiplier { get; set; } = 1;
		/// <summary>
		/// Equal to how much 'Power' a player recieves on their Well Fed buff from having consumed 1 weight unit of this item.<br/>
		/// The Well Fed buff ranges between -3.5 and 3.5.<br/>
		/// Defaults to 0.<br/>
		/// </summary>
		public double WellFedPower { get; set; } = 0;
		/// <summary>
		/// The minimum acid tier required to digest (deal durability damage to) this item.<br/>
		/// Defaults to 0, allowing all acids to churn and gurgle this item down into fat.<br/>
		/// </summary>
		public int AcidResistTier { get; set; } = 0;
		public string MealSizeTextOverride { get; set; } = null;

		public delegate bool DelegatePreSwallow(Item item, Entity pred);
		/// <summary>
		/// Allows you to make an item do stuff right when it's about to be swallowed.<br/>
		/// Return false to prevent the item from actually being swallowed.<br/>
		/// </summary>
		public DelegatePreSwallow PreSwallow { get; set; } = null;
		public delegate void DelegateOnSwallow(Item item, Entity pred);
		public DelegateOnSwallow OnSwallow { get; set; } = null;

		public delegate bool DelegateOnRegurgitate(Item item, Entity pred);
		/// <summary>
		/// Allows you to make an item do stuff right when it gets regurgitated.<br/>
		/// This seems... very niche, actually. But you never know!<br/>
		/// Return true to delete the item after the code runs.<br/>
		/// </summary>
		public DelegateOnRegurgitate OnRegurgitate { get; set; } = null;
		public int OnSwallowDamage { get; set; } = 0;
		public string OnSwallowDeathReason { get; set; } = null;
		public int OnSwallowSoreThroatTime { get; set; } = 0;

		public delegate bool DelegateCanUseInStomach(Item item, Player player, Entity pred);
		public DelegateCanUseInStomach CanUseInStomach { get; set; } = null;
		public delegate void DelegateUseInStomach(Item item, Player player, Entity pred);
		public DelegateUseInStomach UseInStomach { get; set; } = null;

		public PreyData.DelegateUpdateInStomach UpdateInStomach { get; set; } = null;
		/// <summary>
		/// Allows you to make an item do things when it runs out of durability from digestion damage.<br/>
		/// </summary>
		/// <param name="item">
		/// The item which is being broken.<br/>
		/// </param>
		/// <param name="pred">
		/// The pred making a snack of the item.<br/>
		/// </param>
		/// <param name="direct">
		///	If <see langword="true"/>, this item broke from being directly churned up by a hungry tummy.<br/>
		///	If <see langword="false"/>, this item broke as a result of someone that's wearing or wielding it being steadily melted down into sludge.<br/>
		/// </param>
		/// <returns>
		/// <see langword="true"/> by default, which blanks the item on break; return <see langword="false"/> to prevent the item from being deleted and adding to its captor's belly's list of accomplishments when broken.<br/>
		/// </returns>
		public delegate bool DelegateOnBreak(Item item, Entity pred, bool direct);
		/// <summary>
		/// Allows you to make an item do things when it runs out of durability from digestion damage.<br/>
		/// Please look at the documentation for <see cref="DelegateOnBreak"/> for more thorough information.<br/>
		/// </summary>
		public DelegateOnBreak OnBreak { get; set; } = null;

		/// <summary>
		/// Defaults to <see langword="false"/>; if set to <see langword="true"/>, this item can be gulped down on use by holding the Swallow Item keybind.
		/// </summary>
		public bool EdibleOnUse { get; set; } = false;
		/// <summary>
		/// Defaults to <see langword="false"/>; if set to <see langword="true"/>, this item is automatically gulped down on use.<br/>
		/// </summary>
		public bool AlwaysEatenByUse { get; set; } = false;

		public override bool InstancePerEntity => true;

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			if (item.IsAir)
				return;

			if (MaxHealth != -1)
			{
				if (Health == -1 || Health > MaxHealth)
					Health = MaxHealth;

				if (Health == 0)
				{
					item.TurnToAir();
					return;
				}
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (item.IsAir)
				return;

			if (MaxHealth != -1)
			{
				if (Health == -1 || Health > MaxHealth)
					Health = MaxHealth;

				if (Health == 0)
				{
					item.TurnToAir();
					return;
				}
			}
		}

		public override bool CanUseItem(Item item, Player player)
		{
			if (item.IsAir)
				return false;

			if (player.AsPred().ItemCooldownWhenSwallowingANonStackedItemFromTheMouseSlotBecauseThisGameIsCoolAndAwesome > 0)
			{
				return false;
			}

			if (player.CurrentCaptor() is not null)
			{
				if (item.AsFood().CanUseInStomach is not null && item.AsFood().CanUseInStomach.Invoke(item, player, player.CurrentCaptor().Predator))
					item.AsFood().UseInStomach?.Invoke(item, player, player.CurrentCaptor().Predator);
				return false;
			}
			bool gulpOnUseAttempt = item != player.inventory[58] && player.whoAmI == Main.myPlayer && V2.ItemGulpHotkey.Current;
			gulpOnUseAttempt |= item.AsFood().AlwaysEatenByUse;

            bool attemptingToUse = Main.mouseLeft;
			 if (!Main.keyState.IsKeyDown(Keys.LeftShift))
                attemptingToUse &= Main.mouseLeftRelease;

			attemptingToUse &= item == player.HeldItem;
			attemptingToUse &= !player.mouseInterface;
			if (/*item.AsFood().EdibleOnUse && */ gulpOnUseAttempt && attemptingToUse)
			{
				Main.mouseLeftRelease = false;
				int origStack = item.stack;
				item.stack = 1;
				if (PredPlayer.CanSwallow(player, item))
				{
					if (origStack > 1)
					{
						Item eatenItem = new Item();
						eatenItem.SetDefaults(item.type);
						eatenItem.stack = 1;
						player.ForceDropItem(player.Center, ref eatenItem, out Item itemDrop);
						PredPlayer.Swallow(player, itemDrop);
						item.stack = origStack - 1;
						if (player.whoAmI == Main.myPlayer && player.inventory[58] == item)
							Main.mouseItem.stack = origStack - 1;
					}
					else
					{
						player.ForceDropItem(player.Center, ref item, out Item itemDrop);
						PredPlayer.Swallow(player, itemDrop);
						if (player.whoAmI == Main.myPlayer && player.inventory[58] == item)
						{
							player.AsPred().ItemCooldownWhenSwallowingANonStackedItemFromTheMouseSlotBecauseThisGameIsCoolAndAwesome = 7;
							Main.mouseItem.TurnToAir();
						}
					}
					ModContent.GetInstance<FirstItemEaten>().TrySetCompletion(player);
				}
				else
					item.stack = origStack;

				return false;
			}
			//prevents the player from eating an item simultaneously to placing said item, consuming two stacks
			return !(player.whoAmI == Main.myPlayer && V2.ItemGulpHotkey.Current);
		}

		public override bool CanStack(Item destination, Item source)
		{
			switch (destination.AsFood().MaxHealth, source.AsFood().MaxHealth)
			{
				case (-1, -1):
					return true;
				case (int i, int j) when i == -1 && j != -1:
					// This should never be the case, but just in case...
					destination.AsFood().MaxHealth = source.AsFood().MaxHealth;
					destination.AsFood().Health = source.AsFood().Health;
					return true;
				case (int i, int j) when i != -1 && j == -1:
					source.AsFood().MaxHealth = destination.AsFood().MaxHealth;
					source.AsFood().Health = destination.AsFood().Health;
					return true;
				default:
					return destination.AsFood().Health == source.AsFood().Health;
			}
		}

		public override bool CanStackInWorld(Item destination, Item source)
		{
			if (destination.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			if (item.CurrentCaptor() is not null)
				grabRange = 0;
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (item.CurrentCaptor() is not null)
				return false;

			return true;
		}

		public override bool CanPickup(Item item, Player player)
		{
			if (item.IsAir)
				return false;

			if (item.CurrentCaptor() is not null)
				return false;

			if (item.AsFood().MaxHealth != -1 && item.AsFood().Health == 0)
				return false;

			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.AsFood().MaxHealth == -1 || item.AsFood().Health == -1)
				return;

			if (item.favorited)
			{
				tooltips.Insert(
					tooltips.IndexOf(tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc")) + 1,
					new TooltipLine(
						V2.Instance,
						"FavoriteNoNoms",
						"Swallowing from inventory will be blocked, but can still be digested by other means"
					)
				);
			}

			double healthRemainingRatio = (double)item.AsFood().Health / (double)item.AsFood().MaxHealth;
			Color duraPercentColor = Color.Lerp(Color.White, Color.DarkOliveGreen, (float)(1.0 - healthRemainingRatio));
			V2Utils.FindLastTooltipLineBeforeFlavorText(tooltips, out TooltipLine finalLine);
			tooltips.Insert(
				tooltips.IndexOf(finalLine) + 1,
				new TooltipLine(
					V2.Instance,
					"V2Durability",
					"Durability left: " + item.AsFood().Health + " / " + item.AsFood().MaxHealth + " ([c/" + (duraPercentColor * ((int)Main.mouseTextColor / 255f)).Hex3() + ":" + healthRemainingRatio.ToPercentage(2) + "])"
				)
			);

			double size = item.AsFood().Size;
			string sizeDescription = "Barely a light snack";
			if (size >= 0.04 && size < 0.08)
				sizeDescription = "Light snack";
			if (size >= 0.08 && size < 0.14)
				sizeDescription = "Snack";
			if (size >= 0.14 && size < 0.21)
				sizeDescription = "Large snack";
			if (size >= 0.21 && size < 0.3)
				sizeDescription = "Small meal";
			if (size >= 0.3 && size < 0.4)
				sizeDescription = "Somewhat-small meal";
			if (size >= 0.4 && size < 0.52)
				sizeDescription = "Modest meal";
			if (size >= 0.52 && size < 0.65)
				sizeDescription = "Medium meal";
			if (size >= 0.65 && size < 0.82)
				sizeDescription = "Noteworthy meal";
			if (size >= 0.82 && size < 1)
				sizeDescription = "Sizable meal";
			if (size >= 1 && size < 1.2)
				sizeDescription = "Large meal";
			if (size >= 1.2 && size < 1.5)
				sizeDescription = "Huge meal";
			if (size >= 1.5 && size < 2.0)
				sizeDescription = "Massive meal";
			if (size >= 2.0)
				sizeDescription = "Potentially, a vaguely satisfying meal";

			if (item.AsFood().MealSizeTextOverride is not null or "")
				sizeDescription = item.AsFood().MealSizeTextOverride;

			tooltips.Insert(
				tooltips.IndexOf(finalLine) + 2,
				new TooltipLine(
					V2.Instance,
					"V2SizeAsFood",
					sizeDescription + " (size of " + size + ")"
				)
			);

			string canBeDigestedBy = Language.GetTextValue("Mods.V2.ItemTooltip.Generic.AcidResistTier." + item.AsFood().AcidResistTier);
			tooltips.Insert(
				tooltips.IndexOf(finalLine) + 3,
				new TooltipLine(
					V2.Instance,
					"V2AcidResist",
					canBeDigestedBy
				)
			);

			int linePos = 4;
			if (item.AsFood().EdibleOnUse)
			{
				if (item.AsFood().AlwaysEatenByUse)
				{
					tooltips.Insert(
						tooltips.IndexOf(finalLine) + linePos,
						new TooltipLine(
							V2.Instance,
							"V2EatenByNormalUse",
							Language.GetTextValue("Mods.V2.ItemTooltip.Generic.EatenByNormalUse")
						)
					);
				}
				else
				{
					tooltips.Insert(
						tooltips.IndexOf(finalLine) + linePos,
						new TooltipLine(
							V2.Instance,
							"V2EdibleByNormalUse",
							Language.GetTextValue("Mods.V2.ItemTooltip.Generic.EdibleFromNormalUse")
						)
					);
				}
				linePos++;
			}

			if (item.AsFood().Health == 0)
			{
				tooltips.Insert(
					tooltips.IndexOf(finalLine) + linePos,
					new TooltipLine(
						V2.Instance,
						"V2EdibleByNormalUse",
						Language.GetTextValue("Mods.V2.ItemTooltip.Generic.Broken")
					)
				);
				linePos++;
			}
		}

		public override void SaveData(Item item, TagCompound tag)
		{
			tag["VDura"] = Health;
		}

		public override void LoadData(Item item, TagCompound tag)
		{
			Health = tag.GetInt("VDura");
		}
	}
}
