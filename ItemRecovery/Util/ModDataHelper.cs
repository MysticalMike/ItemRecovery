using System.Collections.Generic;
using ItemRecovery.Data;
using StardewModdingAPI;
using StardewValley;

namespace ItemRecovery.Util
{
    public class ModDataHelper
    {
        private static IModHelper helper;
        private static string mod_data_key;

        public ModDataHelper(string _mod_data_key, IModHelper _helper)
        {
            mod_data_key = _mod_data_key;
            helper = _helper;
        }
        
        public static ModData GetHostModData()
        {
            return helper.Data.ReadSaveData<ModData>(mod_data_key) ?? new ModData(new Dictionary<long, int>());
        }

        public static void SaveHostModData(ModData data)
        {
            helper.Data.WriteSaveData(mod_data_key, data);
        }

        private static ModData GetModData()
        {
            return ModEntry.GetManager().GetModData();
        }

        private static void SaveModData(ModData data, bool write = false)
        {
            ModEntry.GetManager().SetModData(data);
            if (write)
                ModEntry.GetManager().WriteToHost();
        }

        public static void AddModData(ModData new_data)
        {
            ModData old_data = GetModData();
            old_data.AddModData(new_data);
            SaveHostModData(old_data);
        }

        // public static void ResetPlayerDSLD(Farmer player, IModHelper helper)
        // {
        //     long id = player.UniqueMultiplayerID;
        //     if (!player.IsMainPlayer)
        //     {
        //         MessageData message_data = new MessageData("ResetPlayerDSLD", id);
        //         helper.Multiplayer.SendMessage(message_data,"MessageData", null, new[] {PlayerHelper.GetHost().UniqueMultiplayerID});
        //         return;
        //     }
        //     ResetPlayerDSLD(id, helper);
        // }
        
        public static void ResetPlayerDSLD(long id)
        {
            ModData mod_data = GetModData();
            mod_data.DaysSinceLastDeath[id] = 0;
            SaveModData(mod_data, true);
        }

        private static void AdvancePlayerDSLD(long id)
        {
            ModData mod_data = GetModData();
            if (!mod_data.DaysSinceLastDeath.ContainsKey(id))
                mod_data.DaysSinceLastDeath.Add(id, 0);
            mod_data.DaysSinceLastDeath[id]++;
            SaveModData(mod_data);
        }

        public static void AdvanceAllPlayerDSLD()
        {
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                AdvancePlayerDSLD(farmer.UniqueMultiplayerID);
            }
            SaveModData(GetModData(), true);
        }

        public static void GetAllPlayerDSLD()
        {
            ModData mod_data = GetModData();
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                ModEntry.Log($"Name: {farmer.Name} Count: {mod_data.DaysSinceLastDeath[farmer.UniqueMultiplayerID]}");
            }
        }
        
        public static int GetPlayerDSLD(long id)
        {
            ModData mod_data = GetModData();
            if (!mod_data.DaysSinceLastDeath.ContainsKey(id))
                mod_data.DaysSinceLastDeath.Add(id, 0);
            return mod_data.DaysSinceLastDeath[id];
        }
    }
}