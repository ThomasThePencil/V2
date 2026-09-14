using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core;
using V2.Items;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Sounds.Vore;
using V2.StatusEffects.Voraria.Debuffs;
using V2.UI.VoreBestiary;

namespace V2.Projectiles
{
	public partial class PredProjectile : GlobalProjectile
	{
		public static VoreTracker GetStomachTracker(Projectile projectile)
		{
			if (Main.gameMenu)
				return null;

			return ModContent.GetInstance<V2MasterSystem>().VoreTrackers.FirstOrDefault(x => x.Predator is Projectile predProjectile && predProjectile.whoAmI == projectile.whoAmI);
		}
		public EntityDigestionType DigestionType { get; set; }
		public double MaxStomachCapacity { get; set; }
		public float MaxSwallowRange { get; set; }
		public bool IsPredTileEntity { get; set; }
		public double ExtraWeight { get; set; }
		public double WeightGainRatio { get; set; }
		/// <summary>
		/// Tracks how much food this pred has absorbed, including the food's calorie multiplier.<br/>
		/// Can be used for making things happen after having digested enough food, i suppose!<br/>
		/// </summary>
		public double FoodAbsorbed { get; set; }
		/// <summary>
		/// Allows this projectile to eat bosses despite not being a boss themselves.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool CanSwallowBosses { get; set; }

		public Vector2 MouthSoundRawOffset { internal get; set; }
		public static Vector2 MouthSoundOffset(Projectile projectile)
		{
			Vector2 happyBurpyOffsetDirectionized = projectile.AsPred().MouthSoundRawOffset;
			if (projectile.direction != 0)
				happyBurpyOffsetDirectionized.X *= projectile.direction;
			return happyBurpyOffsetDirectionized;
		}

		public SoundStyle? SmallGulps { get; set; }
		public double SmallGulpThreshold { get; set; }
		public SoundStyle? BigGulps { get; set; }

		public SoundStyle? SmallBurps { get; set; }
		public double SmallBurpThreshold { get; set; }
		public SoundStyle? StandardBurps { get; set; }
		public double BigBurpThreshold { get; set; }
		public SoundStyle? BigBurps { get; set; }
		public float BurpPitchOffset { get; set; }

		/// <summary>
		/// If set to true, this projectile can bypass Divine Intergestion.<br/>
		/// Should only be used for powerful predators that make sense to not give a fuck about your preferences; namely, divine entities themselves.<br/>
		/// Defaults to false.<br/>
		/// </summary>
		public bool DIBypass { get; set; }
		public delegate bool DelegateCanBeForceFed(Projectile projectile);
		public DelegateCanBeForceFed CanBeForceFed { get; set; }

		public delegate void DelegateOnForceFed(Projectile projectile, Player player);
		public DelegateOnForceFed OnForceFed { get; set; }


		public delegate double DelegateGetDigestionTickRate(Projectile projectile, PreyData prey);
		public DelegateGetDigestionTickRate GetDigestionTickRate { get; set; }

		public delegate double DelegateGetDigestionTickDamage(Projectile projectile, PreyData prey);
		public DelegateGetDigestionTickDamage GetDigestionTickDamage { get; set; }

		private double _stomachache;
		public double Stomachache
		{
			get => _stomachache;
			set => _stomachache = Math.Max(0, value);
		}
		public double BaseStomachacheMeterCapacity { get; set; }
		public StatModifier StomachacheMeterCapacityModifier;
		public double StomachacheMeterCapacity
		{
			get
			{
				if (BaseStomachacheMeterCapacity == -1)
					return -1;

				double baseStomachacheMeterCapacity = BaseStomachacheMeterCapacity;
				return StomachacheMeterCapacityModifier.ApplyTo((float)baseStomachacheMeterCapacity);
			}
		}
		/// <summary>
		/// Expresses, from 0 to 12, how well this projectile keeps up with struggles as a pred.<br/>
		/// Defaults to 5.<br/>
		/// </summary>
		public int CounterStruggleEffectiveness { get; set; }

