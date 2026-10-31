using Microsoft.Xna.Framework;
using MoreElegance.Common.DamageClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreElegance.Content.Projectiles.Weapons
{
    /// <summary>
    /// Fast thrusting projectile for RapierBase.
    /// Pierces towards the cursor and rapidly returns to the player.
    /// </summary>
    public class ProjRapierBase : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1; // Custom thrusting AI
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1.15f;
            Projectile.DamageType = ModContent.GetInstance<ElegantMeleeDamageClass>();
            Projectile.ownerHitCheck = true;

            // Ensures each thrust hits targets cleanly once per strike
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Abort if the player is dead or cannot use items
            if (!player.active || player.dead || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }

            // Sync lifecycle with player's item animation
            int duration = player.itemAnimationMax;
            if (duration <= 0)
                duration = 15;

            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;

            // Calculate progress (0f = start of thrust, 1f = end)
            float progress = 1f - (float)player.itemAnimation / duration;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            // Lightning-fast thrust curve: accelerates quickly forward, pauses momentarily, then snaps back
            const float maxReach = 58f;
            float extension;
            if (progress < 0.45f)
            {
                // Quick lunging thrust
                extension = MathHelper.SmoothStep(0f, maxReach, progress / 0.45f);
            }
            else
            {
                // Swift, crisp recovery
                extension = MathHelper.SmoothStep(maxReach, 0f, (progress - 0.45f) / 0.55f);
            }

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.MountedCenter + direction * (18f + extension);

            // 45-degree angle offset for diagonal sprite orientation
            Projectile.rotation = direction.ToRotation() + MathHelper.PiOver4;

            // Align player facing and arm direction
            player.direction = direction.X >= 0 ? 1 : -1;
            player.itemRotation = (direction * player.direction).ToRotation();

            // Subtle silver blade sparks along the thrust path
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.SilverCoin,
                    direction.X * 2.5f,
                    direction.Y * 2.5f,
                    120,
                    default,
                    0.85f
                );
                dust.noGravity = true;
                dust.velocity = direction * 1.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Crisp metallic parry / thrust impact sound
            SoundEngineSoundEffect();

            // Burst of fencing sparks on impact
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Smoke, direction.X, direction.Y, 150, default, 0.9f);
                d.velocity = direction.RotatedByRandom(0.4f) * Main.rand.NextFloat(2f, 4f);
            }
        }

        private void SoundEngineSoundEffect()
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }
    }
}
