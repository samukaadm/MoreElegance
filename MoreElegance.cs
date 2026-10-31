using Terraria.ModLoader;

namespace MoreElegance
{
    public class MoreElegance : Mod
    {
        public static MoreElegance Instance { get; private set; } = null!;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null!;
        }
    }
}
