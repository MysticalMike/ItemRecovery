using ItemRecovery.Data;
using StardewModdingAPI;
using StardewValley;

namespace ItemRecovery.Util
{
    public class ModDataHelper
    {
        private static readonly string mod_data_key = "ItemRecoveryData";
        
        private static ModData GetModData(IModHelper helper)
        {
            return helper.Data.ReadSaveData<ModData>(mod_data_key) ?? new ModData();
        }

        private static void SaveModData(IModHelper helper, ModData data)
        {
            helper.Data.WriteSaveData(mod_data_key, data);
        }

        private static void CreateIfNoKey(long id, IModHelper helper)
        {
            ModData mod_data = GetModData(helper);
            if (!mod_data.DaysSinceLastDeath.ContainsKey(id))
                mod_data.DaysSinceLastDeath[id] = 0;
            SaveModData(helper, mod_data);
        }
        
        public static void ResetPlayerDSLD(long id, IModHelper helper)
        {
            ModData mod_data = GetModData(helper);
            mod_data.DaysSinceLastDeath[id] = 0;
            SaveModData(helper, mod_data);
        }

        private static void AdvancePlayerDSLD(long id, IModHelper helper)
        {
            CreateIfNoKey(id, helper);
            ModData mod_data = GetModData(helper);
            mod_data.DaysSinceLastDeath[id]++;
            SaveModData(helper, mod_data);
        }

        public static void AdvanceAllPlayerDSLD(IModHelper helper)
        {
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                AdvancePlayerDSLD(farmer.UniqueMultiplayerID, helper);
            }
        }

        public static void GetAllPlayerDSLD(IModHelper helper, IMonitor monitor)
        {
            ModData mod_data = GetModData(helper);
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                monitor.Log($"Name: {farmer.Name} Count: {mod_data.DaysSinceLastDeath[farmer.UniqueMultiplayerID]}", LogLevel.Info);
            }
        }
        
        public static int GetPlayerDSLD(long id, IModHelper helper)
        {
            ModData mod_data = GetModData(helper);
            return mod_data.DaysSinceLastDeath[id];
        }
    }
}