using System.Collections.Generic;
using System.Linq;
using ItemRecovery.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ItemRecovery.Events
{
    public class DeathEvents
    {
        private static IModHelper helper;
        private static IMonitor monitor;

        private static Dictionary<long, int> _DaysSinceLastDeath;

        public DeathEvents(IModHelper ihelper, IMonitor imonitor)
        {
            helper = ihelper;
            monitor = imonitor;
            
            // helper.Events.GameLoop.DayEnding += OnGameSaving;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private static void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            Farmer farmer = e.Player;

            if (farmer.itemsLostLastDeath.Count < 1)
                return;
            
            if (ListHelper.SortListsThenCompareItemNames(farmer.itemsLostLastDeath.ToList(), e.Removed.ToList()))
            {
                DataHelper.ResetPlayerDSLD("ItemRecoveryData", farmer.UniqueMultiplayerID, helper);
            }
        }
        
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            
            DataHelper.AdvanceAllPlayerDSLD("ItemRecoveryData", helper);
        }
        
        // private void OnGameSaving(object sender, DayEndingEventArgs e)
        // {
        //     Farmer main_farmer = (Farmer)sender;
        //     if (Game1.IsMultiplayer && !main_farmer.IsMainPlayer)
        //         return;
        //     
        //     helper.Data.WriteSaveData("ItemRecoveryData", _DaysSinceLastDeath);
        // }

        // private static void OnGameSaving(object sender, SavingEventArgs e)
        // {
        //     Farmer main_farmer = (Farmer)sender;
        //     if (!main_farmer.IsMainPlayer)
        //         return;
        //     
        //     ModData data = helper.Data.ReadSaveData<ModData>("ItemRecoveryData");
        //     Dictionary<long, int> DaysSinceLastDeath = data == null ? new Dictionary<long, int>() : data.DaysSinceLastDeath;
        //     
        //     foreach (Farmer farmer in Game1.getAllFarmers())
        //     {
        //         long multiplayer_id = farmer.UniqueMultiplayerID;
        //         
        //         if (!DaysSinceLastDeath.ContainsKey(multiplayer_id))
        //         {
        //             DaysSinceLastDeath.Add(multiplayer_id, 0);
        //         }
        //         
        //         if (farmer.itemsLostLastDeath.Count > 0)
        //         {
        //             DaysSinceLastDeath[multiplayer_id]++;
        //         }
        //         else
        //         {
        //             DaysSinceLastDeath[multiplayer_id] = 0;
        //         }
        //     }
        //     
        //     helper.Data.WriteSaveData("ItemRecoveryData", data);
        // }
    }
}