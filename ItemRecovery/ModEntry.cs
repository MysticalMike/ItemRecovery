using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

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
            // helper.Events.GameLoop.Saving += this.OnGameSaving;
            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        }

        public void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            Farmer farmer = e.Player;

            if (farmer.itemsLostLastDeath.Count < 1)
                return;
            
            if (SortThenCompareListsItemNames(farmer.itemsLostLastDeath.ToList(), e.Removed.ToList()))
            {
                Monitor.Log("Player just died.", LogLevel.Debug);
            }
            else
            {
                Monitor.Log("LostItems: " + ListNames(farmer.itemsLostLastDeath.ToList()), LogLevel.Debug);
                Monitor.Log("ItemsRemoved: " + ListNames(e.Removed.ToList()), LogLevel.Debug);
            }
        }

        public string ListNames(List<Item> list)
        {
            list.Sort();
            List<string> names = new List<string>();

            foreach (var item in list)
            {
                names.Add(item.DisplayName);
            }

            return String.Join(", ", names);
        }

        public bool SortThenCompareListsItemNames(List<Item> list1, List<Item> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            
            list1.Sort();
            list2.Sort();
            
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Name.Equals(list2[i].Name))
                    return false;
            }

            return true;
        }

        private void OnGameSaving(object sender, SavingEventArgs e)
        {
            Farmer main_farmer = (Farmer)sender;
            if (!main_farmer.IsMainPlayer)
                return;
            
            ModData data = this.Helper.Data.ReadSaveData<ModData>("ItemRecoveryData");
            Dictionary<long, int> DaysSinceLastDeath = data == null ? new Dictionary<long, int>() : data.DaysSinceLastDeath;
            
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                long multiplayer_id = farmer.UniqueMultiplayerID;
                
                if (!DaysSinceLastDeath.ContainsKey(multiplayer_id))
                {
                    DaysSinceLastDeath.Add(multiplayer_id, 0);
                }
                
                if (farmer.itemsLostLastDeath.Count > 0)
                {
                    DaysSinceLastDeath[multiplayer_id]++;
                }
                else
                {
                    DaysSinceLastDeath[multiplayer_id] = 0;
                }
            }
            
            this.Helper.Data.WriteSaveData("ItemRecoveryData", data);
        }
    }
}