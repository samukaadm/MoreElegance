using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreElegance.Content.Tiles
{
    /// <summary>
    /// An unbreakable, reinforced block used for fencing chambers and combat training grounds.
    /// Completely indestructible by pickaxes, hammers, and explosives.
    /// </summary>
    public class TrainingTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;

            // Enforce extreme pickaxe requirement: impossible to break with standard tools
            MinPick = 9999;
            DustType = DustID.Platinum;
            HitSound = SoundID.Tink;

            // Map entry color: pristine reinforced platinum-steel
            AddMapEntry(new Color(175, 195, 215), CreateMapEntryName());
        }

        public override bool CanExplode(int i, int j)
        {
            // Explosives cannot damage or destroy this tile
            return false;
        }

        public override bool CanKillTile(int i, int j, ref bool blockSave)
        {
            // Completely prevent tile destruction
            return false;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            // Always fail mining attempts
            fail = true;
            effectOnly = true;
        }
    }
}
