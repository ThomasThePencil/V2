using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using V2.Core.MainDetours;
using V2.PlayerHandling;
using V2.PlayerHandling.PredPlayerGoals.Amateur;
using V2.PlayerHandling.PredPlayerGoals.Beginner;
using V2.Sounds.MuffledSounds;
using V2.Sounds.Vore;

namespace V2.Items.Vanilla.Placeables.Tiles
{
	public class Silt : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public static bool ExtractableBlockWasJustDigested { get; set; }
		private static MethodInfo Player_ExtractinatorUse => typeof(Player).GetMethod(
			"ExtractinatorUse",
			BindingFlags.NonPublic|BindingFlags.Instance,
			[typeof(int),typeof(int)]
			);
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => 
			entity.type is ItemID.SiltBlock or ItemID.SlushBlock;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 30;
			item.AsFood().Size = 0.1;
			item.AsFood().OnBreak += OnBreak;
		}
		public static bool OnBreak(Item item, Entity pred, bool direct)
		{
			SoundEngine.PlaySound(MuffledMiscSounds.Shatter, pred.Center);
			SoundEngine.PlaySound(StomachNoises.Muffled, pred.Center);

			for (int i = 0; i < item.stack; i++)
			{
				if (pred is Player playerPred)
				{
					int extractType = ItemID.Sets.ExtractinatorMode[item.type];
					V2Utils.MarkLootDigested(playerPred);
					Player_ExtractinatorUse.Invoke(playerPred, [extractType, TileID.Extractinator]);
				}
				else if (pred is NPC NPCPred)
				{

				}
			}
			return true;
		}
	}
	public class Fossil : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.DesertFossil;

		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 250;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().Size = 0.15;

			item.AsFood().OnBreak += Silt.OnBreak;
		}
	}
}