		public delegate void DelegateOnDigestionKill(Projectile projectile, PreyData digestedPrey);
		public DelegateOnDigestionKill OnDigestionKill { get; set; }

		public delegate void DelegateGetDigestedPlayerAdditionalDeathMessages(Projectile projectile, Player player, List<string> deathMessageKeyList);
		public DelegateGetDigestedPlayerAdditionalDeathMessages GetAdditionalDigestedPlayerMessages { get; set; }

		public delegate double DelegateGetPreyAbsorptionRate(Projectile projectile);
		public DelegateGetPreyAbsorptionRate GetPreyAbsorptionRate { get; set; }

		public delegate int DelegateGetVisualBellySize(Projectile projectile);
		public DelegateGetVisualBellySize GetVisualBellySize { get; set; }

		public delegate int DelegateGetVisualWeightStage(Projectile projectile);
		public DelegateGetVisualWeightStage GetVisualWeightStage { get; set; }

		public Item TiedToSummonItem { get; set; }
		public int TiedToSummonIndex { get; set; }

		public SlotId ActiveStomachNoises { get; set; }

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => true;

		public PredProjectile()
		{
			MaxStomachCapacity = 0.0;
			MaxSwallowRange = 36f;
			ExtraWeight = 0.0;
			WeightGainRatio = 0.4;
			CanSwallowBosses = false;

			TiedToSummonItem = null;
			TiedToSummonIndex = -1;

			IsPredTileEntity = false;

			GetDigestionTickRate = null;
			GetDigestionTickDamage = null;
			GetPreyAbsorptionRate = null;

			DIBypass = false;
			CanBeForceFed = (Projectile projectile) => false;
			OnForceFed = null;

			Stomachache = 0;
			BaseStomachacheMeterCapacity = 100.0;
			StomachacheMeterCapacityModifier = StatModifier.Default;
			CounterStruggleEffectiveness = 5;

			MouthSoundRawOffset = Vector2.Zero;
			SmallGulps = Gulps.Short;
			SmallGulpThreshold = 0.2;
			BigGulps = Gulps.Standard;
			SmallBurps = null;
			SmallBurpThreshold = 0.2;
			StandardBurps = null;
			BigBurpThreshold = 2.0;
			BigBurps = null;
			BurpPitchOffset = 0f;

			OnDigestionKill = null;
			GetAdditionalDigestedPlayerMessages = null;

			GetVisualBellySize = null;
			GetVisualWeightStage = null;
		}

		public static void ResetEffects(Projectile projectile)
		{
			double stomachacheQuellPerTick = projectile.AsPred().StomachacheMeterCapacity * (0.05 / (double)V2Utils.SensibleTime(seconds: 1));
			if (GetStomachTracker(projectile) is not null && AnyPreyStillAlive(projectile))
				stomachacheQuellPerTick *= 0.1;
			projectile.AsPred().Stomachache -= stomachacheQuellPerTick;

			projectile.AsPred().StomachacheMeterCapacityModifier = StatModifier.Default;
		}

