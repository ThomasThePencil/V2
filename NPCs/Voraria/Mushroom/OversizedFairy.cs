using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.Items.Voraria.Consumables;
using V2.Projectiles.Voraria.Weapons.Summon.ShroomFairy;
using V2.Sounds.Vore;

namespace V2.NPCs.Voraria.Mushroom
{
	public class OversizedFairy : ModNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override string Texture => "V2/NPCs/Voraria/Mushroom/FATFUCK";
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "V2/NPCs/Voraria/Mushroom/FATFUCK",
				Position = new Vector2(-8, 8),
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.dontTakeDamageFromHostiles = true;
			NPC.width = 154;
			NPC.height = 66;
			NPC.aiStyle = -1;
			NPC.damage = 0;
			NPC.defense = 45;
			NPC.lifeMax = 14000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 37500f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.AsV2NPC().CanBeDamagedByFallingPeople = false;
			NPC.AsV2NPC().Gender = EntityGender.Female;
			NPC.AsFood().DefinedBaseSize = 50.5;
			NPC.AsPred().WeightGainRatio = 0;
			NPC.AsPred().MaxStomachCapacity = 25;
			NPC.AsPred().BaseStomachacheMeterCapacity = 1750.0;
			NPC.AsFood().WellFedPower = 0.33;

			NPC.AsPred().DigestionType = EntityDigestionType.Acidic;
			NPC.AsPred().GetDigestionTickDamage = GetDigestionTickDamage;
			NPC.AsPred().GetDigestionTickRate = GetDigestionTickRate;

			NPC.AsPred().GetPreyAbsorptionRate = GetPreyAbsorptionRate;

			NPC.AsPred().GetVisualBellySize = GetVisualBellySize;
			NPC.AsPred().GetVisualWeightStage = GetVisualWeightStage;

			NPC.AsPred().CanBeForceFed = CanFatassFairyBeForceFed;

			NPC.AsPred().SmallBurps = Burps.Humanoid.Small;
			NPC.AsPred().SmallBurpThreshold = 0.35;
			NPC.AsPred().StandardBurps = Burps.Humanoid.Standard;
			NPC.AsPred().SmallGulps = Gulps.Short;
			NPC.AsPred().SmallGulpThreshold = 0.35;
			NPC.AsPred().BigGulps = Gulps.Standard;

		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.SpawnTileType != TileID.MushroomGrass)
				return 0f;

			return 0.015f;
		}
		public override void OnSpawn(IEntitySource source)
		{
			NPC.direction = Main.rand.NextBool().ToDirectionInt();
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundMushroom,
				new FlavorTextBestiaryInfoElement("Mods.V2.Bestiary.Mushroom.OversizedFairy"),
			});
		}
		public static bool CanFatassFairyBeForceFed(NPC npc) => true;
		public static double GetDigestionTickDamage(NPC npc, PreyData prey) => ShroomFairyStuff.DigestDamage * 2;
		public static double GetDigestionTickRate(NPC npc, PreyData prey) => ShroomFairyStuff.DigestRate;
		public static double GetPreyAbsorptionRate(NPC npc) => ShroomFairyStuff.AbsorbRate * 12;
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}
		public static int GetVisualBellySize(NPC npc) => 0;

		public static int GetVisualWeightStage(NPC npc) => 0;

		public override void AI()
		{
			NPC.velocity.X *= 0.9f;
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.3f, 10f);
			FatFuckMethods.OnUpdate(NPC);
			FatFuckMethods.PushPlayers(NPC);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Rectangle sourceRect = new Rectangle(0, 0, 170, 82);
			Texture2D sprite = ModContent.Request<Texture2D>("V2/NPCs/Voraria/Mushroom/FATFUCK").Value;
			spriteBatch.Draw(sprite, NPC.position - Main.screenPosition - new Vector2(8, 16), sourceRect, drawColor, NPC.rotation, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
			return false;
		}
	}
}
