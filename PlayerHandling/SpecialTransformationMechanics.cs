using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core;
using V2.NPCs;

namespace V2.PlayerHandling
{
	public class OllieDamageRampUp : GlobalNPC
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			Player attacker = Main.player[projectile.owner];
			if (!attacker.AsV2Player().OllieTransformation) return;
			if (modifiers.DamageType != DamageClass.Ranged) return;
			if (ProjectileID.Sets.CultistIsResistantTo[projectile.type]) return;
			int distance = (int)Math.Floor(attacker.Center.Distance(npc.Center));

			float multiplier = Math.Clamp(0.67f + distance / 667f, 1.0f, 1.75f);
			Main.NewText(multiplier.ToString());

			modifiers.FinalDamage = modifiers.FinalDamage * multiplier;
		}
	}
}
