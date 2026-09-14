using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.NPCs;
using V2.PlayerHandling;
using V2.Projectiles;
using V2.Tiles.Voraria.Paintings;

namespace V2.Items.Vanilla.Weapons.Ranged.Ammo
{
	public class MusketBall : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.MusketBall;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 90;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 1;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 7;
				float knockback = 2f;
				float velocity = 1f; 
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X * velocity, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.Bullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class MeteorShot : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.MeteorShot;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 175;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 8;
				float knockback = 1f;
				float velocity = 0.75f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.MeteorShot, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class SilverBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SilverBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 130;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 9;
				float knockback = 3f;
				float velocity = 1.125f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.SilverBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class TungstenBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.TungstenBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 139;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 9;
				float knockback = 4f;
				float velocity = 1.125f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.Bullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class CrystalBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CrystalBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 120;
			item.AsFood().Size = 0.008;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 9;
				float knockback = 1f;
				float velocity = 1.25f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.CrystalBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class CursedBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.CursedBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 210;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = -1;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 12;
				float knockback = 4f;
				float velocity = 1.25f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.CursedBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class IchorBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.IchorBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 210;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 1;
			item.AsFood().WellFedPower = -1;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 13;
				float knockback = 4f;
				float velocity = 1.3125f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.IchorBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class ChlorophyteBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.ChlorophyteBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 333;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 9;
				float knockback = 4.5f;
				float velocity = 1.25f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.ChlorophyteBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class HighVelocityBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.HighVelocityBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 190;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 1;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 11;
				float knockback = 4f;
				float velocity = 1f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.BulletHighVelocity, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class VenomBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.VenomBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 333;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 1;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 15;
				float knockback = 4.1f;
				float velocity = 1.325f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.VenomBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class PartyBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.PartyBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 125;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 1;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 10;
				float knockback = 5f;
				float velocity = 1.275f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.PartyBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class NanoBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.NanoBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 295;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 15;
				float knockback = 3.6f;
				float velocity = 1.15f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.NanoBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class ExplodingBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.ExplodingBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 295;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 10;
				float knockback = 6.6f;
				float velocity = 1.175f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.ExplosiveBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class GoldenBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.GoldenBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 230;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 15;
				float knockback = 3.6f;
				float velocity = 1.15f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.GoldenBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class LuminiteBullet : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.MoonlordBullet;
		public override void SetDefaults(Item item)
		{
			item.AsFood().MaxHealth = 230;
			item.AsFood().Size = 0.008;
			item.AsFood().AcidResistTier = 2;

			item.AsFood().OnRegurgitate += OnRegurgitate;
		}
		public static bool OnRegurgitate(Item item, Entity pred)
		{
			if (pred is Player predPlayer)
			{
				int Owner = predPlayer.whoAmI;
				int damage = 20;
				float knockback = 3f;
				float velocity = 0.5f;
				damage = (int)(predPlayer.GetTotalDamage(DamageClass.Ranged).ApplyTo(damage) + predPlayer.AsPred().GLP.Total / 1.5f);
				knockback = predPlayer.GetTotalKnockback(DamageClass.Ranged).ApplyTo(knockback) + predPlayer.AsPred().GLP.Total / 30f;
				int projectileID = Projectile.NewProjectile(pred.GetSource_FromThis(), item.position,
					new Vector2(item.velocity.X, Main.rand.Next(-100, 101) / 100f) * velocity, ProjectileID.MoonlordBullet, damage, knockback, Owner);
				return true;
			}
			return false;
		}
	}
	public class EndlessMusketPouch : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.EndlessMusketPouch;

		public override void SetDefaults(Item item)
		{
			item.AsFood().PreSwallow = PreSwallow;
		}

		public static bool PreSwallow(Item item, Entity pred)
		{
			Item eatenItem = new Item();
			eatenItem.SetDefaults(ItemID.MusketBall);
			eatenItem.stack = 1;
			if (pred is Player)
			{
				Player predPlayer = pred as Player;
				V2Utils.SummonItemHere(pred, pred.Center, ref eatenItem, out Item itemDrop);
				PredPlayer.Swallow(predPlayer, itemDrop, ForceSwallow: true);
			}
			else if (pred is NPC)
			{
				NPC predNPC = pred as NPC;
				V2Utils.SummonItemHere(pred, pred.Center, ref eatenItem, out Item itemDrop);
				PredNPC.Swallow(predNPC, itemDrop);
			}
			else if (pred is Projectile)
			{
				Projectile predProjectile = pred as Projectile;
				V2Utils.SummonItemHere(pred, pred.Center, ref eatenItem, out Item itemDrop);
				PredProjectile.Swallow(predProjectile, itemDrop);
			}
			return false;
		}
	}
}
