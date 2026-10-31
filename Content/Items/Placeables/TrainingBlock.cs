using MoreElegance.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreElegance.Content.Items.Placeables
{
    /// <summary>
    /// Item form of the TrainingTile. Allows building indestructible training arenas.
    /// </summary>
    public class TrainingBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TrainingTile>());
            Item.width = 16;
            Item.height = 16;
            Item.value = Item.buyPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
