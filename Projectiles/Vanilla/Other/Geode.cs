using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using V2.Core.MainDetours;
using V2.PlayerHandling;

namespace V2.Projectiles.Vanilla.Other;

public class Geode : GlobalProjectile
{
	public override bool IsLoadingEnabled(Mod mod) => !V2.BasicMode;
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type is ProjectileID.Geode;

    public override void SetDefaults(Projectile entity)
    {
        entity.AsFood().DefinedSize = 0.4d;
        entity.AsFood().MaxHealth = 960;
        
        entity.AsFood().OnKilledByDigestion += OnKilledByDigestion;
    }

    private static void OnKilledByDigestion(Projectile projectile, Entity pred)
    {
        if (pred is Player predPlayer)
        {
            // TODO: this probably needs a better implementation 
            predPlayer.AsPred().LootRecentlyDigested = true;
            Projectile.DropGeodeLoot(pred);
            predPlayer.AsPred().LootRecentlyDigested = false;
        }
    }
}