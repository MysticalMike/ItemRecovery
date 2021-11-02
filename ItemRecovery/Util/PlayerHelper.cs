using StardewValley;

namespace ItemRecovery.Util
{
    public class PlayerHelper
    {
        public static Farmer GetHost()
        {
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.IsMainPlayer)
                    return farmer;
            }
            ModEntry.Log("Host = Null");
            return null;
        }
    }
}