		public static bool CanSwallow(Projectile pred, Entity prey)
		{
			if (V2.VoreNPCBlacklist is not null && V2.VoreProjectileBlacklist.Count > 0 && V2.VoreProjectileBlacklist.Contains(pred.type))
				return false;

			if (pred.AsPred().MaxStomachCapacity == 0.0)
				return false;

			if (GetCurrentBellyWeight(pred) >= pred.AsPred().MaxStomachCapacity)
				return false;

			if (prey.CurrentCaptor() is not null)
				return false;

			if (prey is Player preyPlayer)
			{
				if (!pred.AsPred().DIBypass)
				{
					foreach (DISwitch toggle in DISwitchHandling.Switches)
					{
						if (toggle.CompareCanBeEatenBy(preyPlayer, pred))
							return false;
					}
				}

				if (preyPlayer.AsFood().PerfectMeal)
					return true;
			}
			else if (prey is NPC preyNPC)
			{
				if (preyNPC.AsFood().CannotBeEatenDueToShenanigans)
					return false;
				if (V2.VoreNPCBlacklist is not null && V2.VoreNPCBlacklist.Count > 0 && V2.VoreNPCBlacklist.Contains(preyNPC.type))
					return false;

				bool tastesLikeSkittles = preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress;
				if (tastesLikeSkittles)
					return preyNPC.CurrentCaptor() is null;

				bool isThePreyAFuckingBoss = preyNPC.boss || (preyNPC.type >= NPCID.EaterofWorldsHead && preyNPC.type <= NPCID.EaterofWorldsTail);  // I hate EoW
				if (!pred.AsPred().CanSwallowBosses && isThePreyAFuckingBoss)
					return false;
			}
			else if (prey is Projectile preyProjectile)
			{
				if (preyProjectile.AsFood().CannotBeEatenDueToShenanigans)
					return false;
				if (V2.VoreNPCBlacklist is not null && V2.VoreProjectileBlacklist.Count > 0 && V2.VoreProjectileBlacklist.Contains(preyProjectile.type))
					return false;
			}
			else if (prey is Item preyItem)
			{
				if (preyItem.AsFood().MaxHealth == -1)
					return false;

				if (preyItem.favorited)
					return false;
			}

			if (GetCurrentBellyWeight(pred) >= pred.AsPred().MaxStomachCapacity)
				return false;

			if (pred.AsPred().MaxStomachCapacity != -1 && PreyData.GetPreySize(prey) > pred.AsPred().MaxStomachCapacity - GetCurrentBellyWeight(pred))
				return false;

			return true;
		}

