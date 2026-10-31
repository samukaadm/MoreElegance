using Terraria;
using Terraria.ModLoader;

namespace MoreElegance.Common.DamageClasses
{
    /// <summary>
    /// Custom damage class that inherits 100% of standard Melee stats, scaling, and combat effects.
    /// Represents the refined, swift fencing and rapier fighting style.
    /// </summary>
    public class ElegantMeleeDamageClass : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            // Inherit 100% of Damage, CritChance, AttackSpeed, ArmorPenetration, and Knockback
            // from both Generic bonuses (e.g. general damage accessories) and Melee bonuses (warrior emblems, melee armor).
            if (damageClass == DamageClass.Generic || damageClass == DamageClass.Melee)
            {
                return StatInheritanceData.Full;
            }

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            // Inherits Melee weapon effects such as flasks/imbues, Magma Stone, and autoswing effects.
            if (damageClass == DamageClass.Melee)
            {
                return true;
            }

            return false;
        }

        public override bool UseStandardCritCalcs => true;

        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            // Ensure standard damage, speed, crit, and knockback tooltips are rendered
            return true;
        }
    }
}
