using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace ItemRecovery.Util
{
    public class DataHelper
    {
        private static ModData GetModData(string key, IModHelper helper)
        {
            return helper.Data.ReadSaveData<ModData>(key) ?? new ModData();
        }
        
        // public static Dictionary<long, int> GetDSLDFromKey(string key, IModHelper helper)
        // {
        //     ModData mod_data = GetModData(key, helper);
        //     return mod_data.DaysSinceLastDeath ?? new Dictionary<long, int>();
        // }
        //
        // public static void SetDSLDToKey(string key, Dictionary<long, int> data, IModHelper helper)
        // {
        //     ModData mod_data = GetModData(key, helper);
        //     mod_data.DaysSinceLastDeath = data;
        //     helper.Data.WriteSaveData(key, mod_data);
        // }

        public static void ResetPlayerDSLD(string key, long id, IModHelper helper)
        {
            ModData mod_data = GetModData(key, helper);
            mod_data.DaysSinceLastDeath[id] = 0;
            helper.Data.WriteSaveData(key, mod_data);
        }

        private static void AdvancePlayerDSLD(string key, long id, IModHelper helper)
        {
            ModData mod_data = GetModData(key, helper);
            mod_data.DaysSinceLastDeath[id]++;
            helper.Data.WriteSaveData(key, mod_data);
        }

        public static void AdvanceAllPlayerDSLD(string key, IModHelper helper)
        {
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                AdvancePlayerDSLD(key, farmer.UniqueMultiplayerID, helper);
            }
        }
    }
}