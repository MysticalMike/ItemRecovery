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

        public DeathEvents(IModHelper ihelper, IMonitor imonitor)
        {
            helper = ihelper;
            monitor = imonitor;
            
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
                ModDataHelper.ResetPlayerDSLD(farmer.UniqueMultiplayerID, helper);
            }
        }
        
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            
            ModDataHelper.AdvanceAllPlayerDSLD(helper);
        }
    }
}