using System.Collections.Generic;
using ItemRecovery.Events;
using ItemRecovery.Util;
using StardewModdingAPI;
using StardewValley;

namespace ItemRecovery
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            double CostMultiplier = Config.CostMultiplier;
            int DaysTillRecoverable = Config.DaysTillRecoverable;

            new DeathEvents(helper, Monitor);
            new ShopEvents(helper, Monitor, CostMultiplier);
            new EmulatedShopMenu(helper);

            helper.ConsoleCommands.Add("get_dsld", "", this.GetDSLD);
        }

        private void GetDSLD(string command, string[] args)
        {
            ModData data = this.Helper.Data.ReadSaveData<ModData>("ItemRecoveryData") ?? new ModData();
            Dictionary<long, int> DaysSinceLastDeath = data.DaysSinceLastDeath ?? new Dictionary<long, int>();
            
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                long multiplayer_id = farmer.UniqueMultiplayerID;
                
                if (!DaysSinceLastDeath.ContainsKey(multiplayer_id))
                {
                    DaysSinceLastDeath.Add(multiplayer_id, 0);
                }
                
                Monitor.Log($"Name: {farmer.Name} Count: {DaysSinceLastDeath[multiplayer_id]}", LogLevel.Info);
            }
        }
    }
}