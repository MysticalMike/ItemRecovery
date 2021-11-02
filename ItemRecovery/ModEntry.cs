using ItemRecovery.Events;
using ItemRecovery.Util;
using StardewModdingAPI;

namespace ItemRecovery
{
    public class ModEntry : Mod
    {
        private const string mod_data_key = "ItemRecoveryData";

        public static ModConfig config;
        private static IMonitor _monitor;

        private static ModDataManager manager;
        
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            _monitor = Monitor;

            manager = new ModDataManager(helper, ModManifest);

            new ModDataHelper(mod_data_key, helper);
            new DebugCommands(_monitor, helper, config.DaysTillRecoverable);

            new MessageEvents(helper, ModManifest);
            new DeathEvents(helper);
            new ShopEvents(helper, _monitor, config.CostMultiplier);
            new ShopHelper(helper, config.DaysTillRecoverable);

            helper.ConsoleCommands.Add("ir", "Item recovery command prefix", DebugCommands.GetIR);
        }

        public static void Log(string msg)
        {
            _monitor.Log(msg, LogLevel.Info);
        }

        public static ModDataManager GetManager()
        {
            return manager;
        }
    }
}