using Microsoft.Xna.Framework;
using MoreElegance.Common.DamageClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreElegance.Content.Projectiles.Weapons
{
    /// <summary>
    /// Thrusting projectile for the Aerialite Rapier.
    /// Trailed by sky-blue wind particles, firing a gale pulse at peak extension.
    /// </summary>
    public class ProjAerialiteRapier : ModProjectile
    {
        private bool hasFiredGale = false;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1.2f;
            Projectile.DamageType = ModContent.GetInstance<ElegantMeleeDamageClass>();
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }

            int duration = player.itemAnimationMax;
            if (duration <= 0)
                duration = 12;

            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;

            float progress = 1f - (float)player.itemAnimation / duration;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            const float maxReach = 68f;
            float extension;
            if (progress < 0.45f)
            {
                extension = MathHelper.SmoothStep(0f, maxReach, progress / 0.45f);
            }
            else
            {
                extension = MathHelper.SmoothStep(maxReach, 0f, (progress - 0.45f) / 0.55f);
            }

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.MountedCenter + direction * (20f + extension);
            Projectile.rotation = direction.ToRotation() + MathHelper.PiOver4;

            player.direction = direction.X >= 0 ? 1 : -1;
            player.itemRotation = (direction * player.direction).ToRotation();

            // Release aerial dust trail
            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Skyware,
                    direction.X * 3f,
                    direction.Y * 3f,
                    100,
                    Color.Cyan,
                    1.1f
                );
                dust.noGravity = true;
                dust.velocity = direction * 2.5f;
            }

            // At peak thrust extension, spawn a piercing aerial gust
            if (!hasFiredGale && progress >= 0.45f && Projectile.owner == Main.myPlayer)
            {
                hasFiredGale = true;
                Vector2 gustVelocity = direction * 10f;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    gustVelocity,
                    ProjectileID.BladeOfGrass,
                    (int)(Projectile.damage * 0.75f),
                    Projectile.knockBack * 0.5f,
                    Projectile.owner
                );
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Projectile.position);

            // Cloud/wind burst effect
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Cloud, 0f, 0f, 100, Color.LightCyan, 1.2f);
                d.velocity = Main.rand.NextVector2Circular(3f, 3f);
            }
        }
    }
}
