using System;
using ItemRecovery.Events;
using ItemRecovery.Util;
using StardewModdingAPI;
using StardewValley;

namespace ItemRecovery
{
    public class DebugCommands
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static int _days_till_recoverable;

        public DebugCommands(IMonitor monitor, IModHelper helper, int days_till_recoverable)
        {
            _monitor = monitor;
            _helper = helper;
            _days_till_recoverable = days_till_recoverable;
        }

        public static void GetIR(string command, string[] args)
        {
            if (args.Length < 1)
            {
                _monitor.Log("Unknown command argument. Try 'ir debug'", LogLevel.Error);
                return;
            }

            switch (args[0].ToLower())
            {
                case "debug":
                {
                    string line = "".PadRight(57, '-');
                    _monitor.Log(line, LogLevel.Info);
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        string name = farmer.Name.PadRight(10,' ');
                        string last_death = ModDataHelper.GetPlayerDSLD(farmer.UniqueMultiplayerID, _helper).ToString().PadRight(4, ' ');
                        string till_recoverable = _days_till_recoverable.ToString().PadRight(3, ' ');
                        
                        _monitor.Log($"Player: {name} | LastDeath: {last_death} | RecoverTime: {till_recoverable} |", LogLevel.Info);
                    }
                    _monitor.Log(line, LogLevel.Info);
                    break;
                }
                case "set_rt":
                {
                    if (!HasRequiredArgs(args, 2, "ir set_rt <int>"))
                        return;

                    if (!IsInt(args[1], "ir set_rt <int>"))
                        return;

                    int rt = int.Parse(args[1]);
                    
                    ShopHelper.days_till_recoverable = rt;
                    ModEntry.Config.DaysTillRecoverable = rt;
                    _helper.WriteConfig(ModEntry.Config);
                    _days_till_recoverable = rt;
                    
                    _monitor.Log($"Days till recoverable set to {rt}", LogLevel.Info);
                    break;
                }
                case "set_cm":
                {
                    if (!HasRequiredArgs(args, 2, "ir set_cm <int>"))
                        return;

                    if (!IsDouble(args[1], "ir set_cm <int>"))
                        return;

                    double cm = double.Parse(args[1]);

                    ShopEvents.CostMultiplier = cm;
                    ModEntry.Config.CostMultiplier = cm;
                    _helper.WriteConfig(ModEntry.Config);
                    
                    _monitor.Log($"Cost multiplier set to {cm}", LogLevel.Info);
                    break;
                }
                default:
                {
                    _monitor.Log("Unknown command argument. Try 'ir debug'", LogLevel.Error);
                    break;
                }
            }
        }

        private static bool HasRequiredArgs(string[] args, int length, string msg)
        {
            if (args.Length < length)
            {
                _monitor.Log($"Error. Usage: '{msg}'", LogLevel.Error);
                return false;
            }
            return true;
        }
        
        private static bool IsInt(string arg, string msg)
        {
            try
            {
                int i = int.Parse(arg);
            }
            catch (Exception e)
            {
                _monitor.Log($"Error. Usage: '{msg}'", LogLevel.Error);
                return false;
            }
            return true;
        }
        
        private static bool IsDouble(string arg, string msg)
        {
            try
            {
                double i = double.Parse(arg);
            }
            catch (Exception e)
            {
                _monitor.Log($"Error. Usage: '{msg}'", LogLevel.Error);
                return false;
            }
            return true;
        }
    }
}