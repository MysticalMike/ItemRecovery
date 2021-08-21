using StardewModdingAPI;

namespace ItemRecovery
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            new EmulatedShopMenu(helper);
        }
    }
}