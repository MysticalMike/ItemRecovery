using StardewModdingAPI;

namespace ItemRecovery
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            double CostMultiplier = this.Config.CostMultiplier;
            new EmulatedShopMenu(helper, this.Monitor, CostMultiplier);
        }
    }
}