		/// <summary>
		/// Causes the given predator projectile to swallow the given prey entity, if the given prey entity can be swallowed.
		/// </summary>
		/// <param name="pred">The predator which will attempt to swallow the given prey.</param>
		/// <param name="prey">The prey which will be attempt to be swallowed by the given predator.</param>
		public static void Swallow(Projectile pred, Entity prey, int MPstate = 0, int MPwhoAmI = -1)
		{
			if (!CanSwallow(pred, prey))
				return;

			if (MPstate == 0 && Main.netMode == NetmodeID.MultiplayerClient)
			{
				MPstate = 1;
				MPwhoAmI = Main.myPlayer;
			}

			PreyData food = PreyData.NewData(prey);
			PlaySwallowGulp(pred, food);
			switch (food.Type)
			{
				case PreyType.Player:
					Player player = prey as Player;
					player.AsFood().TotalTimesSwallowed += 1;
					break;
				case PreyType.NPC:
					NPC npc = prey as NPC;
					npc.AsFood().OnSwallowedBy?.Invoke(npc, pred);
					break;
				case PreyType.Projectile:
					Projectile projectile = prey as Projectile;
					if (projectile.AsFood().MaxHealth == -1)
					{
						food = PreyData.NewData(PreyType.Projectile, projectile.type, projectile.Name, PreyData.GetPreySize(projectile));
						projectile.active = false;
					}
					else
						projectile.AsFood().OnSwallowedBy?.Invoke(projectile, pred);
					break;
				case PreyType.Item:
					Item item = prey as Item;
					if (item.AsFood().PreSwallow is not null && !item.AsFood().PreSwallow.Invoke(item, pred))
					{
						food = null;
						return;
					}
					item.AsFood().OnSwallow?.Invoke(item, pred);
					if (item.AsFood().OnSwallowDamage > 0)
						pred.AsFood().Health -= item.AsFood().OnSwallowDamage;
					break;
			}

			AddNewPrey(pred, food);

			pred.netUpdate = true;

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestSwallowPrey);
				packet.Write((byte)2);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send();
			}
			else if (MPstate == 2)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.SyncSwallowPrey);
				packet.Write((byte)2);
				packet.Write(pred.whoAmI);
				packet.Write((byte)food.Type);
				packet.Write(prey.whoAmI);
				packet.Write(MPwhoAmI);
				packet.Send(ignoreClient: MPwhoAmI);
			}
		}

		public static void Regurgitate(Projectile pred, int index = -1, int MPstate = 0, int MPwhoAmI = -1)
		{
			if (MPstate == 0 && Main.netMode == NetmodeID.MultiplayerClient)
			{
				MPstate = 1;
				MPwhoAmI = Main.myPlayer;
			}

			double totalRegurgiweight = 0.0;

			void Regurgitate_Inner(Projectile pred, PreyData prey)
			{
				Entity realPrey = prey.Type switch
				{
					PreyType.Player => prey.Instance as Player,
					PreyType.NPC => prey.Instance as NPC,
					PreyType.Projectile => prey.Instance as Projectile,
					PreyType.Item => prey.Instance as Item,
					PreyType.Custom => null,
					_ => throw new NotImplementedException(),
				};
				realPrey.position = pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f);
				realPrey.velocity = new Vector2(pred.direction * 10f, -2.5f);
				if (realPrey is NPC realPreyNPC)
				{
					realPreyNPC.AsFood().EatenSafetyFrames = 20;
				}
				else if (realPrey is Projectile realPreyProjectile)
				{

				}
				else if (realPrey is Player realPreyPlayer)
				{

				}
				else if (realPrey is Item realPreyItem)
				{
					realPreyItem.noGrabDelay = 60;
					for (int i = 0; i < realPreyItem.stack; i++)
						if (realPreyItem.AsFood().OnRegurgitate is not null && realPreyItem.AsFood().OnRegurgitate.Invoke(realPreyItem, pred))
						{
							realPreyItem.stack--;
						}
					if (realPreyItem.stack <= 0)
						realPreyItem.TurnToAir();
				}
				totalRegurgiweight += prey.WeightLeftToDigest;
			}

			if (index == -1)
			{
				foreach (PreyData prey in GetStomachTracker(pred).Prey)
					Regurgitate_Inner(pred, prey);

				GetStomachTracker(pred).Prey.Clear();
				GetStomachTracker(pred).RefreshStruggleChartList();
			}
			else
			{
				PreyData prey = GetStomachTracker(pred).Prey[index];
				Regurgitate_Inner(pred, prey);

				GetStomachTracker(pred).Prey.Remove(prey);
			}

			SoundEngine.PlaySound(
				totalRegurgiweight <= 0.3 ? pred.AsPred().SmallBurps : pred.AsPred().StandardBurps,
				pred.TrueCenter() + new Vector2(pred.direction * 8f, -14f)
			);

			if (MPstate == 1)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.RequestRegurgitatePrey);
				packet.Write((byte)2);
				packet.Write(Main.myPlayer);
				packet.Write(index);
				packet.Write(Main.myPlayer);
				packet.Send();
			}
			else if (MPstate == 2)
			{
				ModPacket packet = V2.Instance.GetPacket();
				packet.Write((byte)V2.MessageType.SyncRegurgitatePrey);
				packet.Write((byte)2);
				packet.Write(Main.myPlayer);
				packet.Write(index);
				packet.Write(Main.myPlayer);
				packet.Send(ignoreClient: MPwhoAmI);
			}
		}

		/// <summary>
		/// Runs update ticks on all food in the given predator's stomach.
		/// </summary>
		/// <param name="pred">The projectile to update all food in the stomach of.</param>
		public static void UpdatePrey(Projectile pred)
		{
			if (pred.AsPred().StomachacheMeterCapacity > 0 && pred.AsPred().Stomachache >= pred.AsPred().StomachacheMeterCapacity)
			{
				Regurgitate(pred, MPstate: Main.netMode == NetmodeID.SinglePlayer ? 0 : 2);
				return;
			}
			foreach (PreyData prey in GetStomachTracker(pred).Prey)
			{
				if (!prey.NoHealth)
				{
					prey.UpdateInStomach?.Invoke(prey.Instance, pred, false);

					switch (prey.Type)
					{
						case PreyType.Player:
							Player preyPlayer = prey.Instance as Player;
							if (preyPlayer is null || !preyPlayer.active || preyPlayer.dead)
								continue;

							preyPlayer.velocity = Vector2.Zero;
							preyPlayer.position = pred.position;
							break;
						case PreyType.NPC:
							NPC preyNPC = prey.Instance as NPC;
							if (preyNPC is null || !preyNPC.active)
								continue;

							preyNPC.velocity = Vector2.Zero;
							preyNPC.position = pred.position;
							break;
						case PreyType.Projectile:
							Projectile preyProjectile = prey.Instance as Projectile;
							if (preyProjectile is null || !preyProjectile.active)
								continue;

							preyProjectile.velocity = Vector2.Zero;
							preyProjectile.position = pred.position;
							break;
						case PreyType.Item:
							Item preyItem = prey.Instance as Item;
							if (preyItem is null || !preyItem.active)
								continue;

							preyItem.velocity = Vector2.Zero;
							preyItem.position = pred.position;
							break;
					}

					if (pred.AsPred().GetDigestionTickRate is null || pred.AsPred().GetDigestionTickDamage is null)
					{
						if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
							Main.NewText(pred.Name + " has invalid digestion damage/tick rate methods!");
						break;
					}
					double digestionDamage = pred.AsPred().GetDigestionTickDamage.Invoke(pred, prey);
					double digestionTickRate = pred.AsPred().GetDigestionTickRate.Invoke(pred, prey);
					int digestionTickFrameRate = (int)Math.Round(60.0 / digestionTickRate);
					if (prey.timeSpentInStomach % digestionTickFrameRate == 0)
					{
						switch (prey.Type)
						{
							case PreyType.Player:
								Player preyPlayer = prey.Instance as Player;
								bool shouldDigestPlayer = true;
								if (shouldDigestPlayer)
								{
									prey.NoHealth = preyPlayer.AsFood().TakeDigestionDamage(pred, digestionDamage);
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyPlayer.name);
									if (prey.NoHealth)
									{
										if (pred.AsPred().OnDigestionKill is not null)
											pred.AsPred().OnDigestionKill.Invoke(pred, prey);
										PlayDigestionBelch(pred, prey);
									}
								}
								else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
									Main.NewText("Failed to deal digestion damage to prey: " + preyPlayer.name);
								break;
							case PreyType.NPC:
								NPC preyNPC = prey.Instance as NPC;
								bool shouldDigestNPC = true;
								if (shouldDigestNPC)
								{
									if (preyNPC.type == NPCID.HallowBoss && ModContent.GetInstance<V2ServerConfig>().EasilyEdibleEmpress)
										digestionDamage *= 20.0;
									prey.NoHealth = PreyNPC.TakeDigestionDamage(preyNPC, pred, digestionDamage);
									preyNPC.netUpdate = true;
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyNPC.GivenOrTypeName);
									else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyNPC.GivenOrTypeName);
									if (prey.NoHealth)
									{
										if (pred.AsPred().OnDigestionKill is not null)
											pred.AsPred().OnDigestionKill.Invoke(pred, prey);
										PlayDigestionBelch(pred, prey);
									}
								}
								break;
							case PreyType.Projectile:
								Projectile preyProjectile = prey.Instance as Projectile;
								bool shouldDigestProjectile = true;
								if (shouldDigestProjectile)
								{
									prey.NoHealth = preyProjectile.TakeDigestionDamage(pred, digestionDamage);
									preyProjectile.netUpdate = true;
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyProjectile.Name);
									else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyProjectile.Name);
									if (prey.NoHealth)
									{
										if (pred.AsPred().OnDigestionKill is not null)
											pred.AsPred().OnDigestionKill.Invoke(pred, prey);
										PlayDigestionBelch(pred, prey);
									}
								}
								break;
							case PreyType.Item:
								Item preyItem = prey.Instance as Item;
								if (preyItem.IsAir)
									break;

								bool shouldDigestItem = true;
								if (shouldDigestItem)
								{
									prey.NoHealth = preyItem.TakeDigestionDamage(pred, digestionDamage);
									if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Successfully dealt digestion damage to prey: " + preyItem.Name);
									else if (ModContent.GetInstance<V2ServerConfig>().DebugChatMessages)
										Main.NewText("Failed to deal digestion damage to prey: " + preyItem.Name);
									if (prey.NoHealth)
									{
										if (pred.AsPred().OnDigestionKill is not null)
											pred.AsPred().OnDigestionKill.Invoke(pred, prey);
										PlayDigestionBelch(pred, prey);
									}
								}
								break;
						}
					}
				}
				else
				{
					prey.UpdateInStomach?.Invoke(null, pred, true);

					if (pred.AsPred().GetPreyAbsorptionRate is null)
						break;

					double digestedWeightPerTick = pred.AsPred().GetPreyAbsorptionRate.Invoke(pred) / (double)GetStomachTracker(pred).Prey.Count;
					if (prey.WeightLeftToDigest <= digestedWeightPerTick)
					{
						pred.AsPred().ExtraWeight += prey.WeightLeftToDigest * prey.CalorieMultiplier * pred.AsPred().WeightGainRatio;
						pred.AsPred().FoodAbsorbed += prey.WeightLeftToDigest * prey.CalorieMultiplier;

						prey.WeightLeftToDigest = 0;
					}
					else
					{
						pred.AsPred().ExtraWeight += digestedWeightPerTick * prey.CalorieMultiplier * pred.AsPred().WeightGainRatio;
						pred.AsPred().FoodAbsorbed += digestedWeightPerTick * prey.CalorieMultiplier;

						prey.WeightLeftToDigest -= digestedWeightPerTick;
					}
				}
			}

			if (pred.CurrentCaptor() is null && pred.AsPred().GetVisualBellySize is not null)
			{
				bool stomachNoisesPlaying = SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out ActiveSound stomachNoises);
				if (!stomachNoisesPlaying)
				{
					pred.AsPred().ActiveStomachNoises = SoundEngine.PlaySound(
						(V2.GetFooled
							? StomachNoises.AprilFools
							: StomachNoises.Muffled) with
						{ Volume = 0.25f + (0.15f * pred.AsPred().GetVisualBellySize.Invoke(pred)) },
						pred.TrueCenter()
					);
					SoundEngine.TryGetActiveSound(pred.AsPred().ActiveStomachNoises, out stomachNoises);
				}

				if (stomachNoises is null)
					return;

				stomachNoises.Position = pred.TrueCenter();
				stomachNoises.Volume = 0.25f;
				stomachNoises.Volume += 0.15f * pred.AsPred().GetVisualBellySize.Invoke(pred);
			}
		}

		public static void AddNewPrey(Projectile pred, PreyData prey)
		{
			if (GetStomachTracker(pred) is null)
				VoreTracker.NewTracker(pred, new List<PreyData>() { prey });
			else
				GetStomachTracker(pred).QueueNewPrey(prey);
		}

		public static NetworkText GetDigestedPlayerDeathReason(Projectile projectile, Player player)
		{
			List<string> deathMessageKeyList = [
				"Mods.V2.Death.DigestedPlayer.Universal.1",
				"Mods.V2.Death.DigestedPlayer.Universal.2",
				"Mods.V2.Death.DigestedPlayer.Universal.3",
				"Mods.V2.Death.DigestedPlayer.Universal.4",
				"Mods.V2.Death.DigestedPlayer.Universal.5",
				"Mods.V2.Death.DigestedPlayer.Universal.6",
				"Mods.V2.Death.DigestedPlayer.Universal.7",
				"Mods.V2.Death.DigestedPlayer.Universal.8",
				"Mods.V2.Death.DigestedPlayer.Universal.9",
				"Mods.V2.Death.DigestedPlayer.Universal.10",
				"Mods.V2.Death.DigestedPlayer.Universal.11",
				"Mods.V2.Death.DigestedPlayer.Universal.12",
				"Mods.V2.Death.DigestedPlayer.Universal.13",
				"Mods.V2.Death.DigestedPlayer.Universal.14",
				"Mods.V2.Death.DigestedPlayer.Universal.15",
				"Mods.V2.Death.DigestedPlayer.Universal.16",
				"Mods.V2.Death.DigestedPlayer.Universal.17",
				"Mods.V2.Death.DigestedPlayer.Universal.18",
				"Mods.V2.Death.DigestedPlayer.Universal.19",
				"Mods.V2.Death.DigestedPlayer.Universal.20",
				"Mods.V2.Death.DigestedPlayer.Universal.21",
				"Mods.V2.Death.DigestedPlayer.Universal.22",
			];
			switch (projectile.AsPred().DigestionType)
			{
				case EntityDigestionType.Acidic:
					deathMessageKeyList.AddRange([
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.1",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.2",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.3",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.4",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.5",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.6",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.7",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.8",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.9",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Acidic.10",
					]);
					break;
				case EntityDigestionType.Thermal:
					deathMessageKeyList.AddRange([
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.1",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.2",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.3",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.4",
						"Mods.V2.Death.DigestedPlayer.SpecificDigestionType.Thermal.5",
					]);
					break;
				case EntityDigestionType.Other:
					break;
			}

			if (projectile.AsPred().GetAdditionalDigestedPlayerMessages is not null)
				projectile.AsPred().GetAdditionalDigestedPlayerMessages.Invoke(projectile, player, deathMessageKeyList);
			string finalDeathReasonKey = Main.rand.NextFromCollection(deathMessageKeyList);

			return NetworkText.FromKey(
				finalDeathReasonKey,
				player.name,
				projectile.Name
			);
		}

		public override bool PreKill(Projectile projectile, int timeLeft)
		{
			if (GetStomachTracker(projectile) is not null)
			{
				if (projectile.CurrentCaptor() is not null)
				{
					foreach (PreyData prey in GetStomachTracker(projectile).Prey)
					{
						projectile.CurrentCaptor().QueueNewPrey(prey);
					}
				}
				GetStomachTracker(projectile).Prey.Clear();
			}
			
			return true;
		}

		/// <summary>
		/// Calculates the current weight of the given predator's stomach, based on all the prey inside of it.<br/>
		/// Used primarily in conjunction with <see cref="MaxStomachCapacity"/> to safeguard against overeating.<br/>
		/// </summary>
		/// <param name="pred">The predator whose stomach is to be weighed.</param>
		/// <returns>The current total weight of the given predator's stomach.</returns>
		public static double GetCurrentBellyWeight(Projectile pred)
		{
			if (GetStomachTracker(pred) is null)
				return 0.0;

			double totalBellyWeight = 0.0;
			foreach (PreyData prey in GetStomachTracker(pred).Prey)
			{
				totalBellyWeight += prey.WeightLeftToDigest;
				if (prey.NoHealth)
					continue;

				switch (prey.Type)
				{
					case PreyType.Player:
						Player preyPredPlayer = prey.Instance as Player;
						totalBellyWeight += preyPredPlayer.AsPred().StomachWeight;
						break;
					case PreyType.NPC:
						NPC preyPredNPC = prey.Instance as NPC;
						totalBellyWeight += PredNPC.GetCurrentBellyWeight(preyPredNPC);
						break;
					case PreyType.Projectile:
						Projectile preyPredProjectile = prey.Instance as Projectile;
						totalBellyWeight += GetCurrentBellyWeight(preyPredProjectile);
						break;
				}
			}
			return totalBellyWeight;
		}

		public static bool AnyPreyStillAlive(Projectile pred)
		{
			if (GetStomachTracker(pred) is not null)
			{
				foreach (PreyData prey in GetStomachTracker(pred).Prey)
				{
					if (!prey.NoHealth)
						return true;
				}
			}
			return false;
		}
	}
}
