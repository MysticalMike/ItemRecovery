using ItemRecovery.Events;
using ItemRecovery.Util;
using StardewModdingAPI;

namespace ItemRecovery
{
    public class ModEntry : Mod
    {
        public static ModConfig Config;
        private static IMonitor _monitor;
        
        // test
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            _monitor = Monitor;
            
            new DebugCommands(Monitor, helper, Config.DaysTillRecoverable);

            new DeathEvents(helper, Monitor);
            new ShopEvents(helper, Monitor, Config.CostMultiplier);
            new ShopHelper(helper, Config.DaysTillRecoverable);

            helper.ConsoleCommands.Add("ir", "Item recovery command prefix", DebugCommands.GetIR);
        }

        public static void Log(string msg)
        {
            _monitor.Log(msg, LogLevel.Info);
        }
    }
}