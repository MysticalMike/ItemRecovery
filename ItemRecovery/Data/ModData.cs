using System.Collections.Generic;

namespace ItemRecovery.Data
{
    public class ModData
    {
        public Dictionary<long, int> DaysSinceLastDeath { get; set; }

        public ModData()
        {
            DaysSinceLastDeath = new Dictionary<long, int>();
        }
    }
}