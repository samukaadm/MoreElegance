using System.Collections.Generic;
using MoreElegance.Common.DamageClasses;
using MoreElegance.Content.Projectiles.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreElegance.Content.Items.Weapons
{
    /// <summary>
    /// Calamity Mod Cross-Integration Weapon.
    /// Infused with Aerialite Bars, granting high attack speed and aerial slipstream strikes.
    /// Only craftable when the Calamity Mod is installed and active.
    /// </summary>
    public class AerialiteRapier : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = ModContent.GetInstance<ElegantMeleeDamageClass>();
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ProjAerialiteRapier>();
            Item.shootSpeed = 3.2f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                tooltips.Add(new TooltipLine(Mod, "CalamityNotice", "[Calamity Integration Active] Infused with Aerialite. Releases cutting wind currents."));
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "CalamityNotice", "[Calamity Exclusive] Crafting this item requires CalamityMod to be active."));
            }
        }

        public override void AddRecipes()
        {
            // Weakly resolve AerialiteBar from CalamityMod without compile-time hard dependency
            if (ModLoader.TryFind("CalamityMod", "AerialiteBar", out ModItem aerialiteBar))
            {
                CreateRecipe()
                    .AddIngredient(aerialiteBar.Type, 6)
                    .AddIngredient(ItemID.SunplateBlock, 5)
                    .AddIngredient(ItemID.Feather, 3)
                    .AddIngredient(ModContent.ItemType<RapierBase>(), 1)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }
    }
}
