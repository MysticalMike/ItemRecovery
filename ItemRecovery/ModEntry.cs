using ItemRecovery.Events;
using ItemRecovery.Util;
using StardewModdingAPI;

namespace ItemRecovery
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            new DeathEvents(helper, Monitor);
            new ShopEvents(helper, Monitor, Config.CostMultiplier);
            new ShopHelper(helper, Config.DaysTillRecoverable);

            helper.ConsoleCommands.Add("get_dsld", "Get days since last death", this.GetDSLD);
        }

        private void GetDSLD(string command, string[] args)
        {
            ModDataHelper.GetAllPlayerDSLD(Helper, Monitor);
        }
    }
}