using ItemRecovery.Events;
using ItemRecovery.Util;
using StardewModdingAPI;

namespace ItemRecovery
{
    public class ModEntry : Mod
    {
        public static ModConfig Config;
        
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            
            new DebugCommands(Monitor, helper, Config.DaysTillRecoverable);

            new DeathEvents(helper, Monitor);
            new ShopEvents(helper, Monitor, Config.CostMultiplier);
            new ShopHelper(helper, Config.DaysTillRecoverable);

            helper.ConsoleCommands.Add("ir", "Item recovery command prefix", DebugCommands.GetIR);
        }
    }
}