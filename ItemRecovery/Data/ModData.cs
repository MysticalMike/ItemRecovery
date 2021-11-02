using System.Collections.Generic;
using Newtonsoft.Json;

namespace ItemRecovery.Data
{
    public class ModData
    {
        [JsonProperty]
        public Dictionary<long, int> DaysSinceLastDeath { get; } = new Dictionary<long, int>();

        public ModData(Dictionary<long, int> DaysSinceLastDeath)
        {
            this.DaysSinceLastDeath = DaysSinceLastDeath;
        }

        public void AddModData(ModData mod_data)
        {
            foreach (var keyValuePair in mod_data.DaysSinceLastDeath)
            {
                if (DaysSinceLastDeath.ContainsKey(keyValuePair.Key))
                    DaysSinceLastDeath[keyValuePair.Key] = keyValuePair.Value;
                else
                    DaysSinceLastDeath.Add(keyValuePair.Key,keyValuePair.Value);
            }
        }

        public void PrintData()
        {
            ModEntry.Log("Values:");
            foreach (var keyValuePair in DaysSinceLastDeath)
            {
                ModEntry.Log($"{keyValuePair.Key} : {keyValuePair.Value}");
            }
        }
    }
}