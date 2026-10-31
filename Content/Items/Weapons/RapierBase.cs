using MoreElegance.Common.DamageClasses;
using MoreElegance.Content.Projectiles.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreElegance.Content.Items.Weapons
{
    /// <summary>
    /// Foundational fencing weapon of the Elegant Melee subclass.
    /// Fast, precise thrusting blade.
    /// </summary>
    public class RapierBase : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = ModContent.GetInstance<ElegantMeleeDamageClass>();
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true; // Projectile handles weapon collision
            Item.noUseGraphic = true; // Projectile handles visuals during thrust
            Item.shoot = ModContent.ProjectileType<ProjRapierBase>();
            Item.shootSpeed = 2.5f;
        }

        public override bool CanUseItem(Player player)
        {
            // Only allow one thrust at a time
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 4)
                .AddIngredient(ItemID.BorealWood, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
