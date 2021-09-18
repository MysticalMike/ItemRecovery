using StardewModdingAPI;

namespace ItemRecovery.Util
{
    public class TranslationHelper
    {
        public static string GetTranslation(string key, IModHelper helper)
        {
            return helper.Translation.Get(key).Default("missing translation").ToString();
        }
    }
}