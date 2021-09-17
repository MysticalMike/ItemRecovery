using System;
using System.Collections.Generic;
using StardewValley;

namespace ItemRecovery.Util
{
    public class ListHelper
    {
        public static bool SortListsThenCompareItemNames<T>(List<T> list1, List<T> list2) where T : ISalable
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
    }
}